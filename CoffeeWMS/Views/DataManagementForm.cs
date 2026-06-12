using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Models;

namespace CoffeeWMS.Views
{
    public class DataManagementForm : UserControl
    {
        private DataGridView dgvSuppliers;
        private DataGridView dgvDestinations;
        private DataGridView dgvProducts;
        private ComboBox cmbCoffeeType;
        private ComboBox cmbCategory;
        private ComboBox cmbOrigin;
        private ComboBox cmbRoastLevel;
        private Label lblRoastLevel;

        // Supplier Form Controls
        private Panel pnlSupplierForm;
        private TextBox txtSupplierName;
        private TextBox txtSupplierAddress;
        private TextBox txtSupplierPhone;
        private CheckBox chkSupplierActive;
        private CheckBox chkShowInactiveSupplier;
        private int _editingSupplierId = 0;

        // Destination Form Controls
        private Panel pnlDestForm;
        private TextBox txtDestName;
        private TextBox txtDestAddress;
        private CheckBox chkDestActive;
        private CheckBox chkShowInactiveDest;
        private int _editingDestinationId = 0;

        public event EventHandler LoadDataRequested;
        public event EventHandler<Supplier> SaveSupplierRequested;
        public event EventHandler<int> DeleteSupplierRequested;
        public event EventHandler<Destination> SaveDestinationRequested;
        public event EventHandler<int> DeleteDestinationRequested;
        public event EventHandler<(int coffeeId, int categoryId, int originId, int minimumStock, int? roastLevelId)> AddCoffeeProductRequested;
        public event EventHandler<int> DeleteCoffeeProductRequested;
        public event EventHandler<CoffeeOrigin> AddCoffeeOriginRequested;

        public bool ShowInactiveSupplier => chkShowInactiveSupplier.Checked;
        public bool ShowInactiveDestination => chkShowInactiveDest.Checked;

        public DataManagementForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(15, 10),
                Font = new Font("Segoe UI", 10)
            };

            // ==========================================
            // TAB 1: SUPPLIERS
            // ==========================================
            var tabSuppliers = new TabPage("Supplier");
            var pnlSuppliers = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var pnlSupplierButtons = new Panel { Dock = DockStyle.Top, Height = 50 };
            
