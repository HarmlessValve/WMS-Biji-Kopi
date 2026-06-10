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
        }

        private void OnLoadDataRequested(object sender, EventArgs e)
        {
            _view.DisplaySuppliers(_repo.GetSuppliers());
            _view.DisplayDestinations(_repo.GetDestinations());
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
                _repo.AddSupplier(adminId, e);
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
                _repo.SoftDeleteSupplier(adminId, supplierId);
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
                _repo.AddDestination(adminId, e);
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
                _repo.SoftDeleteDestination(adminId, destId);
                _view.ShowMessage("Destinasi berhasil dihapus!");
                OnLoadDataRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menghapus Destinasi: " + ex.Message, true);
            }
        }
    }
}
