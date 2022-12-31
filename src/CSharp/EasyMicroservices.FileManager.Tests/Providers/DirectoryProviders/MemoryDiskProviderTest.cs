using EasyMicroservices.FileManager.Providers.DirectoryProviders;
using EasyMicroservices.FileManager.Providers.PathProviders;

namespace EasyMicroservices.FileManager.Tests.Providers.DirectoryProviders
{
    public class MemoryDiskProviderTest : BaseDirectoryProviderTest
    {
        public MemoryDiskProviderTest() : base(new MemoryDirectoryProvider("MemoryDisk"))
        {
        }
    }

    public class MemoryDiskProviderSystemPathProviderTest : BaseDirectoryProviderTest
    {
        public MemoryDiskProviderSystemPathProviderTest() : base(new MemoryDirectoryProvider("MemoryDiskSystemPathProvider", new SystemPathProvider()))
        {
        }
    }
}
