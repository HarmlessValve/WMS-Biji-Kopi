using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Theme;

using System.Linq;
using CoffeeWMS.Models;
using CoffeeWMS.Repositories;

namespace CoffeeWMS.Forms
{
    public class LoginForm : Form
    {
        private Panel pnlBox;
        private Label lblTitle;
        private Label lblSubtitle;
        
        private Label lblUsername;
        private TextBox txtUsername;
        
        private Label lblPassword;
        private TextBox txtPassword;
        
        private Button btnLogin;
        private Label lblError;
        private Label lblVersion;

        public bool IsAuthenticated { get; private set; } = false;

        public LoginForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(1366, 768);
            this.Font = DesignTokens.BodyFont;
            this.BackColor = DesignTokens.Background;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Login - CoffeeWMS";

            // Centered Box
            pnlBox = new Panel();
            pnlBox.Size = new Size(400, 380);
            pnlBox.BackColor = DesignTokens.Surface;
            pnlBox.BorderStyle = BorderStyle.FixedSingle;
            pnlBox.Left = (this.ClientSize.Width - pnlBox.Width) / 2;
            pnlBox.Top = (this.ClientSize.Height - pnlBox.Height) / 2;
            
            // Allow manual centering in resize
            this.Resize += (s, e) => {
                pnlBox.Left = (this.ClientSize.Width - pnlBox.Width) / 2;
                pnlBox.Top = (this.ClientSize.Height - pnlBox.Height) / 2;
            };

            lblTitle = new Label {
                Text = "☕ CoffeeWMS",
                Font = DesignTokens.TitleFont,
                ForeColor = DesignTokens.Primary,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 30, Width = pnlBox.Width, Height = 30
            };

            lblSubtitle = new Label {
                Text = "Sistem Manajemen Gudang Kopi",
                Font = DesignTokens.BodyFont,
                ForeColor = DesignTokens.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 60, Width = pnlBox.Width, Height = 20
            };

            lblUsername = new Label {
                Text = "Username", ForeColor = DesignTokens.TextSecondary,
                Left = 40, Top = 100, Width = 320
            };
            txtUsername = new TextBox { Left = 40, Top = 125, Width = 320, Height = 32 };

            lblPassword = new Label {
                Text = "Password", ForeColor = DesignTokens.TextSecondary,
                Left = 40, Top = 170, Width = 320
            };
            txtPassword = new TextBox { Left = 40, Top = 195, Width = 320, Height = 32, PasswordChar = '•' };

            btnLogin = new Button {
                Text = "MASUK",
                BackColor = DesignTokens.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Left = 40, Top = 250, Width = 320, Height = 36,
                Font = DesignTokens.ButtonFont
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = DesignTokens.PrimaryLight;
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = DesignTokens.Primary;

            lblError = new Label {
                Text = "Username atau password salah",
                ForeColor = DesignTokens.Error,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 40, Top = 295, Width = 320, Visible = false
            };
            
            lblVersion = new Label {
                Text = "v1.0.0",
                ForeColor = DesignTokens.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 340, Width = pnlBox.Width
            };

            pnlBox.Controls.AddRange(new Control[] {
                lblTitle, lblSubtitle,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                btnLogin, lblError, lblVersion
            });

            this.Controls.Add(pnlBox);
            
            this.AcceptButton = btnLogin; // Press Enter to login
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var repo = new UserRepository();
            var matchUser = repo.AuthenticateUser(txtUsername.Text, txtPassword.Text);

            if (matchUser != null)
            {
                Session.CurrentUser = matchUser;
                IsAuthenticated = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                if (!string.IsNullOrEmpty(UserRepository.LastError))
                {
                    MessageBox.Show("Terjadi error Database: " + UserRepository.LastError, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lblError.Visible = true;
                }
            }
        }
    }
}
