using System;
using System.Linq;
using CoffeeWMS.Views.Interfaces;
using CoffeeWMS.Repositories;

namespace CoffeeWMS.Controllers
{
    public class UserManagementController
    {
        private readonly IUserManagementView _view;
        private readonly UserRepository _repo;

        public UserManagementController(IUserManagementView view, UserRepository repo)
        {
            _view = view;
            _repo = repo;

            _view.LoadUsersRequested += OnLoadUsersRequested;
            _view.SaveUserRequested += OnSaveUserRequested;
        }

        private void OnLoadUsersRequested(object sender, EventArgs e)
        {
            var users = _repo.GetAllUsers();
            
            var displayList = users.Select(u => new {
                u.UserId,
                u.Username,
                Status = u.IsActive ? "Aktif" : "Nonaktif",
                TanggalDibuat = u.CreatedAt.ToString("dd/MM/yyyy"),
                Roles = u.RolesString
            }).ToList();
            
            _view.DisplayUsers(displayList);
            _view.SetAvailableRoles(_repo.GetAllRoles());
        }

        private void OnSaveUserRequested(object sender, UserManagementEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Username))
            {
                _view.ShowMessage("Username tidak boleh kosong!", true);
                return;
            }
            if (e.UserId == 0 && string.IsNullOrWhiteSpace(e.Password))
            {
                _view.ShowMessage("Password tidak boleh kosong untuk user baru!", true);
                return;
            }

            try
            {
                if (e.UserId == 0)
                {
                    _repo.AddUser(e.Username, e.Password, e.SelectedRoleIds.ToArray());
                    _view.ShowMessage("Berhasil ditambahkan!", false);
                }
                else
                {
                    _repo.UpdateUser(e.UserId, e.Username, e.Password, e.IsActive, e.SelectedRoleIds.ToArray());
                    _view.ShowMessage("Berhasil disimpan!", false);
                }
                _view.CloseForm();
                OnLoadUsersRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menyimpan ke database (pastikan koneksi/mocking sudah benar). Detail: " + ex.Message, true);
                _view.CloseForm();
            }
        }
    }
}
