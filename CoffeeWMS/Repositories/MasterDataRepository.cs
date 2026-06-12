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

        public void AddSupplier(int adminId, Supplier supplier)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_add_supplier(@admin_id, @n, @a, @p)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
                    cmd.Parameters.AddWithValue("n", supplier.CompanyName);
                    cmd.Parameters.AddWithValue("a", supplier.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p", supplier.Phone ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SoftDeleteSupplier(int adminId, int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_soft_delete_supplier(@admin_id, @id)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
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

        public void AddDestination(int adminId, Destination dest)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_add_destination(@admin_id, @n, @a)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
                    cmd.Parameters.AddWithValue("n", dest.DestinationName);
                    cmd.Parameters.AddWithValue("a", dest.Address ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SoftDeleteDestination(int adminId, int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_soft_delete_destination(@admin_id, @id)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ====================================================================
        // COFFEE TYPES — view vw_coffee_types + SP sp_add_coffee_type / sp_soft_delete_coffee_type
        // ====================================================================
        public List<CoffeeType> GetCoffeeTypes()
        {
            var list = new List<CoffeeType>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT coffee_id, coffee_name, is_active FROM coffee_types ORDER BY coffee_name", conn))
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

        public void AddCoffeeType(int adminId, CoffeeType coffee)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_add_coffee_type(@admin_id, @n)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
                    cmd.Parameters.AddWithValue("n", coffee.CoffeeName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SoftDeleteCoffee(int adminId, int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_soft_delete_coffee_type(@admin_id, @id)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ====================================================================
        // COFFEE PRODUCTS — view vw_coffee_products + SP sp_add_coffee_product
        // ====================================================================
        public List<CoffeeProduct> GetCoffeeProducts()
        {
            var list = new List<CoffeeProduct>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT product_id, coffee_name, category_id, category_name, origin_id, origin_name, roast_level_id, roast_level_name, current_quantity, minimum_stock, is_active FROM vw_coffee_products ORDER BY coffee_name", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CoffeeProduct
                            {
                                ProductId = reader.GetInt32(0),
                                CoffeeName = reader.GetString(1),

                                CategoryId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                                CategoryName = reader.IsDBNull(3) ? "Tanpa Kategori" : reader.GetString(3),

                                OriginId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                OriginName = reader.IsDBNull(5) ? "Tanpa Origin" : reader.GetString(5),

                                RoastLevelId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                RoastLevelName = reader.IsDBNull(7) ? "-" : reader.GetString(7),

                                CurrentQuantity = reader.GetInt32(8),
                                MinimumStock = reader.GetInt32(9),
                                IsActive = reader.GetBoolean(10)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetCoffeeProducts): " + ex.Message);
            }
            return list;
        }

        public void AddCoffeeProduct(int adminId, int coffeeId, int categoryId, int originId, int minimumStock, int? roastLevelId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                @"INSERT INTO coffee_products 
                    (coffee_id, category_id, origin_id, roast_level_id, current_quantity, minimum_stock, last_updated, is_active)
                VALUES 
                    (@cf, @ct, @o, @rl, 0, @m, NOW(), true)", conn))
                {
                    cmd.Parameters.AddWithValue("cf", coffeeId);
                    cmd.Parameters.AddWithValue("ct", categoryId);
                    cmd.Parameters.AddWithValue("o", originId);
                    cmd.Parameters.AddWithValue("rl", roastLevelId.HasValue ? roastLevelId.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("m", minimumStock);

                    cmd.ExecuteNonQuery();
                }

                using (var logCmd = new NpgsqlCommand(
                    "INSERT INTO activity_logs (user_id, action, description) VALUES (@u, 'TAMBAH', 'Tambah Produk Kopi')", conn))
                {
                    logCmd.Parameters.AddWithValue("u", adminId);
                    logCmd.ExecuteNonQuery();
                }
            }
        }

        public void SoftDeleteCoffeeProduct(int adminId, int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_soft_delete_coffee_product(@admin_id, @id)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
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
        // COFFEE ORIGINS — langsung dari tabel coffee_origins
        // ====================================================================
        public List<RoastLevel> GetRoastLevels()
        {
            var list = new List<RoastLevel>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(
                        "SELECT roast_level_id, roast_level_name, description, is_active FROM roast_levels WHERE is_active = true ORDER BY roast_level_id", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RoastLevel
                            {
                                RoastLevelId = reader.GetInt32(0),
                                RoastLevelName = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                IsActive = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetRoastLevels): " + ex.Message);
            }

            return list;
        }

        // ====================================================================
        // COFFEE ORIGINS — langsung dari tabel coffee_origins
        // ====================================================================
        public List<CoffeeOrigin> GetCoffeeOrigins()
        {
            var list = new List<CoffeeOrigin>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT origin_id, origin_name, region, description, is_active FROM coffee_origins WHERE is_active = true ORDER BY origin_name", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CoffeeOrigin
                            {
                                OriginId = reader.GetInt32(0),
                                OriginName = reader.GetString(1),
                                Region = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                IsActive = reader.GetBoolean(4)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (GetCoffeeOrigins): " + ex.Message);
            }
            return list;
        }

        public void AddCoffeeOrigin(int adminId, CoffeeOrigin origin)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                // We'll try direct insert or sp_add_coffee_origin if it exists. 
                // Using an INSERT INTO with RETURNING or just simple INSERT if SP isn't around.
                // Since this might not have a stored procedure, let's use standard INSERT for safety if it fails, but looking at others we'll try SP first.
                // Wait, others use sp_. I will assume an insert for origin, but maybe sp_add_coffee_origin is safer. 
                // Let's use INSERT because there is no guarantee `sp_add_coffee_origin` exists, wait, let's use INSERT to be safe because origins just inserted. 
                using (var cmd = new NpgsqlCommand("INSERT INTO coffee_origins (origin_name, region, description, is_active) VALUES (@n, @r, @d, true)", conn))
                {
                    cmd.Parameters.AddWithValue("n", origin.OriginName);
                    cmd.Parameters.AddWithValue("r", origin.Region ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("d", origin.Description ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                    
                    // We also need to log this if we bypassed SP. 
                    using (var logCmd = new NpgsqlCommand("INSERT INTO activity_logs (user_id, action, description) VALUES (@u, 'TAMBAH', 'Tambah Coffee Origin: ' || @n)", conn))
                    {
                        logCmd.Parameters.AddWithValue("u", adminId);
                        logCmd.Parameters.AddWithValue("n", origin.OriginName);
                        logCmd.ExecuteNonQuery();
                    }
                }
            }
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
                    using (var cmd = new NpgsqlCommand("SELECT product_id, coffee_name, category_name, origin_name, roast_level_name, current_quantity, minimum_stock, status FROM stock_summary", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new StockSummary
                            {
                                ProductId = reader.GetInt32(0),
                                CoffeeName = reader.GetString(1),
                                CategoryName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                OriginName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                RoastLevelName = reader.IsDBNull(4) ? "-" : reader.GetString(4),
                                CurrentQuantity = reader.GetInt32(5),
                                MinimumStock = reader.GetInt32(6),
                                Status = reader.GetString(7)
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