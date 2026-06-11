using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Models;
using CoffeeWMS.Repositories;
using Npgsql;
using CoffeeWMS.Data;

namespace CoffeeWMS.Views
{
    public class PenerimaanView : UserControl
    {
        private ComboBox cmbJenisKopi;
        private ComboBox cmbSupplier; // Ditambahkan agar supplier tidak di-hardcode teks lagi
        private TextBox txtJumlah;
        private Button btnSimpan;
        private DataGridView dgvPenerimaan;
        private TransaksiRepository repo = new TransaksiRepository();

        public PenerimaanView()
        {
            BuildUI();
            LoadComboBoxData(); // Mengisi data master dari DB ke ComboBox
            RefreshGrid();
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

        private void LoadComboBoxData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    // 1. Load Data Kopi
                    string qKopi = "SELECT coffee_id, coffee_name FROM coffee_types WHERE is_active = true";
                    DataTable dtKopi = new DataTable();
                    using (var da = new NpgsqlDataAdapter(qKopi, conn)) { da.Fill(dtKopi); }
                    
                    cmbJenisKopi.DataSource = dtKopi;
                    cmbJenisKopi.DisplayMember = "coffee_name";
                    cmbJenisKopi.ValueMember = "coffee_id";

                    // 2. Load Data Supplier
                    string qSupplier = "SELECT supplier_id, company_name FROM suppliers WHERE is_active = true";
                    DataTable dtSupplier = new DataTable();
                    using (var da = new NpgsqlDataAdapter(qSupplier, conn)) { da.Fill(dtSupplier); }
                    
                    cmbSupplier.DataSource = dtSupplier;
                    cmbSupplier.DisplayMember = "company_name";
                    cmbSupplier.ValueMember = "supplier_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data master ComboBox: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSimpan_Click(object? sender, EventArgs e)
        {
            // Validasi input angka bulat (int) sesuai tipe data db
            if (!int.TryParse(txtJumlah.Text, out int jumlah) || jumlah <= 0)
            {
                MessageBox.Show("Masukkan jumlah yang valid (angka bulat di atas 0)!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ambil ID (ValueMember) bukan Teks bebas
            int supplierId = (cmbSupplier.SelectedValue as int?) ?? 0;
            int coffeeId = (cmbJenisKopi.SelectedValue as int?) ?? 0;
            int petugasId = Session.CurrentUser?.UserId ?? 1;

            if (supplierId == 0 || coffeeId == 0)
            {
                MessageBox.Show("Data Supplier atau Jenis Kopi belum dipilih dengan benar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kirim 4 argumen int sesuai dengan repository baru
            bool sukses = repo.InsertPenerimaan(supplierId, coffeeId, jumlah, petugasId);

            if (sukses)
            {
                MessageBox.Show("Data penerimaan kopi berhasil disimpan ke Database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtJumlah.Clear();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show("Koneksi DB gagal/Tabel belum siap. Data dialihkan ke simulasi layar.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SimulasiLokal(cmbJenisKopi.Text ?? "Unknown", jumlah);
                txtJumlah.Clear();
            }
        }

        private void RefreshGrid()
        {
            DataTable dt = repo.GetDataPenerimaan();
            if (dt != null && dt.Rows.Count > 0)
            {
                dgvPenerimaan.DataSource = dt;
            }
            else if (dgvPenerimaan.Columns.Count == 0)
            {
                dgvPenerimaan.Columns.Add("Tanggal", "Tanggal");
                dgvPenerimaan.Columns.Add("Supplier", "Supplier");
                dgvPenerimaan.Columns.Add("JenisKopi", "Jenis Kopi");
                dgvPenerimaan.Columns.Add("Jumlah", "Jumlah (Kg)");
            }
        }

        private void SimulasiLokal(string jenis, int jumlah)
        {
            if (dgvPenerimaan.DataSource != null)
            {
                dgvPenerimaan.DataSource = null;
                dgvPenerimaan.Columns.Clear();
                dgvPenerimaan.Columns.Add("Tanggal", "Tanggal");
                dgvPenerimaan.Columns.Add("Supplier", "Supplier");
                dgvPenerimaan.Columns.Add("JenisKopi", "Jenis Kopi");
                dgvPenerimaan.Columns.Add("Jumlah", "Jumlah (Kg)");
            }
            dgvPenerimaan.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), cmbSupplier.Text, jenis, jumlah);
        }
    }
}