using System;
using CoffeeWMS.Views.Interfaces;
using CoffeeWMS.Repositories;
using CoffeeWMS.Models;

namespace CoffeeWMS.Controllers
{
    public class LoginController
    {
        private readonly ILoginView _view;
        private readonly UserRepository _repo;

        public LoginController(ILoginView view, UserRepository repo)
        {
            _view = view;
            _repo = repo;

            _view.LoginAttempted += OnLoginAttempted;
        }

        private void OnLoginAttempted(object sender, EventArgs e)
        {
            var matchUser = _repo.AuthenticateUser(_view.Username, _view.Password);

            if (matchUser != null)
            {
                Session.CurrentUser = matchUser;
                _view.IsAuthenticated = true;
                _view.CloseView();
            }
            else
            {
                if (!string.IsNullOrEmpty(UserRepository.LastError))
                {
                    _view.ShowError("Terjadi error Database: " + UserRepository.LastError);
                }
                else
                {
                    _view.ShowError("Username atau password salah");
                }
            }
        }
    }
}
