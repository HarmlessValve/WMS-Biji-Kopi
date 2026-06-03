using System;
using System.Windows.Forms;
using CoffeeWMS.Views;
using CoffeeWMS.Controllers;
using CoffeeWMS.Repositories;

namespace CoffeeWMS;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        var loginView = new LoginForm();
        var loginController = new LoginController(loginView, new UserRepository());

        if (loginView.ShowDialog() == DialogResult.OK)
        {
            var mainView = new MainForm();
            var mainController = new MainController(mainView);
            Application.Run(mainView);
        }
    }    
}