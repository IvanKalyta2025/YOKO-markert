using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Routing.Constraints;
using System.IO;

namespace Api.Services
{
    public class ProfileService
    {
        private readonly IProfileRepository _profileRepository;
        //private readonly IUserRepository _userRepository;
        private readonly FileService _fileService;

        public ProfileService(IProfileRepository profileRepository, FileService fileService)
        {
            _profileRepository = profileRepository;
            _fileService = fileService;
        }

        public async Task CreateProfileAsync(Guid userId, string firstName, string lastName, byte[] fileData, string fileName)
        {
            var fileUrl = await _fileService.UploadFileAsync("sape", fileName, fileData);

            var profile = new Profile
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                AvatarUrl = fileUrl
            };
            await _profileRepository.AddAsync(profile);
        }
    }
}
