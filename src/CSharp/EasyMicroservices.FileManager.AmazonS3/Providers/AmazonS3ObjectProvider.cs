using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using EasyMicroservices.FileManager.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.AmazonS3.Providers
{
    /// <summary>
    /// Working with AWS S3 storage
    /// </summary>
    public class AmazonS3ObjectProvider : IFileManagerProvider
    {
        private readonly IAmazonS3 _client;
        private readonly string _objectName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryManagerProvider"></param>
        /// <param name="client"></param>
        /// <param name="objectName"></param>
        public AmazonS3ObjectProvider(IDirectoryManagerProvider directoryManagerProvider, IAmazonS3 client, string objectName)
        {
            DirectoryManagerProvider = directoryManagerProvider;
            PathProvider = directoryManagerProvider.PathProvider;
            _client = client;
            _objectName = objectName;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDirectoryManagerProvider DirectoryManagerProvider { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IPathProvider PathProvider { get; set; }

        /// <summary>
        /// Create a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<FileDetail> CreateFileAsync(string path)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = path,
                Key = _objectName,
                FilePath = path,
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
        public async Task<bool> DeleteFileAsync(string path)
        {
            DeleteObjectRequest request = new()
            {
                BucketName = path,
                Key = _objectName 
            };

            DeleteObjectResponse response = await _client.DeleteObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;

        }
        /// <summary>
        /// get file's details
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<FileDetail> GetFileAsync(string path)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// check if file is exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<bool> IsExistFileAsync(string path)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<Stream> OpenFileAsync(string path)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = path,
                Key = _objectName,
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
        public Task TruncateFileAsync(string path)
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
        public Task WriteStreamToFileAsync(string path, Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
