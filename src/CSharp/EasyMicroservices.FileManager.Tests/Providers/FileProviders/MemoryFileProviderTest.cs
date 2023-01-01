using EasyMicroservices.FileManager.Providers.DirectoryProviders;
using EasyMicroservices.FileManager.Providers.FileProviders;

namespace EasyMicroservices.FileManager.Tests.Providers.FileProviders
{
    public class MemoryFileProviderTest : BaseFileProviderTest
    {
        public MemoryFileProviderTest() : base(new MemoryFileProvider(new MemoryDirectoryProvider("MemoryFileProviderTest")))
        {
        }
    }
}
