

using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Api.Domain.Entities
{
    public class HumanResources
    {
        public Guid IdHumanResources { get; set; } = Guid.NewGuid();
        public string KeyForHumanResources { get; set; } = "HRcompany";
        public string EmailHumanResources { get; set; } = string.Empty;
        public string PasswordHashHumanResources { get; set; } = string.Empty;
    }
}