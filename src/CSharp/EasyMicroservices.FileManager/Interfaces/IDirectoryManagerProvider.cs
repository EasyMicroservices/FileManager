using EasyMicroservices.FileManager.Models;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Interfaces
{
    /// <summary>
    /// Provider of directory to manage folders stuff
    /// </summary>
    public interface IDirectoryManagerProvider
    {
        /// <summary>
        /// Root folder's path to save data in it
        /// </summary>
        string Root { get; set; }
        /// <summary>
        /// Path manager
        /// </summary>
        IPathProvider PathProvider { get; set; }
        /// <summary>
        /// Create new directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DirectoryDetail> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// Get directory's details
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DirectoryDetail> GetDirectoryAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// check if directory is exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsExistDirectoryAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// delete the directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default);
        /// <summary>
        /// delete directory recursive
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken = default);
    }
}
