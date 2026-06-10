using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using CoffeeWMS.Models;
using CoffeeWMS.Repositories;
using CoffeeWMS.Theme;

using CoffeeWMS.Views.Interfaces;

namespace CoffeeWMS.Views
{
    public partial class UserManagementForm : UserControl, IUserManagementView
    {
        
        private int _editingUserId = 0;
        private List<Role> _availableRoles;

        public event EventHandler LoadUsersRequested;
        public event EventHandler<UserManagementEventArgs> SaveUserRequested;
        public event EventHandler<int> DeleteUserRequested;

        public bool ShowInactive => chkShowInactive.Checked;

        public UserManagementForm()
        {
            InitializeComponent();
            btnTambah.Click += (s, e) => ShowForm(0);
            btnHapus.Click += (s, e) => {
                if (_editingUserId > 0)
                {
                    if (MessageBox.Show("Apakah Anda yakin ingin menghapus user ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        DeleteUserRequested?.Invoke(this, _editingUserId);
                    }
                }
            };
            btnBatal.Click += (s, e) => { pnlForm.Visible = false; };
        }

        public void TriggerLoad()
        {
            LoadUsersRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ChkShowInactive_CheckedChanged(object sender, EventArgs e)
        {
            TriggerLoad();
        }

        public void DisplayUsers(object dataSource)
        {
            dgvUsers.DataSource = dataSource;
        }

        public void SetAvailableRoles(List<Role> roles)
        {
            _availableRoles = roles;
            clbRoles.Items.Clear();
            foreach (var role in _availableRoles)
            {
                clbRoles.Items.Add(role.RoleName);
            }
        }

        private void ShowForm(int userId, User user = null)
        {
            _editingUserId = userId;
            pnlForm.Visible = true;
            
            // Reset checked state
            for (int i = 0; i < clbRoles.Items.Count; i++)
                clbRoles.SetItemChecked(i, false);
                
            if (userId == 0) // New
            {
                txtUsername.Text = "";
                txtPassword.Text = "";
                chkIsActive.Checked = true;
            }
            else // Edit
            {
                txtUsername.Text = user.Username;
                txtPassword.Text = "";
                chkIsActive.Checked = user.IsActive;

                if (!string.IsNullOrEmpty(user.RolesString))
                {
                    var rolesArr = user.RolesString.Split(',').Select(r => r.Trim()).ToList();
                    for (int i = 0; i < clbRoles.Items.Count; i++)
                    {
                        if (rolesArr.Contains(clbRoles.Items[i].ToString()))
                            clbRoles.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void DgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int userId = (int)dgvUsers.Rows[e.RowIndex].Cells["UserId"].Value;
                string username = dgvUsers.Rows[e.RowIndex].Cells["Username"].Value.ToString();
                string rolesStr = dgvUsers.Rows[e.RowIndex].Cells["Roles"].Value?.ToString() ?? "";
                string status = dgvUsers.Rows[e.RowIndex].Cells["Status"].Value.ToString();
                
                var mockUser = new User { Username = username, IsActive = (status == "Aktif"), RolesString = rolesStr };
                ShowForm(userId, mockUser);
            }
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            var selectedRoleIds = new List<int>();
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                if (clbRoles.GetItemChecked(i))
                {
                    string roleName = clbRoles.Items[i].ToString();
                    var role = _availableRoles.FirstOrDefault(r => r.RoleName == roleName);
                    if (role != null) selectedRoleIds.Add(role.RoleId);
                }
            }

            var args = new UserManagementEventArgs
            {
                UserId = _editingUserId,
                Username = txtUsername.Text,
                Password = txtPassword.Text,
                IsActive = chkIsActive.Checked,
                SelectedRoleIds = selectedRoleIds
            };
            SaveUserRequested?.Invoke(this, args);
        }

        public void CloseForm()
        {
            pnlForm.Visible = false;
        }

        public void ShowMessage(string message, bool isError = false)
        {
            MessageBox.Show(message, isError ? "Peringatan/Error" : "Info", MessageBoxButtons.OK, isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }
    }
}
