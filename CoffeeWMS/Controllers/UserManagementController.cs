using System;
using System.Linq;
using System.Collections.Generic;
using Npgsql;
using CoffeeWMS.Views;
using CoffeeWMS.Models;
using CoffeeWMS.Data;

namespace CoffeeWMS.Controllers
{
    public class UserManagementController
    {
        private readonly UserManagementForm _view;

        public UserManagementController(UserManagementForm view)
        {
            _view = view;

            _view.LoadUsersRequested += OnLoadUsersRequested;
            _view.SaveUserRequested += OnSaveUserRequested;
            _view.DeleteUserRequested += OnDeleteUserRequested;
        }

        private void OnDeleteUserRequested(object sender, int userId)
        {
            try
            {
                int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                SoftDeleteUser(adminId, userId);
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
            var users = GetAllUsers(!_view.ShowInactive);
            
            var displayList = users.Select(u => new {
                u.UserId,
                u.Username,
                Status = u.IsActive ? "Aktif" : "Nonaktif",
                TanggalDibuat = u.CreatedAt.ToString("dd/MM/yyyy"),
                Roles = u.RolesString
            }).ToList();
            
            _view.DisplayUsers(displayList);
            _view.SetAvailableRoles(GetAllRoles());
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
                    int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                    AddUser(adminId, e.Username, e.Password, e.SelectedRoleIds.ToArray());
                    _view.ShowMessage("Berhasil ditambahkan!", false);
                }
                else
                {
                    int adminId = CoffeeWMS.Models.Session.CurrentUser?.UserId ?? 1;
                    UpdateUser(adminId, e.UserId, e.Username, e.Password, e.IsActive, e.SelectedRoleIds.ToArray());
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

        private List<User> GetAllUsers(bool isActive = true)
        {
            var users = new List<User>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT user_id, username, is_active, created_at, roles_string FROM vw_user_roles WHERE is_active = @isActive ORDER BY username", conn))
                    {
                        cmd.Parameters.AddWithValue("isActive", isActive);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    IsActive = reader.GetBoolean(2),
                                    CreatedAt = reader.GetDateTime(3),
                                    RolesString = reader.IsDBNull(4) ? "" : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error: " + ex.Message);
                if (users.Count == 0)
                {
                    users.Add(new User { UserId = 1, Username = "admin", IsActive = true, RolesString = "Admin" });
                    users.Add(new User { UserId = 2, Username = "manager", IsActive = true, RolesString = "Manager" });
                    users.Add(new User { UserId = 3, Username = "petugas", IsActive = true, RolesString = "Petugas" });
                }
            }
            return users;
        }

        private List<Role> GetAllRoles()
        {
            var roles = new List<Role>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT role_id, role_name, description FROM vw_roles", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roles.Add(new Role
                                {
                                    RoleId = reader.GetInt32(0),
                                    RoleName = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                roles.Add(new Role { RoleId = 1, RoleName = "Admin" });
                roles.Add(new Role { RoleId = 2, RoleName = "Manager" });
                roles.Add(new Role { RoleId = 3, RoleName = "Petugas" });
            }
            return roles;
        }

        private void AddUser(int adminId, string username, string password, int[] roleIds)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_add_user(@admin_id, @u, @p, @r)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("p", password);
                    cmd.Parameters.AddWithValue("r", roleIds);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateUser(int adminId, int userId, string username, string password, bool isActive, int[] roleIds)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_update_user(@admin_id, @i, @u, @p, @a, @r)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
                    cmd.Parameters.AddWithValue("i", userId);
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("p", password ?? "");
                    cmd.Parameters.AddWithValue("a", isActive);
                    cmd.Parameters.AddWithValue("r", roleIds);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SoftDeleteUser(int adminId, int userId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("CALL sp_soft_delete_user(@admin_id, @i)", conn))
                {
                    cmd.Parameters.AddWithValue("admin_id", adminId);
                    cmd.Parameters.AddWithValue("i", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}


namespace CoffeeWMS.Controllers
{
    public class UserManagementEventArgs : EventArgs
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public List<int> SelectedRoleIds { get; set; }
    }
}
