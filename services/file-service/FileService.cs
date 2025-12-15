using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minio;

namespace ivan.Desktop.Prosjektuke.minifilehub.services.file_service
{
    public class FileService
    {
        private readonly MinioClient _minioClient;

        public FileService(Iconfiguration configuration)
        {
            _minioClient = new MinioClient()
                .WithEndpoint(configuration["Minio:Endpoint"])
                .WithCredentials(configuration["Minio:AccessKey"], configuration["Minio:SecretKey"])
                .Build();
        }
    }
}