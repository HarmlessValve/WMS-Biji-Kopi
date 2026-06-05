using System;
using CoffeeWMS.Views.Interfaces;
using CoffeeWMS.Repositories;

namespace CoffeeWMS.Controllers
{
    public class AdminDashboardController
    {
        private readonly IAdminDashboardView _view;
        private readonly MasterDataRepository _repo;

        public AdminDashboardController(IAdminDashboardView view, MasterDataRepository repo)
        {
            _view = view;
            _repo = repo;

            _view.LoadDashboardRequested += OnLoadDashboardRequested;
        }

        private void OnLoadDashboardRequested(object sender, EventArgs e)
        {
            _view.DisplayLogs(_repo.GetLogs());
            _view.DisplaySuppliers(_repo.GetSuppliers());
            _view.DisplayDestinations(_repo.GetDestinations());
        }
    }
}
