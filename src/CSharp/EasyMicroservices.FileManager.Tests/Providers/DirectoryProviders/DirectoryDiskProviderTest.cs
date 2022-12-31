using EasyMicroservices.FileManager.Providers.DirectoryProviders;
using EasyMicroservices.FileManager.Providers.PathProviders;
using System;
using System.IO;

namespace EasyMicroservices.FileManager.Tests.Providers.DirectoryProviders
{
    public class DirectoryDiskProviderTest : BaseDirectoryProviderTest
    {
        public DirectoryDiskProviderTest() : base(new DiskDirectoryProvider(AppDomain.CurrentDomain.BaseDirectory))
        {
        }
    }

    public class DirectoryDiskProviderSystemPathProviderTest : BaseDirectoryProviderTest
    {
        public DirectoryDiskProviderSystemPathProviderTest() : base(new DiskDirectoryProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemPathProvider"), new SystemPathProvider()))
        {
        }
    }
}
