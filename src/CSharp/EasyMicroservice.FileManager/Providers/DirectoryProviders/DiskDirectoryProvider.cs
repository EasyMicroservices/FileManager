using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.DirectoryProviders
{
    public class DiskDirectoryProvider : BaseDirectoryProvider
    {
        public DiskDirectoryProvider(string root, IPathProvider pathProvider) : base(root, pathProvider)
        {
        }

        public DiskDirectoryProvider(string root) : base(root)
        {
        }

        public override Task<DirectoryDetail> CreateDirectoryAsync(string path)
        {
            path = NormalizePath(path);
            Directory.CreateDirectory(path);
            return GetDirectoryAsync(path);
        }

        public override Task<bool> DeleteDirectoryAsync(string path, bool recursive)
        {
            path = NormalizePath(path);
            if (!path.StartsWith(Root))
                throw new Exception($"Warning to delete disk folder {path}");
            Directory.Delete(path, recursive);
            return Task.FromResult(true);
        }

        public override Task<bool> IsExistDirectoryAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult(Directory.Exists(path));
        }
    }
}
