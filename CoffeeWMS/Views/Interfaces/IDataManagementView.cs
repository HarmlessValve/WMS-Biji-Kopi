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

        event EventHandler<(int coffeeId, int categoryId, int originId, int minimumStock, int? roastLevelId)> AddCoffeeProductRequested;
        event EventHandler<int> DeleteCoffeeProductRequested;

        // TAMBAHAN BARU: buat tambah origin dari aplikasi
        event EventHandler<CoffeeOrigin> AddCoffeeOriginRequested;

        void DisplaySuppliers(object dataSource);
        void DisplayDestinations(object dataSource);
        void DisplayCoffeeProducts(object dataSource);

        void PopulateCoffeeTypes(object dataSource);
        void PopulateCategories(object dataSource);
        void PopulateOrigins(object dataSource);

        void ShowMessage(string message, bool isError = false);

        void PopulateRoastLevels(object dataSource);
    }
}