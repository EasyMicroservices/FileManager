using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using EasyMicroservices.FileManager.Providers.FileProviders;
using System.Reflection;

namespace EasyMicroservices.FileManager.AzureStorageBlobs.Providers;

/// <summary>
/// 
/// </summary>
public class AzureStorageBlobsProvider : BaseFileProvider
{
    private readonly BlobContainerClient _container;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="container"></param>
    /// <param name="directoryManagerProvider"></param>
    public AzureStorageBlobsProvider(BlobContainerClient container, IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
    {
        _container = container;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storageConnectionString"></param>
    /// <param name="storageContainerName"></param>
    public AzureStorageBlobsProvider(string storageConnectionString, string storageContainerName, IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
    {
        _container = new BlobContainerClient(storageConnectionString, storageContainerName);
    }


    public async Task<FileDetail> GetFileAsync(string path)
    {
        FileDetail file = new FileDetail(this);
        BlobClient client = _container.GetBlobClient(path);
        if (await client.ExistsAsync())
        {
            file.DirectoryPath = path;
            file.Name = path;
            Stream stream = await OpenFileAsync(path);
            file.Length = stream.Length;

        }

        return file;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path is FileName for creating</param>
    /// <returns></returns>
    public override async Task<FileDetail> CreateFileAsync(string path, CancellationToken cancellationToken = default)
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
    public override async Task<Stream> OpenFileAsync(string path, CancellationToken cancellationToken = default)
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

        await client.UploadAsync(stream, true);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path is FileName for checking out</param>
    /// <returns></returns>
    public override async Task<bool> IsExistFileAsync(string path, CancellationToken cancellationToken = default)
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
    public override async Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default)
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

    public override Task TruncateFileAsync(string path, CancellationToken cancellationToken = default)
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
                Length = item.Properties.ContentLength ?? 0,
                DirectoryPath = uri
            });
        }

        return blobsList;
    }
}

