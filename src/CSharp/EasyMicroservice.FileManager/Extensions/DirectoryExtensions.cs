using EasyMicroservice.FileManager.Models;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager
{
    public static class DirectoryExtensions
    {
        public static Task<bool> IsExistAsync(this DirectoryDetail directory)
        {
            return directory.Provider.IsExistDirectoryAsync(directory.FullPath);
        }

        public static Task<DirectoryDetail> CreateDirectory(this DirectoryDetail directory)
        {
            return directory.Provider.CreateDirectoryAsync(directory.FullPath);
        }
    }
}
