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
        public bool InsertPenerimaan(int supplierId, int coffeeId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("CALL sp_add_incoming_transaction(@s, @c, @q, @p)", conn))
                    {
                        cmd.Parameters.AddWithValue("s", supplierId);
                        cmd.Parameters.AddWithValue("c", coffeeId);
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
        public bool InsertPengiriman(int destinationId, int coffeeId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("CALL sp_add_outgoing_transaction(@d, @c, @q, @p)", conn))
                    {
                        cmd.Parameters.AddWithValue("d", destinationId);
                        cmd.Parameters.AddWithValue("c", coffeeId);
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

namespace CoffeeWMS.Repositories
{
    public class TransactionRepository
    {
        private readonly string _connectionString;

        public TransactionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Metode untuk mengeksekusi Stored Procedure Penerimaan
        public void AddIncomingTransaction(int supplierId, int coffeeId, int quantity, int petugasId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_add_incoming_transaction(@supplier_id, @coffee_id, @quantity, @petugas_id)", conn))
                {
                    cmd.Parameters.AddWithValue("supplier_id", supplierId);
                    cmd.Parameters.AddWithValue("coffee_id", coffeeId);
                    cmd.Parameters.AddWithValue("quantity", quantity);
                    cmd.Parameters.AddWithValue("petugas_id", petugasId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        
    }
}