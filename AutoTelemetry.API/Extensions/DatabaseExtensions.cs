using AutoTelemetryInfrastructure.Context;

namespace AutoTelemetryAPI.Extensions
{
    public static class DatabaseExtensions
    {
        public static void InicializarBaseDeDatos(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.EnsureCreated();

        }
    }
}
