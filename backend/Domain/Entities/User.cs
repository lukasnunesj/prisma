using Microsoft.AspNetCore.Identity;

namespace backend.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}