

namespace Api.Domain.Entities
{
    public sealed class UserRole
    {
        public Guid UserId { get; init; }
        public User User { get; set; } = null!;

        public int RoleId { get; init; }
        public Role Role { get; set; } = null!;
    }
}
