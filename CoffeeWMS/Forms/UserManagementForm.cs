using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using CoffeeWMS.Models;
using CoffeeWMS.Repositories;
using CoffeeWMS.Theme;

namespace CoffeeWMS.Forms
{
    public class UserManagementForm : UserControl
    {
        private DataGridView dgvUsers;
        private PrimaryButton btnTambah;
        private PrimaryButton btnSimpan;
        private PrimaryButton btnBatal;
        
        private Panel pnlForm;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkIsActive;
        private CheckedListBox clbRoles;
        
        private UserRepository _repo;
        private int _editingUserId = 0;
        private List<Role> _availableRoles;

        public UserManagementForm()
        {
            _repo = new UserRepository();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = DesignTokens.Background;
            this.Font = DesignTokens.BodyFont;

            // Top action bar
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60 };
            btnTambah = new PrimaryButton { Text = "+ Tambah Pengguna", Width = 150, Top = 10, Left = 20 };
            btnTambah.Click += (s, e) => ShowForm(0); 
            pnlTop.Controls.Add(btnTambah);
            this.Controls.Add(pnlTop);

            // Data Grid View
            dgvUsers = new DataGridView();
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.ReadOnly = true;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.BackgroundColor = DesignTokens.Surface;
            dgvUsers.BorderStyle = BorderStyle.FixedSingle;
            
            // Basic styling for DataGridView
            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = DesignTokens.Primary;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = DesignTokens.ButtonFont;
            dgvUsers.ColumnHeadersHeight = 36;
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = DesignTokens.SurfaceAlt;
            dgvUsers.GridColor = DesignTokens.Border;
            dgvUsers.RowTemplate.Height = 32;

            dgvUsers.CellDoubleClick += DgvUsers_CellDoubleClick;
            this.Controls.Add(dgvUsers);

            // Right Panel Form (Edit/Add)
            pnlForm = new Panel { Dock = DockStyle.Right, Width = 350, BackColor = DesignTokens.Surface, BorderStyle = BorderStyle.FixedSingle, Visible = false };
            
            Label lblTitle = new Label { Text = "Form Pengguna", Font = DesignTokens.SubheadingFont, Top = 20, Left = 20, AutoSize = true };
            
            Label lblUser = new Label { Text = "Username *", Top = 60, Left = 20, AutoSize = true };
            txtUsername = new TextBox { Top = 80, Left = 20, Width = 300, Height = 32 };
            
            Label lblPass = new Label { Text = "Password (kosongkan jika tidak diubah)", Top = 120, Left = 20, AutoSize = true };
            txtPassword = new TextBox { Top = 140, Left = 20, Width = 300, Height = 32, PasswordChar = '•' };

            chkIsActive = new CheckBox { Text = "Aktif", Top = 180, Left = 20, AutoSize = true, Checked = true };

            Label lblRoles = new Label { Text = "Roles (Multiple)", Top = 220, Left = 20, AutoSize = true };
            clbRoles = new CheckedListBox { Top = 240, Left = 20, Width = 300, Height = 100 };
            
            btnBatal = new PrimaryButton { Text = "Batal", Top = 360, Left = 20, Width = 140, BackColor = DesignTokens.Border, ForeColor = DesignTokens.TextPrimary };
            btnBatal.Click += (s, e) => { pnlForm.Visible = false; };
            
            btnSimpan = new PrimaryButton { Text = "Simpan", Top = 360, Left = 180, Width = 140 };
            btnSimpan.Click += BtnSimpan_Click;

            pnlForm.Controls.AddRange(new Control[] { lblTitle, lblUser, txtUsername, lblPass, txtPassword, chkIsActive, lblRoles, clbRoles, btnBatal, btnSimpan });
            
            this.Controls.Add(pnlForm);
            pnlForm.BringToFront(); // Ensure it overlays if needed, though Dock Right pushes grid left.
        }

        private void LoadData()
        {
            var users = _repo.GetAllUsers();
            
            var displayList = users.Select(u => new {
                u.UserId,
                u.Username,
                Status = u.IsActive ? "Aktif" : "Nonaktif",
                TanggalDibuat = u.CreatedAt.ToString("dd/MM/yyyy"),
                Roles = u.RolesString
            }).ToList();
            
            dgvUsers.DataSource = displayList;

            _availableRoles = _repo.GetAllRoles();
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
                var users = _repo.GetAllUsers();
                var user = users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    ShowForm(userId, user);
                }
            }
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_editingUserId == 0 && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password tidak boleh kosong untuk user baru!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

            try
            {
                if (_editingUserId == 0)
                {
                    _repo.AddUser(txtUsername.Text, txtPassword.Text, selectedRoleIds.ToArray());
                    MessageBox.Show("Berhasil ditambahkan!");
                }
                else
                {
                    _repo.UpdateUser(_editingUserId, txtUsername.Text, txtPassword.Text, chkIsActive.Checked, selectedRoleIds.ToArray());
                    MessageBox.Show("Berhasil disimpan!");
                }
                pnlForm.Visible = false;
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan ke database (pastikan koneksi/mocking sudah benar). Detail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // For prototype without active DB: 
                pnlForm.Visible = false;
            }
        }
    }
}
