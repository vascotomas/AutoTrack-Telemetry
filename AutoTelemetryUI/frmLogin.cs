using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientSDK; // Asegurate de tener la referencia al SDK

namespace AutoTelemetryUI
{
    public partial class frmLogin : Form
    {
        public string AccessToken { get; private set; } = string.Empty;

        private readonly Client _apiClient;

        public frmLogin(Client apiClient)
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
            _apiClient = apiClient;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            btnLogin.Text = "Autenticando...";

            string authUrl = ConfigurationManager.AppSettings["AuthServerUrl"] ?? "http://localhost:5155";
            string clientSecret = ConfigurationManager.AppSettings["AuthClientSecret"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                MessageBox.Show("Falta configurar AuthClientSecret en App.config.", "Configuración inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RestaurarBoton();
                return;
            }

            var authResult = await _apiClient.LoginAsync(authUrl, txtUser.Text, txtPassword.Text, clientSecret);

            if (authResult.IsSuccess)
            {
                AccessToken = authResult.AccessToken;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(authResult.ErrorMessage, "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Text = "";
                txtPassword.Focus();
                RestaurarBoton();
            }
        }

        private void RestaurarBoton()
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "Ingresar";
        }
    }
}