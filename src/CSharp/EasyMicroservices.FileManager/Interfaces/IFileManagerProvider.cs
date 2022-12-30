using EasyMicroservices.FileManager.Models;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Interfaces
{
    /// <summary>
    /// Provider of file to manage files stuff
    /// </summary>
    public interface IFileManagerProvider
    {
        /// <summary>
        /// Directory manager of this file manager
        /// place of files
        /// </summary>
        IDirectoryManagerProvider DirectoryManagerProvider { get; set; }
        /// <summary>
        /// Path manager
        /// </summary>
        IPathProvider PathProvider { get; set; }
        /// <summary>
        /// Get file's details
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<FileDetail> GetFileAsync(string path);
        /// <summary>
        /// Create new file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<FileDetail> CreateFileAsync(string path);
        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<Stream> OpenFileAsync(string path);
        /// <summary>
        /// write stream to file path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task WriteStreamToFileAsync(string path, Stream stream);
        /// <summary>
        /// check if file is exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<bool> IsExistFileAsync(string path);
        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<bool> DeleteFileAsync(string path);
        /// <summary>
        /// set length of file as 0
        /// make a file data empty
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task TruncateFileAsync(string path);
    }
}
