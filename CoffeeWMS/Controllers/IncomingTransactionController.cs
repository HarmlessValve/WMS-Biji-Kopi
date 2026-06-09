using System;
using System.Collections.Generic;
using CoffeeWMS.Models;
using CoffeeWMS.Repositories;
using CoffeeWMS.Views.Interfaces;

namespace CoffeeWMS.Controllers
{
    public class IncomingTransactionController
    {
        private readonly IIncomingTransactionView _view;
        private readonly TransactionRepository _transactionRepo;
        private readonly MasterDataRepository _masterDataRepo;

        public IncomingTransactionController(IIncomingTransactionView view)
        {
            _view = view;
            _transactionRepo = new TransactionRepository();
            _masterDataRepo = new MasterDataRepository();

            _view.SaveEvent += OnSaveEvent;

            LoadData();
        }

        private void LoadData()
        {
            var suppliers = _masterDataRepo.GetSuppliers();
            _view.SetSupplierList(suppliers);

            var coffeeTypes = _masterDataRepo.GetCoffeeTypes();
            _view.SetCoffeeTypeList(coffeeTypes);
        }

        private void OnSaveEvent(object sender, EventArgs e)
        {
            try
            {
                if (_view.SelectedSupplierId <= 0)
                {
                    _view.ShowMessage("Silakan pilih Supplier.", true);
                    return;
                }

                if (_view.SelectedCoffeeId <= 0)
                {
                    _view.ShowMessage("Silakan pilih Jenis Kopi.", true);
                    return;
                }

                if (_view.Quantity <= 0)
                {
                    _view.ShowMessage("Quantity harus lebih dari 0.", true);
                    return;
                }

                int petugasId = Session.CurrentUser?.UserId ?? 0;
                if (petugasId <= 0)
                {
                    _view.ShowMessage("Petugas tidak valid. Silakan login kembali.", true);
                    return;
                }

                _transactionRepo.AddIncomingTransaction(
                    _view.SelectedSupplierId,
                    _view.SelectedCoffeeId,
                    _view.Quantity,
                    petugasId
                );

                _view.ShowMessage("Transaksi Penerimaan Kopi berhasil disimpan.");
                _view.ClearFields();
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menyimpan transaksi: " + ex.Message, true);
            }
        }
    }
}
