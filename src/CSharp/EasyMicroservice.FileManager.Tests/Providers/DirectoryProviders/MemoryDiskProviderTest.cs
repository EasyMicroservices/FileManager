using EasyMicroservice.FileManager.Providers.DirectoryProviders;
using EasyMicroservice.FileManager.Providers.PathProviders;

namespace EasyMicroservice.FileManager.Tests.Providers.DirectoryProviders
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
