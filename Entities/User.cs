using Microsoft.AspNetCore.Identity;

namespace NearzoAPI.Entities
{
    public class User: IdentityUser
    {
        // Id not required as Identity is already doing that
        //public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FullName { get; set; } = null!;
        //public string Email { get; set; } = null!;    // Identity already have, if add it will raise conflict
        //public string PasswordHash { get; set; } = null!;  // Identity already have, if add it will raise conflict
        public string Role { get; set; } = null!; // "User" or "Shop"

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
