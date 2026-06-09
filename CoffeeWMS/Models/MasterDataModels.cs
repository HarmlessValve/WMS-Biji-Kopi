using System;

namespace CoffeeWMS.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
    }

    public class Destination
    {
        public int DestinationId { get; set; }
        public string DestinationName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class LogEntry
    {
        public int LogId { get; set; }
        public string Actor { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime LogTime { get; set; }
    }
}

public class TransaksiMasuk
{
    public int Id { get; set; }
    public DateTime Tanggal { get; set; }
    public string JenisKopi { get; set; }  // Contoh: Arabika, Robusta
    public string Origin { get; set; }     // Contoh: Aceh, Toraja (Tambahan baru)
    public string Type { get; set; }       // Contoh: Greenbean, Roasted (Tambahan baru)
    public decimal JumlahKg { get; set; }
}

namespace CoffeeWMS.Models
{
    public class Coffee
    {
        public int CoffeeId { get; set; }
        public string JenisKopi { get; set; }
        public string Origin { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}