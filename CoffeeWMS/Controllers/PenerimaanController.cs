using System;
using System.Collections.Generic;
using Npgsql;
using CoffeeWMS.Data;
using CoffeeWMS.Models;

namespace CoffeeWMS.Controllers
{
    /// <summary>
    /// Controller untuk semua operasi Penerimaan Kopi.
    /// Semua query SQL dan logic ADO.NET ada di sini; View hanya menerima Model.
    /// </summary>
    public class PenerimaanController
    {
        // =====================================================================
        // DATA MASTER - menggunakan View & Function PostgreSQL
        // =====================================================================

        public List<DropdownItem> GetSuppliers()
        {
            var result = new List<DropdownItem>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT supplier_id, company_name FROM vw_active_suppliers", conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            result.Add(new DropdownItem { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                }
            }
            catch (Exception ex) { Console.WriteLine("GetSuppliers error: " + ex.Message); }
            return result;
        }

        public List<DropdownItem> GetCascadingJenisKopi()
        {
            var result = new List<DropdownItem>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT coffee_id, coffee_name FROM fn_get_cascading_jenis_kopi()", conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            result.Add(new DropdownItem { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                }
            }
            catch (Exception ex) { Console.WriteLine("GetCascadingJenisKopi error: " + ex.Message); }
            return result;
        }

        public List<DropdownItem> GetCascadingKategori(int coffeeId)
        {
            var result = new List<DropdownItem>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT category_id, category_name FROM fn_get_cascading_kategori(@c)", conn))
                    {
                        cmd.Parameters.AddWithValue("c", coffeeId);
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                result.Add(new DropdownItem { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("GetCascadingKategori error: " + ex.Message); }
            return result;
        }

        public List<DropdownItem> GetCascadingOrigin(int coffeeId, int categoryId)
        {
            var result = new List<DropdownItem>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT origin_id, origin_name FROM fn_get_cascading_origin(@c, @cat)", conn))
                    {
                        cmd.Parameters.AddWithValue("c", coffeeId);
                        cmd.Parameters.AddWithValue("cat", categoryId);
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                result.Add(new DropdownItem { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("GetCascadingOrigin error: " + ex.Message); }
            return result;
        }

        public List<DropdownItem> GetCascadingRoastLevel(int coffeeId, int categoryId, int originId)
        {
            var result = new List<DropdownItem>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT roast_level_id, roast_level_name FROM fn_get_cascading_roast_level(@c, @cat, @o)", conn))
                    {
                        cmd.Parameters.AddWithValue("c", coffeeId);
                        cmd.Parameters.AddWithValue("cat", categoryId);
                        cmd.Parameters.AddWithValue("o", originId);
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                result.Add(new DropdownItem { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("GetCascadingRoastLevel error: " + ex.Message); }
            return result;
        }

        // =====================================================================
        // DATA GRID
        // =====================================================================

        public List<LaporanPenerimaanItem> GetDataPenerimaan()
        {
            var result = new List<LaporanPenerimaanItem>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT tanggal, supplier, jenis_kopi, jumlah, petugas FROM vw_incoming_transactions ORDER BY tanggal DESC", conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            result.Add(new LaporanPenerimaanItem
                            {
                                Tanggal = reader.GetDateTime(0),
                                Supplier = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                JenisKopi = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Jumlah = reader.GetInt32(3),
                                Petugas = reader.IsDBNull(4) ? "" : reader.GetString(4)
                            });
                }
            }
            catch (Exception ex) { Console.WriteLine("GetDataPenerimaan error: " + ex.Message); }
            return result;
        }

        // =====================================================================
        // OPERASI SIMPAN
        // =====================================================================

        /// <summary>
        /// Memanggil fn_get_or_create_product dan sp_add_incoming_transaction dalam satu transaksi.
        /// </summary>
        public bool SimpanPenerimaan(int supplierId, int coffeeId, int categoryId, int originId, int roastLevelId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Langkah 1: Dapatkan atau buat product_id menggunakan function PostgreSQL
                    int productId;
                    using (var cmd = new NpgsqlCommand("SELECT fn_get_or_create_product(@c, @cat, @o, @r)", conn))
                    {
                        cmd.Parameters.AddWithValue("c", coffeeId);
                        cmd.Parameters.AddWithValue("cat", categoryId);
                        cmd.Parameters.AddWithValue("o", originId);
                        cmd.Parameters.AddWithValue("r", roastLevelId);
                        productId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Langkah 2: Panggil stored procedure untuk menyimpan transaksi
                    using (var cmd = new NpgsqlCommand("CALL sp_add_incoming_transaction(@s, @p_id, @q, @p)", conn))
                    {
                        cmd.Parameters.AddWithValue("s", supplierId);
                        cmd.Parameters.AddWithValue("p_id", productId);
                        cmd.Parameters.AddWithValue("q", quantity);
                        cmd.Parameters.AddWithValue("p", petugasId);
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SimpanPenerimaan error: " + ex.Message);
                return false;
            }
        }
    }
}
