using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TokenHash { get; set; } = null!;
        public DateTime Expires { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public bool IsRevoked { get; set; } = false;

        // Relation
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}
