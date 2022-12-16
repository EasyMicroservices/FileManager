namespace EasyMicroservice.FileManager.Providers.FileProviders
{
    using EasyMicroservice.FileManager.Interfaces;
    using EasyMicroservice.FileManager.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class MemoryFileProvider : BaseFileProvider
    {
        public MemoryFileProvider(IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
        {
        }

        Dictionary<string, Stream> Files = new Dictionary<string, Stream>();

        public override Task<FileDetail> CreateFileAsync(string path)
        {
            string directory = Path.GetDirectoryName(NormalizePath(path));
            var file = new FileDetail(this)
            {
                DirectoryPath = directory,
                Name = Path.GetFileName(path),
            };
            if (!Files.ContainsKey(file.FullPath))
                Files.Add(file.FullPath, new MemoryStream());
            return Task.FromResult(file);
        }

        public override Task<bool> DeleteFileAsync(string path)
        {
            path = NormalizePath(path);
            Files.Remove(path);
            return Task.FromResult(true);
        }

        public override Task<bool> IsExistFileAsync(string path)
        {
            path = NormalizePath(path);
            return Task.FromResult(Files.ContainsKey(path));
        }

        public override async Task<Stream> OpenFileAsync(string path)
        {
            path = NormalizePath(path);
            if (Files.TryGetValue(path, out Stream fileStream))
            {
                var memoryStream = new MemoryStream();
                await CopyToStreamAsync(fileStream, fileStream.Length, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            throw new Exception($"File {path} not found!");
        }

        public override Task TruncateFileAsync(string path)
        {
            path = NormalizePath(path);
            if (Files.TryGetValue(path, out Stream fileStream))
            {
                fileStream.SetLength(0);
#if(NET45)
                return Task.Delay(0);
#else
                return Task.CompletedTask;
#endif
            }
            throw new Exception($"File {path} not found!");
        }
    }
}
