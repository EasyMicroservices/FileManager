using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using EasyMicroservices.FileManager.Models;
using EasyMicroservices.FileManager.Providers.DirectoryProviders;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.AmazonS3.Providers
{
    /// <summary>
    /// Working with AWS S3 storage
    /// </summary>
    public class AmazonS3BucketProvider : BaseDirectoryProvider
    {
        internal readonly IAmazonS3 _client;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucket"></param>
        public AmazonS3BucketProvider(IAmazonS3 client, string bucket) : base(bucket, new AmazonS3PathProvider())
        {
            _client = client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessKey">Storage AccessKey</param>
        /// <param name="secretKey">Storage SecretKey</param>
        /// <param name="endpointUrl">Endpoint Url</param>
        /// <param name="bucket"></param>
        public AmazonS3BucketProvider(string accessKey, string secretKey, string endpointUrl, string bucket) : base(bucket, new AmazonS3PathProvider())
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonS3Config { ServiceURL = endpointUrl, UseAccelerateEndpoint = false, UseHttp = true, ForcePathStyle = true };
            _client = new AmazonS3Client(awsCredentials, config);
        }


        /// <summary>
        /// create a new Amazon S3 bucket.
        /// </summary>
        /// <param name="path">The name of the bucket to create.</param>
        /// <returns>return result message.</returns>
        public override async Task<DirectoryDetail> CreateDirectoryAsync(string path)
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = path,
                UseClientRegion = true
            };

            await _client.PutBucketAsync(putBucketRequest);

            return new DirectoryDetail(this)
            {
                Name = path,
            };
        }
        /// <summary>
        /// Get a list of the buckets owned by the default user.
        /// </summary>
        /// <returns>The response from the ListingBuckets call that contains a
        /// list of the buckets owned by the default user.</returns>
        public override Task<DirectoryDetail> GetDirectoryAsync(string path)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// check the S3 bucket bucketName exists.
        /// </summary>
        /// <param name="path">The name of the bucket to check.</param>
        /// <returns>return true if bucket exists.</returns>
        public override async Task<bool> IsExistDirectoryAsync(string path)
        {
            return await AmazonS3Util.DoesS3BucketExistV2Async(_client, path);
        }
        /// <summary>
        /// delete the S3 bucket.
        /// </summary>
        /// <param name="path">The name of the bucket to be deleted.</param>
        /// <returns>return result message.</returns>
        public override async Task<bool> DeleteDirectoryAsync(string path)
        {
            var response = await _client.DeleteBucketAsync(path);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Task<bool> DeleteDirectoryAsync(string path, bool recursive)
        {
            throw new System.NotImplementedException();
        }
    }
}
