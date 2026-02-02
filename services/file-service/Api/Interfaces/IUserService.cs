using System;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IUserService
    {
        Task<Guid> RegisterUserAsync(string email, string password);
        Task<string?> AuthenticateUserAsync(string email, string password);
    }
}
