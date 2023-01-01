 
using EasyMicroservices.FileManager.AzureStorageBlobs.Models;
using Microsoft.AspNetCore.Http;

namespace EasyMicroservices.FileManager.AzureStorageBlobs.Interfaces;

public interface IAzureStorageBlobs
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<ResponseDetail> UploadAsync(IFormFile file);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<ResponseDetail> DownloadAsync(string fileName);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<ResponseDetail> DeleteAsync(string fileName);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<List<BlobDetail>> GetListAsync();
}