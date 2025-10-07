using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using SimpchatWeb.Services.Interfaces.Minio;
using SimpchatWeb.Services.Settings;

namespace SimpchatWeb.Services.Minio
{
    public class MinioFileStorageService : IFileStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _minioSettings;

        // Dependency Injection orqali IMinioClient va MinioSettings ni qabul qiladi
        public MinioFileStorageService(IMinioClient minioClient, IOptions<MinioSettings> minioSettings)
        {
            _minioClient = minioClient;
            _minioSettings = minioSettings.Value;
        }

        public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream data, string contentType)
        {
            try
            {
                // Agar bucket (saqlash joyi) mavjud bo'lmasa, uni yaratamiz
                bool found = await _minioClient.BucketExistsAsync(
                    new BucketExistsArgs().WithBucket(bucketName)
                ).ConfigureAwait(false);

                if (!found)
                {
                    await _minioClient.MakeBucketAsync(
                        new MakeBucketArgs().WithBucket(bucketName)
                    ).ConfigureAwait(false);
                }

                // Faylni Minio'ga yuklash
                await _minioClient.PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithStreamData(data) // Yuklanayotgan fayl stream'i
                        .WithObjectSize(data.Length) // Faylning hajmi
                        .WithContentType(contentType) // Faylning turi (masalan, "image/jpeg")
                ).ConfigureAwait(false);

                // Yuklangan faylga to'g'ridan-to'g'ri kirish URL'ini qaytarish
                // Bu URL Minio serverining manzili va bucket/object nomini o'z ichiga oladi.
                // Masalan: http://localhost:9000/my-bucket/my-image.jpg
                return $"http://{_minioSettings.Endpoint}/{bucketName}/{objectName}";
            }
            catch (MinioException e) // Minio'dan kelgan xatoliklarni qayd etish
            {
                Console.WriteLine($"[Minio] Upload Error: {e.Message}");
                throw; // Xatolikni yuqoriga uzatamiz
            }
            catch (Exception e) // Boshqa umumiy xatoliklarni qayd etish
            {
                Console.WriteLine($"[General] Error during upload: {e.Message}");
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string bucketName, string objectName)
        {
            try
            {
                var memoryStream = new MemoryStream();
                await _minioClient.GetObjectAsync(
                    new GetObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithCallbackStream(async (stream) => // Fayl streamini memoryStream ga nusxalash
                        {
                            await stream.CopyToAsync(memoryStream);
                        })
                ).ConfigureAwait(false);

                memoryStream.Position = 0; // Streamni boshiga qaytarish, chunki undan o'qish mumkin bo'lishi uchun
                return memoryStream;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"[Minio] Download Error: {e.Message}");
                throw;
            }
        }

        public async Task<bool> FileExistsAsync(string bucketName, string objectName)
        {
            try
            {
                // StatObjectAsync fayl haqida ma'lumotni oladi, agar mavjud bo'lmasa xato tashlaydi
                await _minioClient.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                ).ConfigureAwait(false);
                return true; // Fayl mavjud
            }
            catch (MinioException e) when (e.Message.Contains("Object does not exist")) // Fayl topilmaganligini aniqlash
            {
                return false; // Fayl mavjud emas
            }
            catch (Exception) // Boshqa har qanday xato
            {
                throw;
            }
        }

        public async Task<bool> RemoveFileAsync(string bucketName, string objectName)
        {
            try
            {
                await _minioClient.RemoveObjectAsync(
                    new RemoveObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                ).ConfigureAwait(false);
                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"[Minio] Remove Error: {e.Message}");
                throw;
            }
        }

        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            try
            {
                return await _minioClient.BucketExistsAsync(
                    new BucketExistsArgs().WithBucket(bucketName)
                ).ConfigureAwait(false);
            }
            catch (MinioException e)
            {
                Console.WriteLine($"[Minio] Bucket Check Error: {e.Message}");
                throw;
            }
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            try
            {
                bool found = await _minioClient.BucketExistsAsync(
                    new BucketExistsArgs().WithBucket(bucketName)
                ).ConfigureAwait(false);

                if (!found)
                {
                    await _minioClient.MakeBucketAsync(
                        new MakeBucketArgs().WithBucket(bucketName)
                    ).ConfigureAwait(false);
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine($"[Minio] Create Bucket Error: {e.Message}");
                throw;
            }
        }
    }
}
