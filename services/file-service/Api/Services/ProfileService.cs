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
        private readonly MinioService _fileService;

        public ProfileService(IProfileRepository profileRepository, MinioService fileService)
        {
            _profileRepository = profileRepository;
            _fileService = fileService;
        }

        public async Task CreateProfileAsync(Guid userId,
        string firstName,
        string lastName,
        Stream fileData,
        string fileName,
        int age, //update to version 2.1
        string gender, //update to version 2.1
        string hobby, //update to version 2.1
        string myPlaceOfBirth) //update to version 2.1
        {
            var fileUrl = await _fileService.UploadFileAsync(_fileService.DefaultBucketName, fileName, fileData);

            var profile = new Profile
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Age = age, //update to version 2.1
                Gender = gender, //update to version 2.1
                Hobby = hobby, //update to version 2.1
                MyPlaceOfBirth = myPlaceOfBirth, //update to version 2.1
                AvatarUrl = fileUrl
            };
            await _profileRepository.AddAsync(profile);
        }
    }
}
