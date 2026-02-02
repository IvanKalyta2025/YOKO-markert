using System;
using System.IO;
using System.Threading.Tasks;
using Api.Data;
using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Api.Interfaces;


namespace Api.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _db;
        private readonly MinioService _fileService;

        public ProfileRepository(AppDbContext db, MinioService fileService)
        {
            _db = db;
            _fileService = fileService;
        }

        public async Task AddAsync(Profile profile)
        {
            await _db.Profiles.AddAsync(profile);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Profile profile)
        {
            _db.Profiles.Update(profile);
            await _db.SaveChangesAsync();
        }

        public async Task<Profile?> GetByUserIdAsync(Guid userId)
        {
            return await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task UploadFileAsync(string objectName, Stream fileData)
        {

            await _fileService.UploadFileAsync(_fileService.DefaultBucketName, objectName, fileData);
        }
    }
}
