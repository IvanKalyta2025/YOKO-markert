using Api.Models;
using Api.Repositories;
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

        var userId = GetCurrentUserId(); // Метод ниже

        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Logg inn først" }); // Сначала войдите

        string avatarUrl = string.Empty;

        // 2. Если есть файл — грузим через твой метод UploadFileAsync
        if (request.AvatarFile != null)
        {
            // Генерируем уникальное имя файла для MinIO
            var fileExtension = Path.GetExtension(request.AvatarFile.FileName);
            var objectName = $"avatar_{userId}{fileExtension}";

            using var stream = request.AvatarFile.OpenReadStream();

            // Вызываем ТВОЙ метод репозитория
            await _repository.UploadFileAsync(objectName, stream);

            // Формируем ссылку, по которой файл будет доступен
            // (предполагаю, что у тебя есть контроллер FileController с методом download)
            avatarUrl = $"/File/download/{objectName}";
        }

        // 3. Собираем твою модель Profile
        var profile = new Profile
        {
            UserId = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            AvatarUrl = avatarUrl
            // User = ... (EF Core сам подтянет связь, если ID верный)
        };

        // 4. Сохраняем в БД через твой метод
        await _repository.AddAsync(profile);

        return Ok(new { success = true, message = "Profil lagret!", data = profile });
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile == null) return NotFound();
        return Ok(profile);
    }

    // Хелпер для получения ID
    // Вспомогательный метод для получения ID из Токена
    private Guid GetCurrentUserId()
    {
        // Ищем в "паспорте" (Claims) строчку с ID пользователя
        // ClaimTypes.NameIdentifier мы положили туда в JwtTokenService
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim != null && Guid.TryParse(idClaim.Value, out var guid))
        {
            return guid; // Возвращаем РЕАЛЬНЫЙ ID
        }

        return Guid.Empty; // Если токена нет или он кривой
    }
}