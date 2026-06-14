using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Models;
namespace CoffeeWMS.Views
{
    public partial class MainForm : BaseForm
    {
        public event EventHandler ViewLoaded;
        public event EventHandler LogoutRequested;

        private bool _isPetugasMode = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeMenus();
            this.Load += (s, e) => ViewLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeMenus()
        {
            // Adding Menus to sidebar
            int currentTop = 120; // Diturunkan agar tidak tertimpa panel Logo
            
            AddMenuItem("Dashboard", currentTop, ShowDashboard);
            currentTop += 40;

            if (Session.IsAdmin)
            {
                AddMenuItem("Pengguna", currentTop, ShowUserManagement);
                currentTop += 40;
            }

            if (Session.IsAdmin || Session.IsManager)
            {
                AddMenuItem("Master Data", currentTop, ShowDataManagement);
                currentTop += 40;
            }

            if (Session.IsAdmin || Session.IsPetugas)
            {
                AddMenuItem("Penerimaan", currentTop, () => ShowPlaceholder("Input Penerimaan Kopi"));
                currentTop += 40;
                AddMenuItem("Pengiriman", currentTop, () => ShowPlaceholder("Input Pengiriman Kopi"));
                currentTop += 40;
            }

            if (Session.IsAdmin || Session.IsManager)
            {
                AddMenuItem("Laporan", currentTop, () => ShowPlaceholder("Laporan Transaksi"));
                currentTop += 40;
            }
            
            // Logout
            Button btnLogout = AddMenuItem("Logout", this.pnlSidebar.Height - 60, null);
            btnLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLogout.Click += (s, e) => LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private Button AddMenuItem(string text, int top, Action onClick)
        {
            Button btn = new Button();
            btn.Text = "  " + text;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(41, 53, 65);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10F);
            btn.Width = 250;
            btn.Height = 40;
            btn.Top = top;
            btn.Cursor = Cursors.Hand;
            
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(61, 73, 85);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(41, 53, 65);

            if (onClick != null)
                btn.Click += (s, e) => onClick();

            this.pnlSidebar.Controls.Add(btn);
            return btn;
        }

        public void ShowDashboard()
        {
            bool canToggle = Session.IsAdmin || (Session.IsManager && Session.IsPetugas);

            if (!canToggle)
            {
                _isPetugasMode = Session.IsPetugas && !Session.IsAdmin && !Session.IsManager;
            }

            if (!_isPetugasMode)
            {
                if (Session.IsAdmin || Session.IsManager)
                {
                    var view = new AdminDashboardForm();
                    var controller = new CoffeeWMS.Controllers.AdminDashboardController(view);
                    LoadView(view, Session.IsAdmin ? "Admin Dashboard" : "Manager Dashboard");
                }
                else
                {
                    // Fallback if not admin/manager and not petugas mode (should not happen usually)
                    var view = new PetugasDashboardForm();
                    var controller = new CoffeeWMS.Controllers.PetugasDashboardController(view);
                    LoadView(view, "Dashboard Petugas");
                }
            }
            else
            {
                var view = new PetugasDashboardForm();
                var controller = new CoffeeWMS.Controllers.PetugasDashboardController(view);
                LoadView(view, "Dashboard Petugas");
            }

            if (canToggle)
            {
                FlowLayoutPanel togglePanel = new FlowLayoutPanel();
                togglePanel.AutoSize = true;
                togglePanel.FlowDirection = FlowDirection.LeftToRight;
                // Position it at the right side of the header. pnlHeader.Width will adjust, but using anchors is better.
                togglePanel.Location = new Point(pnlHeader.Width - 260, 25);
                togglePanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                Label lblMode = new Label { Text = "Mode Dashboard:", AutoSize = true, Margin = new Padding(0, 2, 5, 0), Font = new Font("Segoe UI", 10F, FontStyle.Regular) };

                LinkLabel lnkManager = new LinkLabel { Text = Session.IsAdmin ? "Admin" : "Manager", AutoSize = true, Margin = new Padding(0, 2, 5, 0), Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
                lnkManager.LinkBehavior = LinkBehavior.HoverUnderline;
                lnkManager.LinkColor = _isPetugasMode ? Color.Gray : Color.FromArgb(41, 53, 65);
                lnkManager.Enabled = _isPetugasMode;
                lnkManager.Click += (s, e) => { _isPetugasMode = false; ShowDashboard(); };

                Label lblSep = new Label { Text = "|", AutoSize = true, Margin = new Padding(0, 2, 5, 0), Font = new Font("Segoe UI", 10F, FontStyle.Regular) };

                LinkLabel lnkPetugas = new LinkLabel { Text = "Petugas", AutoSize = true, Margin = new Padding(0, 2, 5, 0), Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
                lnkPetugas.LinkBehavior = LinkBehavior.HoverUnderline;
                lnkPetugas.LinkColor = !_isPetugasMode ? Color.Gray : Color.FromArgb(41, 53, 65);
                lnkPetugas.Enabled = !_isPetugasMode;
                lnkPetugas.Click += (s, e) => { _isPetugasMode = true; ShowDashboard(); };

                togglePanel.Controls.Add(lblMode);
                togglePanel.Controls.Add(lnkManager);
                togglePanel.Controls.Add(lblSep);
                togglePanel.Controls.Add(lnkPetugas);

                pnlHeader.Controls.Add(togglePanel);
                togglePanel.BringToFront();
            }
        }

private void ShowPlaceholder(string module)
{
    if (module == "Input Penerimaan Kopi")
    {
        LoadView(new PenerimaanView(), "Penerimaan Kopi");
    }
    else if (module == "Input Pengiriman Kopi")
    {
        LoadView(new PengirimanView(), "Pengiriman Kopi");
    }
    else if (module == "Laporan Transaksi")
    {
        LoadView(new LaporanView(), "Laporan Transaksi");
    }
}

        public void ShowUserManagement()
        {
            var umView = new UserManagementForm();
            var umController = new CoffeeWMS.Controllers.UserManagementController(umView);
            LoadView(umView, "Manajemen Pengguna");
            umView.TriggerLoad();
        }

        public void ShowDataManagement()
        {
            var view = new DataManagementForm();
            var controller = new CoffeeWMS.Controllers.DataManagementController(view);
            LoadView(view, "Manajemen Master Data");
        }

        public void CloseView()
        {
            this.Close();
        }
    }
}

