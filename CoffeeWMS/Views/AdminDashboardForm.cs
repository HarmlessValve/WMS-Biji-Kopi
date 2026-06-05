using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Views.Interfaces;

namespace CoffeeWMS.Views
{
    public class AdminDashboardForm : UserControl, IAdminDashboardView
    {
        private DataGridView dgvLogs;
        private DataGridView dgvSuppliers;
        private DataGridView dgvDestinations;

        public event EventHandler LoadDashboardRequested;

        public AdminDashboardForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            var splitMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 600
            };

            // Left Side: Logs
            var pnlLogs = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var lblLogs = new Label { Text = "Log Aktivitas (vw_logs)", Dock = DockStyle.Top, Font = new Font("Segoe UI", 12, FontStyle.Bold), Height = 30 };
            dgvLogs = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White };
            pnlLogs.Controls.Add(dgvLogs);
            pnlLogs.Controls.Add(lblLogs);

            splitMain.Panel1.Controls.Add(pnlLogs);

            // Right Side: Suppliers and Destinations
            var splitRight = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 250
            };

            var pnlSuppliers = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var lblSuppliers = new Label { Text = "Suppliers (vw_suppliers)", Dock = DockStyle.Top, Font = new Font("Segoe UI", 12, FontStyle.Bold), Height = 30 };
            dgvSuppliers = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White };
            pnlSuppliers.Controls.Add(dgvSuppliers);
            pnlSuppliers.Controls.Add(lblSuppliers);

            splitRight.Panel1.Controls.Add(pnlSuppliers);

            var pnlDestinations = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var lblDestinations = new Label { Text = "Destinations (vw_destinations)", Dock = DockStyle.Top, Font = new Font("Segoe UI", 12, FontStyle.Bold), Height = 30 };
            dgvDestinations = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White };
            pnlDestinations.Controls.Add(dgvDestinations);
            pnlDestinations.Controls.Add(lblDestinations);

            splitRight.Panel2.Controls.Add(pnlDestinations);

            splitMain.Panel2.Controls.Add(splitRight);
            this.Controls.Add(splitMain);

            this.Load += (s, e) => LoadDashboardRequested?.Invoke(this, EventArgs.Empty);
        }

        public void DisplayLogs(object dataSource) => dgvLogs.DataSource = dataSource;
        public void DisplaySuppliers(object dataSource) => dgvSuppliers.DataSource = dataSource;
        public void DisplayDestinations(object dataSource) => dgvDestinations.DataSource = dataSource;
    }
}
