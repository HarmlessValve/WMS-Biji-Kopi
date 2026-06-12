using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using CoffeeWMS.Models;
using CoffeeWMS.Views;
using CoffeeWMS.Data;

namespace CoffeeWMS.Controllers
{
    public class DataManagementController
    {
        private readonly DataManagementForm _view;

        public DataManagementController(DataManagementForm view)
        {
            _view = view;

            _view.LoadDataRequested += OnLoadDataRequested;

            _view.AddSupplierRequested += OnAddSupplierRequested;
            _view.DeleteSupplierRequested += OnDeleteSupplierRequested;

            _view.AddDestinationRequested += OnAddDestinationRequested;
            _view.DeleteDestinationRequested += OnDeleteDestinationRequested;

            _view.AddCoffeeProductRequested += OnAddCoffeeProductRequested;
            _view.DeleteCoffeeProductRequested += OnDeleteCoffeeProductRequested;

            _view.AddCoffeeOriginRequested += OnAddCoffeeOriginRequested;
        }

        private void OnLoadDataRequested(object sender, EventArgs e)
        {
            _view.DisplaySuppliers(GetSuppliers());
            _view.DisplayDestinations(GetDestinations());
            _view.DisplayCoffeeProducts(GetCoffeeProducts());

            _view.PopulateCoffeeTypes(GetCoffeeTypes());
            _view.PopulateCategories(GetCoffeeCategories());
            _view.PopulateOrigins(GetCoffeeOrigins());
            _view.PopulateRoastLevels(GetRoastLevels());
        }

        private void OnAddSupplierRequested(object sender, Supplier e)
        {
            if (string.IsNullOrWhiteSpace(e.CompanyName))
            {
                _view.ShowMessage("Nama Perusahaan (Supplier) wajib diisi!", true);
                return;
            }

            try
            {
                int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                AddSupplier(adminId, e);
                _view.ShowMessage("Supplier berhasil ditambahkan!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menambah Supplier: " + ex.Message, true);
            }
        }

        private void OnDeleteSupplierRequested(object sender, int supplierId)
        {
            try
            {
                int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                SoftDeleteSupplier(adminId, supplierId);
                _view.ShowMessage("Supplier berhasil dihapus!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menghapus Supplier: " + ex.Message, true);
            }
        }

        private void OnAddDestinationRequested(object sender, Destination e)
        {
            if (string.IsNullOrWhiteSpace(e.DestinationName))
            {
                _view.ShowMessage("Nama Destinasi wajib diisi!", true);
                return;
            }

            try
            {
                int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                AddDestination(adminId, e);
                _view.ShowMessage("Destinasi berhasil ditambahkan!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menambah Destinasi: " + ex.Message, true);
            }
        }

        private void OnDeleteDestinationRequested(object sender, int destId)
        {
            try
            {
                int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                SoftDeleteDestination(adminId, destId);
                _view.ShowMessage("Destinasi berhasil dihapus!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menghapus Destinasi: " + ex.Message, true);
            }
        }

        private void OnAddCoffeeProductRequested(object sender, (int coffeeId, int categoryId, int originId, int minimumStock, int? roastLevelId) data)
        {
            try
            {
                int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                AddCoffeeProduct(adminId, data.coffeeId, data.categoryId, data.originId, data.minimumStock, data.roastLevelId);
                _view.ShowMessage("Produk Kopi berhasil ditambahkan!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menambah Produk Kopi: kemungkinan kombinasi sudah ada atau error lain. " + ex.Message, true);
            }
        }

        private void OnDeleteCoffeeProductRequested(object sender, int productId)
        {
            try
            {
                int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                SoftDeleteCoffeeProduct(adminId, productId);
                _view.ShowMessage("Produk Kopi berhasil dihapus!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menghapus Produk Kopi: " + ex.Message, true);
            }
        }

        private void OnAddCoffeeOriginRequested(object sender, CoffeeOrigin origin)
        {
            if (string.IsNullOrWhiteSpace(origin.OriginName))
            {
                _view.ShowMessage("Nama origin wajib diisi!", true);
                return;
            }

            try
            {
                int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                AddCoffeeOrigin(adminId, origin);
                _view.ShowMessage("Origin kopi berhasil ditambahkan!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menambah origin kopi: " + ex.Message, true);
            }
        }

        // --- Database Logic ---
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return list;
        }

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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return list;
        }

        public void AddCoffeeProduct(int adminId, int coffeeId, int categoryId, int originId, int minimumStock, int? roastLevelId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO coffee_products (coffee_id, category_id, origin_id, roast_level_id, current_quantity, minimum_stock, last_updated, is_active) VALUES (@cf, @ct, @o, @rl, 0, @m, NOW(), true)", conn))
                {
                    cmd.Parameters.AddWithValue("cf", coffeeId);
                    cmd.Parameters.AddWithValue("ct", categoryId);
                    cmd.Parameters.AddWithValue("o", originId);
                    cmd.Parameters.AddWithValue("rl", roastLevelId.HasValue ? roastLevelId.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("m", minimumStock);
                    cmd.ExecuteNonQuery();
                }

                using (var logCmd = new NpgsqlCommand("INSERT INTO activity_logs (user_id, action, description) VALUES (@u, 'TAMBAH', 'Tambah Produk Kopi')", conn))
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return list;
        }

        public List<RoastLevel> GetRoastLevels()
        {
            var list = new List<RoastLevel>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT roast_level_id, roast_level_name, description, is_active FROM roast_levels WHERE is_active = true ORDER BY roast_level_id", conn))
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return list;
        }

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
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return list;
        }

        public void AddCoffeeOrigin(int adminId, CoffeeOrigin origin)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO coffee_origins (origin_name, region, description, is_active) VALUES (@n, @r, @d, true)", conn))
                {
                    cmd.Parameters.AddWithValue("n", origin.OriginName);
                    cmd.Parameters.AddWithValue("r", origin.Region ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("d", origin.Description ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                    
                    using (var logCmd = new NpgsqlCommand("INSERT INTO activity_logs (user_id, action, description) VALUES (@u, 'TAMBAH', 'Tambah Coffee Origin: ' || @n)", conn))
                    {
                        logCmd.Parameters.AddWithValue("u", adminId);
                        logCmd.Parameters.AddWithValue("n", origin.OriginName);
                        logCmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
