using EasyMicroservice.FileManager.Providers.DirectoryProviders;

namespace EasyMicroservice.FileManager.Tests.Providers
{
    public class MemoryDiskProviderTest : BaseDirectoryProviderTest
    {
        public MemoryDiskProviderTest() : base(new MemoryDirectoryProvider("MemoryDisk"))
        {
        }
    }
}
