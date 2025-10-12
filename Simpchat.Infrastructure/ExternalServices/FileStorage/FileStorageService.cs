using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Simpchat.Application.Common.Interfaces.FileStorage;
using Simpchat.Shared.Config;

namespace Simpchat.Infrastructure.ExternalServices.FileStorage
{
    internal class FileStorageService : IFileStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _minioSettings;

        public FileStorageService(IMinioClient minioClient, IOptions<AppSettings> appSettings)
        {
            _minioClient = minioClient;
            _minioSettings = appSettings.Value.MinioSettings;
        }

        public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream data, string contentType)
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

                await _minioClient.PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithStreamData(data)
                        .WithObjectSize(data.Length)
                        .WithContentType(contentType)
                ).ConfigureAwait(false);

                return $"http://{_minioSettings.Endpoint}/{bucketName}/{objectName}";
            }
            catch (MinioException e)
            {
                Console.WriteLine($"[Minio] Upload Error: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[General] Error during upload: {e.Message}");
                throw;
            }
        }

        public async Task<MemoryStream> DownloadFileAsync(string bucketName, string objectName)
        {
            try
            {
                var memoryStream = new MemoryStream();
                await _minioClient.GetObjectAsync(
                    new GetObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithCallbackStream(async (stream) =>
                        {
                            await stream.CopyToAsync(memoryStream);
                        })
                ).ConfigureAwait(false);

                memoryStream.Position = 0;
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
                await _minioClient.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                ).ConfigureAwait(false);
                return true;
            }
            catch (MinioException e) when (e.Message.Contains("Object does not exist"))
            {
                return false;
            }
            catch (Exception)
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
