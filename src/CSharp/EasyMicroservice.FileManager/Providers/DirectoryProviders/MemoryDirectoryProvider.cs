using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.DirectoryProviders
{
    public class MemoryDirectoryProvider : BaseDirectoryProvider
    {
        public MemoryDirectoryProvider(string root, IPathProvider pathProvider) : base(root, pathProvider)
        {
        }

        public MemoryDirectoryProvider(string root) : base(root)
        {
        }

        HashSet<string> Directories = new HashSet<string>();

        public override async Task<DirectoryDetail> CreateDirectoryAsync(string path)
        {
            string fullName = NormalizePath(path);
            var directory = await GetDirectoryAsync(fullName);
            Directories.Add(directory.FullPath);
            return directory;
        }

        public override Task<bool> DeleteDirectoryAsync(string path, bool recursive)
        {
            string fullName = NormalizePath(path);
            Directories.Remove(fullName);
            return Task.FromResult(true);
        }

        public override Task<bool> IsExistDirectoryAsync(string path)
        {
            string fullName = NormalizePath(path);
            return Task.FromResult(Directories.Contains(fullName));
        }
    }
}
