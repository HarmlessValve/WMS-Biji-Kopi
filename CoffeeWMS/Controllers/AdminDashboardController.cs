using System;
using System.Collections.Generic;
using Npgsql;
using CoffeeWMS.Views;
using CoffeeWMS.Models;
using CoffeeWMS.Data;

namespace CoffeeWMS.Controllers
{
    public class AdminDashboardController
    {
        private readonly AdminDashboardForm _view;

        public AdminDashboardController(AdminDashboardForm view)
        {
            _view = view;

            _view.LoadDashboardRequested += OnLoadDashboardRequested;
        }

        private void OnLoadDashboardRequested(object sender, EventArgs e)
        {
            _view.DisplayLogs(GetLogs());
            _view.DisplaySuppliers(GetSuppliers());
            _view.DisplayDestinations(GetDestinations());
        }

        private List<LogEntry> GetLogs()
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

        private List<Supplier> GetSuppliers()
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

        private List<Destination> GetDestinations()
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
    }
}
