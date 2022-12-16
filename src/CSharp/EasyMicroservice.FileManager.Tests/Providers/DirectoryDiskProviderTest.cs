using EasyMicroservice.FileManager.Providers.DirectoryProviders;
using System;

namespace EasyMicroservice.FileManager.Tests.Providers
{
    public class DirectoryDiskProviderTest : BaseDirectoryProviderTest
    {
        public DirectoryDiskProviderTest() : base(new DiskDirectoryProvider(AppDomain.CurrentDomain.BaseDirectory))
        {
        }
    }
}
