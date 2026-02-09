
using System.Security;

namespace Api.Domain.Entities
{
    public sealed class RolePermission
    {
        public int RoleId { get; init; }
        public Role Role { get; set; } = null!;

        public int PermissionId { get; init; }
        public Permission Permission { get; set; } = null!;
    }
}