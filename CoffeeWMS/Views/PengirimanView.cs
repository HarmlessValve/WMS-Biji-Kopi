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
        private ComboBox cmbOrigin;
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

        private void CmbJenisKopi_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadKategori();
        }

        private void CmbKategori_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isRoasted = false;
            if (cmbKategori.SelectedItem is DataRowView drv && drv.Row.Table.Columns.Contains("category_name"))
            {
                isRoasted = drv["category_name"].ToString()!.Equals("Roasted Bean", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                isRoasted = cmbKategori.Text.Equals("Roasted Bean", StringComparison.OrdinalIgnoreCase);
            }
            
            lblRoastLevel.Visible = isRoasted;
            cmbRoastLevel.Visible = isRoasted;
            LoadOrigin();
        }

        private void CmbOrigin_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadRoastLevel();
        }

        private void LoadComboBoxData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    // Load Data Destinasi
                    string qDest = "SELECT destination_id, destination_name FROM destinations WHERE is_active = true";
                    DataTable dtDest = new DataTable();
                    using (var da = new NpgsqlDataAdapter(qDest, conn)) { da.Fill(dtDest); }
                    
                    cmbDestinasi.DataSource = dtDest;
                    cmbDestinasi.DisplayMember = "destination_name";
                    cmbDestinasi.ValueMember = "destination_id";
                }
                
                LoadJenisKopi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data master ComboBox: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadJenisKopi()
        {
            cmbJenisKopi.SelectedIndexChanged -= CmbJenisKopi_SelectedIndexChanged;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string q = "SELECT DISTINCT ct.coffee_id, ct.coffee_name FROM coffee_products cp JOIN coffee_types ct ON cp.coffee_id = ct.coffee_id WHERE cp.is_active = true AND ct.is_active = true ORDER BY ct.coffee_name";
                    DataTable dt = new DataTable();
                    using (var da = new NpgsqlDataAdapter(q, conn)) { da.Fill(dt); }
                    
                    cmbJenisKopi.DataSource = dt;
                    cmbJenisKopi.DisplayMember = "coffee_name";
                    cmbJenisKopi.ValueMember = "coffee_id";
                    cmbJenisKopi.SelectedIndex = -1;
                }
            }
            catch {}
            finally { cmbJenisKopi.SelectedIndexChanged += CmbJenisKopi_SelectedIndexChanged; }
            LoadKategori();
        }

        private void LoadKategori()
        {
            cmbKategori.SelectedIndexChanged -= CmbKategori_SelectedIndexChanged;
            try
            {
                if (cmbJenisKopi.SelectedValue == null || !(cmbJenisKopi.SelectedValue is int))
                {
                    cmbKategori.DataSource = null;
                }
                else
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        string q = "SELECT DISTINCT cc.category_id, cc.category_name FROM coffee_products cp JOIN coffee_categories cc ON cp.category_id = cc.category_id WHERE cp.coffee_id = @c AND cp.is_active = true ORDER BY cc.category_name";
                        using (var cmd = new NpgsqlCommand(q, conn))
                        {
                            cmd.Parameters.AddWithValue("c", (int)cmbJenisKopi.SelectedValue);
                            DataTable dt = new DataTable();
                            using (var da = new NpgsqlDataAdapter(cmd)) { da.Fill(dt); }
                            cmbKategori.DataSource = dt;
                            cmbKategori.DisplayMember = "category_name";
                            cmbKategori.ValueMember = "category_id";
                            cmbKategori.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch {}
            finally { cmbKategori.SelectedIndexChanged += CmbKategori_SelectedIndexChanged; }
            
            CmbKategori_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void LoadOrigin()
        {
            cmbOrigin.SelectedIndexChanged -= CmbOrigin_SelectedIndexChanged;
            try
            {
                if (cmbJenisKopi.SelectedValue == null || !(cmbJenisKopi.SelectedValue is int) || 
                    cmbKategori.SelectedValue == null || !(cmbKategori.SelectedValue is int))
                {
                    cmbOrigin.DataSource = null;
                }
                else
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        string q = "SELECT DISTINCT co.origin_id, co.origin_name FROM coffee_products cp JOIN coffee_origins co ON cp.origin_id = co.origin_id WHERE cp.coffee_id = @c AND cp.category_id = @cat AND cp.is_active = true AND co.is_active = true ORDER BY co.origin_name";
                        using (var cmd = new NpgsqlCommand(q, conn))
                        {
                            cmd.Parameters.AddWithValue("c", (int)cmbJenisKopi.SelectedValue);
                            cmd.Parameters.AddWithValue("cat", (int)cmbKategori.SelectedValue);
                            DataTable dt = new DataTable();
                            using (var da = new NpgsqlDataAdapter(cmd)) { da.Fill(dt); }
                            cmbOrigin.DataSource = dt;
                            cmbOrigin.DisplayMember = "origin_name";
                            cmbOrigin.ValueMember = "origin_id";
                            cmbOrigin.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch {}
            finally { cmbOrigin.SelectedIndexChanged += CmbOrigin_SelectedIndexChanged; }
            LoadRoastLevel();
        }

        private void LoadRoastLevel()
        {
            try
            {
                if (cmbJenisKopi.SelectedValue == null || !(cmbJenisKopi.SelectedValue is int) || 
                    cmbKategori.SelectedValue == null || !(cmbKategori.SelectedValue is int) || 
                    cmbOrigin.SelectedValue == null || !(cmbOrigin.SelectedValue is int))
                {
                    cmbRoastLevel.DataSource = null;
                }
                else
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        string q = "SELECT DISTINCT rl.roast_level_id, rl.roast_level_name FROM coffee_products cp JOIN roast_levels rl ON cp.roast_level_id = rl.roast_level_id WHERE cp.coffee_id = @c AND cp.category_id = @cat AND cp.origin_id = @o AND cp.is_active = true AND rl.is_active = true ORDER BY rl.roast_level_name";
                        using (var cmd = new NpgsqlCommand(q, conn))
                        {
                            cmd.Parameters.AddWithValue("c", (int)cmbJenisKopi.SelectedValue);
                            cmd.Parameters.AddWithValue("cat", (int)cmbKategori.SelectedValue);
                            cmd.Parameters.AddWithValue("o", (int)cmbOrigin.SelectedValue);
                            DataTable dt = new DataTable();
                            using (var da = new NpgsqlDataAdapter(cmd)) { da.Fill(dt); }
                            cmbRoastLevel.DataSource = dt;
                            cmbRoastLevel.DisplayMember = "roast_level_name";
                            cmbRoastLevel.ValueMember = "roast_level_id";
                            cmbRoastLevel.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch {}
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
            int originId = (cmbOrigin.SelectedValue as int?) ?? 0;
            int petugasId = Session.CurrentUser?.UserId ?? 1;

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

            // Eksekusi repository dengan tipe data integer (Sesuai modifikasi DB kemarin)
            bool sukses = this.InsertPengiriman(destinationId, coffeeId, categoryId, originId, roastLevelId, jumlah, petugasId);

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

        private void RefreshGrid()
        {
            var dt = this.GetDataPengiriman();
            if (dt != null) 
            {
                dgvPengiriman.DataSource = dt;
            }
        }

        private int GetOrCreateProduct(NpgsqlConnection conn, int coffeeId, int categoryId, int originId, int roastLevelId)
        {
            conn.Open(); 
            int productId = 0;
            string query = "SELECT product_id FROM coffee_products WHERE coffee_id = @c AND category_id = @cat AND origin_id = @o ";
            if (roastLevelId > 0) query += "AND roast_level_id = @r ";
            else query += "AND roast_level_id IS NULL ";
            query += "LIMIT 1";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("c", coffeeId);
                cmd.Parameters.AddWithValue("cat", categoryId);
                cmd.Parameters.AddWithValue("o", originId);
                if (roastLevelId > 0) cmd.Parameters.AddWithValue("r", roastLevelId);
                
                var res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value) productId = Convert.ToInt32(res);
            }

            if (productId == 0)
            {
                string insertQuery = "INSERT INTO coffee_products (coffee_id, category_id, origin_id, roast_level_id, minimum_stock) VALUES (@c, @cat, @o, @r, 20) RETURNING product_id";
                using (var cmd = new NpgsqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("c", coffeeId);
                    cmd.Parameters.AddWithValue("cat", categoryId);
                    cmd.Parameters.AddWithValue("o", originId);
                    if (roastLevelId > 0) cmd.Parameters.AddWithValue("r", roastLevelId);
                    else cmd.Parameters.AddWithValue("r", DBNull.Value);
                    
                    productId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return productId;
        }

        private bool InsertPengiriman(int destinationId, int coffeeId, int categoryId, int originId, int roastLevelId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    // conn.Open(); <- Remove this because we opened it inside GetOrCreateProduct
                    int productId = GetOrCreateProduct(conn, coffeeId, categoryId, originId, roastLevelId);

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