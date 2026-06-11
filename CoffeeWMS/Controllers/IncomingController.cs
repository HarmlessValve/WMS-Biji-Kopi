using System;
using System.Data;
using Npgsql;
using CoffeeWMS.Data;
using CoffeeWMS.Repositories;
using CoffeeWMS.Views.Interfaces;

namespace CoffeeWMS.Controllers
{
    public class IncomingController
    {
        private TransaksiRepository _repo;
        private IIncomingView _view;

        // Controller menerima View lewat konstruktor
        public IncomingController(IIncomingView view)
        {
            _repo = new TransaksiRepository();
            _view = view;

            // Berlangganan mendengarkan sinyal dari View
            _view.LoadDataRequested += OnLoadDataRequested;
            _view.AddIncomingRequested += OnAddIncomingRequested;
        }

        private void OnLoadDataRequested(object? sender, EventArgs e)
        {
            // Saat sinyal memuat data ditekan, ambil dari DB lalu lemparkan ke View
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    DataTable dtKopi = new DataTable();
                    using (var da = new NpgsqlDataAdapter("SELECT coffee_id, coffee_name FROM coffee_types WHERE is_active = true", conn))
                    { da.Fill(dtKopi); }
                    _view.PopulateCoffeeCombobox(dtKopi);

                    DataTable dtSupplier = new DataTable();
                    using (var da = new NpgsqlDataAdapter("SELECT supplier_id, company_name FROM suppliers WHERE is_active = true", conn))
                    { da.Fill(dtSupplier); }
                    _view.PopulateSupplierCombobox(dtSupplier);
                }

                DataTable dtGrid = _repo.GetDataPenerimaan();
                _view.DisplayTransactions(dtGrid);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal memuat data master: " + ex.Message, true);
            }
        }

        private void OnAddIncomingRequested(object? sender, Tuple<int, int, int> data)
        {
            // Ekstrak data dari Tuple: Item1 = SupplierId, Item2 = CoffeeId, Item3 = Jumlah
            int petugasId = 1; // Asumsi ID Session sementara
            
            bool sukses = _repo.InsertPenerimaan(data.Item1, data.Item2, data.Item3, petugasId);
            
            if (sukses)
            {
                _view.ShowMessage("Data penerimaan kopi berhasil disimpan!", false);
                
                // Segarkan grid dengan meminta data terbaru dari Repository
                DataTable dtGrid = _repo.GetDataPenerimaan();
                _view.DisplayTransactions(dtGrid);
            }
            else
            {
                _view.ShowMessage("Gagal menyimpan data ke database.", true);
            }
        }
    }
}