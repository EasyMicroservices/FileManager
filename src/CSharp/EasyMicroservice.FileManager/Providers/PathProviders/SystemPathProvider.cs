using EasyMicroservice.FileManager.Interfaces;
using System.IO;

namespace EasyMicroservice.FileManager.Providers.PathProviders
{
    public class SystemPathProvider : IPathProvider
    {
        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public string GetObjectName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetObjectParentPath(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}
