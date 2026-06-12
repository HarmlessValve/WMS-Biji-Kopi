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

        public event EventHandler LoadDataRequested;
        public event EventHandler<Supplier> AddSupplierRequested;
        public event EventHandler<int> DeleteSupplierRequested;
        public event EventHandler<Destination> AddDestinationRequested;
        public event EventHandler<int> DeleteDestinationRequested;
        public event EventHandler<(int coffeeId, int categoryId, int originId, int minimumStock, int? roastLevelId)> AddCoffeeProductRequested;
        public event EventHandler<int> DeleteCoffeeProductRequested;
        public event EventHandler<CoffeeOrigin> AddCoffeeOriginRequested;

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

            // TAB 1: SUPPLIERS
            var tabSuppliers = new TabPage("Supplier");
            var pnlSuppliers = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var pnlSupplierButtons = new Panel { Dock = DockStyle.Top, Height = 100 };
            var txtSupplierName = new TextBox { PlaceholderText = "Nama Supplier", Location = new Point(10, 10), Width = 200 };
            var txtSupplierAddress = new TextBox { PlaceholderText = "Alamat", Location = new Point(220, 10), Width = 200 };
            var txtSupplierPhone = new TextBox { PlaceholderText = "Telepon", Location = new Point(430, 10), Width = 200 };
            
            var btnAddSupplier = new Button { Text = "Tambah Supplier", Location = new Point(10, 50), FlatStyle = FlatStyle.Flat, Width = 150, Height = 35, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            var btnDelSupplier = new Button { Text = "Hapus Supplier", Location = new Point(170, 50), FlatStyle = FlatStyle.Flat, Width = 150, Height = 35, BackColor = Color.FromArgb(222, 5, 0), ForeColor = Color.White };
            
            pnlSupplierButtons.Controls.AddRange(new Control[] { txtSupplierName, txtSupplierAddress, txtSupplierPhone, btnAddSupplier, btnDelSupplier });
            dgvSuppliers = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pnlSuppliers.Controls.Add(dgvSuppliers);
            pnlSuppliers.Controls.Add(pnlSupplierButtons);
            tabSuppliers.Controls.Add(pnlSuppliers);

            // TAB 2: DESTINATIONS
            var tabDestinations = new TabPage("Destinasi");
            var pnlDest = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var pnlDestButtons = new Panel { Dock = DockStyle.Top, Height = 100 };
            var txtDestName = new TextBox { PlaceholderText = "Nama Destinasi", Location = new Point(10, 10), Width = 200 };
            var txtDestAddress = new TextBox { PlaceholderText = "Alamat", Location = new Point(220, 10), Width = 300 };
            
            var btnAddDest = new Button { Text = "Tambah Destinasi", Location = new Point(10, 50), FlatStyle = FlatStyle.Flat, Width = 150, Height = 35, BackColor = Color.FromArgb(0, 170, 100), ForeColor = Color.White };
            var btnDelDest = new Button { Text = "Hapus Destinasi", Location = new Point(170, 50), FlatStyle = FlatStyle.Flat, Width = 150, Height = 35, BackColor = Color.FromArgb(222, 5, 0), ForeColor = Color.White };
            
            pnlDestButtons.Controls.AddRange(new Control[] { txtDestName, txtDestAddress, btnAddDest, btnDelDest });
            dgvDestinations = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pnlDest.Controls.Add(dgvDestinations);
            pnlDest.Controls.Add(pnlDestButtons);
            tabDestinations.Controls.Add(pnlDest);

            // TAB 3: PRODUK KOPI
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
                cmbCoffeeType, 
                cmbCategory, 
                cmbOrigin, 
                lblRoastLevel,
                cmbRoastLevel,
                lblStock, 
                numStock, 
                btnAddProduct, 
                btnDelProduct 
            });
            
            dgvProducts = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pnlProducts.Controls.Add(dgvProducts);
            pnlProducts.Controls.Add(pnlProdButtons);
            tabProducts.Controls.Add(pnlProducts);

            tabControl.TabPages.Add(tabSuppliers);
            tabControl.TabPages.Add(tabDestinations);
            tabControl.TabPages.Add(tabProducts);

            this.Controls.Add(tabControl);

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
            
            cmbCategory.SelectedIndexChanged += (s, e) =>
            {
                UpdateRoastLevelVisibility();
            };
            
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

        private void UpdateRoastLevelVisibility()
        {
            if (cmbCategory.SelectedItem is CoffeeCategory selectedCategory)
            {
                bool isRoastedBean = selectedCategory.CategoryName
                    .ToLower()
                    .Contains("roasted bean");

                lblRoastLevel.Visible = isRoastedBean;
                cmbRoastLevel.Visible = isRoastedBean;

                if (!isRoastedBean)
                {
                    cmbRoastLevel.SelectedIndex = -1;
                }
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
