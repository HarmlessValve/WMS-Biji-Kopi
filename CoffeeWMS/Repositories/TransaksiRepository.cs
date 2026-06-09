using System;
using System.Data;
using Npgsql;
using CoffeeWMS.Data; // Pastikan helper koneksi sudah benar

namespace CoffeeWMS.Repositories
{
    public class TransaksiRepository
    {
        // 1. Simpan data Penerimaan (Disesuaikan dengan tabel incoming_transactions)
        // Parameter diubah dari string (nama) menjadi int (ID sesuai Relasi DB)
        public bool InsertPenerimaan(int supplierId, int coffeeId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Query disesuaikan dengan struktur tabel baru
                    string query = @"INSERT INTO incoming_transactions (supplier_id, coffee_id, quantity, petugas_id) 
                                     VALUES (@supplierId, @coffeeId, @quantity, @petugasId)";
                                     
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("supplierId", supplierId);
                        cmd.Parameters.AddWithValue("coffeeId", coffeeId);
                        cmd.Parameters.AddWithValue("quantity", quantity);
                        cmd.Parameters.AddWithValue("petugasId", petugasId);
                        
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // 2. Simpan data Pengiriman (Disesuaikan dengan tabel outgoing_transactions)
        public bool InsertPengiriman(int destinationId, int coffeeId, int quantity, int petugasId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO outgoing_transactions (destination_id, coffee_id, quantity, petugas_id) 
                                     VALUES (@destinationId, @coffeeId, @quantity, @petugasId)";
                                     
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("destinationId", destinationId);
                        cmd.Parameters.AddWithValue("coffeeId", coffeeId);
                        cmd.Parameters.AddWithValue("quantity", quantity);
                        cmd.Parameters.AddWithValue("petugasId", petugasId);
                        
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        // 3. Ambil data Penerimaan (Menggunakan JOIN agar nama Kopi & Supplier muncul di UI, bukan sekadar ID)
        public DataTable GetDataPenerimaan()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    // Melakukan JOIN ke tabel coffee_types dan suppliers agar datanya informatif saat dibaca
                    string query = @"SELECT t.received_at AS Tanggal, 
                                            s.company_name AS Supplier, 
                                            c.coffee_name AS JenisKopi, 
                                            t.quantity AS Jumlah
                                     FROM incoming_transactions t
                                     LEFT JOIN suppliers s ON t.supplier_id = s.supplier_id
                                     LEFT JOIN coffee_types c ON t.coffee_id = c.coffee_id
                                     ORDER BY t.received_at DESC";
                                     
                    using (var da = new NpgsqlDataAdapter(query, conn)) { da.Fill(dt); }
                }
            }
            catch { }
            return dt;
        }

        // 4. Ambil data Pengiriman (Menggunakan JOIN)
        public DataTable GetDataPengiriman()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"SELECT t.shipped_at AS Tanggal, 
                                            d.destination_name AS Destinasi, 
                                            c.coffee_name AS JenisKopi, 
                                            t.quantity AS Jumlah
                                     FROM outgoing_transactions t
                                     LEFT JOIN destinations d ON t.destination_id = d.destination_id
                                     LEFT JOIN coffee_types c ON t.coffee_id = c.coffee_id
                                     ORDER BY t.shipped_at DESC";
                                     
                    using (var da = new NpgsqlDataAdapter(query, conn)) { da.Fill(dt); }
                }
            }
            catch { }
            return dt;
        }
    }
}