namespace AutoTelemetryUI
{
    partial class frmAutoTelemetry
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            txtChasis = new TextBox();
            gpInputs = new GroupBox();
            btnSend = new Button();
            numTemperatura = new NumericUpDown();
            cmbEstacion = new ComboBox();
            dgvTelemetria = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            gpInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numTemperatura).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTelemetria).BeginInit();
            SuspendLayout();
            // 
            // txtChasis
            // 
            txtChasis.Location = new Point(62, 52);
            txtChasis.Name = "txtChasis";
            txtChasis.Size = new Size(121, 23);
            txtChasis.TabIndex = 0;
            txtChasis.Text = "Chasis";
            // 
            // gpInputs
            // 
            gpInputs.Controls.Add(label3);
            gpInputs.Controls.Add(label2);
            gpInputs.Controls.Add(label1);
            gpInputs.Controls.Add(btnSend);
            gpInputs.Controls.Add(numTemperatura);
            gpInputs.Controls.Add(cmbEstacion);
            gpInputs.Controls.Add(txtChasis);
            gpInputs.Location = new Point(12, 12);
            gpInputs.Name = "gpInputs";
            gpInputs.Size = new Size(283, 281);
            gpInputs.TabIndex = 1;
            gpInputs.TabStop = false;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(79, 192);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 3;
            btnSend.Text = "Enviar";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // numTemperatura
            // 
            numTemperatura.Location = new Point(63, 163);
            numTemperatura.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            numTemperatura.Minimum = new decimal(new int[] { 20, 0, 0, int.MinValue });
            numTemperatura.Name = "numTemperatura";
            numTemperatura.Size = new Size(120, 23);
            numTemperatura.TabIndex = 2;
            // 
            // cmbEstacion
            // 
            cmbEstacion.FormattingEnabled = true;
            cmbEstacion.Location = new Point(63, 106);
            cmbEstacion.Name = "cmbEstacion";
            cmbEstacion.Size = new Size(121, 23);
            cmbEstacion.TabIndex = 1;
            // 
            // dgvTelemetria
            // 
            dgvTelemetria.AllowUserToAddRows = false;
            dgvTelemetria.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.SelectionBackColor = Color.Transparent;
            dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            dgvTelemetria.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            dgvTelemetria.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTelemetria.Location = new Point(308, 22);
            dgvTelemetria.MultiSelect = false;
            dgvTelemetria.Name = "dgvTelemetria";
            dgvTelemetria.ReadOnly = true;
            dgvTelemetria.Size = new Size(540, 274);
            dgvTelemetria.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(62, 34);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 4;
            label1.Text = "Id Chasis";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(62, 88);
            label2.Name = "label2";
            label2.Size = new Size(64, 15);
            label2.TabIndex = 5;
            label2.Text = "Id Estacion";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(63, 145);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 6;
            label3.Text = "Temperatura";
            // 
            // frmAutoTelemetry
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(860, 305);
            Controls.Add(dgvTelemetria);
            Controls.Add(gpInputs);
            Name = "frmAutoTelemetry";
            Text = "AutoTelemetry";
            gpInputs.ResumeLayout(false);
            gpInputs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numTemperatura).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvTelemetria).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TextBox txtChasis;
        private GroupBox gpInputs;
        private ComboBox cmbEstacion;
        private Button btnSend;
        private NumericUpDown numTemperatura;
        private DataGridView dgvTelemetria;
        private Label label3;
        private Label label2;
        private Label label1;
    }
}