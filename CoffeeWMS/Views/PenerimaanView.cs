using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Repositories;

namespace CoffeeWMS.Views
{
    public class PenerimaanView : UserControl
    {
        private ComboBox cmbJenisKopi;
        private TextBox txtJumlah;
        private Button btnSimpan;
        private DataGridView dgvPenerimaan;
        private TransaksiRepository repo = new TransaksiRepository();

        public PenerimaanView()
        {
            BuildUI();
            RefreshGrid();
        }

        private void BuildUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Input Penerimaan Kopi", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(30, 20) };
            
            Label lblJenis = new Label { Text = "Jenis Kopi:", Location = new Point(35, 70), AutoSize = true };
            cmbJenisKopi = new ComboBox { Location = new Point(35, 95), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList }; // <-- Mengunci input teks bebas
            cmbJenisKopi.Items.AddRange(new string[] { "Arabika", "Robusta", "Liberika", "Excelsa" });
            cmbJenisKopi.SelectedIndex = 0;

            Label lblJumlah = new Label { Text = "Jumlah (Kg):", Location = new Point(260, 70), AutoSize = true };
            txtJumlah = new TextBox { Location = new Point(260, 95), Width = 150 };

            btnSimpan = new Button { Text = "Simpan Data", Location = new Point(35, 135), Width = 120, Height = 30, BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSimpan.Click += BtnSimpan_Click;

            dgvPenerimaan = new DataGridView { Location = new Point(35, 185), Width = 600, Height = 250, BackgroundColor = Color.FromArgb(240, 240, 240), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false };

            this.Controls.AddRange(new Control[] { lblTitle, lblJenis, cmbJenisKopi, lblJumlah, txtJumlah, btnSimpan, dgvPenerimaan });
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            // Validasi Input: Harus berupa angka dan tidak boleh minus/nol
            if (!decimal.TryParse(txtJumlah.Text, out decimal jumlah) || jumlah <= 0)
            {
                MessageBox.Show("Masukkan jumlah yang valid (angka di atas 0)!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string jenis = cmbJenisKopi.SelectedItem.ToString();
            
            // Simpan ke database melalui repository
            bool sukses = repo.InsertPenerimaan(DateTime.Now, "Supplier Utama", jenis, "BATCH-" + DateTime.Now.ToString("yyyyMMdd"), jumlah);

            if (sukses)
            {
                MessageBox.Show($"Data {jenis} sebanyak {jumlah} Kg berhasil disimpan ke Database!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtJumlah.Clear();
                RefreshGrid();
            }
            else
            {
                // Fitur penyelamat demo: Jika tabel di pgAdmin belum dibuat, data akan tetap muncul di aplikasi via simulasi lokal
                MessageBox.Show("Koneksi DB gagal/Tabel belum siap. Data dialihkan ke simulasi layar.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SimulasiLokal(jenis, jumlah);
                txtJumlah.Clear();
            }
        }

        private void RefreshGrid()
        {
            var dt = repo.GetDataPenerimaan();
            if (dt != null && dt.Rows.Count > 0)
            {
                dgvPenerimaan.DataSource = dt;
            }
            else if (dgvPenerimaan.Columns.Count == 0)
            {
                dgvPenerimaan.Columns.Add("Tanggal", "Tanggal");
                dgvPenerimaan.Columns.Add("JenisKopi", "Jenis Kopi");
                dgvPenerimaan.Columns.Add("Jumlah", "Jumlah (Kg)");
            }
        }

        private void SimulasiLokal(string jenis, decimal jumlah)
        {
            dgvPenerimaan.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), jenis, jumlah);
        }
    }
}