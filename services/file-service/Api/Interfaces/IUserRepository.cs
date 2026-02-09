using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByEmailAsync(string email);

        Task SaveChangesAsync();

    }

}
