using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTelemetryUI
{
    public partial class frmLogin : Form
    {
        public string AccessToken { get; private set; } = string.Empty;
        public frmLogin()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            btnLogin.Text = "Autenticando...";
            string authUrl = ConfigurationManager.AppSettings["AuthServerUrl"] ?? "http://localhost:5155";
            var authClient = new HttpClient();

            var requestBody = new Dictionary<string, string>
                    {
                        {"grant_type", "password"},
                        {"client_id", "3d6f9a1c-4f64-49f8-b8e5-0a7c4e97f017"},
                        {"client_secret", "secreto123"}, // Texto plano, el servidor se encarga de hashearlo
                        {"username", txtUser.Text},
                        {"password", txtPassword.Text},
                        {"scope", "AutoTelemetry"}
                    };

            try
            {
                var response = await authClient.PostAsync($"{authUrl}/connect/token", new FormUrlEncodedContent(requestBody));

                if (response.IsSuccessStatusCode)
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonStr);

                     AccessToken = doc.RootElement.GetProperty("access_token").GetString();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Credenciales incorrectas.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Text = "";
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo conectar al servidor de autenticación.\nDetalle: {ex.Message}", "Error de Red", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Ingresar";
            }
        }
    }
}
