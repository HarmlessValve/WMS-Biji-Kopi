using System;

namespace CoffeeWMS.Models
{
    // Sesuai tabel: suppliers
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
    }

    // Sesuai tabel: destinations
    public class Destination
    {
        public int DestinationId { get; set; }
        public string DestinationName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }

    // Sesuai view: vw_logs (join activity_logs + users)
    public class LogEntry
    {
        public int LogId { get; set; }
        public string Actor { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime LogTime { get; set; }
    }

    // Sesuai tabel: coffee_types
    public class CoffeeType
    {
        public int CoffeeId { get; set; }
        public string CoffeeName { get; set; }
        public bool IsActive { get; set; }
    }

    // Sesuai view: vw_coffee_products
    public class CoffeeProduct
    {
        public int ProductId { get; set; }
        public string CoffeeName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? OriginId { get; set; }
        public string OriginName { get; set; }
        public int CurrentQuantity { get; set; }
        public int MinimumStock { get; set; }
        public bool IsActive { get; set; }
    }

    // Sesuai tabel: coffee_categories
    public class CoffeeCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    // Sesuai view: stock_summary
    public class StockSummary
    {
        public int ProductId { get; set; }
        public string CoffeeName { get; set; }
        public string CategoryName { get; set; }
        public string OriginName { get; set; }
        public int CurrentQuantity { get; set; }
        public int MinimumStock { get; set; }
        public string Status { get; set; } // "LOW" atau "SAFE"
    }

    // Sesuai view: vw_dashboard_summary
    public class DashboardSummary
    {
        public int TotalCoffeeTypes { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalDestinations { get; set; }
        public int TotalIncoming { get; set; }
        public int TotalOutgoing { get; set; }
        public int TotalLowStock { get; set; }
    }
}