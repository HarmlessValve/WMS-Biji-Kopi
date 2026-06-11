using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Controllers;
using CoffeeWMS.Views.Interfaces; // Wajib ditambahkan untuk mengambil IIncomingView

namespace CoffeeWMS.Views
{
    // View sekarang mewarisi IIncomingView
    public class PenerimaanView : UserControl, IIncomingView
    {
        private ComboBox cmbJenisKopi;
        private ComboBox cmbSupplier;
        private TextBox txtJumlah;
        private Button btnSimpan;
        private DataGridView dgvPenerimaan;

        private IncomingController _controller;

        // --- IMPLEMENTASI EVENT (Sinyal HT) ---
        public event EventHandler? LoadDataRequested;
        public event EventHandler<Tuple<int, int, int>>? AddIncomingRequested;

        public PenerimaanView()
        {
            BuildUI();
            
            // Controller dipasang dan View ini diserahkan kepadanya
            _controller = new IncomingController(this); 
            
            // Tembakkan sinyal pertama agar Controller memuat data awal
            LoadDataRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BuildUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Input Penerimaan Kopi", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(30, 20) };
            
            Label lblSupplier = new Label { Text = "Supplier:", Location = new Point(35, 70), AutoSize = true };
            cmbSupplier = new ComboBox { Location = new Point(35, 95), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblJenis = new Label { Text = "Jenis Kopi:", Location = new Point(235, 70), AutoSize = true };
            cmbJenisKopi = new ComboBox { Location = new Point(235, 95), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblJumlah = new Label { Text = "Jumlah (Kg):", Location = new Point(435, 70), AutoSize = true };
            txtJumlah = new TextBox { Location = new Point(435, 95), Width = 100 };

            btnSimpan = new Button { Text = "Simpan Data", Location = new Point(35, 135), Width = 120, Height = 30, BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSimpan.Click += BtnSimpan_Click;

            dgvPenerimaan = new DataGridView { Location = new Point(35, 185), Width = 600, Height = 250, BackgroundColor = Color.FromArgb(240, 240, 240), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false };

            this.Controls.AddRange(new Control[] { lblTitle, lblSupplier, cmbSupplier, lblJenis, cmbJenisKopi, lblJumlah, txtJumlah, btnSimpan, dgvPenerimaan });
        }

        // --- IMPLEMENTASI FUNGSI DARI INTERFACE ---
        public void ShowMessage(string message, bool isError = false)
        {
            MessageBoxIcon icon = isError ? MessageBoxIcon.Error : MessageBoxIcon.Information;
            MessageBox.Show(message, isError ? "Error" : "Info", MessageBoxButtons.OK, icon);
        }

        public void PopulateSupplierCombobox(DataTable data)
        {
            cmbSupplier.DataSource = data;
            cmbSupplier.DisplayMember = "company_name";
            cmbSupplier.ValueMember = "supplier_id";
        }

        public void PopulateCoffeeCombobox(DataTable data)
        {
            cmbJenisKopi.DataSource = data;
            cmbJenisKopi.DisplayMember = "coffee_name";
            cmbJenisKopi.ValueMember = "coffee_id";
        }

        public void DisplayTransactions(DataTable data)
        {
            if (data != null && data.Rows.Count > 0)
            {
                dgvPenerimaan.DataSource = data;
            }
            else if (dgvPenerimaan.Columns.Count == 0)
            {
                dgvPenerimaan.Columns.Add("Tanggal", "Tanggal");
                dgvPenerimaan.Columns.Add("Supplier", "Supplier");
                dgvPenerimaan.Columns.Add("JenisKopi", "Jenis Kopi");
                dgvPenerimaan.Columns.Add("Jumlah", "Jumlah (Kg)");
            }
        }

        // --- EVENT HANDLER TOMBOL UI ---
        private void BtnSimpan_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtJumlah.Text, out int jumlah) || jumlah <= 0)
            {
                ShowMessage("Masukkan jumlah yang valid (angka bulat di atas 0)!", true);
                return;
            }

            int supplierId = (cmbSupplier.SelectedValue as int?) ?? 0;
            int coffeeId = (cmbJenisKopi.SelectedValue as int?) ?? 0;

            if (supplierId == 0 || coffeeId == 0)
            {
                ShowMessage("Data Supplier atau Jenis Kopi belum dipilih dengan benar!", true);
                return;
            }

            // Memicu event untuk mengirim Tuple (SupplierId, CoffeeId, Jumlah) ke Controller
            AddIncomingRequested?.Invoke(this, new Tuple<int, int, int>(supplierId, coffeeId, jumlah));
            
            txtJumlah.Clear();
        }
    }
}