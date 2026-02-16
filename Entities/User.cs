using Microsoft.AspNetCore.Identity;

namespace NearzoAPI.Entities
{
    public class User: IdentityUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!; // "User" or "Shop"

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
