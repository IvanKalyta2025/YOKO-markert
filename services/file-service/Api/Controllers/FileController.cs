using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Api
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly MinioService _fileService;
        private const long MaxFileSize = 10 * 1024 * 1024;
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };

        public FileController(MinioService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран.");

            if (file.Length > MaxFileSize)
                return BadRequest("Размер файла превышает допустимый лимит.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !((IList<string>)_permittedExtensions).Contains(extension))
                return BadRequest("Недопустимый тип файла.");

            var safeFileName = Path.GetFileName(file.FileName);
            string bucketName = _fileService.DefaultBucketName;

            using (var stream = file.OpenReadStream())
            {
                await _fileService.UploadFileAsync(bucketName, safeFileName, stream);
            }

            return Ok(new { FileName = safeFileName, Message = "Файл успешно загружен." });
        }

        [AllowAnonymous]
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            var safeFileName = Path.GetFileName(fileName);

            Response.ContentType = _fileService.GetContentType(safeFileName);
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{safeFileName}\"");

            var result = await _fileService.DownloadFileAsync(_fileService.DefaultBucketName, safeFileName, Response.Body);

            if (result == null)
            {

                return NotFound("Файл не найден в хранилище.");
            }

            return new EmptyResult();
        }
    }

}