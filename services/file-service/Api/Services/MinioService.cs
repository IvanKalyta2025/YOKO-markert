using System.IO;
using System.Threading.Tasks;
using Minio;
using Microsoft.Extensions.Configuration;
using Minio.DataModel.Args;
using Microsoft.AspNetCore.StaticFiles;

namespace Api
{
    public class MinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public MinioService(IConfiguration configuration)
        {
            _minioClient = new MinioClient()
                .WithEndpoint(configuration["Minio:Endpoint"])
                .WithCredentials(configuration["Minio:AccessKey"], configuration["Minio:SecretKey"])
                .Build();

            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        public async Task<string> UploadFileAsync(string bucketName, string objectName, byte[] fileData)
        {
            var existsArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool found = await _minioClient.BucketExistsAsync(existsArgs).ConfigureAwait(false);

            if (!found)
            {
                var makeArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeArgs).ConfigureAwait(false);
            }
            if (!_contentTypeProvider.TryGetContentType(objectName, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            using var stream = new MemoryStream(fileData);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);

            var result = await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            var fileUrl = $"http://file-service.localhost/{bucketName}/{objectName}";
            return fileUrl;
        }
        public async Task<(byte[] Data, string ContentType)> DownloadFileAsync(string bucketName, string objectName) //добавлен ContentType
        {
            try
            {
                using var memoryStream = new MemoryStream();

                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(async (stream) =>
                    {
                        await stream.CopyToAsync(memoryStream);
                    });

                await _minioClient.GetObjectAsync(getObjectArgs).ConfigureAwait(false);


                if (memoryStream.Length == 0)
                {
                    return (null, null);
                }
                if (!_contentTypeProvider.TryGetContentType(objectName, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                return (memoryStream.ToArray(), contentType);

            }
            catch (Minio.Exceptions.ObjectNotFoundException)
            {
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL ERROR] Тип ошибки: {ex.GetType().Name}");
                Console.WriteLine($"[CRITICAL ERROR] Сообщение: {ex.Message}");
                Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");

                return (null, null);
            }
        }

    }
}