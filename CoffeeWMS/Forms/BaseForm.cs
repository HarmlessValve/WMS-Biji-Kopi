using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Theme;

namespace CoffeeWMS.Forms
{
    public class BaseForm : Form
    {
        public Panel pnlSidebar;
        public Panel pnlHeader;
        public Panel pnlContent;
        public StatusStrip statusStrip;
        public ToolStripStatusLabel lblStatus;

        private Label lblLogo;

        public BaseForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(1366, 768);
            this.MinimumSize = new Size(1024, 768);
            this.Font = DesignTokens.BodyFont;
            this.BackColor = DesignTokens.Background;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "CoffeeWMS";

            // Sidebar
            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = DesignTokens.SidebarWidth;
            pnlSidebar.BackColor = DesignTokens.Primary;

            // Logo Header inside Sidebar
            Panel pnlLogo = new Panel();
            pnlLogo.Dock = DockStyle.Top;
            pnlLogo.Height = DesignTokens.HeaderHeight;
            
            lblLogo = new Label();
            lblLogo.Text = "☕ CoffeeWMS";
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font(DesignTokens.FontFamily, 14, FontStyle.Bold);
            lblLogo.AutoSize = false;
            lblLogo.Dock = DockStyle.Fill;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            pnlLogo.Controls.Add(lblLogo);
            
            // Separator inside Sidebar
            Panel pnlSeparator = new Panel();
            pnlSeparator.Dock = DockStyle.Top;
            pnlSeparator.Height = 1;
            pnlSeparator.BackColor = Color.FromArgb(50, 255, 255, 255);

            pnlSidebar.Controls.Add(pnlSeparator);
            pnlSidebar.Controls.Add(pnlLogo);

            // Header
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = DesignTokens.HeaderHeight;
            pnlHeader.BackColor = DesignTokens.Surface;
            
            // Bottom Status Strip
            statusStrip = new StatusStrip();
            statusStrip.BackColor = DesignTokens.Surface; // using Surface for status strip to blend in
            lblStatus = new ToolStripStatusLabel();
            lblStatus.Text = "v1.0.0 | Offline";
            lblStatus.Font = DesignTokens.SmallFont;
            statusStrip.Items.Add(lblStatus);

            // Content Panel (Dynamic)
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = DesignTokens.Background;
            pnlContent.Padding = new Padding(DesignTokens.HeaderHeight / 2); // default padding

            // Add all to Form
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(statusStrip);
        }

        // Helper to load child forms or controls into content panel
        public void LoadView(Control viewControl, string title)
        {
            pnlContent.Controls.Clear();
            viewControl.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(viewControl);

            // Set simple header title
            pnlHeader.Controls.Clear();
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = DesignTokens.TitleFont;
            lblTitle.ForeColor = DesignTokens.TextPrimary;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, (DesignTokens.HeaderHeight - 25) / 2);
            pnlHeader.Controls.Add(lblTitle);
        }
    }
}
