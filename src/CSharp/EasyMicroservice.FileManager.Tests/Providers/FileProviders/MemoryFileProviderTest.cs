using EasyMicroservice.FileManager.Providers.DirectoryProviders;
using EasyMicroservice.FileManager.Providers.FileProviders;

namespace EasyMicroservice.FileManager.Tests.Providers.FileProviders
{
    public class MemoryFileProviderTest : BaseFileProviderTest
    {
        public MemoryFileProviderTest() : base(new MemoryFileProvider(new MemoryDirectoryProvider("MemoryFileProviderTest")))
        {
        }
    }
}
