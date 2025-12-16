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
        public async Task<byte[]> DownloadFileAsync(string bucketName, string objectName)
        {
            try
            {
                // Создаем MemoryStream для хранения данных
                using var memoryStream = new MemoryStream();

                // 1. Создаем аргументы для GET-запроса
                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(async (stream) =>
                    {
                        // 2. Копируем поток из MinIO в наш MemoryStream
                        await stream.CopyToAsync(memoryStream);
                    });

                // 3. Выполняем запрос
                await _minioClient.GetObjectAsync(getObjectArgs).ConfigureAwait(false);

                // 4. Возвращаем null, если файл пустой (0 байт),
                //    чтобы это было обработано в контроллере как "не найдено/проблема".
                if (memoryStream.Length == 0)
                {
                    // Это необходимо, чтобы поймать случаи, когда файл не найден,
                    // но исключение ObjectNotFoundException не было брошено.
                    return null;
                }

                // 5. Возвращаем данные в виде массива байтов
                return memoryStream.ToArray();
            }
            catch (Minio.Exceptions.ObjectNotFoundException)
            {
                // Если MinIO не нашел объект, возвращаем null (как ожидает контроллер)
                return null;
            }
            catch (Exception ex)
            {
                // 🚨 ВЫВОДИТЕ ВЕСЬ ТИП И ТЕКСТ ОШИБКИ
                Console.WriteLine($"[CRITICAL ERROR] Тип ошибки: {ex.GetType().Name}");
                Console.WriteLine($"[CRITICAL ERROR] Сообщение: {ex.Message}");
                Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");

                // Если MinIO не смог соединиться, здесь будет указана причина.
                return null;
            }
        }

    }
}