using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Models;
using CoffeeWMS.Theme;

namespace CoffeeWMS.Forms
{
    public class MainForm : BaseForm
    {
        public MainForm()
        {
            InitializeMenus();
            ShowDashboard();
        }

        private void InitializeMenus()
        {
            // Adding Menus to sidebar
            int currentTop = 60;
            
            AddMenuItem("📊 Dashboard", currentTop, ShowDashboard);
            currentTop += 40;

            if (Session.IsAdmin)
            {
                AddMenuItem("👥 Pengguna", currentTop, () => LoadView(new UserManagementForm(), "Manajemen Pengguna"));
                currentTop += 40;
            }

            AddMenuItem("📦 Penerimaan", currentTop, () => ShowPlaceholder("Input Penerimaan Kopi"));
            currentTop += 40;
            AddMenuItem("📤 Pengiriman", currentTop, () => ShowPlaceholder("Input Pengiriman Kopi"));
            currentTop += 40;
            AddMenuItem("📈 Laporan", currentTop, () => ShowPlaceholder("Laporan Transaksi"));
            
            // Logout
            Button btnLogout = AddMenuItem("🚪 Logout", this.pnlSidebar.Height - 60, null);
            btnLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLogout.Click += (s, e) => { this.Close(); };
        }

        private Button AddMenuItem(string text, int top, Action onClick)
        {
            Button btn = new Button();
            btn.Text = "  " + text;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = DesignTokens.Primary;
            btn.ForeColor = Color.White;
            btn.Font = DesignTokens.BodyFont;
            btn.Width = DesignTokens.SidebarWidth;
            btn.Height = 40;
            btn.Top = top;
            btn.Cursor = Cursors.Hand;
            
            btn.MouseEnter += (s, e) => btn.BackColor = DesignTokens.PrimaryLight;
            btn.MouseLeave += (s, e) => btn.BackColor = DesignTokens.Primary;

            if (onClick != null)
                btn.Click += (s, e) => onClick();

            this.pnlSidebar.Controls.Add(btn);
            return btn;
        }

        private void ShowDashboard()
        {
            Panel p = new Panel();
            
            Label l = new Label();
            l.Text = "Selamat datang di CoffeeWMS!";
            l.Font = DesignTokens.HeadingFont;
            l.AutoSize = true;
            l.Location = new Point(20, 20);
            p.Controls.Add(l);

            PrimaryButton btn = new PrimaryButton();
            btn.Text = "Test Tombol Reusable";
            btn.Location = new Point(20, 60);
            btn.Width = 200;
            p.Controls.Add(btn);

            LoadView(p, "📊 Dashboard");
        }

        private void ShowPlaceholder(string module)
        {
            Panel p = new Panel();
            Label l = new Label();
            l.Text = $"Modul {module} akan datang.";
            l.Font = DesignTokens.BodyFont;
            l.AutoSize = true;
            l.Location = new Point(20, 20);
            p.Controls.Add(l);
            LoadView(p, module);
        }
    }
}
