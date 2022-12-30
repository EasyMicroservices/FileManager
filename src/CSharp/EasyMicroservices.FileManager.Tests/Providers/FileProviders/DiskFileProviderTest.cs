using EasyMicroservices.FileManager.Providers.DirectoryProviders;
using EasyMicroservices.FileManager.Providers.FileProviders;
using System;

namespace EasyMicroservices.FileManager.Tests.Providers.FileProviders
{
    public class DiskFileProviderTest : BaseFileProviderTest
    {
        public DiskFileProviderTest() : base(new DiskFileProvider(new DiskDirectoryProvider(AppDomain.CurrentDomain.BaseDirectory)) { BufferSize = 1024 * 1024 })
        {
        }
    }
}
