using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EasyMicroservices.FileManager.AzureStorageBlobs.Interfaces;
using EasyMicroservices.FileManager.AzureStorageBlobs.Models;
using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using Microsoft.AspNetCore.Http;

namespace EasyMicroservices.FileManager.AzureStorageBlobs.Providers;

public class AzureStorageBlobsProvider :IAzureStorageBlobs
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
    public AzureStorageBlobsProvider(string storageConnectionString , string storageContainerName)
    {
        _container = new BlobContainerClient(storageConnectionString, storageContainerName);
    }


    public async Task<ResponseDetail> UploadAsync(IFormFile file)
    {
        ResponseDetail responseDetail = new ResponseDetail();
        try
        {
            BlobClient client = _container.GetBlobClient(file.FileName);

            await using (Stream? stream = file.OpenReadStream())
            {
                await client.UploadAsync(stream);
            }

            responseDetail.Error = false;
            responseDetail.Status = $"File {file.FileName} Uploaded SuccessFully in Azure Container";
            responseDetail.Blob.Uri = client.Uri.AbsoluteUri;
            responseDetail.Blob.Name = client.Name;
            responseDetail.Blob.ContentType = file.ContentType;

        }
        catch (RequestFailedException e) when (e.ErrorCode== BlobErrorCode.BlobAlreadyExists)
        {
            responseDetail.Error = true;
            responseDetail.Status = $"Exception Occured! {file.FileName} is already exists.";

            return responseDetail;
        }
        catch (Exception e)
        {
            responseDetail.Error = true;
            responseDetail.Status = $"Exception Occured! {file.FileName}";

            return responseDetail;
        }

        return responseDetail;

    }

    public async Task<ResponseDetail> DownloadAsync(string fileName)
    {
        ResponseDetail responseDetail = new ResponseDetail();
        try
        {
            BlobClient client = _container.GetBlobClient(fileName);

            if (await client.ExistsAsync())
            {
                Stream blob = await client.OpenReadAsync();
                var data = await client.DownloadContentAsync();

                BlobDetail blobDetail = new BlobDetail()
                {
                    Name = fileName,
                    Content = blob,
                    ContentType = data.Value.Details.ContentType,
                };
                responseDetail = new ResponseDetail()
                {
                    Blob = { Name = fileName, Content = blob, ContentType = data.Value.Details.ContentType },
                    Error = false,
                    Status = ""
                };
                

            }
        }
        catch (RequestFailedException e) when (e.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            responseDetail = new ResponseDetail()
            {
                Error = true,
                Status = $@"Exception Occured! {fileName} not found."
            };
            return responseDetail;
        }
        catch (Exception e)
        {
            responseDetail.Error = true;
            responseDetail.Status = $"Exception Occured! {fileName}";

            return responseDetail;
        }
        return responseDetail;
    }

    public async Task<ResponseDetail> DeleteAsync(string fileName)
    {
        ResponseDetail responseDetail = new ResponseDetail();
        try
        {
            BlobClient client = _container.GetBlobClient(fileName);
         await   client.DeleteAsync();
        }
        catch (RequestFailedException e) when (e.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            responseDetail.Error = true;
            responseDetail.Status = $@"Exception Occured! {fileName} not found.";

            return responseDetail;
        }
        catch (Exception e)
        {
            responseDetail.Error = true;
            responseDetail.Status = $"Exception Occured! {fileName}";

            return responseDetail;
        }

        return responseDetail;
    }

    public async Task<List<BlobDetail>> GetListAsync()
    {
        List<BlobDetail> blobsList = new List<BlobDetail>();
        await foreach (BlobItem item in _container.GetBlobsAsync())
        {
            string name = item.Name;
            string uri = $"{_container.Uri.ToString()}/{name}";
            blobsList.Add(new BlobDetail()
            {
                Name = name,
                ContentType = item.Properties.ContentType,
                Uri = uri
            });
        }

        return blobsList;
    }
}