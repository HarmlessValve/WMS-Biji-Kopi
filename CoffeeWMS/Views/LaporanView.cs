using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Controllers;
using CoffeeWMS.Models;

namespace CoffeeWMS.Views
{
    public class LaporanView : UserControl
    {
        private Label lblTitle;
        private Label lblSubtitle;

        private DateTimePicker dtpMulai;
        private DateTimePicker dtpSelesai;
        private ComboBox cmbJenisLaporan;

        private Button btnTampilkan;
        private Button btnRefresh;

        private Label lblTotalPenerimaan;
        private Label lblTotalPengiriman;
        private Label lblTotalStok;
        private Label lblStokRendah;

        private DataGridView dgvLaporan;

        private readonly LaporanController _controller = new LaporanController();

        public LaporanView()
        {
            InitializeComponent();
            DateTime mulai   = dtpMulai.Value.Date;
            DateTime selesai = dtpSelesai.Value.Date;
            LoadSummary(mulai, selesai);
            LoadLaporan();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.Padding = new Padding(25);

            lblTitle = new Label
            {
                Text      = "Laporan Gudang Kopi",
                Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize  = true,
                Location  = new Point(25, 20)
            };

            lblSubtitle = new Label
            {
                Text      = "Rekap penerimaan, pengiriman, stok, stok rendah, dan log aktivitas gudang.",
                Font      = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize  = true,
                Location  = new Point(28, 55)
            };

            Label lblMulai = new Label
            {
                Text     = "Dari Tanggal",
                Font     = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 95)
            };

            dtpMulai = new DateTimePicker
            {
                Format   = DateTimePickerFormat.Short,
                Width    = 130,
                Location = new Point(30, 118)
            };

            Label lblSelesai = new Label
            {
                Text     = "Sampai Tanggal",
                Font     = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(180, 95)
            };

            dtpSelesai = new DateTimePicker
            {
                Format   = DateTimePickerFormat.Short,
                Width    = 130,
                Location = new Point(180, 118)
            };

            Label lblJenis = new Label
            {
                Text     = "Jenis Laporan",
                Font     = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(330, 95)
            };

            cmbJenisLaporan = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width         = 190,
                Location      = new Point(330, 118)
            };

            cmbJenisLaporan.Items.AddRange(new object[]
            {
                "Penerimaan",
                "Pengiriman",
                "Stok",
                "Stok Rendah",
                "Log Aktivitas"
            });

            cmbJenisLaporan.SelectedIndex = 0;

