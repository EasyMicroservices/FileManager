using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using EasyMicroservice.FileManager.Providers.PathProviders;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.DirectoryProviders
{
    public abstract class BaseDirectoryProvider : BasePathProvider, IDirectoryManagerProvider
    {
        public BaseDirectoryProvider(string root, IPathProvider pathProvider)
        {
            Root = root;
            PathProvider = pathProvider;
        }

        public BaseDirectoryProvider(string root)
        {
            Root = root;
            PathProvider = new SystemPathProvider();
        }

        public string Root { get; set; }
        public IPathProvider PathProvider { get; set; }

        protected string NormalizePath(string path)
        {
            return NormalizePath(path, this, PathProvider);
        }

        public abstract Task<DirectoryDetail> CreateDirectoryAsync(string path);
        public virtual Task<DirectoryDetail> GetDirectoryAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult(new DirectoryDetail(this)
            {
                DirectoryPath = PathProvider.GetObjectParentPath(path),
                Name = PathProvider.GetObjectName(path),
            });
        }

        public abstract Task<bool> IsExistDirectoryAsync(string path);

        public virtual Task<bool> DeleteDirectoryAsync(string path)
        {
            return DeleteDirectoryAsync(path, false);
        }

        public abstract Task<bool> DeleteDirectoryAsync(string path, bool recursive);
    }
}
