using AutoTelemetryCommon;
using AutoTelemetryEntities;
using AutoTelemetryWorker.Processor;
using AutoTelemetryWorker.Worker;
using FastEndpoints;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.Channels;
using System.Threading.RateLimiting;
using AutoTelemetryInfrastructure.Context;
using AutoTelemetryEntities.Entities;
using AutoTelemetryEntities.Interfaces;
using AutoTelemetryInfrastructure.NotificationsTelemetry;

Log.Logger = LoggerSetup.Configure("API");

try
{
    Log.Information("Iniciando AutoTelemetry API");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();
    builder.Services.AddFastEndpoints();
    builder.Services.AddOpenApi();
    builder.Services.AddHealthChecks();

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

    builder.Services.AddScoped<TelemetryProcessor>();
    builder.Services.AddHostedService<JobWorker>();
    builder.Services.AddSignalR();

    builder.Services.AddScoped<IRealTimeNotifier, SignalRNotifier>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
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