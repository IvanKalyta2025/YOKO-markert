using Api.Contracts.Requests;
using Api.Domain.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileRepository _repository;

    public ProfileController(IProfileRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateProfile([FromForm] ProfileRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Logg inn først" });

        var existingProfile = await _repository.GetByUserIdAsync(userId);
        string avatarUrl = existingProfile?.AvatarUrl ?? string.Empty;

        if (request.AvatarFile != null)
        {
            var fileExtension = Path.GetExtension(request.AvatarFile.FileName);
            var objectName = $"avatar_{userId}{fileExtension}";

            using var stream = request.AvatarFile.OpenReadStream();
            await _repository.UploadFileAsync(objectName, stream);
            avatarUrl = $"/File/download/{objectName}";
        }

        if (existingProfile == null)
        {
            var profile = new Profile
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age, //update to version 2.1
                Gender = request.Gender, //update to version 2.1
                Hobby = request.Hobby, //update to version 2.1
                MyPlaceOfBirth = request.MyPlaceOfBirth, //update to version 2.1
                AvatarUrl = avatarUrl
            };
            await _repository.AddAsync(profile);
        }
        else
        {
            existingProfile.FirstName = request.FirstName;
            existingProfile.LastName = request.LastName;
            existingProfile.Age = request.Age; //update to version 2.1
            existingProfile.Gender = request.Gender; //update to version 2.1
            existingProfile.Hobby = request.Hobby; //update to version 2.1
            existingProfile.MyPlaceOfBirth = request.MyPlaceOfBirth; //update to version 2.1
            existingProfile.AvatarUrl = avatarUrl;
            await _repository.UpdateAsync(existingProfile);
        }

        return Ok(new { success = true, message = "Profil lagret!" });
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile == null) return NotFound();
        return Ok(profile);
    }

    private Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim != null && Guid.TryParse(idClaim.Value, out var guid))
        {
            return guid;
        }

        return Guid.Empty;
    }

}
