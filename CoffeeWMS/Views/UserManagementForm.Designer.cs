using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Theme;

namespace CoffeeWMS.Views
{
    partial class UserManagementForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            this.pnlTop = new Panel();
            this.btnTambah = new PrimaryButton();
            this.dgvUsers = new DataGridView();
            this.pnlForm = new Panel();
            this.lblTitle = new Label();
            this.lblUser = new Label();
            this.txtUsername = new TextBox();
            this.lblPass = new Label();
            this.txtPassword = new TextBox();
            this.chkIsActive = new CheckBox();
            this.lblRoles = new Label();
            this.clbRoles = new CheckedListBox();
            this.btnBatal = new PrimaryButton();
            this.btnSimpan = new PrimaryButton();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.pnlForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnTambah);
            this.pnlTop.Dock = DockStyle.Top;
            this.pnlTop.Location = new Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new Size(1442, 60);
            this.pnlTop.TabIndex = 0;
            // 
            // btnTambah
            // 
            this.btnTambah.BackColor = Color.FromArgb(41, 53, 65);
            this.btnTambah.FlatStyle = FlatStyle.Flat;
            this.btnTambah.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnTambah.ForeColor = Color.White;
            this.btnTambah.Location = new Point(20, 10);
            this.btnTambah.Name = "btnTambah";
            this.btnTambah.Size = new Size(150, 36);
            this.btnTambah.TabIndex = 0;
            this.btnTambah.Text = "+ Tambah Pengguna";
            this.btnTambah.UseVisualStyleBackColor = false;
            // 
            // dgvUsers
            // 
            this.dgvUsers.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(240, 240, 240);
            this.dgvUsers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.BackgroundColor = Color.White;
            this.dgvUsers.BorderStyle = BorderStyle.FixedSingle;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(41, 53, 65);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            this.dgvUsers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvUsers.ColumnHeadersHeight = 36;
            this.dgvUsers.Dock = DockStyle.Fill;
            this.dgvUsers.EnableHeadersVisualStyles = false;
            this.dgvUsers.GridColor = Color.LightGray;
            this.dgvUsers.Location = new Point(0, 60);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.RowTemplate.Height = 32;
            this.dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsers.Size = new Size(1092, 747);
            this.dgvUsers.TabIndex = 1;
            this.dgvUsers.CellDoubleClick += new DataGridViewCellEventHandler(this.DgvUsers_CellDoubleClick);
            // 
            // pnlForm
            // 
            this.pnlForm.BackColor = Color.White;
            this.pnlForm.BorderStyle = BorderStyle.FixedSingle;
            this.pnlForm.Controls.Add(this.lblTitle);
            this.pnlForm.Controls.Add(this.lblUser);
            this.pnlForm.Controls.Add(this.txtUsername);
            this.pnlForm.Controls.Add(this.lblPass);
            this.pnlForm.Controls.Add(this.txtPassword);
            this.pnlForm.Controls.Add(this.chkIsActive);
            this.pnlForm.Controls.Add(this.lblRoles);
            this.pnlForm.Controls.Add(this.clbRoles);
            this.pnlForm.Controls.Add(this.btnBatal);
            this.pnlForm.Controls.Add(this.btnSimpan);
            this.pnlForm.Dock = DockStyle.Right;
            this.pnlForm.Location = new Point(1092, 60);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Size = new Size(350, 747);
            this.pnlForm.TabIndex = 2;
            this.pnlForm.Visible = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.Location = new Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(133, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Form Pengguna";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new Point(20, 60);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new Size(82, 19);
            this.lblUser.TabIndex = 1;
            this.lblUser.Text = "Username *";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new Point(20, 80);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new Size(300, 25);
            this.txtUsername.TabIndex = 2;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new Point(20, 120);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new Size(265, 19);
            this.lblPass.TabIndex = 3;
            this.lblPass.Text = "Password (kosongkan jika tidak diubah)";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new Point(20, 140);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.Size = new Size(300, 25);
            this.txtPassword.TabIndex = 4;
            // 
            // chkIsActive
            // 
            this.chkIsActive.AutoSize = true;
            this.chkIsActive.Checked = true;
            this.chkIsActive.CheckState = CheckState.Checked;
            this.chkIsActive.Location = new Point(20, 180);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Size = new Size(57, 23);
            this.chkIsActive.TabIndex = 5;
            this.chkIsActive.Text = "Aktif";
            this.chkIsActive.UseVisualStyleBackColor = true;
            // 
            // lblRoles
            // 
            this.lblRoles.AutoSize = true;
            this.lblRoles.Location = new Point(20, 220);
            this.lblRoles.Name = "lblRoles";
            this.lblRoles.Size = new Size(106, 19);
            this.lblRoles.TabIndex = 6;
            this.lblRoles.Text = "Roles (Multiple)";
            // 
            // clbRoles
            // 
            this.clbRoles.FormattingEnabled = true;
            this.clbRoles.Location = new Point(20, 240);
            this.clbRoles.Name = "clbRoles";
            this.clbRoles.Size = new Size(300, 100);
            this.clbRoles.TabIndex = 7;
            // 
            // btnBatal
            // 
            this.btnBatal.BackColor = Color.LightGray;
            this.btnBatal.FlatStyle = FlatStyle.Flat;
            this.btnBatal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnBatal.ForeColor = Color.Black;
            this.btnBatal.Location = new Point(20, 360);
            this.btnBatal.Name = "btnBatal";
            this.btnBatal.Size = new Size(140, 36);
            this.btnBatal.TabIndex = 8;
            this.btnBatal.Text = "Batal";
            this.btnBatal.UseVisualStyleBackColor = false;
            // 
            // btnSimpan
            // 
            this.btnSimpan.BackColor = Color.FromArgb(41, 53, 65);
            this.btnSimpan.FlatStyle = FlatStyle.Flat;
            this.btnSimpan.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnSimpan.ForeColor = Color.White;
            this.btnSimpan.Location = new Point(180, 360);
            this.btnSimpan.Name = "btnSimpan";
            this.btnSimpan.Size = new Size(140, 36);
            this.btnSimpan.TabIndex = 9;
            this.btnSimpan.Text = "Simpan";
            this.btnSimpan.UseVisualStyleBackColor = false;
            this.btnSimpan.Click += new EventHandler(this.BtnSimpan_Click);
            // 
            // UserManagementForm
            // 
            this.BackColor = Color.White;
            this.Controls.Add(this.dgvUsers);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.pnlTop);
            this.Font = new Font("Segoe UI", 10F);
            this.Name = "UserManagementForm";
            this.Size = new Size(1442, 807);
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.pnlForm.ResumeLayout(false);
            this.pnlForm.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUsers;
        private CoffeeWMS.Views.PrimaryButton btnTambah;
        private CoffeeWMS.Views.PrimaryButton btnSimpan;
        private CoffeeWMS.Views.PrimaryButton btnBatal;
        
        private System.Windows.Forms.Panel pnlForm;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkIsActive;
        private System.Windows.Forms.CheckedListBox clbRoles;
        private Panel pnlTop;
        private Label lblTitle;
        private Label lblUser;
        private Label lblPass;
        private Label lblRoles;
    }
}
