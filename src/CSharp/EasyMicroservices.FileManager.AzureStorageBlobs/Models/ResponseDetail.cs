namespace EasyMicroservices.FileManager.AzureStorageBlobs.Models;

public class ResponseDetail
{
    public string? Status { get; set; }
    public bool Error { get; set; }
    public BlobDetail Blob { get; }

    public ResponseDetail()
    {
        Blob = new BlobDetail();
    }

}