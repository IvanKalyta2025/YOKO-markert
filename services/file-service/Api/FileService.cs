using System.IO;
using System.Threading.Tasks;
using Minio;
using Microsoft.Extensions.Configuration;
using Minio.DataModel.Args;

namespace Api
{
    public class FileService
    {
        private readonly IMinioClient _minioClient;

        public FileService(IConfiguration configuration)
        {
            _minioClient = new MinioClient()
                .WithEndpoint(configuration["Minio:Endpoint"])
                .WithCredentials(configuration["Minio:AccessKey"], configuration["Minio:SecretKey"])
                .Build();
        }

        public async Task UploadFileAsync(string bucketName, string objectName, byte[] fileData)
        {
            var existsArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool found = await _minioClient.BucketExistsAsync(existsArgs).ConfigureAwait(false);

            if (!found)
            {
                var makeArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeArgs).ConfigureAwait(false);
            }

            using var stream = new MemoryStream(fileData);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType("application/octet-stream");

            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        }
    }
}