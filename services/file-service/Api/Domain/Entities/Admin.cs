

namespace Api.Domain.Entities
{
    public class Admin
    {
        public string AdminName { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string Marker { get; set; } = "ADMIN_ACCESS";

    }
}
