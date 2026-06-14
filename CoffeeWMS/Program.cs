using System;
using System.Windows.Forms;
using CoffeeWMS.Views;
using CoffeeWMS.Controllers;


namespace CoffeeWMS;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        bool isLogout = true;
        while (isLogout)
        {
            isLogout = false;
            var loginView = new LoginForm();
            var loginController = new LoginController(loginView);

            if (loginView.ShowDialog() == DialogResult.OK)
            {
                var mainView = new MainForm();
                var mainController = new MainController(mainView);
                Application.Run(mainView);
                
                if (mainView.DialogResult == DialogResult.Retry)
                {
                    isLogout = true;
                }
            }
        }
    }
}