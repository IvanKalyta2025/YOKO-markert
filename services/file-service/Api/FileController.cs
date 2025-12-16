using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            // ... (проверки)

            string bucketName = "sape";

            byte[] fileData = await _fileService.DownloadFileAsync(bucketName, fileName);

            if (fileData == null)
            {
                return NotFound($"File '{fileName}' was not found.");
            }

            // 🚨 ВРЕМЕННЫЙ ТЕСТ: Возвращаем текст вместо файла
            if (fileData.Length > 0)
            {
                // 🟢 Сюда попадаем, если сервис вернул данные.
                // КОНСОЛЬ В БРАУЗЕРЕ ДОЛЖНА ПОКАЗАТЬ ДЛИНУ БОЛЬШЕ 0
                return Ok($"SUCCESS! File found. Expected length: {fileData.Length} bytes.");
            }
            else
            {
                // 🔴 Сюда попадаем, если сервис вернул пустой массив (byte[0])
                return BadRequest($"FAILURE! File found, but returned zero bytes. Length: {fileData.Length}");
            }

            // return File(fileData, contentType, fileName); // Закомментируйте эту строку
        }
    }
}