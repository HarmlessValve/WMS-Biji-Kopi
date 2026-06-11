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
    public class PengirimanView : UserControl
    {
        private ComboBox cmbJenisKopi;
        private ComboBox cmbDestinasi; // Mengubah TextBox menjadi ComboBox agar sesuai relasi ID
        private TextBox txtJumlah;
        private Button btnSimpan;
        private DataGridView dgvPengiriman;
        private TransaksiRepository repo = new TransaksiRepository();

        public PengirimanView()
        {
            BuildUI();
            LoadComboBoxData(); // Mengisi data master dari DB
            RefreshGrid();
        }

        private void BuildUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Input Pengiriman Kopi", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(30, 20) };
            
            Label lblJenis = new Label { Text = "Jenis Kopi:", Location = new Point(35, 70), AutoSize = true };
            cmbJenisKopi = new ComboBox { Location = new Point(35, 95), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblDestinasi = new Label { Text = "Destinasi / Tujuan:", Location = new Point(205, 70), AutoSize = true };
            cmbDestinasi = new ComboBox { Location = new Point(205, 95), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblJumlah = new Label { Text = "Jumlah (Kg):", Location = new Point(375, 70), AutoSize = true };
            txtJumlah = new TextBox { Location = new Point(375, 95), Width = 100 };

            btnSimpan = new Button { Text = "Simpan Pengiriman", Location = new Point(35, 135), Width = 150, Height = 30, BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSimpan.Click += BtnSimpan_Click;

            dgvPengiriman = new DataGridView { Location = new Point(35, 185), Width = 600, Height = 250, BackgroundColor = Color.FromArgb(240, 240, 240), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false };

            this.Controls.AddRange(new Control[] { lblTitle, lblJenis, cmbJenisKopi, lblDestinasi, cmbDestinasi, lblJumlah, txtJumlah, btnSimpan, dgvPengiriman });
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

                    // 2. Load Data Destinasi
                    string qDest = "SELECT destination_id, destination_name FROM destinations WHERE is_active = true";
                    DataTable dtDest = new DataTable();
                    using (var da = new NpgsqlDataAdapter(qDest, conn)) { da.Fill(dtDest); }
                    
                    cmbDestinasi.DataSource = dtDest;
                    cmbDestinasi.DisplayMember = "destination_name";
                    cmbDestinasi.ValueMember = "destination_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data master ComboBox: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSimpan_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtJumlah.Text, out int jumlah) || jumlah <= 0)
            {
                MessageBox.Show("Isi jumlah data dengan benar (angka bulat > 0)!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int destinationId = (cmbDestinasi.SelectedValue as int?) ?? 0;
            int coffeeId = (cmbJenisKopi.SelectedValue as int?) ?? 0;
            int petugasId = Session.CurrentUser?.UserId ?? 1;

            if (destinationId == 0 || coffeeId == 0)
            {
                MessageBox.Show("Data Destinasi atau Jenis Kopi belum valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Eksekusi repository dengan tipe data integer (Sesuai modifikasi DB kemarin)
            bool sukses = repo.InsertPengiriman(destinationId, coffeeId, jumlah, petugasId);

            if (sukses)
            {
                MessageBox.Show("Pengiriman berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtJumlah.Clear();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show("Koneksi DB gagal. Data dialihkan ke simulasi layar.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SimulasiLokal(cmbJenisKopi.Text ?? "Unknown", jumlah);
                txtJumlah.Clear();
            }
        }

        private void RefreshGrid()
        {
            var dt = repo.GetDataPengiriman();
            if (dt != null && dt.Rows.Count > 0) 
            {
                dgvPengiriman.DataSource = dt;
            }
            else if (dgvPengiriman.Columns.Count == 0)
            {
                dgvPengiriman.Columns.Add("Tanggal", "Tanggal");
                dgvPengiriman.Columns.Add("Destinasi", "Destinasi");
                dgvPengiriman.Columns.Add("JenisKopi", "Jenis Kopi");
                dgvPengiriman.Columns.Add("Jumlah", "Jumlah (Kg)");
            }
        }

        private void SimulasiLokal(string jenis, int jumlah)
        {
            if (dgvPengiriman.DataSource != null)
            {
                dgvPengiriman.DataSource = null;
                dgvPengiriman.Columns.Clear();
                dgvPengiriman.Columns.Add("Tanggal", "Tanggal");
                dgvPengiriman.Columns.Add("Destinasi", "Destinasi");
                dgvPengiriman.Columns.Add("JenisKopi", "Jenis Kopi");
                dgvPengiriman.Columns.Add("Jumlah", "Jumlah (Kg)");
            }
            dgvPengiriman.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), cmbDestinasi.Text, jenis, jumlah);
        }
    }
}