using System.Reflection;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;

namespace EasyMicroservices.FileManager.AzureStorageBlobs.Providers;

public class AzureStorageBlobsProvider : IFileManagerProvider
{
    private readonly BlobContainerClient _container;

    public AzureStorageBlobsProvider(BlobContainerClient container)
    {
        _container = container;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storageConnectionString"></param>
    /// <param name="storageContainerName"></param>
    public AzureStorageBlobsProvider(string storageConnectionString, string storageContainerName)
    {
        _container = new BlobContainerClient(storageConnectionString, storageContainerName);
    }

   
    public IDirectoryManagerProvider DirectoryManagerProvider { get; set; }

    public IPathProvider PathProvider
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public async Task<FileDetail> GetFileAsync(string path)
    {
        FileDetail file =new FileDetail(this);
        BlobClient client = _container.GetBlobClient(path);
        if (await client.ExistsAsync())
        {
            file.DirectoryPath = path;
            file.Name = path;
            Stream stream =await OpenFileAsync(path);
            file.Length = stream.Length;

        }

        return file;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path is FileName for creating</param>
    /// <returns></returns>
    public async Task<FileDetail> CreateFileAsync(string path)
    {
        BlobClient client = _container.GetBlobClient(path);
        if (await IsExistFileAsync(path))
            await DeleteFileAsync(path);
        var response = await client.UploadAsync(Stream.Null);

        var objectBlob = new FileDetail(this);
        foreach (PropertyInfo prop in response.GetType().GetProperties())
        {
            objectBlob.Name = path;
            objectBlob.DirectoryPath = path;
        }

        return objectBlob;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path is FileName for opening</param>
    /// <returns></returns>
    public async Task<Stream> OpenFileAsync(string path)
    {
        Stream blob = Stream.Null;

        BlobClient client = _container.GetBlobClient(path);

        if (await client.ExistsAsync())
        {
            blob = await client.OpenReadAsync();
        }

        return blob;
    }

    public async Task WriteStreamToFileAsync(string path, Stream stream)
    {
        BlobClient client = _container.GetBlobClient(path);

         await client.UploadAsync(stream ,true);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path is FileName for checking out</param>
    /// <returns></returns>
    public async Task<bool> IsExistFileAsync(string path)
    {
        BlobClient client = _container.GetBlobClient(path);
        bool exists = await client.ExistsAsync();
        return exists;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path is FileName to delete.</param>
    /// <returns></returns>
    public async Task<bool> DeleteFileAsync(string path)
    {
        try
        {
            BlobClient client = _container.GetBlobClient(path);
            await client.DeleteAsync();
        }
        catch (RequestFailedException e) when (e.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            return false;
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    public Task TruncateFileAsync(string path)
    {
        BlobClient client = _container.GetBlobClient(path);
      
        throw new NotImplementedException();
    }
    
    public async Task<List<FileDetail>> GetListAsync()
    {
        List<FileDetail> blobsList = new List<FileDetail>();
        await foreach (BlobItem item in _container.GetBlobsAsync())
        {
            string name = item.Name;
            string uri = $"{_container.Uri.ToString()}/{name}";
            blobsList.Add(new FileDetail(this)
            {
                Name = name,
                Length = item.Properties.ContentLength??0,
                DirectoryPath = uri
            });
        }

        return blobsList;
    }
}
