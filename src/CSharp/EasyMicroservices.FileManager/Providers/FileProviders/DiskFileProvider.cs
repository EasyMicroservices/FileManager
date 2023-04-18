using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Providers.FileProviders
{
    /// <summary>
    /// Manage files from disk
    /// </summary>
    public class DiskFileProvider : BaseFileProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryManagerProvider"></param>
        public DiskFileProvider(IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
        {
        }
        /// <summary>
        /// Create a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<FileDetail> CreateFileAsync(string path, CancellationToken cancellationToken = default)
        {
            var file = await GetFileAsync(NormalizePath(path));
            await CreateDirectoryIfNotExist(file);
            if (await CheckPermissionAsync(path))
                File.Create(file.FullPath).Dispose();
            return file;
        }
        /// <summary>
        /// get file's details
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<FileDetail> GetFileAsync(string path, CancellationToken cancellationToken = default)
        {
            var file = await base.GetFileAsync(path);
            if (await CheckPermissionAsync(path))
            {
                if (await file.IsExistAsync())
                {
                    var fileInfo = new FileInfo(file.FullPath);
                    file.Length = fileInfo.Length;
                }
            }
            return file;
        }
        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default)
        {
            path = NormalizePath(path);
            if (await CheckPermissionAsync(path))
                File.Delete(path);
            return true;
        }
        /// <summary>
        /// check if file is exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<bool> IsExistFileAsync(string path, CancellationToken cancellationToken = default)
        {
            path = NormalizePath(path);
            return Task.FromResult(File.Exists(path));
        }
        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<Stream> OpenFileAsync(string path, CancellationToken cancellationToken = default)
        {
            path = NormalizePath(path);
            await CheckPermissionAsync(path);
            return File.Open(path, FileMode.Open);
        }
        /// <summary>
        /// set length of file as 0
        /// make a file data empty
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task TruncateFileAsync(string path, CancellationToken cancellationToken = default)
        {
            path = NormalizePath(path);
            using var fileStream = await OpenFileAsync(path);
            fileStream.SetLength(0);
        }
    }
}