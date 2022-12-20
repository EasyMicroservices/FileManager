using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using EasyMicroservice.FileManager.Providers.PathProviders;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.DirectoryProviders
{
    /// <summary>
    /// Base logics of the directory providers
    /// </summary>
    public abstract class BaseDirectoryProvider : BasePathProvider, IDirectoryManagerProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root">default root path</param>
        /// <param name="pathProvider">path provider</param>
        public BaseDirectoryProvider(string root, IPathProvider pathProvider)
        {
            Root = root;
            PathProvider = pathProvider;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public BaseDirectoryProvider(string root)
        {
            Root = root;
            PathProvider = new SystemPathProvider();
        }

        /// <summary>
        /// Root Path's of directory
        /// </summary>
        public string Root { get; set; }
        /// <summary>
        /// Path manager
        /// </summary>
        public IPathProvider PathProvider { get; set; }
        /// <summary>
        /// normalize with the path manager
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string NormalizePath(string path)
        {
            return NormalizePath(path, this, PathProvider);
        }
        /// <summary>
        /// Create new directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract Task<DirectoryDetail> CreateDirectoryAsync(string path);
        /// <summary>
        /// Get directory's details
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual Task<DirectoryDetail> GetDirectoryAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult(new DirectoryDetail(this)
            {
                DirectoryPath = PathProvider.GetObjectParentPath(path),
                Name = PathProvider.GetObjectName(path),
            });
        }
        /// <summary>
        /// check if directory is exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract Task<bool> IsExistDirectoryAsync(string path);
        /// <summary>
        /// delete the directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual Task<bool> DeleteDirectoryAsync(string path)
        {
            return DeleteDirectoryAsync(path, false);
        }
        /// <summary>
        /// delete directory recursive
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public abstract Task<bool> DeleteDirectoryAsync(string path, bool recursive);
    }
}
