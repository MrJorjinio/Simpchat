namespace SimpchatWeb.Services.Interfaces.Minio
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(string bucketName, string objectName, Stream data, string contentType);

        Task<Stream> DownloadFileAsync(string bucketName, string objectName);

        Task<bool> FileExistsAsync(string bucketName, string objectName);

        Task<bool> RemoveFileAsync(string bucketName, string objectName);

        Task<bool> BucketExistsAsync(string bucketName);

        Task CreateBucketAsync(string bucketName);
    }
}
