using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Interfaces.External.FileStorage
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(string bucketName, string objectName, Stream data, string contentType);
        Task<MemoryStream> DownloadFileAsync(string bucketName, string objectName);
        Task<bool> FileExistsAsync(string bucketName, string objectName);
        Task<bool> RemoveFileAsync(string bucketName, string objectName);
        Task<bool> BucketExistsAsync(string bucketName);
        Task CreateBucketAsync(string bucketName);
    }
}
