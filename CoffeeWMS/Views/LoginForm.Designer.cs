using System;
using System.Drawing;
using System.Windows.Forms;
namespace CoffeeWMS.Views
{
    partial class LoginForm
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
            this.pnlBox = new Panel();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.lblUsername = new Label();
            this.txtUsername = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.lblError = new Label();
            this.lblVersion = new Label();
            this.pnlBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBox
            // 
            this.pnlBox.BackColor = Color.White;
            this.pnlBox.BorderStyle = BorderStyle.FixedSingle;
            this.pnlBox.Controls.Add(this.lblTitle);
            this.pnlBox.Controls.Add(this.lblSubtitle);
            this.pnlBox.Controls.Add(this.lblUsername);
            this.pnlBox.Controls.Add(this.txtUsername);
            this.pnlBox.Controls.Add(this.lblPassword);
            this.pnlBox.Controls.Add(this.txtPassword);
            this.pnlBox.Controls.Add(this.btnLogin);
            this.pnlBox.Controls.Add(this.lblError);
            this.pnlBox.Controls.Add(this.lblVersion);
            this.pnlBox.Location = new Point(475, 174);
            this.pnlBox.Name = "pnlBox";
            this.pnlBox.Size = new Size(400, 380);
            this.pnlBox.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(41, 53, 65);
            this.lblTitle.Location = new Point(0, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(400, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "â˜• CoffeeWMS";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Font = new Font("Segoe UI", 10F);
            this.lblSubtitle.ForeColor = Color.Gray;
            this.lblSubtitle.Location = new Point(0, 60);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new Size(400, 20);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Sistem Manajemen Gudang Kopi";
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblUsername
            // 
            this.lblUsername.ForeColor = Color.Gray;
            this.lblUsername.Location = new Point(40, 100);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new Size(320, 23);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new Point(40, 125);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new Size(320, 25);
            this.txtUsername.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.ForeColor = Color.Gray;
            this.lblPassword.Location = new Point(40, 170);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(320, 23);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new Point(40, 195);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.Size = new Size(320, 25);
            this.txtPassword.TabIndex = 5;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = Color.FromArgb(41, 53, 65);
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.Location = new Point(40, 250);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new Size(320, 36);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "MASUK";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new EventHandler(this.BtnLogin_Click);
            // 
            // lblError
            // 
            this.lblError.ForeColor = Color.Red;
            this.lblError.Location = new Point(40, 295);
            this.lblError.Name = "lblError";
            this.lblError.Size = new Size(320, 23);
            this.lblError.TabIndex = 7;
            this.lblError.Text = "Username atau password salah";
            this.lblError.TextAlign = ContentAlignment.MiddleCenter;
            this.lblError.Visible = false;
            // 
            // lblVersion
            // 
            this.lblVersion.ForeColor = Color.Gray;
            this.lblVersion.Location = new Point(0, 340);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new Size(400, 23);
            this.lblVersion.TabIndex = 8;
            this.lblVersion.Text = "v1.0.0";
            this.lblVersion.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btnLogin;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.ClientSize = new Size(1350, 729);
            this.Controls.Add(this.pnlBox);
            this.Font = new Font("Segoe UI", 10F);
            this.Name = "LoginForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Login - CoffeeWMS";
            this.pnlBox.ResumeLayout(false);
            this.pnlBox.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlBox;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblVersion;
    }
}

