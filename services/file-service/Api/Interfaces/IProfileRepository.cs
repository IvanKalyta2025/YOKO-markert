using System;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Repositories
{
    public interface IProfileRepository
    {
        Task AddAsync(Profile profile);
        Task UpdateAsync(Profile profile);
        Task<Profile?> GetByUserIdAsync(Guid userId);
        Task UploadFileAsync(string objectName, Stream fileData);
    }
}