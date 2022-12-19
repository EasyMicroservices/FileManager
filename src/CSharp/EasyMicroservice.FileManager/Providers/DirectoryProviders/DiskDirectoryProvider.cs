using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.DirectoryProviders
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
        /// <returns></returns>
        public override Task<DirectoryDetail> CreateDirectoryAsync(string path)
        {
            path = NormalizePath(path);
            Directory.CreateDirectory(path);
            return GetDirectoryAsync(path);
        }
        /// <summary>
        /// delete the directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override Task<bool> DeleteDirectoryAsync(string path, bool recursive)
        {
            path = NormalizePath(path);
            if (!path.StartsWith(Root))
                throw new Exception($"Warning to delete disk folder {path}");
            Directory.Delete(path, recursive);
            return Task.FromResult(true);
        }
        /// <summary>
        /// check if directory is exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override Task<bool> IsExistDirectoryAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult(Directory.Exists(path));
        }
    }
}
