using AutoTelemetryEntities.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using System.Configuration;
using System.Net.Http.Json;
using ClientSDK;
namespace AutoTelemetryUI
{
    public partial class frmAutoTelemetry : Form
    {
        private readonly string _token = null!;
        private static string apiUrl = null!;
        private Client _apiClient = null!;
        private ClientSignalR _clientSignalR = null!;

        public frmAutoTelemetry(string token)
        {
            InitializeComponent();
            _token = token;
            ConfigurarConexiones();
            ConfigurarControles();
            ConfigurarGrilla();
        }

        private async void ConfigurarConexiones()
        {
            apiUrl = ConfigurationManager.AppSettings["ApiUrl"] ?? "https://localhost:7105";
            _apiClient = new Client(apiUrl, _token);

            await IniciarConexionSignalRAsync();
        }

        private async Task IniciarConexionSignalRAsync()
        {
            _clientSignalR = new ClientSignalR($"{apiUrl}/telemetryHub", _token);

            _clientSignalR.SuscribirEvento<Guid, string>("TelemetryProcessed", InvocarActualizacionTelemetria);

            bool conectado = await _clientSignalR.ConectarAsync();

            if (conectado)
            {
                this.Text = "Panel de Telemetría - Conectado (SignalR)";
            }
            else
            {
                this.Text = "Panel de Telemetría - Desconectado (Trabajando Offline)";
            }
        }

        private void InvocarActualizacionTelemetria(Guid idTransaccion, string estadoString)
        {
            if (Enum.TryParse<EstadoTelemetria>(estadoString, out var estadoEnum))
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ActualizarFilaGrilla(idTransaccion, estadoEnum);
                });
            }
        }

        private void ConfigurarControles()
        {
            cmbEstacion.DataSource = Enum.GetValues(typeof(Estacion));
            cmbEstacion.SelectedIndex = -1;
        }

        private void ConfigurarGrilla()
        {
            dgvTelemetria.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTelemetria.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTelemetria.BackgroundColor = Color.White;
            dgvTelemetria.BorderStyle = BorderStyle.None;

            dgvTelemetria.Columns.Add("IdTransaccion", "ID Transacción");
            dgvTelemetria.Columns["IdTransaccion"]?.Visible = false;
            dgvTelemetria.Columns.Add("ChasisId", "ID Chasis");
            dgvTelemetria.Columns.Add("Estacion", "Estación");
            dgvTelemetria.Columns.Add("Temperatura", "Temperatura (°C)");
            dgvTelemetria.Columns.Add("Estado", "Estado");
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            var chasis = txtChasis.Text;
            var estacion = cmbEstacion.Text;
            var temp = (double)numTemperatura.Value;

            if (string.IsNullOrWhiteSpace(chasis) || string.IsNullOrWhiteSpace(estacion))
            {
                MessageBox.Show("Completá los datos del sensor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var idTransaccion = Guid.NewGuid();

            int rowIndex = dgvTelemetria.Rows.Add(
                                                    idTransaccion,
                                                    chasis,
                                                    estacion,
                                                    temp,
                                                    EstadoTelemetria.Enviando.ToString()
                                                );

            dgvTelemetria.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;

            var payload = new
            {
                Id = idTransaccion,
                ChasisId = chasis,
                Estacion = estacion,
                Temperatura = temp
            };

            btnSend.Enabled = false;
            try
            {
                var response = await _apiClient.EnviarChasisAsync(payload);
                if (response.IsSuccess)
                {
                    dgvTelemetria.Rows[rowIndex].Cells["Estado"].Value = EstadoTelemetria.Pendiente.ToString();
                }
                else
                {
                    MessageBox.Show(response.ErrorMessage, "Error de Envío", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    dgvTelemetria.Rows[rowIndex].Cells["Estado"].Value = EstadoTelemetria.Error_HTTP.ToString();
                    dgvTelemetria.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                }
            }
            catch (Exception)
            {
                dgvTelemetria.Rows[rowIndex].Cells["Estado"].Value = EstadoTelemetria.Error_Conexion.ToString();
                dgvTelemetria.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
            }
            finally
            {
                btnSend.Enabled = true;
                txtChasis.Text = "";
            }
        }

        private void ActualizarFilaGrilla(Guid idTransaccion, EstadoTelemetria nuevoEstado)
        {
            foreach (DataGridViewRow row in dgvTelemetria.Rows)
            {
                if (row.Cells["IdTransaccion"].Value is Guid idFila && idFila == idTransaccion)
                {
                    row.Cells["Estado"].Value = nuevoEstado.ToString().Replace("_", " ");

                    switch (nuevoEstado)
                    {
                        case EstadoTelemetria.Alerta_Temperatura:
                            row.DefaultCellStyle.BackColor = Color.Salmon;
                            row.DefaultCellStyle.ForeColor = Color.White;
                            break;
                        case EstadoTelemetria.Error_Duplicado:
                        case EstadoTelemetria.Error_Conexion:
                        case EstadoTelemetria.Error_HTTP:
                            row.DefaultCellStyle.BackColor = Color.LightCoral;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                            break;
                        case EstadoTelemetria.Procesado:
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                            row.DefaultCellStyle.ForeColor = Color.Black;
                            break;
                    }

                    break;
                }
            }
        }
    }
}
