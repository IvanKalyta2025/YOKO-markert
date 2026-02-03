
using Api.Interfaces;
using Api.Domain.Entities;

namespace Api.Repositories
{
    public class СompanyRepository : IHumanResourcesRepository
    {
        public Task AddAsync(HumanResources humanResources)
        {
            throw new NotImplementedException();
        }

        public Task<HumanResources?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}