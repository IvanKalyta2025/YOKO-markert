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
        private readonly FileService _fileService;

        public ProfileRepository(AppDbContext db, FileService fileService)
        {
            _db = db;
            _fileService = fileService;
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

        public async Task UploadFileAsync(string objectName, Stream fileData)
        {
            using var ms = new MemoryStream();
            await fileData.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            await _fileService.UploadFileAsync("profiles", objectName, bytes);
        }
    }
}