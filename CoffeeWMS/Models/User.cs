using System;
using System.Collections.Generic;

namespace CoffeeWMS.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Used on creation/update only
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // From vw_user_roles
        public string RolesString { get; set; }
        
        // Structured roles representation
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}
