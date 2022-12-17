using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.FileProviders
{
    public class DiskFileProvider : BaseFileProvider
    {
        public DiskFileProvider(IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
        {
        }

        public override async Task<FileDetail> CreateFileAsync(string path)
        {
            var file = await GetFileAsync(NormalizePath(path));
            await CreateDirectoryIfNotExist(file);
            File.Create(file.FullPath).Dispose();
            return file;
        }

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

        public override Task<bool> DeleteFileAsync(string path)
        {
            path = NormalizePath(path);
            File.Delete(path);
            return Task.FromResult(true);
        }

        public override Task<bool> IsExistFileAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult(File.Exists(path));
        }

        public override Task<Stream> OpenFileAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult((Stream)File.Open(path, FileMode.Open));
        }

        public override async Task TruncateFileAsync(string path)
        {
            path = NormalizePath(path);
            using var fileStream = await OpenFileAsync(path);
            fileStream.SetLength(0);
        }
    }
}