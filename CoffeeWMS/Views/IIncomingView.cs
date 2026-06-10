using System;

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
        
        // (Opsional nanti) void DisplayTransactions(DataTable data);
        // (Opsional nanti) void PopulateSupplierCombobox(DataTable data);
        // (Opsional nanti) void PopulateCoffeeCombobox(DataTable data);
    }
}