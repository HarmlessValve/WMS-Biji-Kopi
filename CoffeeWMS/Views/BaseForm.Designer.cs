using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Theme;

namespace CoffeeWMS.Views
{
    partial class BaseForm
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
            this.Size = new Size(1366, 768);
            this.MinimumSize = new Size(1024, 768);
            this.Font = new Font("Segoe UI", 10F);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "CoffeeWMS";

            // Sidebar
            this.pnlSidebar = new Panel();
            this.pnlSidebar.Dock = DockStyle.Left;
            this.pnlSidebar.Width = 250;
            this.pnlSidebar.BackColor = Color.FromArgb(41, 53, 65);

            // Logo Header inside Sidebar
            Panel pnlLogo = new Panel();
            pnlLogo.Dock = DockStyle.Top;
            pnlLogo.Height = 70;
            
            this.lblLogo = new Label();
            this.lblLogo.Text = "☕ CoffeeWMS";
            this.lblLogo.ForeColor = Color.White;
            this.lblLogo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblLogo.AutoSize = false;
            this.lblLogo.Dock = DockStyle.Fill;
            this.lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            pnlLogo.Controls.Add(lblLogo);
            
            // Separator inside Sidebar
            Panel pnlSeparator = new Panel();
            pnlSeparator.Dock = DockStyle.Top;
            pnlSeparator.Height = 1;
            pnlSeparator.BackColor = Color.FromArgb(50, 255, 255, 255);

            this.pnlSidebar.Controls.Add(pnlSeparator);
            this.pnlSidebar.Controls.Add(pnlLogo);

            // Header
            this.pnlHeader = new Panel();
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 70;
            this.pnlHeader.BackColor = Color.White;
            
            // Bottom Status Strip
            this.statusStrip = new StatusStrip();
            this.statusStrip.BackColor = Color.White; 
            this.lblStatus = new ToolStripStatusLabel();
            this.lblStatus.Text = "v1.0.0 | Offline";
            this.lblStatus.Font = new Font("Segoe UI", 8F);
            this.statusStrip.Items.Add(lblStatus);

            // Content Panel (Dynamic)
            this.pnlContent = new Panel();
            this.pnlContent.Dock = DockStyle.Fill;
            this.pnlContent.BackColor = Color.FromArgb(245, 245, 245);
            this.pnlContent.Padding = new Padding(35);

            // Add all to Form
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.statusStrip);
        }

        #endregion

        public System.Windows.Forms.Panel pnlSidebar;
        public System.Windows.Forms.Panel pnlHeader;
        public System.Windows.Forms.Panel pnlContent;
        public System.Windows.Forms.StatusStrip statusStrip;
        public System.Windows.Forms.ToolStripStatusLabel lblStatus;

        private System.Windows.Forms.Label lblLogo;
    }
}
