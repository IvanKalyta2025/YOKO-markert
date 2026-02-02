using System;
using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Interfaces
{
    public interface IProfileRepository
    {
        Task AddAsync(Profile profile);
        Task UpdateAsync(Profile profile);
        Task<Profile?> GetByUserIdAsync(Guid userId);
        Task UploadFileAsync(string objectName, Stream fileData);
    }
}
