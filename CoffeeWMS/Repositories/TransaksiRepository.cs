using System;
using System.Data;
using Npgsql;

namespace CoffeeWMS.Repositories
{
    public class TransaksiRepository
    {
        // Silakan sesuaikan password database pgAdmin kamu di sini
        private string connString = "Host=localhost;Username=postgres;Password=123;Database=WMS";

        // 1. Simpan data Penerimaan
        public bool InsertPenerimaan(DateTime tanggal, string supplier, string jenis, string noBatch, decimal jumlah)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = @"INSERT INTO TransaksiMasuk (Tanggal, Supplier, JenisKopi, NoBatch, JumlahKg) 
                                     VALUES (@tgl, @sup, @jenis, @batch, @jml)";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("tgl", tanggal);
                        cmd.Parameters.AddWithValue("sup", supplier);
                        cmd.Parameters.AddWithValue("jenis", jenis);
                        cmd.Parameters.AddWithValue("batch", noBatch);
                        cmd.Parameters.AddWithValue("jml", jumlah);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; } // Mengembalikan false jika tabel/DB belum siap
        }

        // 2. Simpan data Pengiriman
        public bool InsertPengiriman(DateTime tanggal, string customer, string jenis, decimal jumlah)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = @"INSERT INTO TransaksiKeluar (Tanggal, Customer, JenisKopi, JumlahKg) 
                                     VALUES (@tgl, @cust, @jenis, @jml)";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("tgl", tanggal);
                        cmd.Parameters.AddWithValue("cust", customer);
                        cmd.Parameters.AddWithValue("jenis", jenis);
                        cmd.Parameters.AddWithValue("jml", jumlah);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // 3. Ambil data Penerimaan
        public DataTable GetDataPenerimaan()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    string query = "SELECT Tanggal, JenisKopi, JumlahKg FROM TransaksiMasuk ORDER BY Tanggal DESC";
                    using (var da = new NpgsqlDataAdapter(query, conn)) { da.Fill(dt); }
                }
            }
            catch { }
            return dt;
        }

        // 4. Ambil data Pengiriman
        public DataTable GetDataPengiriman()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    string query = "SELECT Tanggal, JenisKopi, JumlahKg FROM TransaksiKeluar ORDER BY Tanggal DESC";
                    using (var da = new NpgsqlDataAdapter(query, conn)) { da.Fill(dt); }
                }
            }
            catch { }
            return dt;
        }
    }
}

using System;
using System.Data;
using Npgsql; // Pastikan menggunakan library PostgreSQL yang sesuai dengan proyekmu
using CoffeeWMS.Models; // Sesuaikan namespace-nya

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