using System;
using System.Collections.Generic;
using Npgsql;
using CoffeeWMS.Models;
using CoffeeWMS.Data;

namespace CoffeeWMS.Repositories
{
    public class MasterDataRepository
    {
        public List<LogEntry> GetLogs()
        {
            var logs = new List<LogEntry>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT log_id, actor, action, description, log_time FROM vw_logs ORDER BY log_time DESC", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logs.Add(new LogEntry
                            {
                                LogId = reader.GetInt32(0),
                                Actor = reader.IsDBNull(1) ? "System" : reader.GetString(1),
                                Action = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                LogTime = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetLogs): " + ex.Message);
                logs.Add(new LogEntry { LogId = 1, Actor = "Mock", Action = "ERROR", Description = "Koneksi database gagal / View vw_logs belum ada.", LogTime = DateTime.Now });
            }
            return logs;
        }

        public List<Supplier> GetSuppliers()
        {
            var list = new List<Supplier>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT supplier_id, company_name, address, phone, is_active FROM vw_suppliers ORDER BY company_name", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Supplier
                            {
                                SupplierId = reader.GetInt32(0),
                                CompanyName = reader.GetString(1),
                                Address = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                IsActive = reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetSuppliers): " + ex.Message);
            }
            return list;
        }

        public void AddSupplier(Supplier supplier)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO suppliers (company_name, address, phone) VALUES (@n, @a, @p)", conn))
                {
                    cmd.Parameters.AddWithValue("n", supplier.CompanyName);
                    cmd.Parameters.AddWithValue("a", supplier.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p", supplier.Phone ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SoftDeleteSupplier(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE suppliers SET is_active = false WHERE supplier_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Destination> GetDestinations()
        {
            var list = new List<Destination>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT destination_id, destination_name, address, is_active FROM vw_destinations ORDER BY destination_name", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Destination
                            {
                                DestinationId = reader.GetInt32(0),
                                DestinationName = reader.GetString(1),
                                Address = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                IsActive = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetDestinations): " + ex.Message);
            }
            return list;
        }

        public void AddDestination(Destination dest)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO destinations (destination_name, address) VALUES (@n, @a)", conn))
                {
                    cmd.Parameters.AddWithValue("n", dest.DestinationName);
                    cmd.Parameters.AddWithValue("a", dest.Address ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SoftDeleteDestination(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE destinations SET is_active = false WHERE destination_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<CoffeeType> GetCoffeeTypes()
        {
            var list = new List<CoffeeType>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT coffee_id, coffee_name, is_active FROM coffee_types WHERE is_active = true ORDER BY coffee_name", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CoffeeType
                            {
                                CoffeeId = reader.GetInt32(0),
                                CoffeeName = reader.GetString(1),
                                IsActive = reader.GetBoolean(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetCoffeeTypes): " + ex.Message);
            }
            return list;
        }
    }
}
