using EasyMicroservice.FileManager.Interfaces;
using System.IO;

namespace EasyMicroservice.FileManager.Providers.PathProviders
{
    /// <summary>
    /// System provider
    /// </summary>
    public class SystemPathProvider : IPathProvider
    {
        /// <summary>
        /// Combine multiple paths
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        /// Get object's name
        /// it's like File name or Directory name
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetObjectName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Get object parent's path
        /// it's file's directory path
        /// or directory's parent's directory path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetObjectParentPath(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}
