using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Controllers;
using CoffeeWMS.Models;

namespace CoffeeWMS.Views
{
    public class PengirimanView : UserControl
    {
        private ComboBox cmbJenisKopi;
        private ComboBox cmbKategori;
        private ComboBox cmbOrigin;
        private ComboBox cmbRoastLevel;
        private Label lblRoastLevel;
        private ComboBox cmbDestinasi;
        private TextBox txtJumlah;
        private Button btnSimpan;
        private DataGridView dgvPengiriman;

        private readonly PengirimanController _controller = new PengirimanController();

        public PengirimanView()
        {
            BuildUI();
            LoadInitialData();
            RefreshGrid();
        }

        private void BuildUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Input Pengiriman Kopi", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(30, 20) };

            Label lblDestinasi = new Label { Text = "Destinasi / Tujuan:", Location = new Point(35, 70), AutoSize = true };
            cmbDestinasi = new ComboBox { Location = new Point(35, 95), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblJenis = new Label { Text = "Jenis Kopi:", Location = new Point(205, 70), AutoSize = true };
            cmbJenisKopi = new ComboBox { Location = new Point(205, 95), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblKategori = new Label { Text = "Kategori Kopi:", Location = new Point(375, 70), AutoSize = true };
            cmbKategori = new ComboBox { Location = new Point(375, 95), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblJumlah = new Label { Text = "Jumlah (Kg):", Location = new Point(545, 70), AutoSize = true };
            txtJumlah = new TextBox { Location = new Point(545, 95), Width = 100 };

            Label lblOrigin = new Label { Text = "Origin / Asal:", Location = new Point(205, 125), AutoSize = true };
            cmbOrigin = new ComboBox { Location = new Point(205, 150), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            lblRoastLevel = new Label { Text = "Roast Level:", Location = new Point(375, 125), AutoSize = true, Visible = false };
            cmbRoastLevel = new ComboBox { Location = new Point(375, 150), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Visible = false };

            cmbJenisKopi.SelectedIndexChanged += CmbJenisKopi_SelectedIndexChanged;
            cmbKategori.SelectedIndexChanged += CmbKategori_SelectedIndexChanged;
            cmbOrigin.SelectedIndexChanged += CmbOrigin_SelectedIndexChanged;

            btnSimpan = new Button { Text = "Simpan Pengiriman", Location = new Point(35, 140), Width = 150, Height = 30, BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSimpan.Click += BtnSimpan_Click;

            dgvPengiriman = new DataGridView { Location = new Point(35, 190), Width = 610, Height = 245, BackgroundColor = Color.FromArgb(240, 240, 240), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false };

            this.Controls.AddRange(new Control[] { lblTitle, lblJenis, cmbJenisKopi, lblKategori, cmbKategori, lblDestinasi, cmbDestinasi, lblJumlah, txtJumlah, lblOrigin, cmbOrigin, lblRoastLevel, cmbRoastLevel, btnSimpan, dgvPengiriman });
        }

        // =====================================================================
        // INISIALISASI DATA
        // =====================================================================

        private void LoadInitialData()
        {
            try
            {
                BindDropdown(cmbDestinasi, _controller.GetDestinations());
                LoadJenisKopi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data awal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================================
        // CASCADING DROPDOWNS — Memanggil Controller, tanpa SQL di sini
        // =====================================================================

        private void LoadJenisKopi()
        {
            cmbJenisKopi.SelectedIndexChanged -= CmbJenisKopi_SelectedIndexChanged;
            BindDropdown(cmbJenisKopi, _controller.GetCascadingJenisKopi());
            cmbJenisKopi.SelectedIndex = -1;
            cmbJenisKopi.SelectedIndexChanged += CmbJenisKopi_SelectedIndexChanged;
            LoadKategori();
        }

        private void LoadKategori()
        {
            cmbKategori.SelectedIndexChanged -= CmbKategori_SelectedIndexChanged;
            if (cmbJenisKopi.SelectedValue is int coffeeId)
                BindDropdown(cmbKategori, _controller.GetCascadingKategori(coffeeId));
            else
                cmbKategori.DataSource = null;
            cmbKategori.SelectedIndex = -1;
            cmbKategori.SelectedIndexChanged += CmbKategori_SelectedIndexChanged;
            CmbKategori_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void LoadOrigin()
        {
            cmbOrigin.SelectedIndexChanged -= CmbOrigin_SelectedIndexChanged;
            if (cmbJenisKopi.SelectedValue is int coffeeId && cmbKategori.SelectedValue is int categoryId)
                BindDropdown(cmbOrigin, _controller.GetCascadingOrigin(coffeeId, categoryId));
            else
                cmbOrigin.DataSource = null;
            cmbOrigin.SelectedIndex = -1;
            cmbOrigin.SelectedIndexChanged += CmbOrigin_SelectedIndexChanged;
            LoadRoastLevel();
        }

        private void LoadRoastLevel()
        {
            if (cmbJenisKopi.SelectedValue is int coffeeId &&
                cmbKategori.SelectedValue is int categoryId &&
                cmbOrigin.SelectedValue is int originId)
                BindDropdown(cmbRoastLevel, _controller.GetCascadingRoastLevel(coffeeId, categoryId, originId));
            else
                cmbRoastLevel.DataSource = null;
            cmbRoastLevel.SelectedIndex = -1;
        }

        // =====================================================================
        // EVENT HANDLERS
        // =====================================================================

        private void CmbJenisKopi_SelectedIndexChanged(object? sender, EventArgs e) => LoadKategori();

        private void CmbKategori_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isRoasted = cmbKategori.Text.Equals("Roasted Bean", StringComparison.OrdinalIgnoreCase);
            lblRoastLevel.Visible = isRoasted;
            cmbRoastLevel.Visible = isRoasted;
            LoadOrigin();
        }

        private void CmbOrigin_SelectedIndexChanged(object? sender, EventArgs e) => LoadRoastLevel();

        private void BtnSimpan_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtJumlah.Text, out int jumlah) || jumlah <= 0)
            {
                MessageBox.Show("Isi jumlah data dengan benar (angka bulat > 0)!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int destinationId = (cmbDestinasi.SelectedValue as int?) ?? 0;
            int coffeeId      = (cmbJenisKopi.SelectedValue as int?) ?? 0;
            int categoryId    = (cmbKategori.SelectedValue as int?) ?? 0;
            int originId      = (cmbOrigin.SelectedValue as int?) ?? 0;
            int petugasId     = Session.CurrentUser?.UserId ?? 1;

            if (destinationId == 0 || coffeeId == 0 || categoryId == 0 || originId == 0)
            {
                MessageBox.Show("Data Destinasi, Jenis, Kategori Kopi, atau Origin belum valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int roastLevelId = 0;
            if (cmbKategori.Text.Equals("Roasted Bean", StringComparison.OrdinalIgnoreCase))
            {
                roastLevelId = (cmbRoastLevel.SelectedValue as int?) ?? 0;
                if (roastLevelId == 0)
                {
                    MessageBox.Show("Data Roast Level belum dipilih!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            DialogResult dialogResult = MessageBox.Show($"Apakah Anda yakin jumlah berat yang dimasukkan adalah {jumlah} Kg?", "Konfirmasi Jumlah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            bool sukses = _controller.SimpanPengiriman(destinationId, coffeeId, categoryId, originId, roastLevelId, jumlah, petugasId);

            if (sukses)
            {
                MessageBox.Show("Pengiriman berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtJumlah.Clear();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show("Gagal menyimpan data pengiriman ke Database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================================
        // HELPERS
        // =====================================================================

        private void RefreshGrid()
        {
            var data = _controller.GetDataPengiriman();
            dgvPengiriman.DataSource = data.Count > 0 ? (object)data : null;
        }

        private static void BindDropdown(ComboBox cmb, List<DropdownItem> items)
        {
            // DisplayMember & ValueMember harus diset SEBELUM DataSource di WinForms
            cmb.DisplayMember = nameof(DropdownItem.Name);
            cmb.ValueMember = nameof(DropdownItem.Id);
            cmb.DataSource = items.Count > 0 ? (object)items : null;
        }
    }
}