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
            _view.DeleteUserRequested += OnDeleteUserRequested;
        }

        private void OnDeleteUserRequested(object sender, int userId)
        {
            try
            {
                _repo.SoftDeleteUser(userId);
                _view.ShowMessage("Berhasil dihapus!", false);
                _view.CloseForm();
                OnLoadUsersRequested(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Gagal menghapus user: " + ex.Message, true);
            }
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
            if (e.Username.Length < 8 || !e.Username.All(char.IsLetter))
            {
                _view.ShowMessage("Username minimal 8 karakter dan hanya boleh berisi huruf!", true);
                return;
            }

            if (e.UserId == 0 && string.IsNullOrWhiteSpace(e.Password))
            {
                _view.ShowMessage("Password tidak boleh kosong untuk user baru!", true);
                return;
            }
            if (!string.IsNullOrWhiteSpace(e.Password) && e.Password.Length < 8)
            {
                _view.ShowMessage("Password minimal 8 karakter!", true);
                return;
            }
            if (e.SelectedRoleIds == null || !e.SelectedRoleIds.Any())
            {
                _view.ShowMessage("Gagal menyimpan! Pengguna minimal harus memiliki 1 Role.", true);
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
