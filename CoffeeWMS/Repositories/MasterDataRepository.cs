using System;
using System.Collections.Generic;
using Npgsql;
using CoffeeWMS.Models;
using CoffeeWMS.Data;

namespace CoffeeWMS.Repositories
{
    public class MasterDataRepository
    {
        // ====================================================================
        // LOGS — dari view vw_logs
        // ====================================================================
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

        // ====================================================================
        // SUPPLIERS — view vw_suppliers + SP sp_add_supplier / sp_soft_delete_supplier
        // ====================================================================
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
                using (var cmd = new NpgsqlCommand("CALL sp_add_supplier(@n, @a, @p)", conn))
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
                using (var cmd = new NpgsqlCommand("CALL sp_soft_delete_supplier(@id)", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ====================================================================
        // DESTINATIONS — view vw_destinations + SP sp_add_destination / sp_soft_delete_destination
        // ====================================================================
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
                using (var cmd = new NpgsqlCommand("CALL sp_add_destination(@n, @a)", conn))
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
                using (var cmd = new NpgsqlCommand("CALL sp_soft_delete_destination(@id)", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ====================================================================
        // COFFEE TYPES — view vw_coffee_types + SP sp_add_coffee_type / sp_soft_delete_coffee_type
        // ====================================================================
        public List<Coffee> GetCoffees()
        {
            var list = new List<Coffee>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT coffee_id, coffee_name, category_id, category_name, minimum_stock, is_active FROM vw_coffee_types ORDER BY coffee_name", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Coffee
                            {
                                CoffeeId = reader.GetInt32(0),
                                CoffeeName = reader.GetString(1),
                                CategoryId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                                CategoryName = reader.IsDBNull(3) ? "Tanpa Kategori" : reader.GetString(3),
                                MinimumStock = reader.GetInt32(4),
                                IsActive = reader.GetBoolean(5)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetCoffees): " + ex.Message);
            }
            return list;
        }

        public void AddCoffee(Coffee coffee)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_add_coffee_type(@n, @c, @m)", conn))
                {
                    cmd.Parameters.AddWithValue("n", coffee.CoffeeName);
                    cmd.Parameters.AddWithValue("c", coffee.CategoryId.HasValue ? (object)coffee.CategoryId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("m", coffee.MinimumStock);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SoftDeleteCoffee(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_soft_delete_coffee_type(@id)", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ====================================================================
        // COFFEE CATEGORIES — langsung dari tabel coffee_categories
        // ====================================================================
        public List<CoffeeCategory> GetCoffeeCategories()
        {
            var list = new List<CoffeeCategory>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT category_id, category_name, description FROM coffee_categories ORDER BY category_name", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CoffeeCategory
                            {
                                CategoryId = reader.GetInt32(0),
                                CategoryName = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetCoffeeCategories): " + ex.Message);
            }
            return list;
        }

        // ====================================================================
        // STOCK SUMMARY — dari view stock_summary
        // ====================================================================
        public List<StockSummary> GetStockSummary()
        {
            var list = new List<StockSummary>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT coffee_id, coffee_name, category_name, current_quantity, minimum_stock, status FROM stock_summary", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new StockSummary
                            {
                                CoffeeId = reader.GetInt32(0),
                                CoffeeName = reader.GetString(1),
                                CategoryName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                CurrentQuantity = reader.GetInt32(3),
                                MinimumStock = reader.GetInt32(4),
                                Status = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetStockSummary): " + ex.Message);
            }
            return list;
        }

        // ====================================================================
        // DASHBOARD SUMMARY — dari view vw_dashboard_summary
        // ====================================================================
        public DashboardSummary GetDashboardSummary()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT total_coffee_types, total_suppliers, total_destinations, total_incoming, total_outgoing, total_low_stock FROM vw_dashboard_summary", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DashboardSummary
                            {
                                TotalCoffeeTypes = reader.GetInt32(0),
                                TotalSuppliers = reader.GetInt32(1),
                                TotalDestinations = reader.GetInt32(2),
                                TotalIncoming = reader.GetInt32(3),
                                TotalOutgoing = reader.GetInt32(4),
                                TotalLowStock = reader.GetInt32(5)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetDashboardSummary): " + ex.Message);
            }
            return new DashboardSummary(); // Return default jika gagal
        }
    }
}