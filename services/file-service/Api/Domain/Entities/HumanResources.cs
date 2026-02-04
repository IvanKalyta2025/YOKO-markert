

namespace Api.Domain.Entities
{
    public class HumanResources
    {
        public Guid IdHumanResources { get; set; } = Guid.NewGuid();
        public string EmailHumanResources { get; set; } = string.Empty;
        public string PasswordHashHumanResources { get; set; } = string.Empty;
    }
}