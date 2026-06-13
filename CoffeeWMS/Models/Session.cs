using CoffeeWMS.Models;

namespace CoffeeWMS.Models
{
    public static class Session
    {
        public static User CurrentUser { get; set; }
        
        public static bool IsAdmin => CurrentUser != null && (CurrentUser.RolesString ?? "").Contains("Admin");
        public static bool IsManager => CurrentUser != null && (CurrentUser.RolesString ?? "").Contains("Manager");
        public static bool IsPetugas => CurrentUser != null && (CurrentUser.RolesString ?? "").Contains("Petugas");
    }
}
