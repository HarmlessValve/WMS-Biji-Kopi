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
        private DataGridView dgvKopi;

        private ComboBox cmbJenisKopi;
        private TextBox txtOrigin;

        // Input untuk Kopi
        private TextBox txtKopiName;
        private ComboBox cmbKategoriKopi;
        private NumericUpDown nudMinStock;

        public event EventHandler? LoadDataRequested;
        public event EventHandler<Supplier>? AddSupplierRequested;
        public event EventHandler<int>? DeleteSupplierRequested;
        public event EventHandler<Destination>? AddDestinationRequested;
        public event EventHandler<int>? DeleteDestinationRequested;
        
        // Event Kopi (Sekarang menggunakan Objek Coffee)
        public event EventHandler<Coffee>? AddCoffeeRequested;
        public event EventHandler<int>? DeleteCoffeeRequested;

        public DataManagementForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            TabControl tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), ItemSize = new Size(150, 30) };

            // ==========================================
            // TAB 1: SUPPLIER
            // ==========================================
            TabPage tabSupplier = new TabPage("Manajemen Supplier") { BackColor = Color.White };
            var pnlSupplierButtons = new Panel { Dock = DockStyle.Top, Height = 150 };
            var txtSupplierName = new TextBox { PlaceholderText = "Nama Supplier", Location = new Point(10, 15), Width = 200 };
            var txtSupplierAddress = new TextBox { PlaceholderText = "Alamat", Location = new Point(10, 50), Width = 200 };
            var txtSupplierPhone = new TextBox { PlaceholderText = "Telepon", Location = new Point(10, 85), Width = 200 };
            var btnAddSupplier = new Button { Text = "Tambah Supplier", Location = new Point(230, 15),FlatStyle = FlatStyle.Flat, Width = 130, Height = 35, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            var btnDelSupplier = new Button { Text = "Hapus Supplier", Location = new Point(230, 60), FlatStyle = FlatStyle.Flat, Width = 130, Height = 35, BackColor = Color.FromArgb(222, 5, 0), ForeColor = Color.White };
            
            pnlSupplierButtons.Controls.AddRange(new Control[] { txtSupplierName, txtSupplierAddress, txtSupplierPhone, btnAddSupplier, btnDelSupplier });
            dgvSuppliers = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            tabSupplier.Controls.Add(dgvSuppliers);
            tabSupplier.Controls.Add(pnlSupplierButtons);

            // ==========================================
            // TAB 2: DESTINASI
            // ==========================================
            TabPage tabDestinasi = new TabPage("Manajemen Destinasi") { BackColor = Color.White };
            var pnlDestButtons = new Panel { Dock = DockStyle.Top, Height = 100 };
            var txtDestName = new TextBox { PlaceholderText = "Nama Destinasi", Location = new Point(10, 15), Width = 200 };
            var txtDestAddress = new TextBox { PlaceholderText = "Alamat", Location = new Point(10, 50), Width = 200 };
            var btnAddDest = new Button { Text = "Tambah Destinasi", Location = new Point(230, 15), FlatStyle = FlatStyle.Flat, Width = 130, Height = 35, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            var btnDelDest = new Button { Text = "Hapus Destinasi", Location = new Point(230, 60), FlatStyle = FlatStyle.Flat, Width = 130, Height = 35, BackColor = Color.FromArgb(222, 5, 0), ForeColor = Color.White };
            
            pnlDestButtons.Controls.AddRange(new Control[] { txtDestName, txtDestAddress, btnAddDest, btnDelDest });
            dgvDestinations = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            tabDestinasi.Controls.Add(dgvDestinations);
            tabDestinasi.Controls.Add(pnlDestButtons);

            // TAB 3: JENIS KOPI
            
            TabPage tabKopi = new TabPage("Manajemen Jenis Kopi") { BackColor = Color.White };
            var pnlKopiButtons = new Panel { Dock = DockStyle.Top, Height = 100 };
            
            // 1. Dropdown Jenis Kopi (Arabica, Robusta, dll)
            Label lblJenis = new Label { Text = "Jenis:", Location = new Point(10, 15), Width = 50 };
            cmbJenisKopi = new ComboBox { Location = new Point(60, 12), Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbJenisKopi.Items.AddRange(new string[] { "Arabica", "Robusta", "Liberica", "Excelsa" });
            
            // 2. Textbox Baru Khusus KOPI ORIGIN
            Label lblOrigin = new Label { Text = "Origin:", Location = new Point(185, 15), Width = 50 };
            txtOrigin = new TextBox { PlaceholderText = "Cth: Gayo / Ijen", Location = new Point(235, 12), Width = 130 };
            
            // 3. Dropdown Kategori / Proses Kopi
            Label lblCat = new Label { Text = "Kategori:", Location = new Point(10, 50), Width = 60 };
            cmbKategoriKopi = new ComboBox { Location = new Point(70, 47), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "CategoryName", ValueMember = "CategoryId" };
            
            // 4. Input Stok Minimal
            Label lblStock = new Label { Text = "Min. Stok:", Location = new Point(235, 50), Width = 65 };
            nudMinStock = new NumericUpDown { Location = new Point(305, 47), Width = 60, Minimum = 0, Maximum = 10000 };
            
            // Tombol Kontrol
            var btnAddKopi = new Button { Text = "Tambah Kopi", Location = new Point(390, 12), FlatStyle = FlatStyle.Flat, Width = 120, Height = 30, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            var btnDelKopi = new Button { Text = "Hapus Kopi", Location = new Point(390, 47), FlatStyle = FlatStyle.Flat, Width = 120, Height = 30, BackColor = Color.FromArgb(222, 5, 0), ForeColor = Color.White };
            
            pnlKopiButtons.Controls.AddRange(new Control[] { lblJenis, cmbJenisKopi, lblOrigin, txtOrigin, lblCat, cmbKategoriKopi, lblStock, nudMinStock, btnAddKopi, btnDelKopi });
            dgvKopi = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            tabKopi.Controls.Add(dgvKopi);
            tabKopi.Controls.Add(pnlKopiButtons);

            // 1. Wadah utama untuk menampung tab
            TabControl mainTabControl = new TabControl { Dock = DockStyle.Fill };
            
            // 2. Masukkan ketiga Tab ke dalam wadah tersebut
            mainTabControl.TabPages.Add(tabSupplier);    // Tab Manajemen Supplier
            mainTabControl.TabPages.Add(tabDestinasi);   // Tab Manajemen Destinasi
            mainTabControl.TabPages.Add(tabKopi);        // Tab Manajemen Jenis Kopi yang baru kita edit
            
            // 3. Tampilkan ke layar
            this.Controls.Add(mainTabControl);

            // ==========================================
            // EVENT HANDLERS
            // ==========================================
            this.Load += (s, e) => LoadDataRequested?.Invoke(this, EventArgs.Empty);
            
            btnAddSupplier.Click += (s, e) => {
                AddSupplierRequested?.Invoke(this, new Supplier { CompanyName = txtSupplierName.Text, Address = txtSupplierAddress.Text, Phone = txtSupplierPhone.Text });
                txtSupplierName.Clear(); txtSupplierAddress.Clear(); txtSupplierPhone.Clear();
            };

            btnDelSupplier.Click += (s, e) => {
                if (dgvSuppliers.SelectedRows.Count > 0) {
                    int id = Convert.ToInt32(dgvSuppliers.SelectedRows[0].Cells[0].Value);
                    if (MessageBox.Show("Yakin hapus supplier ini?", "Hapus", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        DeleteSupplierRequested?.Invoke(this, id);
                }
            };

            btnAddDest.Click += (s, e) => {
                AddDestinationRequested?.Invoke(this, new Destination { DestinationName = txtDestName.Text, Address = txtDestAddress.Text });
                txtDestName.Clear(); txtDestAddress.Clear();
            };

            btnDelDest.Click += (s, e) => {
                if (dgvDestinations.SelectedRows.Count > 0) {
                    int id = Convert.ToInt32(dgvDestinations.SelectedRows[0].Cells[0].Value);
                    if (MessageBox.Show("Yakin hapus destinasi ini?", "Hapus", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        DeleteDestinationRequested?.Invoke(this, id);
                }
            };

            // Event Tombol Kopi (Sesuai Logika Penggabungan Nama)
            btnAddKopi.Click += (s, e) => {
                // Validasi input
                if (cmbJenisKopi.SelectedIndex == -1) { ShowMessage("Silakan pilih Jenis Kopi terlebih dahulu!", true); return; }
                if (string.IsNullOrWhiteSpace(txtOrigin.Text)) { ShowMessage("Asal Kopi (Origin) tidak boleh kosong!", true); return; }
                if (cmbKategoriKopi.SelectedValue == null) { ShowMessage("Silakan pilih Kategori Kopi terlebih dahulu!", true); return; }

                // Ambil nilai dari UI
                string jenisKopi = cmbJenisKopi.SelectedItem.ToString();
                string originKopi = txtOrigin.Text.Trim();

                Coffee newCoffee = new Coffee {
                    CoffeeName = jenisKopi,       // Contoh: "Arabica"
                    Origin = originKopi,           // Contoh: "Gayo"
                    CategoryId = (int)cmbKategoriKopi.SelectedValue,
                    MinimumStock = (int)nudMinStock.Value
                };

                // Kirim data ke Controller / Presenter
                AddCoffeeRequested?.Invoke(this, newCoffee);
                
                // Reset Form Input setelah berhasil input
                cmbJenisKopi.SelectedIndex = -1;
                txtOrigin.Clear();
                nudMinStock.Value = 0;
            };

            btnDelKopi.Click += (s, e) => {
                if (dgvKopi.SelectedRows.Count > 0) {
                    try {
                        int id = Convert.ToInt32(dgvKopi.SelectedRows[0].Cells[0].Value);
                        if (MessageBox.Show("Yakin hapus jenis kopi ini?", "Hapus", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            DeleteCoffeeRequested?.Invoke(this, id);
                    } catch { ShowMessage("Gagal mengambil ID data.", true); }
                }
            };
        }

        public void DisplaySuppliers(object dataSource) => dgvSuppliers.DataSource = dataSource;
        public void DisplayDestinations(object dataSource) => dgvDestinations.DataSource = dataSource;
        
        public void DisplayCoffeeTypes(object dataSource) => dgvKopi.DataSource = dataSource;
        
        // Mengisi dropdown Kategori dari Database
        public void DisplayCoffeeCategories(object dataSource)
        {
            cmbKategoriKopi.DataSource = dataSource;
        }

        public void ShowMessage(string message, bool isError = false)
        {
            MessageBox.Show(message, isError ? "Error" : "Info", MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }
    }
}