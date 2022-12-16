using EasyMicroservice.FileManager.Interfaces;

namespace EasyMicroservice.FileManager.Providers
{
    public class BasePathProvider
    {
        protected string NormalizePath(string path, IDirectoryManagerProvider directoryManagerProvider, IPathProvider pathProvider)
        {
            if (path.StartsWith(directoryManagerProvider.Root))
                return path;
            return pathProvider.Combine(directoryManagerProvider.Root, path);
        }
    }
}
