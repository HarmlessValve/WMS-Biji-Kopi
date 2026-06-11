using System;
using CoffeeWMS.Models;

namespace CoffeeWMS.Views.Interfaces
{
    public interface IDataManagementView
    {
        event EventHandler LoadDataRequested;
        
        event EventHandler<Supplier> AddSupplierRequested;
        event EventHandler<int> DeleteSupplierRequested;
        
        event EventHandler<Destination> AddDestinationRequested;
        event EventHandler<int> DeleteDestinationRequested;

        // --- DIREVISI: Mengirim objek Coffee (Bukan sekadar string) ---
        event EventHandler<Coffee> AddCoffeeRequested;
        event EventHandler<int> DeleteCoffeeRequested;

        void DisplaySuppliers(object dataSource);
        void DisplayDestinations(object dataSource);
        
        // --- DIREVISI: Menampilkan data kopi dan kategori ---
        void DisplayCoffeeTypes(object dataSource);
        void DisplayCoffeeCategories(object dataSource); // Untuk Combobox Kategori
        
        void ShowMessage(string message, bool isError = false);
    }
}