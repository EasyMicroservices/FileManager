using EasyMicroservice.FileManager.Providers.DirectoryProviders;
using EasyMicroservice.FileManager.Providers.FileProviders;
using System;

namespace EasyMicroservice.FileManager.Tests.Providers.FileProviders
{
    public class DiskFileProviderTest : BaseFileProviderTest
    {
        public DiskFileProviderTest() : base(new DiskFileProvider(new DiskDirectoryProvider(AppDomain.CurrentDomain.BaseDirectory)) { BufferSize = 1024 * 1024 })
        {
        }
    }
}
