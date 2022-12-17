using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.FileProviders
{
    public class MemoryFileProvider : BaseFileProvider
    {
        public MemoryFileProvider(IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
        {
        }

        Dictionary<string, Stream> Files = new Dictionary<string, Stream>();

        public override async Task<FileDetail> CreateFileAsync(string path)
        {
            var file = await GetFileAsync(NormalizePath(path));
            await CreateDirectoryIfNotExist(file);
            if (!Files.ContainsKey(file.FullPath))
                Files.Add(file.FullPath, new MemoryStream());
            return file;
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

        public override async Task<FileDetail> GetFileAsync(string path)
        {
            var file = await base.GetFileAsync(path);
            if (await file.IsExistAsync())
            {
                if (Files.TryGetValue(file.FullPath, out Stream fileStream))
                {
                    file.Length = fileStream.Length;
                }
            }
            return file;
        }

        public override async Task WriteStreamToFileAsync(string path, Stream stream)
        {
            path = NormalizePath(path);
            if (Files.TryGetValue(path, out Stream fileStream))
            {
                await CopyToStreamAsync(stream, stream.Length, fileStream);
            }
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
