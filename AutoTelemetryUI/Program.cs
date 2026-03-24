using AutoTelemetryCommon;
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

            var login = new frmLogin();

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

          

            if (login.ShowDialog() == DialogResult.OK)
            {
                string token = login.AccessToken;

                Application.Run(new frmAutoTelemetry(token));
            }
            else
            {
                Application.Exit();
            }
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