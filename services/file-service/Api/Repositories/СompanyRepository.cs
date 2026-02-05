
using Api.Interfaces;
using Api.Domain.Entities;
using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class СompanyRepository : IHumanResourcesRepository
    {

        private readonly AppDbContext _dbСompanyRepository;

        public СompanyRepository(AppDbContext dbСompanyRepository)
        {
            _dbСompanyRepository = dbСompanyRepository;
        }
        public async Task AddAsync(HumanResources humanResources)
        {
            await _dbСompanyRepository.HumanResources.AddAsync(humanResources);
            await _dbСompanyRepository.SaveChangesAsync();
        }

        public async Task<HumanResources?> GetByEmailAndKeyForHumanResourcesAsync(string email, string KeyForHumanResources)
        {
            return await _dbСompanyRepository.HumanResources.FirstOrDefaultAsync(hr => hr.EmailHumanResources == email && hr.KeyForHumanResources == KeyForHumanResources);
        }

        public async Task SaveChangesAsync()
        {
            await _dbСompanyRepository.SaveChangesAsync();
        }

        public async Task
    }
}