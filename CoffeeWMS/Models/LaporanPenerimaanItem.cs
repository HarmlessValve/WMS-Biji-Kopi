using System;

namespace CoffeeWMS.Models
{
    public class LaporanPenerimaanItem
    {
        public DateTime Tanggal { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string JenisKopi { get; set; } = string.Empty;
        public int Jumlah { get; set; }
        public string Petugas { get; set; } = string.Empty;
    }
}
