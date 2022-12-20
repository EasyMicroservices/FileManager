using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.FileProviders
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
        /// <returns></returns>
        public override async Task<FileDetail> CreateFileAsync(string path)
        {
            var file = await GetFileAsync(NormalizePath(path));
            await CreateDirectoryIfNotExist(file);
            File.Create(file.FullPath).Dispose();
            return file;
        }
        /// <summary>
        /// get file's details
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override async Task<FileDetail> GetFileAsync(string path)
        {
            var file = await base.GetFileAsync(path);
            if (await file.IsExistAsync())
            {
                var fileInfo = new FileInfo(file.FullPath);
                file.Length = fileInfo.Length;
            }
            return file;
        }
        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override Task<bool> DeleteFileAsync(string path)
        {
            path = NormalizePath(path);
            File.Delete(path);
            return Task.FromResult(true);
        }
        /// <summary>
        /// check if file is exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override Task<bool> IsExistFileAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult(File.Exists(path));
        }
        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override Task<Stream> OpenFileAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult((Stream)File.Open(path, FileMode.Open));
        }
        /// <summary>
        /// set length of file as 0
        /// make a file data empty
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override async Task TruncateFileAsync(string path)
        {
            path = NormalizePath(path);
            using var fileStream = await OpenFileAsync(path);
            fileStream.SetLength(0);
        }
    }
}