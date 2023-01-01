namespace EasyMicroservices.FileManager.AzureStorageBlobs.Models;

public class BlobDetail
{
    public string? Uri { get; set; }
    public string? Name { get; set; }
    public string? ContentType { get; set; }
    public Stream? Content { get; set; }
}