using AutoTelemetryUI.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Net.Http.Json;
using System.Text;
using System.Windows.Forms;
namespace AutoTelemetryUI
{
    public partial class frmAutoTelemetry : Form
    {
        private HubConnection _hubConnection;
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7105/") };
        public frmAutoTelemetry()
        {
            InitializeComponent();
            ConfigurarControles(); // <-- Agregamos esto
            ConfigurarGrilla();
            ConfigurarSignalRAsync();
        }

        private void ConfigurarControles()
        {
            cmbEstacion.DataSource = Enum.GetValues(typeof(Enums.Enums.Estacion));
            cmbEstacion.SelectedIndex = -1;
        }

        private void ConfigurarGrilla()
        {
            dgvTelemetria.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTelemetria.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTelemetria.BackgroundColor = Color.White;
            dgvTelemetria.BorderStyle = BorderStyle.None;

            dgvTelemetria.Columns.Add("ChasisId", "ID Chasis");
            dgvTelemetria.Columns.Add("Estacion", "Estación");
            dgvTelemetria.Columns.Add("Temperatura", "Temperatura (°C)");
            dgvTelemetria.Columns.Add("Estado", "Estado");
        }
        private async void ConfigurarSignalRAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7105/telemetryHub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string>("TelemetryProcessed", (chasisId, nuevoEstado) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ActualizarFilaGrilla(chasisId, nuevoEstado);
                });
            });

            try
            {
                await _hubConnection.StartAsync();
                this.Text = "Panel de Telemetría - 🟢 Conectado (SignalR)";
            }
            catch (Exception)
            {
                this.Text = "Panel de Telemetría - 🔴 Desconectado";
            }
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

            int rowIndex = dgvTelemetria.Rows.Add(chasis, estacion, temp, "Enviando...");
            dgvTelemetria.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;

            var payload = new
            {
                ChasisId = chasis,
                Estacion = estacion,
                Temperatura = temp
            };

            btnSend.Enabled = false;
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/telemetry", payload);
                if (response.IsSuccessStatusCode)
                {
                    dgvTelemetria.Rows[rowIndex].Cells["Estado"].Value = "Pendiente (En Cola)";
                }
                else
                {
                    dgvTelemetria.Rows[rowIndex].Cells["Estado"].Value = "Error HTTP";
                    dgvTelemetria.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                }
            }
            catch (Exception)
            {
                dgvTelemetria.Rows[rowIndex].Cells["Estado"].Value = "Error de Conexión";
                dgvTelemetria.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
            }
            finally
            {
                btnSend.Enabled = true;
                txtChasis.Text = "";
            }
        }

        private void ActualizarFilaGrilla(string chasisId, string nuevoEstado)
        {
            foreach (DataGridViewRow row in dgvTelemetria.Rows)
            {
                if (row.Cells["ChasisId"].Value?.ToString() == chasisId)
                {
                    row.Cells["Estado"].Value = nuevoEstado;

                    if (nuevoEstado == "Alerta_Temperatura")
                    {
                        row.DefaultCellStyle.BackColor = Color.Salmon;
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }
                    else if(nuevoEstado.Contains("Error"))
                    {
                        row.Cells["Estado"].Value = nuevoEstado;
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    break;
                }
            }
        }
    }
}
