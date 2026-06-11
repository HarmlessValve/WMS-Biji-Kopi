using System;
using System.Data; // Ditambahkan agar sistem mengenali 'DataTable'

namespace CoffeeWMS.Views.Interfaces
{
    public interface IIncomingView
    {
        // Event untuk mengirim sinyal ke Controller
        event EventHandler LoadDataRequested;
        
        // Kita menggunakan Tuple untuk mengirim 3 data sekaligus dari Form ke Controller
        event EventHandler<Tuple<int, int, int>> AddIncomingRequested; 

        // Fungsi yang akan dipanggil oleh Controller untuk mengatur UI
        void ShowMessage(string message, bool isError = false);
        
        // Tanda // telah dihapus agar bisa digunakan
        void DisplayTransactions(DataTable data);
        void PopulateSupplierCombobox(DataTable data);
        void PopulateCoffeeCombobox(DataTable data);
    }
}