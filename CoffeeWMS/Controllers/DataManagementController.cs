using System;
using CoffeeWMS.Models;
using CoffeeWMS.Views.Interfaces;
using CoffeeWMS.Repositories;

namespace CoffeeWMS.Controllers
{
    public class DataManagementController
    {
        private readonly IDataManagementView _view;
        private readonly MasterDataRepository _repo;

        public DataManagementController(IDataManagementView view, MasterDataRepository repo)
        {
            _view = view;
            _repo = repo;

            _view.LoadDataRequested += OnLoadDataRequested;
            _view.AddSupplierRequested += OnAddSupplierRequested;
            _view.DeleteSupplierRequested += OnDeleteSupplierRequested;
            _view.AddDestinationRequested += OnAddDestinationRequested;
            _view.DeleteDestinationRequested += OnDeleteDestinationRequested;

            // Event Kopi
            _view.AddCoffeeRequested += OnAddCoffeeRequested;
            _view.DeleteCoffeeRequested += OnDeleteCoffeeRequested;
        }

        private void OnLoadDataRequested(object sender, EventArgs e)
        {
            _view.DisplaySuppliers(_repo.GetSuppliers());
            _view.DisplayDestinations(_repo.GetDestinations());
            
            // Mengambil daftar Kopi & daftar Kategori dari database
            _view.DisplayCoffeeTypes(_repo.GetCoffees());
            _view.DisplayCoffeeCategories(_repo.GetCoffeeCategories());
        }

        private void OnAddSupplierRequested(object sender, Supplier e)
        {
            if (string.IsNullOrWhiteSpace(e.CompanyName)) {
                _view.ShowMessage("Nama Perusahaan wajib diisi!", true); return;
            }
            try {
                int adminId = Session.CurrentUser?.UserId ?? 1;
                _repo.AddSupplier(adminId, e);
                _view.ShowMessage("Supplier berhasil ditambahkan!");
                OnLoadDataRequested(this, EventArgs.Empty);
            } catch (Exception ex) { _view.ShowMessage("Gagal: " + ex.Message, true); }
        }

        private void OnDeleteSupplierRequested(object sender, int supplierId)
        {
            try {
                int adminId = Session.CurrentUser?.UserId ?? 1;
                _repo.SoftDeleteSupplier(adminId, supplierId);
                _view.ShowMessage("Supplier berhasil dihapus!");
                OnLoadDataRequested(this, EventArgs.Empty);
            } catch (Exception ex) { _view.ShowMessage("Gagal: " + ex.Message, true); }
        }

        private void OnAddDestinationRequested(object sender, Destination e)
        {
            if (string.IsNullOrWhiteSpace(e.DestinationName)) {
                _view.ShowMessage("Nama Destinasi wajib diisi!", true); return;
            }
            try {
                int adminId = Session.CurrentUser?.UserId ?? 1;
                _repo.AddDestination(adminId, e);
                _view.ShowMessage("Destinasi berhasil ditambahkan!");
                OnLoadDataRequested(this, EventArgs.Empty);
            } catch (Exception ex) { _view.ShowMessage("Gagal: " + ex.Message, true); }
        }

        private void OnDeleteDestinationRequested(object sender, int destId)
        {
            try {
                int adminId = Session.CurrentUser?.UserId ?? 1;
                _repo.SoftDeleteDestination(adminId, destId);
                _view.ShowMessage("Destinasi berhasil dihapus!");
                OnLoadDataRequested(this, EventArgs.Empty);
            } catch (Exception ex) { _view.ShowMessage("Gagal: " + ex.Message, true); }
        }

        // ==========================================
        // LOGIKA BARU UNTUK KOPI (Sudah Ternormalisasi)
        // ==========================================
        private void OnAddCoffeeRequested(object sender, Coffee e)
        {
            try
            {
                int adminId = Session.CurrentUser?.UserId ?? 1;
                _repo.AddCoffee(adminId, e);
                _view.ShowMessage("Jenis kopi berhasil ditambahkan!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menambah Jenis Kopi: " + ex.Message, true);
            }
        }

        private void OnDeleteCoffeeRequested(object sender, int coffeeId)
        {
            try
            {
                int adminId = Session.CurrentUser?.UserId ?? 1;
                _repo.SoftDeleteCoffee(adminId, coffeeId);
                _view.ShowMessage("Jenis kopi berhasil dihapus!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menghapus Jenis Kopi: " + ex.Message, true);
            }
        }
    }
}