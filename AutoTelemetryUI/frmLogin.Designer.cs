namespace AutoTelemetryUI
{
    partial class frmLogin
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
            txtUser = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();
            gbInputs = new GroupBox();
            label2 = new Label();
            label1 = new Label();
            gbInputs.SuspendLayout();
            SuspendLayout();
            // 
            // txtUser
            // 
            txtUser.Location = new Point(22, 42);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(126, 23);
            txtUser.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(22, 97);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(126, 23);
            txtPassword.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(31, 140);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(104, 23);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Iniciar Sesión";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // gbInputs
            // 
            gbInputs.Controls.Add(label2);
            gbInputs.Controls.Add(label1);
            gbInputs.Controls.Add(txtUser);
            gbInputs.Controls.Add(txtPassword);
            gbInputs.Controls.Add(btnLogin);
            gbInputs.Location = new Point(12, 12);
            gbInputs.Name = "gbInputs";
            gbInputs.Size = new Size(167, 169);
            gbInputs.TabIndex = 3;
            gbInputs.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 79);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 4;
            label2.Text = "Contraseña";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 24);
            label1.Name = "label1";
            label1.Size = new Size(47, 15);
            label1.TabIndex = 3;
            label1.Text = "Usuario";
            // 
            // frmLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(191, 193);
            Controls.Add(gbInputs);
            Name = "frmLogin";
            Text = "frmLogin";
            gbInputs.ResumeLayout(false);
            gbInputs.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox txtUser;
        private TextBox txtPassword;
        private Button btnLogin;
        private GroupBox gbInputs;
        private Label label1;
        private Label label2;
    }
}