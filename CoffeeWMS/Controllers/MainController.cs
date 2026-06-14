using System;
using CoffeeWMS.Views;
using CoffeeWMS.Models;

namespace CoffeeWMS.Controllers
{
    public class MainController
    {
        private readonly MainForm _view;

        public MainController(MainForm view)
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
            _view.DialogResult = System.Windows.Forms.DialogResult.Retry;
            _view.CloseView();
        }
    }
}
