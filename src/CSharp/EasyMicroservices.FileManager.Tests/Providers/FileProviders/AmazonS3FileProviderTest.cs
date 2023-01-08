#if (NET6_0)
using Amazon.S3;
using EasyMicroservices.FileManager.AmazonS3.Providers;
using EasyMicroservices.Laboratory.Constants;
using EasyMicroservices.Laboratory.Engine;
using EasyMicroservices.Laboratory.Engine.Net.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace EasyMicroservices.FileManager.Tests.Providers.FileProviders
{
    public class AmazonS3FileProviderTest : BaseFileProviderTest
    {
        const int amazonS3Port = 1401;
        public AmazonS3FileProviderTest() : base(new AmazonS3ObjectProvider(new AmazonS3BucketProvider(GetClient(), "mybucket/temp")))
        {
        }

        static IAmazonS3 GetClient()
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("ATESTQTEST452TEST76L", "SecretKey");
            var config = new AmazonS3Config
            {
                ServiceURL = $"http://localhost:{amazonS3Port}"
            };
            return new AmazonS3Client(awsCredentials, config);
        }

        static bool _isInitialized = false;
        static SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        public override async Task OnInitialize()
        {
            if (_isInitialized)
                return;
            try
            {
                await Semaphore.WaitAsync();
                _isInitialized = true;

                ResourceManager resourceManager = new ResourceManager();
                HttpHandler httpHandler = new HttpHandler(resourceManager);
                await httpHandler.Start(amazonS3Port);
                resourceManager.Append(@"PUT *RequestSkipBody* HTTP/1.1
Expect: 100-continue
x-amz-meta-title: someTitle
User-Agent: *RequestSkipBody*
amz-sdk-invocation-id: *RequestSkipBody*
amz-sdk-request: attempt=1; max=5
Host: *RequestSkipBody*
X-Amz-Date: *RequestSkipBody*
X-Amz-Content-SHA256: *RequestSkipBody*
Authorization: *RequestSkipBody*
Content-Length: 0
Content-Type: text/plain*RequestSkipBody*"
,
@"HTTP/1.1 200 OK
x-amz-id-2: A/B+B/C+C=
x-amz-request-id: ReqId6Q7
Date: Sat, 07 Jan 2023 16:24:56 GMT
x-amz-version-id: VersionBrGV
ETag: ""d41d8cd98f00b204e9800998ecf8427e""
Server: AmazonS3
Content-Length: 0

");
                //try
                //{
                //    var fileDetails = await _fileManagerProvider.IsExistFileAsync("Ali.txt");
                //}
                //catch (System.Exception ex)
                //{

                //}
                //HttpClient httpClient = new HttpClient();
                //httpClient.DefaultRequestHeaders.Add(RequestTypeHeaderConstants.RequestTypeHeader, RequestTypeHeaderConstants.GiveMeLastFullRequestHeaderValue);
                //var httpResponse = await httpClient.GetAsync($"http://localhost:{amazonS3Port}");
                //var textResponse = await httpResponse.Content.ReadAsStringAsync();

            }
            finally
            {
                Semaphore.Release();
            }
        }

        public override async Task CreateFile(string name)
        {
            await OnInitialize();
            await _fileManagerProvider.CreateFileAsync(name);
        }

        public override Task DeleteFile(string name)
        {
            return TaskHelper.GetCompletedTask();
        }

        public override Task StreamWriteFile(string name)
        {
            return TaskHelper.GetCompletedTask();
        }

        public override Task TruncateFile(string name)
        {
            return TaskHelper.GetCompletedTask();
        }
    }
}
#endif