            btnTampilkan = new Button
            {
                Text      = "Tampilkan",
                Width     = 110,
                Height    = 32,
                Location  = new Point(540, 114),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnTampilkan.FlatAppearance.BorderSize = 0;
            btnTampilkan.Click += BtnTampilkan_Click;

            btnRefresh = new Button
            {
                Text      = "Refresh",
                Width     = 100,
                Height    = 32,
                Location  = new Point(660, 114),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            lblTotalPenerimaan = CreateSummaryCard("Penerimaan", "0 transaksi", 30, 165);
            lblTotalPengiriman = CreateSummaryCard("Pengiriman", "0 transaksi", 230, 165);
            lblTotalStok       = CreateSummaryCard("Total Stok", "0 kg", 430, 165);
            lblStokRendah      = CreateSummaryCard("Stok Rendah", "0 item", 630, 165);

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 255 };
            pnlTop.Controls.AddRange(new Control[]
            {
                lblTitle, lblSubtitle,
                lblMulai, dtpMulai,
                lblSelesai, dtpSelesai,
                lblJenis, cmbJenisLaporan,
                btnTampilkan, btnRefresh,
                lblTotalPenerimaan, lblTotalPengiriman, lblTotalStok, lblStokRendah
            });

            Panel pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30, 0, 30, 30) };
            dgvLaporan = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                BackgroundColor       = Color.White,
                BorderStyle           = BorderStyle.FixedSingle,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly              = true,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible     = false
            };
            pnlGrid.Controls.Add(dgvLaporan);

            dgvLaporan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            dgvLaporan.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLaporan.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvLaporan.EnableHeadersVisualStyles               = false;

            dgvLaporan.DefaultCellStyle.Font                  = new Font("Segoe UI", 9);
            dgvLaporan.DefaultCellStyle.SelectionBackColor     = Color.FromArgb(52, 152, 219);
            dgvLaporan.DefaultCellStyle.SelectionForeColor     = Color.White;

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlTop);
        }

        private Label CreateSummaryCard(string title, string value, int x, int y)
        {
            return new Label
            {
                Text      = $"{title}\n{value}",
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(41, 53, 65),
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(x, y),
                Size      = new Size(170, 65)
            };
        }

        // =====================================================================
        // EVENT HANDLERS
        // =====================================================================

        private void BtnTampilkan_Click(object sender, EventArgs e)
        {
            DateTime mulai   = dtpMulai.Value.Date;
            DateTime selesai = dtpSelesai.Value.Date;
            LoadSummary(mulai, selesai);
            LoadLaporan();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            dtpMulai.Value  = DateTime.Now.Date;
            dtpSelesai.Value = DateTime.Now.Date;
            cmbJenisLaporan.SelectedIndex = 0;
            LoadSummary(DateTime.Now.Date, DateTime.Now.Date);
            LoadLaporan();
        }

        // =====================================================================
        // LOAD DATA — Semua memanggil _controller, tidak ada SQL di sini
        // =====================================================================

        private void LoadSummary(DateTime mulai, DateTime selesai)
        {
            try
            {
                int totalIncoming = _controller.GetLaporanPenerimaan(mulai, selesai).Count;
                int totalOutgoing = _controller.GetLaporanPengiriman(mulai, selesai).Count;
                int totalStok     = _controller.GetTotalStok();
                int totalLowStock = _controller.GetLaporanStokRendah().Rows.Count;

                lblTotalPenerimaan.Text = $"Penerimaan Total\n{totalIncoming} transaksi";
                lblTotalPengiriman.Text = $"Pengiriman Total\n{totalOutgoing} transaksi";
                lblTotalStok.Text       = $"Total Stok\n{totalStok} kg";
                lblStokRendah.Text      = $"Stok Rendah\n{totalLowStock} item";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat summary laporan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLaporan()
        {
            try
            {
                DateTime mulai   = dtpMulai.Value.Date;
                DateTime selesai = dtpSelesai.Value.Date;

                if (mulai > selesai)
                {
                    MessageBox.Show("Tanggal mulai tidak boleh lebih besar dari tanggal selesai.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string jenisLaporan = cmbJenisLaporan.SelectedItem?.ToString() ?? "";

                // Untuk Penerimaan & Pengiriman: map List<Model> ke DataTable agar tetap konsisten dengan grid binding
                switch (jenisLaporan)
                {
                    case "Penerimaan":
                        dgvLaporan.DataSource = _controller.GetLaporanPenerimaan(mulai, selesai);
                        break;
                    case "Pengiriman":
                        dgvLaporan.DataSource = _controller.GetLaporanPengiriman(mulai, selesai);
                        break;
                    case "Stok":
                        dgvLaporan.DataSource = _controller.GetLaporanStok();
                        break;
                    case "Stok Rendah":
                        dgvLaporan.DataSource = _controller.GetLaporanStokRendah();
                        break;
                    case "Log Aktivitas":
                        dgvLaporan.DataSource = _controller.GetLogAktivitas(mulai, selesai);
                        break;
                }

                ApplyGridStyle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyGridStyle()
        {
            if (dgvLaporan.Columns.Count == 0) return;

            dgvLaporan.ClearSelection();

            foreach (DataGridViewColumn col in dgvLaporan.Columns)
                col.SortMode = DataGridViewColumnSortMode.Automatic;

            // Warna baris berdasarkan kolom "Status" jika ada (untuk Stok)
            if (dgvLaporan.Columns.Contains("Status"))
            {
                foreach (DataGridViewRow row in dgvLaporan.Rows)
                {
                    var statusValue = row.Cells["Status"].Value?.ToString();
                    if (statusValue == "LOW")
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                    }
                    else if (statusValue == "SAFE")
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(27, 94, 32);
                    }
                }
            }
        }
    }
}