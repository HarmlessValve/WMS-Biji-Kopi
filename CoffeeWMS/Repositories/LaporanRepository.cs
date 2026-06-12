using System;
using System.Data;
using Npgsql;
using CoffeeWMS.Data;

namespace CoffeeWMS.Repositories
{
    public class LaporanRepository
    {
        public DataTable GetLaporanPenerimaan(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                        SELECT 
                            tanggal AS ""Tanggal"",
                            supplier AS ""Supplier"",
                            jenis_kopi AS ""Jenis Kopi"",
                            jumlah AS ""Jumlah (Kg)"",
                            petugas AS ""Petugas""
                        FROM vw_incoming_transactions
                        WHERE tanggal >= @startDate 
                          AND tanggal < @endDate
                        ORDER BY tanggal DESC;
                    ";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("startDate", startDate.Date);
                        cmd.Parameters.AddWithValue("endDate", endDate.Date.AddDays(1));

                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetLaporanPenerimaan): " + ex.Message);
            }

            return dt;
        }

        public DataTable GetLaporanPengiriman(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                        SELECT 
                            tanggal AS ""Tanggal"",
                            destinasi AS ""Destinasi"",
                            jenis_kopi AS ""Jenis Kopi"",
                            jumlah AS ""Jumlah (Kg)"",
                            petugas AS ""Petugas""
                        FROM vw_outgoing_transactions
                        WHERE tanggal >= @startDate 
                          AND tanggal < @endDate
                        ORDER BY tanggal DESC;
                    ";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("startDate", startDate.Date);
                        cmd.Parameters.AddWithValue("endDate", endDate.Date.AddDays(1));

                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetLaporanPengiriman): " + ex.Message);
            }

            return dt;
        }

        public DataTable GetLaporanStok()
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                        SELECT
                            coffee_id AS ""ID Kopi"",
                            coffee_name AS ""Nama Kopi"",
                            category_name AS ""Kategori"",
                            COALESCE(roast_level_name, '-') AS ""Roast Level"",
                            current_quantity AS ""Stok Saat Ini"",
                            minimum_stock AS ""Minimum Stok"",
                            status AS ""Status""
                        FROM stock_summary
                        ORDER BY coffee_name;
                    ";

                    using (var adapter = new NpgsqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetLaporanStok): " + ex.Message);
            }

            return dt;
        }

        public DataTable GetLaporanStokRendah()
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                        SELECT
                            coffee_id AS ""ID Kopi"",
                            coffee_name AS ""Nama Kopi"",
                            category_name AS ""Kategori"",
                            COALESCE(roast_level_name, '-') AS ""Roast Level"",
                            current_quantity AS ""Stok Saat Ini"",
                            minimum_stock AS ""Minimum Stok"",
                            minimum_stock - current_quantity AS ""Kekurangan""
                        FROM stock_summary
                        WHERE status = 'LOW'
                        ORDER BY coffee_name;
                    ";

                    using (var adapter = new NpgsqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetLaporanStokRendah): " + ex.Message);
            }

            return dt;
        }

        public DataTable GetLogAktivitas(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"
                        SELECT
                            log_time AS ""Waktu"",
                            actor AS ""Actor"",
                            action AS ""Aksi"",
                            description AS ""Deskripsi""
                        FROM vw_logs
                        WHERE log_time >= @startDate 
                          AND log_time < @endDate
                        ORDER BY log_time DESC;
                    ";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("startDate", startDate.Date);
                        cmd.Parameters.AddWithValue("endDate", endDate.Date.AddDays(1));

                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetLogAktivitas): " + ex.Message);
            }

            return dt;
        }

        public int GetTotalStok()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT COALESCE(SUM(current_quantity), 0) FROM stock_summary;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetTotalStok): " + ex.Message);
                return 0;
            }
        }

        public DataTable GetDashboardSummary()
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM vw_dashboard_summary;";

                    using (var adapter = new NpgsqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetDashboardSummary): " + ex.Message);
            }

            return dt;
        }
    }
}