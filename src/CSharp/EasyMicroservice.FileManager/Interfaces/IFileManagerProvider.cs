using EasyMicroservice.FileManager.Models;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Interfaces
{
    public interface IFileManagerProvider
    {
        IDirectoryManagerProvider DirectoryManagerProvider { get; set; }
        IPathProvider PathProvider { get; set; }

        Task<FileDetail> GetFileAsync(string path);
        Task<FileDetail> CreateFileAsync(string path);
        Task<Stream> OpenFileAsync(string path);
        Task<bool> IsExistFileAsync(string path);
        Task<bool> DeleteFileAsync(string path);
        Task TruncateFileAsync(string path);
    }
}
