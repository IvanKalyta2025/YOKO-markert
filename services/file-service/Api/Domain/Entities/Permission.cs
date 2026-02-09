using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // f.eks. "User.Read"

        // Kobling tilbake til RolePermission (mange-til-mange)
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
