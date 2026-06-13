using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using CoffeeWMS.Data;
using CoffeeWMS.Models;

namespace CoffeeWMS.Controllers
{
    /// <summary>
    /// Controller untuk semua operasi Laporan.
    /// Semua query SQL dan logic ADO.NET ada di sini; View hanya menerima Model atau DataTable.
    /// </summary>
    public class LaporanController
    {
        // =====================================================================
        // LAPORAN PENERIMAAN — Menggunakan fn_get_laporan_penerimaan
        // =====================================================================

        public List<LaporanPenerimaanItem> GetLaporanPenerimaan(DateTime startDate, DateTime endDate)
        {
            var result = new List<LaporanPenerimaanItem>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT \"Tanggal\", \"Supplier\", \"JenisKopi\", \"Jumlah\", \"Petugas\" FROM fn_get_laporan_penerimaan(@s::date, @e::date)", conn))
                    {
                        cmd.Parameters.AddWithValue("s", startDate.Date);
                        cmd.Parameters.AddWithValue("e", endDate.Date.AddDays(1));
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
            }
            catch (Exception ex) { Console.WriteLine("GetLaporanPenerimaan error: " + ex.Message); }
            return result;
        }

        // =====================================================================
        // LAPORAN PENGIRIMAN — Menggunakan fn_get_laporan_pengiriman
        // =====================================================================

        public List<LaporanPengirimanItem> GetLaporanPengiriman(DateTime startDate, DateTime endDate)
        {
            var result = new List<LaporanPengirimanItem>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT \"Tanggal\", \"Destinasi\", \"JenisKopi\", \"Jumlah\", \"Petugas\" FROM fn_get_laporan_pengiriman(@s::date, @e::date)", conn))
                    {
                        cmd.Parameters.AddWithValue("s", startDate.Date);
                        cmd.Parameters.AddWithValue("e", endDate.Date.AddDays(1));
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                result.Add(new LaporanPengirimanItem
                                {
                                    Tanggal = reader.GetDateTime(0),
                                    Destinasi = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    JenisKopi = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Jumlah = reader.GetInt32(3),
                                    Petugas = reader.IsDBNull(4) ? "" : reader.GetString(4)
                                });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("GetLaporanPengiriman error: " + ex.Message); }
            return result;
        }

        // =====================================================================
        // LAPORAN STOK — Menggunakan view stock_summary (sudah ada di DB)
        // =====================================================================

        public DataTable GetLaporanStok()
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var adapter = new NpgsqlDataAdapter(
                        @"SELECT coffee_name AS ""Nama Kopi"", category_name AS ""Kategori"",
                          origin_name AS ""Asal"", COALESCE(roast_level_name, '-') AS ""Roast Level"",
                          current_quantity AS ""Stok Saat Ini"", minimum_stock AS ""Minimum Stok"",
                          status AS ""Status""
                          FROM stock_summary ORDER BY coffee_name", conn))
                        adapter.Fill(dt);
                }
            }
            catch (Exception ex) { Console.WriteLine("GetLaporanStok error: " + ex.Message); }
            return dt;
        }

        public DataTable GetLaporanStokRendah()
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var adapter = new NpgsqlDataAdapter(
                        @"SELECT coffee_name AS ""Nama Kopi"", category_name AS ""Kategori"",
                          origin_name AS ""Asal"", COALESCE(roast_level_name, '-') AS ""Roast Level"",
                          current_quantity AS ""Stok Saat Ini"", minimum_stock AS ""Minimum Stok"",
                          minimum_stock - current_quantity AS ""Kekurangan""
                          FROM stock_summary WHERE status = 'LOW' ORDER BY coffee_name", conn))
                        adapter.Fill(dt);
                }
            }
            catch (Exception ex) { Console.WriteLine("GetLaporanStokRendah error: " + ex.Message); }
            return dt;
        }

        public DataTable GetLogAktivitas(DateTime startDate, DateTime endDate)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(
                        @"SELECT log_time AS ""Waktu"", actor AS ""Actor"", action AS ""Aksi"", description AS ""Deskripsi""
                          FROM vw_logs WHERE log_time >= @s AND log_time < @e ORDER BY log_time DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("s", startDate.Date);
                        cmd.Parameters.AddWithValue("e", endDate.Date.AddDays(1));
                        using (var adapter = new NpgsqlDataAdapter(cmd))
                            adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("GetLogAktivitas error: " + ex.Message); }
            return dt;
        }

        // =====================================================================
        // SUMMARY STATS
        // =====================================================================

        public int GetTotalStok()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(current_quantity), 0) FROM stock_summary", conn))
                        return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex) { Console.WriteLine("GetTotalStok error: " + ex.Message); return 0; }
        }

        public (int totalPenerimaan, int totalPengiriman) GetSummaryStats()
        {
            int totalIn = 0, totalOut = 0;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(jumlah), 0) FROM vw_incoming_transactions", conn))
                        totalIn = Convert.ToInt32(cmd.ExecuteScalar());
                    using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(jumlah), 0) FROM vw_outgoing_transactions", conn))
                        totalOut = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex) { Console.WriteLine("GetSummaryStats error: " + ex.Message); }
            return (totalIn, totalOut);
        }
    }
}
