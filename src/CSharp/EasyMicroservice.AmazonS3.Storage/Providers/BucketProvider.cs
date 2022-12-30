using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace EasyMicroservice.AmazonS3.Storage.Providers
{
    /// <summary>
    /// Working with AWS S3 storage
    /// </summary>
    public class BucketProvider
    {
        private readonly IAmazonS3 _client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessKey">Storage AccessKey</param>
        /// <param name="secretKey">Storage SecretKey</param>
        /// <param name="endpointUrl">Endpoint Url SecretKey</param>
        public BucketProvider(string accessKey, string secretKey, string endpointUrl)
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonS3Config { ServiceURL = endpointUrl };
            _client = new AmazonS3Client(awsCredentials, config);
        }

        /// <summary>
        /// create a new Amazon S3 bucket.
        /// </summary>
        /// <param name="bucketName">The name of the bucket to create.</param>
        /// <returns>return result message.</returns>
        public async Task<string> PutAsync(string bucketName)
        {
            try
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                var response = await _client.PutBucketAsync(putBucketRequest);
                return $"Result: {response.HttpStatusCode}";
            }
            catch (AmazonS3Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// delete the S3 bucket.
        /// </summary>
        /// <param name="bucketName">The name of the bucket to be deleted.</param>
        /// <returns>return result message.</returns>
        public async Task<string> DeleteAsync(string bucketName)
        {
            try
            {
                var response = await _client.DeleteBucketAsync(bucketName);
                return $"Result: {response.HttpStatusCode}";
            }
            catch (AmazonS3Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// check the S3 bucket bucketName exists.
        /// </summary>
        /// <param name="bucketName">The name of the bucket to check.</param>
        /// <returns>return true if bucket exists.</returns>
        public async Task<bool> IsExistAsync(string bucketName)
        {
            return await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName);
        }

        /// <summary>
        /// Get a list of the buckets owned by the default user.
        /// </summary>
        /// <returns>The response from the ListingBuckets call that contains a
        /// list of the buckets owned by the default user.</returns>
        public async Task<ListBucketsResponse> GetList()
        {
            return await _client.ListBucketsAsync();
        }
    }
}
