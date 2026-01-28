using System.IO;
using System.Threading.Tasks;
using Minio;
using Microsoft.Extensions.Configuration;
using Minio.DataModel.Args;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

namespace Api
{
    public class MinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider; // Content type provider for file extensions
        private readonly string _defaultBucketName;

        //private static readonly Regex SafeNameRegex = new(@"^[a-zA-Z0-9_\-\.]+$", RegexOptions.Compiled); // Regex for safe object names
        //https://www.geeksforgeeks.org/c-sharp/what-is-regular-expression-in-c-sharp/

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

        public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream fileData)
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

            // using var stream = new MemoryStream(); på grunn av jeg kaste byte[] til Stream   

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(fileData)
                .WithObjectSize(fileData.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            return $"/{bucketName}/{objectName}";
        }

        public async Task<string?> DownloadFileAsync(string bucketName, string objectName, Stream destinationStream)
        {
            bucketName = string.IsNullOrWhiteSpace(bucketName) ? _defaultBucketName : bucketName;

            try
            {
                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(destinationStream);
                    });

                await _minioClient.GetObjectAsync(getObjectArgs).ConfigureAwait(false);

                if (!_contentTypeProvider.TryGetContentType(objectName, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                return contentType;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                return null;
            }
        }
        public string GetContentType(string objectName)
        {
            if (!_contentTypeProvider.TryGetContentType(objectName, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
