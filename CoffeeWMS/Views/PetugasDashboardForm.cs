using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CoffeeWMS.Views
{
    public class PetugasDashboardForm : UserControl
    {
        public event EventHandler LoadDashboardRequested;

        private Label lblTotalProdukVal;
        private Label lblStokMenipisVal;
        private Label lblMasukHariIniVal;
        private Label lblKeluarHariIniVal;
        private DataGridView dgvStokKopi;

        public PetugasDashboardForm()
        {
            InitializeUI();
            this.Load += (s, e) => LoadDashboardRequested?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeUI()
        {
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);

            Label title = new Label
            {
                Text = "Dashboard Petugas",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(title);

            // Summary Cards Panel
            FlowLayoutPanel flpSummary = new FlowLayoutPanel
            {
                Location = new Point(20, 70),
                Width = 900,
                Height = 120,
                WrapContents = false
            };

            var pnlTotal = CreateCard("Total Produk", out lblTotalProdukVal);
            var pnlMenipis = CreateCard("Stok Menipis", out lblStokMenipisVal);
            var pnlMasuk = CreateCard("Masuk Hari Ini", out lblMasukHariIniVal);
            var pnlKeluar = CreateCard("Keluar Hari Ini", out lblKeluarHariIniVal);

            flpSummary.Controls.Add(pnlTotal);
            flpSummary.Controls.Add(pnlMenipis);
            flpSummary.Controls.Add(pnlMasuk);
            flpSummary.Controls.Add(pnlKeluar);
            this.Controls.Add(flpSummary);

            // Table Label
            Label lblTable = new Label
            {
                Text = "Tabel Stok Kopi",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 210)
            };
            this.Controls.Add(lblTable);

            // DataGridView
            dgvStokKopi = new DataGridView
            {
                Location = new Point(20, 250),
                Width = 900,
                Height = 350,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 },
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(dgvStokKopi);
        }

        private Panel CreateCard(string title, out Label valLabel)
        {
            Panel p = new Panel
            {
                Width = 200,
                Height = 100,
                BackColor = Color.FromArgb(41, 53, 65),
                Margin = new Padding(0, 0, 20, 0)
            };
            
            Label t = new Label
            {
                Text = title,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            valLabel = new Label
            {
                Text = "0",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 40)
            };

            p.Controls.Add(t);
            p.Controls.Add(valLabel);
            return p;
        }

        public void DisplaySummary(int totalProduk, int stokMenipis, int masukHariIni, int keluarHariIni)
        {
            lblTotalProdukVal.Text = totalProduk.ToString();
            lblStokMenipisVal.Text = stokMenipis.ToString();
            lblMasukHariIniVal.Text = masukHariIni.ToString();
            lblKeluarHariIniVal.Text = keluarHariIni.ToString();
        }

        public void DisplayStokKopi(object dataSource)
        {
            dgvStokKopi.DataSource = dataSource;
            
            // Format DataGridView if columns exist
            if (dgvStokKopi.Columns.Contains("Status"))
            {
                dgvStokKopi.CellFormatting += (s, e) =>
                {
                    if (dgvStokKopi.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
                    {
                        string status = e.Value.ToString();
                        if (status == "Habis")
                        {
                            e.CellStyle.BackColor = Color.LightCoral;
                            e.CellStyle.ForeColor = Color.White;
                        }
                        else if (status == "Menipis")
                        {
                            e.CellStyle.BackColor = Color.Gold;
                            e.CellStyle.ForeColor = Color.Black;
                        }
                        else
                        {
                            e.CellStyle.BackColor = Color.LightGreen;
                            e.CellStyle.ForeColor = Color.Black;
                        }
                    }
                };
            }
        }
    }
}
