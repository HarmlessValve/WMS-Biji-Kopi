using System;
using System.Collections.Generic;
using CoffeeWMS.Models;

namespace CoffeeWMS.Views.Interfaces
{
    public interface IIncomingTransactionView
    {
        int SelectedSupplierId { get; set; }
        int SelectedCoffeeId { get; set; }
        int Quantity { get; set; }

        event EventHandler SaveEvent;

        void SetSupplierList(List<Supplier> suppliers);
        void SetCoffeeTypeList(List<CoffeeType> coffeeTypes);

        void ShowMessage(string message, bool isError = false);
        void ClearFields();
    }
}
