using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTelemetryCommon
{
    public static class LoggerSetup
    {
        public static ILogger Configure(string applicationName, string customPath = null)
        {

            string filePath = customPath ?? $"logs/{applicationName.ToLower()}-log-.txt";

            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("Application", applicationName)
                .WriteTo.Console()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
