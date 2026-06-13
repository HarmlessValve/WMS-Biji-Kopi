using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using CoffeeWMS.Views;
using CoffeeWMS.Data;

namespace CoffeeWMS.Controllers
{
    public class PetugasDashboardController
    {
        private readonly PetugasDashboardForm _view;

        public PetugasDashboardController(PetugasDashboardForm view)
        {
            _view = view;
            _view.LoadDashboardRequested += OnLoadDashboardRequested;
        }

        private void OnLoadDashboardRequested(object sender, EventArgs e)
        {
            LoadSummary();
            LoadStokKopi();
        }

        private void LoadSummary()
        {
            int totalProduk = 0;
            int stokMenipis = 0;
            int masukHariIni = 0;
            int keluarHariIni = 0;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    
                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM vw_coffee_products", conn))
                        totalProduk = Convert.ToInt32(cmd.ExecuteScalar());

                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM vw_coffee_products WHERE current_quantity <= minimum_stock", conn))
                        stokMenipis = Convert.ToInt32(cmd.ExecuteScalar());

                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM vw_incoming_transactions WHERE DATE(tanggal) = CURRENT_DATE", conn))
                        masukHariIni = Convert.ToInt32(cmd.ExecuteScalar());

                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM vw_outgoing_transactions WHERE DATE(tanggal) = CURRENT_DATE", conn))
                        keluarHariIni = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (LoadSummary): " + ex.Message);
            }

            _view.DisplaySummary(totalProduk, stokMenipis, masukHariIni, keluarHariIni);
        }

        private void LoadStokKopi()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Nama Kopi");
            dataTable.Columns.Add("Species");
            dataTable.Columns.Add("Kategori");
            dataTable.Columns.Add("Origin");
            dataTable.Columns.Add("Roast Level");
            dataTable.Columns.Add("Stok Saat Ini", typeof(int));
            dataTable.Columns.Add("Minimum Stok", typeof(int));
            dataTable.Columns.Add("Status");

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT coffee_name, category_name, origin_name, roast_level_name, current_quantity, minimum_stock FROM vw_coffee_products ORDER BY coffee_name", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string species = reader.GetString(0);
                            string category = reader.GetString(1);
                            string origin = reader.GetString(2);
                            string roastLevel = reader.GetString(3);
                            int currentQuantity = reader.GetInt32(4);
                            int minimumStock = reader.GetInt32(5);

                            string namaKopi = $"{species} - {category} - {origin}";
                            if (roastLevel != "-")
                            {
                                namaKopi += $" ({roastLevel})";
                            }

                            string status = "Aman";
                            if (currentQuantity == 0)
                                status = "Habis";
                            else if (currentQuantity <= minimumStock)
                                status = "Menipis";

                            dataTable.Rows.Add(namaKopi, species, category, origin, roastLevel, currentQuantity, minimumStock, status);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error (LoadStokKopi): " + ex.Message);
            }

            _view.DisplayStokKopi(dataTable);
        }
    }
}
