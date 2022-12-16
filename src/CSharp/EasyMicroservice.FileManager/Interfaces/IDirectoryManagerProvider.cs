using EasyMicroservice.FileManager.Models;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Interfaces
{
    public interface IDirectoryManagerProvider
    {
        string Root { get; set; }
        IPathProvider PathProvider { get; set; }

        Task<DirectoryDetail> CreateDirectoryAsync(string path);
        Task<DirectoryDetail> GetDirectoryAsync(string path);
        Task<bool> IsExistDirectoryAsync(string path);
        Task<bool> DeleteDirectoryAsync(string path);
        Task<bool> DeleteDirectoryAsync(string path, bool recursive);
    }
}
