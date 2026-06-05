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

        void DisplaySuppliers(object dataSource);
        void DisplayDestinations(object dataSource);
        void ShowMessage(string message, bool isError = false);
    }
}
