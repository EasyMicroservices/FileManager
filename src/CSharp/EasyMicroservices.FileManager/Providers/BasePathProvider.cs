using EasyMicroservices.FileManager.Interfaces;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Providers
{
    /// <summary>
    /// Base logics of path provider
    /// </summary>
    public class BasePathProvider
    {
        /// <summary>
        /// Nomalize a path with the path provider
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directoryManagerProvider"></param>
        /// <param name="pathProvider"></param>
        /// <returns></returns>
        protected string NormalizePath(string path, IDirectoryManagerProvider directoryManagerProvider, IPathProvider pathProvider)
        {
            if (path.StartsWith(directoryManagerProvider.Root))
                return path;
            return pathProvider.Combine(directoryManagerProvider.Root, path);
        }
        /// <summary>
        /// Check application has permission for a path
        /// </summary>
        /// <returns></returns>
        public virtual Task<bool> CheckPermissionAsync(string path)
        {
            return Task.FromResult(true);
        }
    }
}
