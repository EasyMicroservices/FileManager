using EasyMicroservices.FileManager.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace EasyMicroservices.FileManager.Tests.Providers.DirectoryProviders
{
    public abstract class BaseDirectoryProviderTest
    {
        readonly IDirectoryManagerProvider _directoryManagerProvider;
        public BaseDirectoryProviderTest(IDirectoryManagerProvider directoryManagerProvider)
        {
            _directoryManagerProvider = directoryManagerProvider;
        }

        [Theory]
        [InlineData("Ali")]
        [InlineData("Mahdi")]
        public async Task CreateDirectory(string name)
        {
            if (await _directoryManagerProvider.IsExistDirectoryAsync(name))
                Assert.True(await _directoryManagerProvider.DeleteDirectoryAsync(name, true));
            Assert.False(await _directoryManagerProvider.IsExistDirectoryAsync(name));
            await _directoryManagerProvider.CreateDirectoryAsync(name);
            Assert.True(await _directoryManagerProvider.IsExistDirectoryAsync(name));
            var dir = await _directoryManagerProvider.GetDirectoryAsync(name);
            Assert.NotEmpty(dir.Name);
            Assert.NotEmpty(dir.DirectoryPath);
            Assert.True(await _directoryManagerProvider.IsExistDirectoryAsync(dir.FullPath));
        }

        [Theory]
        [InlineData("Saeed")]
        [InlineData("Reza")]
        public async Task DeleteDirectory(string name)
        {
            Assert.False(await _directoryManagerProvider.IsExistDirectoryAsync(name));
            await _directoryManagerProvider.CreateDirectoryAsync(name);
            Assert.True(await _directoryManagerProvider.IsExistDirectoryAsync(name));
            Assert.True(await _directoryManagerProvider.DeleteDirectoryAsync(name));
            Assert.False(await _directoryManagerProvider.IsExistDirectoryAsync(name));
        }

        [Theory]
        [InlineData("Saeed", "Recursive")]
        [InlineData("Reza", "Recursive")]
        public async Task DeleteDirectoryRecursive(string name, string inside)
        {
            string path = _directoryManagerProvider.PathProvider.Combine(name, inside);
            Assert.False(await _directoryManagerProvider.IsExistDirectoryAsync(path));
            await _directoryManagerProvider.CreateDirectoryAsync(path);
            Assert.True(await _directoryManagerProvider.IsExistDirectoryAsync(path));
            Assert.True(await _directoryManagerProvider.DeleteDirectoryAsync(name, true));
            Assert.False(await _directoryManagerProvider.IsExistDirectoryAsync(name));
        }
    }
}
