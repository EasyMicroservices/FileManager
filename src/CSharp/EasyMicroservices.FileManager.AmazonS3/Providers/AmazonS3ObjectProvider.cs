using Amazon.S3;
using Amazon.S3.Model;
using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using EasyMicroservices.FileManager.Providers.FileProviders;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.AmazonS3.Providers
{
    /// <summary>
    /// Working with AWS S3 storage
    /// </summary>
    public class AmazonS3ObjectProvider : BaseFileProvider
    {
        private readonly IAmazonS3 _client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryManagerProvider"></param>
        /// <param name="client"></param>
        public AmazonS3ObjectProvider(IDirectoryManagerProvider directoryManagerProvider, IAmazonS3 client) : base(directoryManagerProvider)
        {
            DirectoryManagerProvider = directoryManagerProvider;
            PathProvider = directoryManagerProvider.PathProvider;
            _client = client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryManagerProvider"></param>
        public AmazonS3ObjectProvider(IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
        {
            DirectoryManagerProvider = directoryManagerProvider;
            PathProvider = directoryManagerProvider.PathProvider;
            _client = (directoryManagerProvider as AmazonS3BucketProvider)._client;
        }

        /// <summary>
        /// Create a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override async Task<FileDetail> CreateFileAsync(string path)
        {
            var file = await GetFileAsync(path);
            var putRequest = new PutObjectRequest
            {
                BucketName = DirectoryManagerProvider.Root,
                Key = file.Name,
                ContentType = "text/plain"
            };

            putRequest.Metadata.Add("x-amz-meta-title", "someTitle");
            PutObjectResponse response = await _client.PutObjectAsync(putRequest, new System.Threading.CancellationToken());

            var objects3 = new FileDetail(this);
            foreach (PropertyInfo prop in response.GetType().GetProperties())
            {
                objects3.Name = prop.Name;
                objects3.DirectoryPath = path;
            }

            return objects3;
        }
        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override async Task<bool> DeleteFileAsync(string path)
        {
            var file = await GetFileAsync(path);
            DeleteObjectRequest request = new()
            {
                BucketName = path,
                Key = file.Name
            };

            DeleteObjectResponse response = await _client.DeleteObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;

        }

        /// <summary>
        /// check if file is exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override async Task<bool> IsExistFileAsync(string path)
        {
            var file = await GetFileAsync(path);
            return await _client.DoesS3BucketExistAsync(file.Name);
        }
        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override async Task<Stream> OpenFileAsync(string path)
        {
            var file = await GetFileAsync(path);
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = path,
                Key = file.Name,
            };

            using (GetObjectResponse response = await _client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            {
                return responseStream;
            }
        }
        /// <summary>
        /// set length of file as 0
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Task TruncateFileAsync(string path)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// write stream to a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Task WriteStreamToFileAsync(string path, Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
