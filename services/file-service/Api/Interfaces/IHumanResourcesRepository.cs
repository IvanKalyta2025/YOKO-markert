using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Interfaces
{
    public interface IHumanResourcesRepository
    {
        Task AddAsync(HumanResources humanResources);
        Task<HumanResources?> GetByEmailAndKeyForHumanResourcesAsync(string email, string KeyForHumanResources);
        Task SaveChangesAsync();
    }
}