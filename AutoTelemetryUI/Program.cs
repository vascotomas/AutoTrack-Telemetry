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
            var login = new frmLogin();

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
    }
}