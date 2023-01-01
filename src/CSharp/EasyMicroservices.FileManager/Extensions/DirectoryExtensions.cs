using EasyMicroservices.FileManager.Models;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager
{
    /// <summary>
    /// Extensions of directory to easy of use
    /// </summary>
    public static class DirectoryExtensions
    {
        /// <summary>
        /// check if the directory is exist
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static Task<bool> IsExistAsync(this DirectoryDetail directory)
        {
            return directory.Provider.IsExistDirectoryAsync(directory.FullPath);
        }

        /// <summary>
        /// create a directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static Task<DirectoryDetail> CreateDirectory(this DirectoryDetail directory)
        {
            return directory.Provider.CreateDirectoryAsync(directory.FullPath);
        }
    }
}
