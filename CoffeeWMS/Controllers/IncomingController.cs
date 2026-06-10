using System;
using CoffeeWMS.Models;
using CoffeeWMS.Views.Interfaces;
using CoffeeWMS.Repositories;

namespace CoffeeWMS.Controllers
{
    public class IncomingController
    {
        private readonly IIncomingView _view;
        private readonly TransactionRepository _repo;

        public IncomingController(IIncomingView view, TransactionRepository repo)
        {
            _view = view;
            _repo = repo;

            // Mendaftarkan event dari View
            _view.AddIncomingRequested += OnAddIncomingRequested;
        }

        private void OnAddIncomingRequested(object sender, Tuple<int, int, int> e)
        {
            int supplierId = e.Item1;
            int coffeeId = e.Item2;
            int quantity = e.Item3;

            // Validasi dasar di Controller
            if (quantity <= 0)
            {
                _view.ShowMessage("Jumlah (Kg) harus lebih dari 0!", true);
                return;
            }

            try
            {
                // Mengambil ID User yang sedang login (seperti di DataManagementController)
                int petugasId = Session.CurrentUser?.UserId ?? 1;

                // Memanggil Repository yang menjalankan prosedur PostgreSQL
                _repo.AddIncomingTransaction(supplierId, coffeeId, quantity, petugasId);
                
                _view.ShowMessage("Data Penerimaan berhasil disimpan!");
                
                // Nanti bisa dipanggil event untuk me-refresh DataGridView di sini
            }
            catch (Exception ex)
            {
                // Menangkap pesan error, termasuk error validasi dari PostgreSQL
                _view.ShowMessage("Gagal menambah penerimaan: " + ex.Message, true);
            }
        }
    }
}