using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Repositories;

namespace CoffeeWMS.Views
{
    public class LaporanView : UserControl
    {
        private readonly LaporanRepository _repository = new LaporanRepository();

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

        public LaporanView()
        {
            InitializeComponent();
            DateTime mulai = dtpMulai.Value.Date;
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
                Text = "Laporan Gudang Kopi",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(25, 20)
            };

            lblSubtitle = new Label
            {
                Text = "Rekap penerimaan, pengiriman, stok, stok rendah, dan log aktivitas gudang.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(28, 55)
            };

            Label lblMulai = new Label
            {
                Text = "Dari Tanggal",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 95)
            };

            dtpMulai = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 130,
                Location = new Point(30, 118)
            };

            Label lblSelesai = new Label
            {
                Text = "Sampai Tanggal",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(180, 95)
            };

            dtpSelesai = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 130,
                Location = new Point(180, 118)
            };

            Label lblJenis = new Label
            {
                Text = "Jenis Laporan",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(330, 95)
            };

            cmbJenisLaporan = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 190,
                Location = new Point(330, 118)
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
                Text = "Tampilkan",
                Width = 110,
                Height = 32,
                Location = new Point(540, 114),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTampilkan.FlatAppearance.BorderSize = 0;
            btnTampilkan.Click += BtnTampilkan_Click;

            btnRefresh = new Button
            {
                Text = "Refresh",
                Width = 100,
                Height = 32,
                Location = new Point(660, 114),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            lblTotalPenerimaan = CreateSummaryCard("Penerimaan", "0 transaksi", 30, 165);
            lblTotalPengiriman = CreateSummaryCard("Pengiriman", "0 transaksi", 230, 165);
            lblTotalStok = CreateSummaryCard("Total Stok", "0 kg", 430, 165);
            lblStokRendah = CreateSummaryCard("Stok Rendah", "0 item", 630, 165);

            dgvLaporan = new DataGridView
            {
                Location = new Point(30, 255),
                Size = new Size(850, 360),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            dgvLaporan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 53, 65);
            dgvLaporan.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLaporan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvLaporan.EnableHeadersVisualStyles = false;

            dgvLaporan.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvLaporan.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgvLaporan.DefaultCellStyle.SelectionForeColor = Color.White;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);

            this.Controls.Add(lblMulai);
            this.Controls.Add(dtpMulai);

            this.Controls.Add(lblSelesai);
            this.Controls.Add(dtpSelesai);

            this.Controls.Add(lblJenis);
            this.Controls.Add(cmbJenisLaporan);

            this.Controls.Add(btnTampilkan);
            this.Controls.Add(btnRefresh);

            this.Controls.Add(lblTotalPenerimaan);
            this.Controls.Add(lblTotalPengiriman);
            this.Controls.Add(lblTotalStok);
            this.Controls.Add(lblStokRendah);

            this.Controls.Add(dgvLaporan);
        }

        private Label CreateSummaryCard(string title, string value, int x, int y)
        {
            Label card = new Label
            {
                Text = $"{title}\n{value}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(41, 53, 65),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(x, y),
                Size = new Size(170, 65)
            };

            return card;
        }

        private void BtnTampilkan_Click(object sender, EventArgs e)
        {
            DateTime mulai = dtpMulai.Value.Date;
            DateTime selesai = dtpSelesai.Value.Date;
            LoadSummary(mulai, selesai);
            LoadLaporan();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            dtpMulai.Value = DateTime.Now.Date;
            dtpSelesai.Value = DateTime.Now.Date;
            cmbJenisLaporan.SelectedIndex = 0;

            DateTime mulai = dtpMulai.Value.Date;
            DateTime selesai = dtpSelesai.Value.Date;

            LoadSummary(mulai, selesai);
            LoadLaporan();
        }

        private void LoadSummary(DateTime mulai, DateTime selesai)
        {
            try
            {
                int totalIncoming = _repository.GetLaporanPenerimaan(mulai, selesai).Rows.Count;
                int totalOutgoing = _repository.GetLaporanPengiriman(mulai, selesai).Rows.Count;
                int totalLowStock = _repository.GetLaporanStokRendah().Rows.Count;

                int totalStok = _repository.GetTotalStok();

                lblTotalPenerimaan.Text = $"Penerimaan Total\n{totalIncoming} transaksi";
                lblTotalPengiriman.Text = $"Pengiriman Total\n{totalOutgoing} transaksi";
                lblTotalStok.Text = $"Total Stok\n{totalStok} kg";
                lblStokRendah.Text = $"Stok Rendah\n{totalLowStock} item";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Gagal memuat summary laporan: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadLaporan()
        {
            try
            {
                DateTime mulai = dtpMulai.Value.Date;
                DateTime selesai = dtpSelesai.Value.Date;

                if (mulai > selesai)
                {
                    MessageBox.Show(
                        "Tanggal mulai tidak boleh lebih besar dari tanggal selesai.",
                        "Validasi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                string jenisLaporan = cmbJenisLaporan.SelectedItem.ToString();
                DataTable data = new DataTable();

                if (jenisLaporan == "Penerimaan")
                {
                    data = _repository.GetLaporanPenerimaan(mulai, selesai);
                }
                else if (jenisLaporan == "Pengiriman")
                {
                    data = _repository.GetLaporanPengiriman(mulai, selesai);
                }
                else if (jenisLaporan == "Stok")
                {
                    data = _repository.GetLaporanStok();
                }
                else if (jenisLaporan == "Stok Rendah")
                {
                    data = _repository.GetLaporanStokRendah();
                }
                else if (jenisLaporan == "Log Aktivitas")
                {
                    data = _repository.GetLogAktivitas(mulai, selesai);
                }

                dgvLaporan.DataSource = data;
                ApplyGridStyle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Gagal memuat laporan: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ApplyGridStyle()
        {
            if (dgvLaporan.Columns.Count == 0)
                return;

            dgvLaporan.ClearSelection();

            foreach (DataGridViewColumn col in dgvLaporan.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            if (dgvLaporan.Columns.Contains("Status"))
            {
                foreach (DataGridViewRow row in dgvLaporan.Rows)
                {
                    object statusValue = row.Cells["Status"].Value;

                    if (statusValue != null && statusValue.ToString() == "LOW")
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                    }
                    else if (statusValue != null && statusValue.ToString() == "SAFE")
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(27, 94, 32);
                    }
                }
            }
        }
    }
}