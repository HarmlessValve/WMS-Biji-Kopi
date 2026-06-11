using System;
using System.Collections.Generic;
using CoffeeWMS.Models;

namespace CoffeeWMS.Views.Interfaces
{
    public interface IUserManagementView
    {
        event EventHandler LoadUsersRequested;
        event EventHandler<UserManagementEventArgs> SaveUserRequested;
        event EventHandler<int> DeleteUserRequested;

        bool ShowInactive { get; }

        void DisplayUsers(object dataSource);
        void SetAvailableRoles(List<Role> roles);
        void ShowMessage(string message, bool isError = false);
        void CloseForm();
    }

    public class UserManagementEventArgs : EventArgs
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public List<int> SelectedRoleIds { get; set; }
    }
}
