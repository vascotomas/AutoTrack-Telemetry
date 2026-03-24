using AutoTelemetryCommon;
using ClientSDK;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
namespace AutoTelemetryUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Log.Logger = LoggerSetup.Configure("WinForms_UI");

            var services = new ServiceCollection();
            services.AddHttpClient<Client>();
            services.AddTransient<frmLogin>();
            services.AddTransient<frmAutoTelemetry>();
            using var serviceProvider = services.BuildServiceProvider();


            Application.ThreadException += (sender, args) =>
            {
                MostrarError(args.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    MostrarError(ex);
                }
            };

            var login = serviceProvider.GetRequiredService<frmLogin>();

            if (login.ShowDialog() == DialogResult.OK)
            {
                var mainForm = serviceProvider.GetRequiredService<frmAutoTelemetry>();
                mainForm.ConfigurarToken(login.AccessToken);
                Application.Run(mainForm);
            }
            Log.CloseAndFlush();
        }

        private static void MostrarError(Exception ex)
        {
            MessageBox.Show(
                $"Ocurrió un problema inesperado en el sistema.\nEl sistema se recuperará automáticamente.\n\nDetalle técnico para sistemas: {ex.Message}",
                "Error Interno",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}