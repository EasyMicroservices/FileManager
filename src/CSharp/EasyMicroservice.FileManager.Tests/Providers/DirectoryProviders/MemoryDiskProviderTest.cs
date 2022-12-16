using EasyMicroservice.FileManager.Providers.DirectoryProviders;

namespace EasyMicroservice.FileManager.Tests.Providers.DirectoryProviders
{
    public class MemoryDiskProviderTest : BaseDirectoryProviderTest
    {
        public MemoryDiskProviderTest() : base(new MemoryDirectoryProvider("MemoryDisk"))
        {
        }
    }
}
