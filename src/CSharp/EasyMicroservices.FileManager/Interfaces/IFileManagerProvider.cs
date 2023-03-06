using EasyMicroservices.FileManager.Models;
using System.IO;
using System.Text;
using System.Threading;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FileDetail> GetFileAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// Create new file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FileDetail> CreateFileAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> OpenFileAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// write stream to file path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WriteStreamToFileAsync(string path, Stream stream, CancellationToken cancellationToken = default);
        /// <summary>
        /// check if file is exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsExistFileAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// set length of file as 0
        /// make a file data empty
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task TruncateFileAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// Write bytes to a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
        /// <summary>
        /// read bytes from a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// read all text lines from a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);
        /// <summary>
        /// read all text lines from a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// read all text from a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);
        /// <summary>
        /// read all text from a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);
    }
}
