using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Repositories;

namespace CoffeeWMS.Views
{
    public class PengirimanView : UserControl
    {
        private ComboBox cmbJenisKopi;
        private TextBox txtJumlah;
        private TextBox txtCustomer;
        private Button btnSimpan;
        private DataGridView dgvPengiriman;
        private TransaksiRepository repo = new TransaksiRepository();

        public PengirimanView()
        {
            BuildUI();
            RefreshGrid();
        }

        private void BuildUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Input Pengiriman Kopi", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(30, 20) };
            
            Label lblJenis = new Label { Text = "Jenis Kopi:", Location = new Point(35, 70), AutoSize = true };
            cmbJenisKopi = new ComboBox { Location = new Point(35, 95), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbJenisKopi.Items.AddRange(new string[] { "Arabika", "Robusta", "Liberika", "Excelsa" });
            cmbJenisKopi.SelectedIndex = 0;

            Label lblCustomer = new Label { Text = "Nama Customer / Tujuan:", Location = new Point(205, 70), AutoSize = true };
            txtCustomer = new TextBox { Location = new Point(205, 95), Width = 150 };

            Label lblJumlah = new Label { Text = "Jumlah (Kg):", Location = new Point(375, 70), AutoSize = true };
            txtJumlah = new TextBox { Location = new Point(375, 95), Width = 100 };

            btnSimpan = new Button { Text = "Simpan Pengiriman", Location = new Point(35, 135), Width = 150, Height = 30, BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSimpan.Click += BtnSimpan_Click;

            dgvPengiriman = new DataGridView { Location = new Point(35, 185), Width = 600, Height = 250, BackgroundColor = Color.FromArgb(240, 240, 240), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false };

            this.Controls.AddRange(new Control[] { lblTitle, lblJenis, cmbJenisKopi, lblCustomer, txtCustomer, lblJumlah, txtJumlah, btnSimpan, dgvPengiriman });
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomer.Text) || !decimal.TryParse(txtJumlah.Text, out decimal jumlah) || jumlah <= 0)
            {
                MessageBox.Show("Isi nama customer dan jumlah data dengan benar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string jenis = cmbJenisKopi.SelectedItem.ToString();
            bool sukses = repo.InsertPengiriman(DateTime.Now, txtCustomer.Text, jenis, jumlah);

            if (sukses)
            {
                MessageBox.Show("Pengiriman berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtJumlah.Clear(); txtCustomer.Clear();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show("Koneksi DB gagal. Data dialihkan ke simulasi layar.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvPengiriman.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), jenis, jumlah);
                txtJumlah.Clear(); txtCustomer.Clear();
            }
        }

        private void RefreshGrid()
        {
            var dt = repo.GetDataPengiriman();
            if (dt != null && dt.Rows.Count > 0) dgvPengiriman.DataSource = dt;
            else if (dgvPengiriman.Columns.Count == 0)
            {
                dgvPengiriman.Columns.Add("Tanggal", "Tanggal");
                dgvPengiriman.Columns.Add("JenisKopi", "Jenis Kopi");
                dgvPengiriman.Columns.Add("Jumlah", "Jumlah (Kg)");
            }
        }
    }
}