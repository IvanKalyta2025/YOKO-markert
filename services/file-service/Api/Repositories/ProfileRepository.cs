using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Api.Repositories;

namespace Api.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _db;

        public ProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Profile profile)
        {
            await _db.Profiles.AddAsync(profile);
            await _db.SaveChangesAsync();
        }

        public async Task<Profile?> GetByUserIdAsync(Guid userId)
        {
            return await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

    }
}