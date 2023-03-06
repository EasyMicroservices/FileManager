using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Providers.DirectoryProviders
{
    /// <summary>
    /// Memory manager of directory on a disk
    /// </summary>
    public class MemoryDirectoryProvider : BaseDirectoryProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pathProvider"></param>
        public MemoryDirectoryProvider(string root, IPathProvider pathProvider) : base(root, pathProvider)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public MemoryDirectoryProvider(string root) : base(root)
        {
        }

        HashSet<string> Directories = new HashSet<string>();
        /// <summary>
        /// Create new directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<DirectoryDetail> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            string fullName = NormalizePath(path);
            var directory = await GetDirectoryAsync(fullName);
            Directories.Add(directory.FullPath);
            return directory;
        }
        /// <summary>
        /// delete the directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<bool> DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken = default)
        {
            string fullName = NormalizePath(path);
            Directories.Remove(fullName);
            return Task.FromResult(true);
        }
        /// <summary>
        /// check if directory is exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<bool> IsExistDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            string fullName = NormalizePath(path);
            return Task.FromResult(Directories.Contains(fullName));
        }
    }
}
