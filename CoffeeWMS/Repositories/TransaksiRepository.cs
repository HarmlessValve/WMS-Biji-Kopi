using System;
using System.Data;
using Npgsql;
using CoffeeWMS.Data;

namespace CoffeeWMS.Repositories
{
    public class TransaksiRepository
    {
        // ====================================================================
        // PENERIMAAN — menggunakan SP sp_add_incoming_transaction
        // Trigger akan otomatis: update stok + catat log
        // ====================================================================
        public bool InsertPenerimaan(int supplierId, int productId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("CALL sp_add_incoming_transaction(@s, @p_id, @q, @p)", conn))
                    {
                        cmd.Parameters.AddWithValue("s", supplierId);
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
                Console.WriteLine("DB Error (InsertPenerimaan): " + ex.Message);
                return false;
            }
        }

        // ====================================================================
        // PENGIRIMAN — menggunakan SP sp_add_outgoing_transaction
        // Trigger akan otomatis: cek stok, kurangi stok + catat log
        // ====================================================================
        public bool InsertPengiriman(int destinationId, int productId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
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
                Console.WriteLine("DB Error (InsertPengiriman): " + ex.Message);
                return false;
            }
        }

        // ====================================================================
        // DATA PENERIMAAN — dari view vw_incoming_transactions
        // ====================================================================
        public DataTable GetDataPenerimaan()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT tanggal AS Tanggal, supplier AS Supplier, jenis_kopi AS JenisKopi, jumlah AS Jumlah, petugas AS Petugas FROM vw_incoming_transactions";
                    using (var da = new NpgsqlDataAdapter(query, conn)) { da.Fill(dt); }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetDataPenerimaan): " + ex.Message);
            }
            return dt;
        }

        // ====================================================================
        // DATA PENGIRIMAN — dari view vw_outgoing_transactions
        // ====================================================================
        public DataTable GetDataPengiriman()
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