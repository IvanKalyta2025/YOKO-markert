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
        private readonly string _defaultBucketName;

        public MinioService(IConfiguration configuration)
        {
            _defaultBucketName = configuration["Minio:BucketName"] ?? "profiles";
            _minioClient = new MinioClient()
                .WithEndpoint(configuration["Minio:Endpoint"])
                .WithCredentials(configuration["Minio:AccessKey"], configuration["Minio:SecretKey"])
                .WithSSL(false)
                .Build();

            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        public string DefaultBucketName => _defaultBucketName;

        public async Task<string> UploadFileAsync(string bucketName, string objectName, byte[] fileData)
        {
            bucketName = string.IsNullOrWhiteSpace(bucketName) ? _defaultBucketName : bucketName;

            try
            {
                var existsArgs = new BucketExistsArgs().WithBucket(bucketName);
                bool found = await _minioClient.BucketExistsAsync(existsArgs).ConfigureAwait(false);

                if (!found)
                {
                    var makeArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(makeArgs).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bucket check/creation failed: {ex.Message}");
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

            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            return $"/{bucketName}/{objectName}";
        }

        public async Task<(byte[] Data, string ContentType)> DownloadFileAsync(string bucketName, string objectName)
        {
            bucketName = string.IsNullOrWhiteSpace(bucketName) ? _defaultBucketName : bucketName;

            try
            {
                using var memoryStream = new MemoryStream();

                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(memoryStream);
                    });

                await _minioClient.GetObjectAsync(getObjectArgs).ConfigureAwait(false);

                if (memoryStream.Length == 0) return (null, null);

                if (!_contentTypeProvider.TryGetContentType(objectName, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                return (memoryStream.ToArray(), contentType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                return (null, null);
            }
        }
    }
}