            var btnAddSupplier = new Button { Text = "Tambah Supplier", Location = new Point(10, 10), FlatStyle = FlatStyle.Flat, Width = 150, Height = 30, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            chkShowInactiveSupplier = new CheckBox { Text = "Tampilkan yang Tidak Aktif", Location = new Point(180, 15), AutoSize = true };
            
            pnlSupplierButtons.Controls.AddRange(new Control[] { btnAddSupplier, chkShowInactiveSupplier });
            
            pnlSupplierForm = new Panel { Dock = DockStyle.Right, Width = 300, BackColor = Color.FromArgb(245, 245, 245), Padding = new Padding(15), Visible = false };
            Label lblSupTitle = new Label { Text = "Form Supplier", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(15, 15), AutoSize = true };
            txtSupplierName = new TextBox { PlaceholderText = "Nama Perusahaan", Location = new Point(15, 50), Width = 250 };
            txtSupplierAddress = new TextBox { PlaceholderText = "Alamat", Location = new Point(15, 90), Width = 250, Multiline = true, Height = 60 };
            txtSupplierPhone = new TextBox { PlaceholderText = "Telepon", Location = new Point(15, 160), Width = 250 };
            chkSupplierActive = new CheckBox { Text = "Aktif", Location = new Point(15, 200), AutoSize = true, Checked = true };
            
            var btnSaveSupplier = new Button { Text = "Simpan", Location = new Point(15, 240), FlatStyle = FlatStyle.Flat, Width = 100, Height = 30, BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White };
            var btnCancelSupplier = new Button { Text = "Batal", Location = new Point(125, 240), FlatStyle = FlatStyle.Flat, Width = 100, Height = 30, BackColor = Color.Gray, ForeColor = Color.White };
            
            pnlSupplierForm.Controls.AddRange(new Control[] { lblSupTitle, txtSupplierName, txtSupplierAddress, txtSupplierPhone, chkSupplierActive, btnSaveSupplier, btnCancelSupplier });

            dgvSuppliers = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            
            pnlSuppliers.Controls.Add(dgvSuppliers);
            pnlSuppliers.Controls.Add(pnlSupplierForm);
            pnlSuppliers.Controls.Add(pnlSupplierButtons);
            tabSuppliers.Controls.Add(pnlSuppliers);

            // ==========================================
            // TAB 2: DESTINATIONS
            // ==========================================
            var tabDestinations = new TabPage("Destinasi");
            var pnlDest = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var pnlDestButtons = new Panel { Dock = DockStyle.Top, Height = 50 };
            
            var btnAddDest = new Button { Text = "Tambah Destinasi", Location = new Point(10, 10), FlatStyle = FlatStyle.Flat, Width = 150, Height = 30, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            chkShowInactiveDest = new CheckBox { Text = "Tampilkan yang Tidak Aktif", Location = new Point(180, 15), AutoSize = true };
            
            pnlDestButtons.Controls.AddRange(new Control[] { btnAddDest, chkShowInactiveDest });

            pnlDestForm = new Panel { Dock = DockStyle.Right, Width = 300, BackColor = Color.FromArgb(245, 245, 245), Padding = new Padding(15), Visible = false };
            Label lblDestTitle = new Label { Text = "Form Destinasi", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(15, 15), AutoSize = true };
            txtDestName = new TextBox { PlaceholderText = "Nama Destinasi", Location = new Point(15, 50), Width = 250 };
            txtDestAddress = new TextBox { PlaceholderText = "Alamat", Location = new Point(15, 90), Width = 250, Multiline = true, Height = 60 };
            chkDestActive = new CheckBox { Text = "Aktif", Location = new Point(15, 160), AutoSize = true, Checked = true };

            var btnSaveDest = new Button { Text = "Simpan", Location = new Point(15, 200), FlatStyle = FlatStyle.Flat, Width = 100, Height = 30, BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White };
            var btnCancelDest = new Button { Text = "Batal", Location = new Point(125, 200), FlatStyle = FlatStyle.Flat, Width = 100, Height = 30, BackColor = Color.Gray, ForeColor = Color.White };

            pnlDestForm.Controls.AddRange(new Control[] { lblDestTitle, txtDestName, txtDestAddress, chkDestActive, btnSaveDest, btnCancelDest });

            dgvDestinations = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            
            pnlDest.Controls.Add(dgvDestinations);
            pnlDest.Controls.Add(pnlDestForm);
            pnlDest.Controls.Add(pnlDestButtons);
            tabDestinations.Controls.Add(pnlDest);

            // ==========================================
            // TAB 3: PRODUK KOPI
            // ==========================================
            var tabProducts = new TabPage("Produk Kopi");
            var pnlProducts = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var pnlProdButtons = new Panel { Dock = DockStyle.Top, Height = 100 };
            
            cmbCoffeeType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(10, 10), Width = 150 };
            cmbCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(170, 10), Width = 150 };
            cmbOrigin = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(330, 10), Width = 150 };
            
            lblRoastLevel = new Label 
            { 
                Text = "Roast Level:", 
                Location = new Point(490, 12), 
                AutoSize = true,
                Visible = false
            };

            cmbRoastLevel = new ComboBox 
            { 
                DropDownStyle = ComboBoxStyle.DropDownList, 
                Location = new Point(580, 10), 
                Width = 150,
                Visible = false
            };

            var lblStock = new Label { Text = "Minimum Stok:", Location = new Point(740, 12), AutoSize = true };
            var numStock = new NumericUpDown { Location = new Point(850, 10), Width = 80, Maximum = 1000000 };
            
            var btnAddProduct = new Button { Text = "Tambah Produk", Location = new Point(10, 50), FlatStyle = FlatStyle.Flat, Width = 150, Height = 35, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            var btnDelProduct = new Button { Text = "Hapus Produk", Location = new Point(170, 50), FlatStyle = FlatStyle.Flat, Width = 150, Height = 35, BackColor = Color.FromArgb(222, 5, 0), ForeColor = Color.White };
            
            pnlProdButtons.Controls.AddRange(new Control[] 
            { 
                cmbCoffeeType, cmbCategory, cmbOrigin, lblRoastLevel, cmbRoastLevel, lblStock, numStock, btnAddProduct, btnDelProduct 
            });
            
            dgvProducts = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pnlProducts.Controls.Add(dgvProducts);
            pnlProducts.Controls.Add(pnlProdButtons);
            tabProducts.Controls.Add(pnlProducts);

            tabControl.TabPages.Add(tabSuppliers);
            tabControl.TabPages.Add(tabDestinations);
            tabControl.TabPages.Add(tabProducts);

            this.Controls.Add(tabControl);

            // ==========================================
            // EVENT HANDLERS
            // ==========================================
            this.Load += (s, e) => LoadDataRequested?.Invoke(this, EventArgs.Empty);
            
            // SUPPLIER EVENTS
            btnAddSupplier.Click += (s, e) => ShowSupplierForm(0);
            btnCancelSupplier.Click += (s, e) => pnlSupplierForm.Visible = false;
            chkShowInactiveSupplier.CheckedChanged += (s, e) => LoadDataRequested?.Invoke(this, EventArgs.Empty);
            
            dgvSuppliers.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                {
                    var row = dgvSuppliers.Rows[e.RowIndex];
                    int id = (int)row.Cells["SupplierId"].Value;
                    string name = row.Cells["CompanyName"].Value?.ToString() ?? "";
                    string address = row.Cells["Address"].Value?.ToString() ?? "";
                    string phone = row.Cells["Phone"].Value?.ToString() ?? "";
                    bool isActive = (bool)row.Cells["IsActive"].Value;
                    ShowSupplierForm(id, name, address, phone, isActive);
                }
            };

            btnSaveSupplier.Click += (s, e) => {
                SaveSupplierRequested?.Invoke(this, new Supplier { 
                    SupplierId = _editingSupplierId,
                    CompanyName = txtSupplierName.Text, 
                    Address = txtSupplierAddress.Text, 
                    Phone = txtSupplierPhone.Text,
                    IsActive = chkSupplierActive.Checked
                });
                pnlSupplierForm.Visible = false;
            };

            // DESTINATION EVENTS
            btnAddDest.Click += (s, e) => ShowDestinationForm(0);
            btnCancelDest.Click += (s, e) => pnlDestForm.Visible = false;
            chkShowInactiveDest.CheckedChanged += (s, e) => LoadDataRequested?.Invoke(this, EventArgs.Empty);

            dgvDestinations.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                {
                    var row = dgvDestinations.Rows[e.RowIndex];
                    int id = (int)row.Cells["DestinationId"].Value;
                    string name = row.Cells["DestinationName"].Value?.ToString() ?? "";
                    string address = row.Cells["Address"].Value?.ToString() ?? "";
                    bool isActive = (bool)row.Cells["IsActive"].Value;
                    ShowDestinationForm(id, name, address, isActive);
                }
            };

            btnSaveDest.Click += (s, e) => {
                SaveDestinationRequested?.Invoke(this, new Destination { 
                    DestinationId = _editingDestinationId,
                    DestinationName = txtDestName.Text, 
                    Address = txtDestAddress.Text,
                    IsActive = chkDestActive.Checked
                });
                pnlDestForm.Visible = false;
            };

            // PRODUCT EVENTS
            cmbCategory.SelectedIndexChanged += (s, e) => UpdateRoastLevelVisibility();
            
            btnAddProduct.Click += (s, e) => 
            { 
                if (cmbCoffeeType.SelectedValue == null || cmbCategory.SelectedValue == null || cmbOrigin.SelectedValue == null) 
                {
                    MessageBox.Show("Jenis kopi, kategori, dan origin wajib dipilih!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int cTypeId = (int)cmbCoffeeType.SelectedValue; 
                int catId = (int)cmbCategory.SelectedValue; 
                int originId = (int)cmbOrigin.SelectedValue; 
                int stock = (int)numStock.Value; 

                int? roastLevelId = null;

                if (cmbRoastLevel.Visible)
                {
                    if (cmbRoastLevel.SelectedValue == null)
                    {
                        MessageBox.Show("Roast Level wajib dipilih untuk kategori Roasted Bean!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    roastLevelId = (int)cmbRoastLevel.SelectedValue;
                }
    
                AddCoffeeProductRequested?.Invoke(this, (cTypeId, catId, originId, stock, roastLevelId)); 

                numStock.Value = 0;
                cmbRoastLevel.SelectedIndex = -1;
            };

            btnDelProduct.Click += (s, e) => {
                if (dgvProducts.SelectedRows.Count > 0)
                {
                    int id = (int)dgvProducts.SelectedRows[0].Cells["ProductId"].Value;
                    if (MessageBox.Show("Yakin hapus produk ini?", "Hapus", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        DeleteCoffeeProductRequested?.Invoke(this, id);
                }
            };
        }

        private void ShowSupplierForm(int id, string name = "", string address = "", string phone = "", bool isActive = true)
        {
            _editingSupplierId = id;
            txtSupplierName.Text = name;
            txtSupplierAddress.Text = address;
            txtSupplierPhone.Text = phone;
            chkSupplierActive.Checked = isActive;
            pnlSupplierForm.Visible = true;
        }

        private void ShowDestinationForm(int id, string name = "", string address = "", bool isActive = true)
        {
            _editingDestinationId = id;
            txtDestName.Text = name;
            txtDestAddress.Text = address;
            chkDestActive.Checked = isActive;
            pnlDestForm.Visible = true;
        }

        private void UpdateRoastLevelVisibility()
        {
            if (cmbCategory.SelectedItem is CoffeeCategory selectedCategory)
            {
                bool isRoastedBean = selectedCategory.CategoryName.ToLower().Contains("roasted bean");
                lblRoastLevel.Visible = isRoastedBean;
                cmbRoastLevel.Visible = isRoastedBean;
                if (!isRoastedBean) cmbRoastLevel.SelectedIndex = -1;
            }
        }

        public void DisplaySuppliers(object dataSource) => dgvSuppliers.DataSource = dataSource;
        public void DisplayDestinations(object dataSource) => dgvDestinations.DataSource = dataSource;
        public void DisplayCoffeeProducts(object dataSource) => dgvProducts.DataSource = dataSource;
        
        public void PopulateCoffeeTypes(object dataSource)
        {
            cmbCoffeeType.DataSource = dataSource;
            cmbCoffeeType.DisplayMember = "CoffeeName";
            cmbCoffeeType.ValueMember = "CoffeeId";
        }
        
        public void PopulateCategories(object dataSource)
        {
            cmbCategory.DataSource = dataSource;
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryId";
        }

        public void PopulateOrigins(object dataSource)
        {
            cmbOrigin.DataSource = dataSource;
            cmbOrigin.DisplayMember = "OriginName";
            cmbOrigin.ValueMember = "OriginId";
        }

        public void PopulateRoastLevels(object dataSource)
        {
            cmbRoastLevel.DataSource = dataSource;
            cmbRoastLevel.DisplayMember = "RoastLevelName";
            cmbRoastLevel.ValueMember = "RoastLevelId";
            cmbRoastLevel.SelectedIndex = -1;
        }

        public void ShowMessage(string message, bool isError = false)
        {
            MessageBox.Show(message, isError ? "Error" : "Info", MessageBoxButtons.OK, isError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }
    }
}
