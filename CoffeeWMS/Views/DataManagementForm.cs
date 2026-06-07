using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Models;
using CoffeeWMS.Views.Interfaces;

namespace CoffeeWMS.Views
{
    public class DataManagementForm : UserControl, IDataManagementView
    {
        private DataGridView dgvSuppliers;
        private DataGridView dgvDestinations;

        public event EventHandler LoadDataRequested;
        public event EventHandler<Supplier> AddSupplierRequested;
        public event EventHandler<int> DeleteSupplierRequested;
        public event EventHandler<Destination> AddDestinationRequested;
        public event EventHandler<int> DeleteDestinationRequested;

        public DataManagementForm()
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
                SplitterDistance = this.Width / 2
            };

            // Setup Left (Suppliers)
            var pnlSuppliers = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var lblSuppliers = new Label { Text = "Manajemen Supplier", Dock = DockStyle.Top, Font = new Font("Segoe UI", 14, FontStyle.Bold), Height = 40 };
            
            var pnlSupplierButtons = new Panel { Dock = DockStyle.Top, Height = 150 };
            var txtSupplierName = new TextBox { PlaceholderText = "Nama Supplier", Location = new Point(10, 10), Width = 200 };
            var txtSupplierAddress = new TextBox { PlaceholderText = "Alamat", Location = new Point(10, 40), Width = 200 };
            var txtSupplierPhone = new TextBox { PlaceholderText = "Telepon", Location = new Point(10, 70), Width = 200 };
            
            var btnAddSupplier = new Button { Text = "Tambah Supplier", Location = new Point(10, 100),FlatStyle = FlatStyle.Flat, Width = 120, Height = 35, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            var btnDelSupplier = new Button { Text = "Hapus Supplier", Location = new Point(140, 100), FlatStyle = FlatStyle.Flat, Width = 120, Height = 35, BackColor = Color.FromArgb(222, 5, 0), ForeColor = Color.White };
            
            pnlSupplierButtons.Controls.AddRange(new Control[] { txtSupplierName, txtSupplierAddress, txtSupplierPhone, btnAddSupplier, btnDelSupplier });
            
            dgvSuppliers = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pnlSuppliers.Controls.Add(dgvSuppliers);
            pnlSuppliers.Controls.Add(pnlSupplierButtons);
            pnlSuppliers.Controls.Add(lblSuppliers);

            splitMain.Panel1.Controls.Add(pnlSuppliers);

            // Setup Right (Destinations)
            var pnlDest = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var lblDest = new Label { Text = "Manajemen Destinasi", Dock = DockStyle.Top, Font = new Font("Segoe UI", 14, FontStyle.Bold), Height = 40 };
            
            var pnlDestButtons = new Panel { Dock = DockStyle.Top, Height = 150 };
            var txtDestName = new TextBox { PlaceholderText = "Nama Destinasi", Location = new Point(10, 10), Width = 200 };
            var txtDestAddress = new TextBox { PlaceholderText = "Alamat", Location = new Point(10, 40), Width = 200 };
            
            var btnAddDest = new Button { Text = "Tambah Destinasi", Location = new Point(10, 100), FlatStyle = FlatStyle.Flat, Width = 120, Height = 35, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            var btnDelDest = new Button { Text = "Hapus Destinasi", Location = new Point(140, 100), FlatStyle = FlatStyle.Flat, Width = 120, Height = 35, BackColor = Color.FromArgb(222, 5, 0), ForeColor = Color.White };
            
            pnlDestButtons.Controls.AddRange(new Control[] { txtDestName, txtDestAddress, btnAddDest, btnDelDest });
            
            dgvDestinations = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pnlDest.Controls.Add(dgvDestinations);
            pnlDest.Controls.Add(pnlDestButtons);
            pnlDest.Controls.Add(lblDest);

            splitMain.Panel2.Controls.Add(pnlDest);
            this.Controls.Add(splitMain);

            // Event Handlers
            this.Load += (s, e) => LoadDataRequested?.Invoke(this, EventArgs.Empty);
            
            btnAddSupplier.Click += (s, e) => {
                AddSupplierRequested?.Invoke(this, new Supplier { 
                    CompanyName = txtSupplierName.Text, 
                    Address = txtSupplierAddress.Text, 
                    Phone = txtSupplierPhone.Text 
                });
                txtSupplierName.Clear(); txtSupplierAddress.Clear(); txtSupplierPhone.Clear();
            };

            btnDelSupplier.Click += (s, e) => {
                if (dgvSuppliers.SelectedRows.Count > 0)
                {
                    int id = (int)dgvSuppliers.SelectedRows[0].Cells["SupplierId"].Value;
                    if (MessageBox.Show("Yakin hapus supplier ini?", "Hapus", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        DeleteSupplierRequested?.Invoke(this, id);
                }
            };

            btnAddDest.Click += (s, e) => {
                AddDestinationRequested?.Invoke(this, new Destination { 
                    DestinationName = txtDestName.Text, 
                    Address = txtDestAddress.Text 
                });
                txtDestName.Clear(); txtDestAddress.Clear();
            };

            btnDelDest.Click += (s, e) => {
                if (dgvDestinations.SelectedRows.Count > 0)
                {
                    int id = (int)dgvDestinations.SelectedRows[0].Cells["DestinationId"].Value;
                    if (MessageBox.Show("Yakin hapus destinasi ini?", "Hapus", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        DeleteDestinationRequested?.Invoke(this, id);
                }
            };
        }

        public void DisplaySuppliers(object dataSource) => dgvSuppliers.DataSource = dataSource;
        public void DisplayDestinations(object dataSource) => dgvDestinations.DataSource = dataSource;

        public void ShowMessage(string message, bool isError = false)
        {
            MessageBox.Show(message, isError ? "Error" : "Info", MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }
    }
}
