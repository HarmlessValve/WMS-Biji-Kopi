using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeWMS.Theme;

namespace CoffeeWMS.Forms
{
    public class PrimaryButton : Button
    {
        public PrimaryButton()
        {
            this.BackColor = DesignTokens.Primary;
            this.ForeColor = Color.White;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Font = DesignTokens.ButtonFont;
            this.Height = 36;
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.BackColor = DesignTokens.PrimaryLight;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.BackColor = DesignTokens.Primary;
        }
    }
}
