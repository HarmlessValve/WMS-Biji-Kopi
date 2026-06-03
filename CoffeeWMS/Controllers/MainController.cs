using System;
using CoffeeWMS.Views.Interfaces;
using CoffeeWMS.Views;
using CoffeeWMS.Repositories;
using CoffeeWMS.Models;

namespace CoffeeWMS.Controllers
{
    public class MainController
    {
        private readonly IMainView _view;

        public MainController(IMainView view)
        {
            _view = view;
            
            _view.ViewLoaded += OnViewLoaded;
            _view.LogoutRequested += OnLogoutRequested;
        }

        private void OnViewLoaded(object sender, EventArgs e)
        {
            _view.ShowDashboard();
        }

        private void OnLogoutRequested(object sender, EventArgs e)
        {
            Session.CurrentUser = null;
            _view.CloseView();
        }
    }
}
