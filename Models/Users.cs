using System.Data;

namespace CoinKeeper.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }

        public int RoleId { get; set; }   // Foreign key for Role

        public Rol? Role { get; set; }    // Navigation property for Role

    }
}
