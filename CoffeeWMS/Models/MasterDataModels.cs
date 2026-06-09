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

    public class CoffeeType
    {
        public int CoffeeId { get; set; }
        public string CoffeeName { get; set; }
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
