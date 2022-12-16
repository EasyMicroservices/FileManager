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

        public override Task<FileDetail> CreateFileAsync(string path)
        {
            string directory = Path.GetDirectoryName(NormalizePath(path));
            var file = new FileDetail(this)
            {
                DirectoryPath = directory,
                Name = Path.GetFileName(path),
            };
            File.Create(file.FullPath).Dispose();
            return Task.FromResult(file);
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
            var fileStream = await OpenFileAsync(path);
            fileStream.SetLength(0);
        }
    }
}