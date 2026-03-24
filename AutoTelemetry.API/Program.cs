using AutoTelemetryAPI.Extensions; 
using AutoTelemetryCommon;
using AutoTelemetryEntities.Entities;
using AutoTelemetryEntities.Interfaces;
using AutoTelemetryInfrastructure.Context;
using AutoTelemetryInfrastructure.NotificationsTelemetry;
using AutoTelemetryInfrastructure.Repository;
using AutoTelemetryWorker.Processor;
using AutoTelemetryWorker.Worker;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.Channels;
using System.Threading.RateLimiting;

Log.Logger = LoggerSetup.Configure("API");

try
{
    Log.Information("Iniciando AutoTelemetry API");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
          options.Authority = "http://localhost:7173";
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateAudience = false,
              ValidateIssuer = true
          };
      });

    builder.Services.AddAuthorization();
    builder.Services.AddFastEndpoints();
    builder.Services.AddOpenApi();
    builder.Services.AddHealthChecks();
    builder.Services.AddSignalR();

    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("telemetry-policy", opt =>
        {
            opt.PermitLimit = 5;
            opt.Window = TimeSpan.FromSeconds(1);
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 2;
        });
    });

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=telemetry.db"));

    builder.Services.AddSingleton(Channel.CreateBounded<QueueMessage>(
        new BoundedChannelOptions(16) { FullMode = BoundedChannelFullMode.Wait }
    ));

    builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();
    builder.Services.AddScoped<TelemetryProcessor>();
    builder.Services.AddTransient<IRealTimeNotifier, SignalRNotifier>();

    builder.Services.AddHostedService<JobWorker>();

    // CONSTRUCCIÓN DE LA APP
    var app = builder.Build();
    app.UseAuthentication();
    app.UseAuthorization();
    app.InicializarBaseDeDatos();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseRateLimiter();

    app.UseFastEndpoints();
    app.MapHealthChecks("/health");
    app.MapHub<HubTelemetry>("/telemetryHub");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "El servidor se cerró inesperadamente por un error fatal.");
}
finally
{
    Log.CloseAndFlush();
}