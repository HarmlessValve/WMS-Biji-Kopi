using System;
using System.Drawing;
using System.Windows.Forms;
namespace CoffeeWMS.Views
{
    public partial class BaseForm : Form
    {

        public BaseForm()
        {
            InitializeComponent();
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
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;
            lblTitle.AutoSize = true;
            // Pindahkan teks agak ke bawah (misalnya Y = 25) agar tidak mepet atau bertabrakan dengan elemen lain
            lblTitle.Location = new Point(20, 25);
            pnlHeader.Controls.Add(lblTitle);
        }
    }
}

