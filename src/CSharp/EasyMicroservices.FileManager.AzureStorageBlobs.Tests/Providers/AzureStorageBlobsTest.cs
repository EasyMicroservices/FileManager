using Azure.Storage.Blobs;
using EasyMicroservices.FileManager.AzureStorageBlobs.Providers;

namespace EasyMicroservices.FileManager.AzureStorageBlobs.Tests.Providers;

 
public class AzureStorageBlobsTest 
{
    private readonly BlobContainerClient _blobContainerClient ;

    public AzureStorageBlobsTest()
    {
        
        _blobContainerClient = new BlobContainerClient(@"UseDevelopmentStorage=true", "sample-container");
        _blobContainerClient.CreateIfNotExists();
      
    }
    

    [Theory]
    [InlineData("Saba.txt")]
    [InlineData("Ali.txt")]
    [InlineData("CreateFile\\Ali.txt")]
    [InlineData("CreateFile\\Mahdi.txt")]
    public async Task CreateFile(string path)
    {
        AzureStorageBlobsProvider azureStorageBlobsProvider = new AzureStorageBlobsProvider(_blobContainerClient);
       
        if (await azureStorageBlobsProvider.IsExistFileAsync(path))
            Assert.True(await azureStorageBlobsProvider.DeleteFileAsync(path));
        Assert.False(await azureStorageBlobsProvider.IsExistFileAsync(path));
        var createdBlob = await azureStorageBlobsProvider.CreateFileAsync(path);
        Assert.Equal(path, createdBlob.Name);
        Assert.True(await azureStorageBlobsProvider.IsExistFileAsync(path));
        var file = await azureStorageBlobsProvider.GetFileAsync(path);
        Assert.NotEmpty(file.Name);
        Assert.NotEmpty(file.DirectoryPath);
        Assert.True(await azureStorageBlobsProvider.IsExistFileAsync(file.Name));
        
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task WriteStreamToFile(string path, Stream stream)
    {
        AzureStorageBlobsProvider azureStorageBlobsProvider = new AzureStorageBlobsProvider(_blobContainerClient);

        await azureStorageBlobsProvider.WriteStreamToFileAsync(path, stream);
        var file = await azureStorageBlobsProvider.GetFileAsync(path);
        Assert.Equal(path, file.Name);
        Assert.True(file.Length > 0);

        }


    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { "Saba.txt",GenerateStream( "this is test data for Saba.txt") };
        yield return new object[] { "Ali.txt",GenerateStream( "this is test data for Ali.txt") };
        yield return new object[] { "Mahdi.txt",GenerateStream( "this is test data for Mahdi.txt") };
        yield return new object[] { "New.txt",GenerateStream( "this is test data for New.txt") };
    }

    private static Stream GenerateStream(string s)
    {
      
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
  

}