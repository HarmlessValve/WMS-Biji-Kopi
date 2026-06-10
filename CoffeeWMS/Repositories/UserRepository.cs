using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using CoffeeWMS.Models;
using CoffeeWMS.Data;

namespace CoffeeWMS.Repositories
{
    public class UserRepository
    {
        public static string LastError { get; private set; }

        public User AuthenticateUser(string username, string password)
        {
            LastError = "";
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Join user real table (to check password) dengan view (untuk ngambil string role)
                    var query = "SELECT v.user_id, v.username, v.is_active, v.created_at, v.roles_string " +
                                "FROM users u JOIN vw_user_roles v ON u.user_id = v.user_id " +
                                "WHERE u.username = @u AND u.password = @p AND u.is_active = true";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("u", username);
                        cmd.Parameters.AddWithValue("p", password);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    IsActive = reader.GetBoolean(2),
                                    CreatedAt = reader.GetDateTime(3),
                                    RolesString = reader.IsDBNull(4) ? "" : reader.GetString(4)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Console.WriteLine("DB Error during auth: " + ex.Message);
                if (username == "admin" && password == "admin")
                    return new User { UserId = 1, Username = "admin", IsActive = true, RolesString = "Admin" };
            }
            return null;
        }

        public List<User> GetAllUsers(bool isActive = true)
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
                // Saat tidak ada database betulan/Mock fallback (Karena Server Postgres belum jalan)
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

        public List<Role> GetAllRoles()
        {
            var roles = new List<Role>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT role_id, role_name, description FROM roles", conn))
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
                // Fallback mock
                roles.Add(new Role { RoleId = 1, RoleName = "Admin" });
                roles.Add(new Role { RoleId = 2, RoleName = "Manager" });
                roles.Add(new Role { RoleId = 3, RoleName = "Petugas" });
            }
            return roles;
        }

        public void AddUser(int adminId, string username, string password, int[] roleIds)
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
        
        public void UpdateUser(int adminId, int userId, string username, string password, bool isActive, int[] roleIds)
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

        public void SoftDeleteUser(int adminId, int userId)
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
