using System;
using System.Windows.Forms;

namespace CoffeeWMS.Views.Interfaces
{
    public interface IMainView
    {
        event EventHandler ViewLoaded;
        event EventHandler LogoutRequested;
        
        void ShowDashboard();
        void ShowUserManagement();
        void LoadView(Control viewControl, string title);
        void CloseView();
    }
}
