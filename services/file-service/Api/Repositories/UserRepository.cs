using Api.Data;
using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Api.Interfaces;

namespace Api.Repositories
{
    public class UserRepositoryDb : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepositoryDb(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _db.Users.AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
