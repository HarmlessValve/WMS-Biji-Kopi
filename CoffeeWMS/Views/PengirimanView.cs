using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Models;

using Npgsql;
using CoffeeWMS.Data;

namespace CoffeeWMS.Views
{
    public class PengirimanView : UserControl
    {
        private ComboBox cmbJenisKopi;
        private ComboBox cmbKategori; // Dropdown kategori kopi
        private ComboBox cmbRoastLevel;
        private Label lblRoastLevel;
        private ComboBox cmbDestinasi; // Mengubah TextBox menjadi ComboBox agar sesuai relasi ID
        private TextBox txtJumlah;
        private Button btnSimpan;
        private DataGridView dgvPengiriman;


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

            Label lblKategori = new Label { Text = "Kategori Kopi:", Location = new Point(205, 70), AutoSize = true };
            cmbKategori = new ComboBox { Location = new Point(205, 95), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblDestinasi = new Label { Text = "Destinasi / Tujuan:", Location = new Point(375, 70), AutoSize = true };
            cmbDestinasi = new ComboBox { Location = new Point(375, 95), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblJumlah = new Label { Text = "Jumlah (Kg):", Location = new Point(545, 70), AutoSize = true };
            txtJumlah = new TextBox { Location = new Point(545, 95), Width = 100 };

            lblRoastLevel = new Label { Text = "Roast Level:", Location = new Point(205, 125), AutoSize = true, Visible = false };
            cmbRoastLevel = new ComboBox { Location = new Point(205, 150), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Visible = false };

            cmbKategori.SelectedIndexChanged += CmbKategori_SelectedIndexChanged;

            btnSimpan = new Button { Text = "Simpan Pengiriman", Location = new Point(35, 140), Width = 150, Height = 30, BackColor = Color.FromArgb(41, 53, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSimpan.Click += BtnSimpan_Click;

            dgvPengiriman = new DataGridView { Location = new Point(35, 190), Width = 610, Height = 245, BackgroundColor = Color.FromArgb(240, 240, 240), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false, AllowUserToAddRows = false };

            this.Controls.AddRange(new Control[] { lblTitle, lblJenis, cmbJenisKopi, lblKategori, cmbKategori, lblDestinasi, cmbDestinasi, lblJumlah, txtJumlah, lblRoastLevel, cmbRoastLevel, btnSimpan, dgvPengiriman });
        }

        private void CmbKategori_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isRoasted = cmbKategori.Text.Equals("Roasted Bean", StringComparison.OrdinalIgnoreCase);
            lblRoastLevel.Visible = isRoasted;
            cmbRoastLevel.Visible = isRoasted;
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

                    // Load Data Kategori
                    string qKategori = "SELECT category_id, category_name FROM coffee_categories";
                    DataTable dtKategori = new DataTable();
                    using (var da = new NpgsqlDataAdapter(qKategori, conn)) { da.Fill(dtKategori); }
                    
                    cmbKategori.DataSource = dtKategori;
                    cmbKategori.DisplayMember = "category_name";
                    cmbKategori.ValueMember = "category_id";

                    // Load Data Roast Level
                    string qRoast = "SELECT roast_level_id, roast_level_name FROM roast_levels WHERE is_active = true";
                    DataTable dtRoast = new DataTable();
                    using (var da = new NpgsqlDataAdapter(qRoast, conn)) { da.Fill(dtRoast); }
                    
                    cmbRoastLevel.DataSource = dtRoast;
                    cmbRoastLevel.DisplayMember = "roast_level_name";
                    cmbRoastLevel.ValueMember = "roast_level_id";

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
            int categoryId = (cmbKategori.SelectedValue as int?) ?? 0;
            int petugasId = Session.CurrentUser?.UserId ?? 1;

            if (destinationId == 0 || coffeeId == 0 || categoryId == 0)
            {
                MessageBox.Show("Data Destinasi, Jenis, atau Kategori Kopi belum valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // Eksekusi repository dengan tipe data integer (Sesuai modifikasi DB kemarin)
            bool sukses = this.InsertPengiriman(destinationId, coffeeId, categoryId, roastLevelId, jumlah, petugasId);

            if (sukses)
            {
                MessageBox.Show("Pengiriman berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtJumlah.Clear();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show("Koneksi DB gagal. Data dialihkan ke simulasi layar.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string roastText = roastLevelId > 0 ? $" ({cmbRoastLevel.Text})" : "";
                SimulasiLokal(cmbJenisKopi.Text + " - " + cmbKategori.Text + roastText, jumlah);
                txtJumlah.Clear();
            }
        }

        private void RefreshGrid()
        {
            var dt = this.GetDataPengiriman();
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

        private int GetOrCreateProduct(NpgsqlConnection conn, int coffeeId, int categoryId, int roastLevelId)
        {
            conn.Open(); 
            int productId = 0;
            string query = "SELECT product_id FROM coffee_products WHERE coffee_id = @c AND category_id = @cat ";
            if (roastLevelId > 0) query += "AND roast_level_id = @r ";
            else query += "AND roast_level_id IS NULL ";
            query += "LIMIT 1";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("c", coffeeId);
                cmd.Parameters.AddWithValue("cat", categoryId);
                if (roastLevelId > 0) cmd.Parameters.AddWithValue("r", roastLevelId);
                
                var res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value) productId = Convert.ToInt32(res);
            }

            if (productId == 0)
            {
                string insertQuery = "INSERT INTO coffee_products (coffee_id, category_id, roast_level_id, minimum_stock) VALUES (@c, @cat, @r, 20) RETURNING product_id";
                using (var cmd = new NpgsqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("c", coffeeId);
                    cmd.Parameters.AddWithValue("cat", categoryId);
                    if (roastLevelId > 0) cmd.Parameters.AddWithValue("r", roastLevelId);
                    else cmd.Parameters.AddWithValue("r", DBNull.Value);
                    
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return productId;
        }

        private bool InsertPengiriman(int destinationId, int coffeeId, int categoryId, int roastLevelId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open(); // ← wajib ada karena GetConnection() tidak open otomatis
                    int productId = GetOrCreateProduct(conn, coffeeId, categoryId, roastLevelId);

                    using (var cmd = new NpgsqlCommand("CALL sp_add_outgoing_transaction(@d, @p_id, @q, @p)", conn))
                    {
                        cmd.Parameters.AddWithValue("d", destinationId);
                        cmd.Parameters.AddWithValue("p_id", productId);
                        cmd.Parameters.AddWithValue("q", quantity);
                        cmd.Parameters.AddWithValue("p", petugasId);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Detail error:\n" + ex.Message + "\n\n" + ex.GetType().Name,
                                "Debug Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private DataTable GetDataPengiriman()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT tanggal AS Tanggal, destinasi AS Destinasi, jenis_kopi AS JenisKopi, jumlah AS Jumlah, petugas AS Petugas FROM vw_outgoing_transactions";
                    using (var da = new NpgsqlDataAdapter(query, conn)) { da.Fill(dt); }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetDataPengiriman): " + ex.Message);
            }
            return dt;
        }
    }
}