using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Providers.DirectoryProviders
{
    /// <summary>
    /// Disk manager of directory on a disk
    /// </summary>
    public class DiskDirectoryProvider : BaseDirectoryProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pathProvider"></param>
        public DiskDirectoryProvider(string root, IPathProvider pathProvider) : base(root, pathProvider)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public DiskDirectoryProvider(string root) : base(root)
        {
        }
        /// <summary>
        /// Create new directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<DirectoryDetail> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            path = NormalizePath(path);
            if (await CheckPermissionAsync(path))
                Directory.CreateDirectory(path);
            return await GetDirectoryAsync(path);
        }
        /// <summary>
        /// delete the directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<bool> DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken = default)
        {
            path = NormalizePath(path);
            if (!path.StartsWith(Root))
                throw new Exception($"Warning to delete disk folder {path}");
            if (await CheckPermissionAsync(path))
                Directory.Delete(path, recursive);
            return true;
        }
        /// <summary>
        /// check if directory is exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<bool> IsExistDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            path = NormalizePath(path);
            await CheckPermissionAsync(path);
            return Directory.Exists(path);
        }
    }
}
