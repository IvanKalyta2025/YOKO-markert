using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;

namespace Api
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileData = memoryStream.ToArray();

            // BucketName берется из appsettings
            string bucketName = "sape";

            await _fileService.UploadFileAsync(bucketName, file.FileName, fileData);

            return Ok($"File {file.FileName} uploaded successfully!");
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name is required.");

            string bucketName = "sape";

            //byte[] fileData = await _fileService.DownloadFileAsync(bucketName, fileName);
            var (fileData, contentType) = await _fileService.DownloadFileAsync(bucketName, fileName);

            if (fileData == null)
            {
                return NotFound($"File '{fileName}' was not found.");
            }
            // var provider = new FileExtensionContentTypeProvider();

            // //string contentType;
            // // Пытаемся понять тип по имени файла (например, "photo.jpg" -> "image/jpeg")
            // if (!provider.TryGetContentType(fileName, out contentType))
            // {
            //     // Если расширение незнакомое, ставим дефолт
            //     contentType = "application/octet-stream";
            // }

            // Использование FileContentResult для явного указания имени файла
            // Это гарантирует, что браузер получит заголовок Content-Disposition: attachment
            var fileResult = new FileContentResult(fileData, contentType)
            {
                FileDownloadName = fileName
            };
            return fileResult;

        }
    }
}