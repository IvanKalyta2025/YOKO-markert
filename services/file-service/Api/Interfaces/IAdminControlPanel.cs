
using Api.Domain.Entities;

namespace Api.Interfaces
{
    public interface IAdminControlPanel
    {
        Task<Admin> GetAdminDetailsAsync(string adminId, string accessKey);
    }
}