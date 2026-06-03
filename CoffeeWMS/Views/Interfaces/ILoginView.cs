using System;

namespace CoffeeWMS.Views.Interfaces
{
    public interface ILoginView
    {
        string Username { get; }
        string Password { get; }
        bool IsAuthenticated { get; set; }
        
        event EventHandler LoginAttempted;

        void ShowError(string message);
        void CloseView();
    }
}
