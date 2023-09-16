using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using EasyMicroservices.FileManager.Providers.PathProviders;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Providers.DirectoryProviders
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<DirectoryDetail> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// Get directory's details
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<DirectoryDetail> GetDirectoryAsync(string path, CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<bool> IsExistDirectoryAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// delete the directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<bool> DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            return DeleteDirectoryAsync(path, false);
        }
        /// <summary>
        /// delete directory recursive
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<bool> DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken = default);
        /// <summary>
        /// complete check permission
        /// </summary>
        /// <param name="isComplete"></param>
        public virtual void CompleteCheckPermission(bool isComplete)
        {

        }
    }
}
