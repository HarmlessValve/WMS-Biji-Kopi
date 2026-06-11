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

        event EventHandler<(int coffeeId, int categoryId, string originName, int stock)> AddCoffeeProductRequested;
        event EventHandler<int> DeleteCoffeeProductRequested;

        void DisplaySuppliers(object dataSource);
        void DisplayDestinations(object dataSource);
        void DisplayCoffeeProducts(object dataSource);
        
        void PopulateCoffeeTypes(object dataSource);
        void PopulateCategories(object dataSource);

        void ShowMessage(string message, bool isError = false);
    }
}
