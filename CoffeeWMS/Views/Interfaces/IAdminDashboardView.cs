using System;

namespace CoffeeWMS.Views.Interfaces
{
    public interface IAdminDashboardView
    {
        event EventHandler LoadDashboardRequested;
        
        void DisplayLogs(object dataSource);
        void DisplaySuppliers(object dataSource);
        void DisplayDestinations(object dataSource);
    }
}
