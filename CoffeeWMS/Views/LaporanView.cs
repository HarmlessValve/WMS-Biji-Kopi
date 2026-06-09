using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeWMS.Views
{
    public class LaporanView : UserControl
    {
        public LaporanView()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Laporan Transaksi Gudang Kopi", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(30, 20) };
            
            // Rekap info dashboard sederhana
            Label lblInfo = new Label { 
                Text = "Fitur cetak laporan PDF/Excel dapat diintegrasikan di sini.\nSemua riwayat transaksi tercatat otomatis di database.", 
                Font = new Font("Segoe UI", 11), 
                Location = new Point(35, 80), 
                Size = new Size(500, 100) 
            };

            this.Controls.AddRange(new Control[] { lblTitle, lblInfo });
        }
    }
}