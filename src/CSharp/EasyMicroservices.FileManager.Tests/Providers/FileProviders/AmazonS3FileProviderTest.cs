using Amazon.S3;
using EasyMicroservices.FileManager.AmazonS3.Providers;
using EasyMicroservices.Laboratory.Constants;
using EasyMicroservices.Laboratory.Engine;
using EasyMicroservices.Laboratory.Engine.Net.Http;
using EasyMicroservices.Laboratory.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EasyMicroservices.FileManager.Tests.Providers.FileProviders
{
    public class AmazonS3FileProviderTest : BaseFileProviderTest
    {
        static AmazonS3FileProviderTest()
        {
            HttpHandler = new HttpHandler(ResourceManager);
        }

        const int amazonS3Port = 1801;
        public AmazonS3FileProviderTest() : base(new AmazonS3ObjectProvider(new AmazonS3BucketProvider(GetClient(), "mybucket/temp")))
        {
        }

        static HttpHandler HttpHandler = default;
        static ResourceManager ResourceManager = new ResourceManager();

        static IAmazonS3 GetClient()
        {
            string url = $"http://localhost:{amazonS3Port}";
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("ATESTQTEST452TEST76L", "SecretKey");
            var config = new AmazonS3Config
            {
                ServiceURL = url,
            };
            return new AmazonS3Client(awsCredentials, config);
        }

        static bool _isInitialized = false;
        static SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        public override async Task OnInitialize(string methodName)
        {
            try
            {
                await Semaphore.WaitAsync();
                if (!_isInitialized)
                    await HttpHandler.Start(amazonS3Port);
                _isInitialized = true;
                ResourceManager.Clear();

                if (methodName == nameof(CreateFile))
                    ResourceManager.Append(CreateFileScope());
                else
                    ResourceManager.Append(DeleteFileScope());

                //create file
                ResourceManager.Append(@"PUT *RequestSkipBody* HTTP/1.1
*RequestSkipBody*"
                ,
                @"HTTP/1.1 200 OK
x-amz-id-2: e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855
x-amz-request-id: 0A49CE4060975EAC
x-amz-version-id: e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855
Date: Wed, 12 Oct 2009 17:50:00 GMT
ETag: ""D41D8CD98F00B204E9800998ECF8427E""
Content-Length: 0
Connection: close
Server: AmazonS3

");
            }
            finally
            {
                Semaphore.Release();
            }
        }

        Scope CreateFileScope()
        {
            var scope = new Scope();
            AppendExistFile(scope);
            AppendDeleteFile(scope);
            AppendNotExistFile(scope);
            AppendExistFile(scope);
            return scope;
        }

        Scope DeleteFileScope()
        {
            var scope = new Scope();
            AppendNotExistFile(scope);
            AppendExistFile(scope);
            AppendDeleteFile(scope);
            AppendNotExistFile(scope);
            return scope;
        }

        void AppendExistFile(Scope scope)
        {
            //exist file
            scope.AppendNext(@"GET *RequestSkipBody*?acl HTTP/1.1
*RequestSkipBody*"
,
@$"HTTP/1.1 200 OK
x-amz-id-2: eftixk72aD6Ap51TnqcoF8eFidJG9Z/2mkiDFu8yU9AS1ed4OpIszj7UDNEHGran
x-amz-request-id: 318BC8BC148832E5
Date: Wed, 28 Oct 2009 22:32:00 GMT
Last-Modified: Sun, 1 Jan 2006 12:00:00 GMT
Content-Type: application/xml
Connection: close
Server: AmazonS3
Content-Length: 0

<AccessControlPolicy>
  <Owner>
    <ID>{Guid.NewGuid()}</ID>
    <DisplayName>CustomersName@amazon.com</DisplayName>
  </Owner>
  <AccessControlList>
    <Grant>
      <Grantee xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
			xsi:type=""CanonicalUser"">
        <ID>75aa57f09aa0c8caeab4f8c24e99d10f8e7faeebf76c078efc7c6caea54ba06a</ID>
        <DisplayName>CustomersName@amazon.com</DisplayName>
      </Grantee>
      <Permission>FULL_CONTROL</Permission>
    </Grant>
  </AccessControlList>
</AccessControlPolicy> 
");
        }

        void AppendNotExistFile(Scope scope)
        {
            scope.AppendNext(@"GET *RequestSkipBody*?acl HTTP/1.1
*RequestSkipBody*"
,
@"HTTP/1.1 404 NotFound
x-amz-id-2: eftixk72aD6Ap51TnqcoF8eFidJG9Z/2mkiDFu8yU9AS1ed4OpIszj7UDNEHGran
x-amz-request-id: 318BC8BC148832E5
Date: Wed, 28 Oct 2009 22:32:00 GMT
Last-Modified: Sun, 1 Jan 2006 12:00:00 GMT
Content-Type: application/xml
Connection: close
Server: AmazonS3
Content-Length: 0

<Error>
  <Code>NoSuchBucket</Code>
  <Message>The resource you requested does not exist</Message>
  <Resource>/mybucket/myfoto.jpg</Resource> 
  <RequestId>4442587FB7D0A2F9</RequestId>
</Error>
");
        }

        void AppendDeleteFile(Scope scope)
        {
            //delete file
            scope.AppendNext(@"DELETE *RequestSkipBody* HTTP/1.1
*RequestSkipBody*"
,
@"HTTP/1.1 204 No Content
x-amz-id-2: JuKZqmXuiwFeDQxhD7M8KtsKobSzWA1QEjLbTMTagkKdBX2z7Il/jGhDeJ3j6s80
x-amz-request-id: 32FE2CEB32F5EE25
Date: Wed, 01 Mar  2006 12:00:00 GMT
Connection: close
Server: AmazonS3

");
        }

        public override async Task GetException(Exception ex)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(RequestTypeHeaderConstants.RequestTypeHeader, RequestTypeHeaderConstants.GiveMeLastFullRequestHeaderValue);
            var httpResponse = await httpClient.GetAsync($"http://localhost:{amazonS3Port}");
            var textResponse = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(textResponse, ex);
        }

        public override Task StreamWriteFile(string name)
        {
            return TaskHelper.GetCompletedTask();
        }

        public override Task TruncateFile(string name)
        {
            return TaskHelper.GetCompletedTask();
        }

        public override Task WriteAndReadFile(string name)
        {
            return TaskHelper.GetCompletedTask();
        }
    }
}
