using System;
using System.Drawing;
using System.Windows.Forms;
namespace CoffeeWMS.Views
{
    public partial class PrimaryButton : Button
    {
        public PrimaryButton()
        {
            // this.BackColor = ColorTranslator.FromHtml("#2C5F2E");
            this.ForeColor = Color.White;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.Height = 36;
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            // this.BackColor = ColorTranslator.FromHtml("#227700ff");
            this.ForeColor = Color.Black;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            // this.BackColor = ColorTranslator.FromHtml("#2C5F2E");
            this.ForeColor = Color.White;
        }
    }
}

