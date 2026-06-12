using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using CoffeeWMS.Models;
namespace CoffeeWMS.Views
{
    public partial class LoginForm : Form
    {

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsAuthenticated { get; set; } = false;
        public string Username => txtUsername.Text;
        public string Password => txtPassword.Text;

        public event EventHandler LoginAttempted;

        public LoginForm()
        {
            InitializeComponent();

            this.Resize += (s, e) =>
            {
                pnlBox.Left = (this.ClientSize.Width - pnlBox.Width) / 2;
                pnlBox.Top = (this.ClientSize.Height - pnlBox.Height) / 2;
            };
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(61, 73, 85);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(41, 53, 65);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            LoginAttempted?.Invoke(this, EventArgs.Empty);
        }

        public void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        public void CloseView()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void pnlBox_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}


