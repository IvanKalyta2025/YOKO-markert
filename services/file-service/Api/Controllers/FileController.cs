using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;


//локальный контроллер для minio
namespace Api
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly MinioService _fileService;

        public FileController(MinioService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            string bucketName = _fileService.DefaultBucketName;

            await _fileService.UploadFileAsync(bucketName, file.FileName, file.OpenReadStream());

            return Ok($"File {file.FileName} uploaded successfully!");
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            Response.ContentType = _fileService.GetContentType(fileName);
            await _fileService.DownloadFileAsync(_fileService.DefaultBucketName, fileName, Response.Body);

            return new EmptyResult();
        }
    }
}

