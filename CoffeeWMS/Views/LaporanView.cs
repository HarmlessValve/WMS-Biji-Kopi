using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Data;

namespace CoffeeWMS.Views
{
    public partial class LaporanView : UserControl
    {
        //Deklarasi Komponen UI secara manual
        private Label lblPenerimaan;
        private DataGridView dgvLaporanMasuk;
        private Label lblPengiriman;
        private DataGridView dgvLaporanKeluar;
        private Button btnRefresh;

        public LaporanView()
        {
        
            
            // Panggil fungsi untuk menggambar UI secara manual
            SetupUIManual(); 
            
            this.Load += LaporanView_Load;
        }

        //Fungsi untuk mengatur posisi dan ukuran UI (Pengganti Mode Design)
        private void SetupUIManual()
        {
            this.BackColor = Color.White;
            this.Size = new Size(800, 600); // Sesuaikan dengan ukuran tab kamu

            // Label Penerimaan
            lblPenerimaan = new Label();
            lblPenerimaan.Text = "Riwayat Penerimaan Kopi (Barang Masuk)";
            lblPenerimaan.Location = new Point(20, 20);
            lblPenerimaan.AutoSize = true;
            lblPenerimaan.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            // Tabel Penerimaan
            dgvLaporanMasuk = new DataGridView();
            dgvLaporanMasuk.Name = "dgvLaporanMasuk";
            dgvLaporanMasuk.Location = new Point(20, 50);
            dgvLaporanMasuk.Size = new Size(740, 200);
            dgvLaporanMasuk.ReadOnly = true;
            dgvLaporanMasuk.AllowUserToAddRows = false;
            dgvLaporanMasuk.BackgroundColor = Color.WhiteSmoke;

            // Label Pengiriman
            lblPengiriman = new Label();
            lblPengiriman.Text = "Riwayat Pengiriman Kopi (Barang Keluar)";
            lblPengiriman.Location = new Point(20, 270);
            lblPengiriman.AutoSize = true;
            lblPengiriman.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            // Tabel Pengiriman
            dgvLaporanKeluar = new DataGridView();
            dgvLaporanKeluar.Name = "dgvLaporanKeluar";
            dgvLaporanKeluar.Location = new Point(20, 300);
            dgvLaporanKeluar.Size = new Size(740, 200);
            dgvLaporanKeluar.ReadOnly = true;
            dgvLaporanKeluar.AllowUserToAddRows = false;
            dgvLaporanKeluar.BackgroundColor = Color.WhiteSmoke;

            // Tombol Refresh
            btnRefresh = new Button();
            btnRefresh.Text = "Refresh Laporan";
            btnRefresh.Location = new Point(20, 520);
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.BackColor = Color.LightBlue;
            btnRefresh.Click += BtnRefresh_Click;

            // Memasukkan semua elemen yang dibuat ke dalam layar
            this.Controls.Add(lblPenerimaan);
            this.Controls.Add(dgvLaporanMasuk);
            this.Controls.Add(lblPengiriman);
            this.Controls.Add(dgvLaporanKeluar);
            this.Controls.Add(btnRefresh);
        }

        //Logika Backend untuk memuat data saat form dibuka
        private void LaporanView_Load(object sender, EventArgs e)
        {
            LoadLaporanMasuk();
            LoadLaporanKeluar();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadLaporanMasuk();
            LoadLaporanKeluar();
        }

        //Logika menarik data dari database PostgreSQL
        private void LoadLaporanMasuk()
        {
            try
            {
                string query = "SELECT * FROM vw_incoming_transactions;";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvLaporanMasuk.DataSource = dt;
                dgvLaporanMasuk.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat laporan penerimaan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLaporanKeluar()
        {
            try
            {
                string query = "SELECT * FROM vw_outgoing_transactions;";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvLaporanKeluar.DataSource = dt;
                dgvLaporanKeluar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat laporan pengiriman: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}