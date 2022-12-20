using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.FileProviders
{
    /// <summary>
    /// Manage files in memory
    /// </summary>
    public class MemoryFileProvider : BaseFileProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryManagerProvider"></param>
        public MemoryFileProvider(IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
        {
        }

        Dictionary<string, Stream> Files = new Dictionary<string, Stream>();
        /// <summary>
        /// Create new file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override async Task<FileDetail> CreateFileAsync(string path)
        {
            var file = await GetFileAsync(NormalizePath(path));
            await CreateDirectoryIfNotExist(file);
            if (!Files.ContainsKey(file.FullPath))
                Files.Add(file.FullPath, new MemoryStream());
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
            Files.Remove(path);
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
            return Task.FromResult(Files.ContainsKey(path));
        }
        /// <summary>
        /// Get file's details
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
        /// <summary>
        /// write stream to file path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override async Task WriteStreamToFileAsync(string path, Stream stream)
        {
            path = NormalizePath(path);
            if (Files.TryGetValue(path, out Stream fileStream))
            {
                await CopyToStreamAsync(stream, stream.Length, fileStream);
            }
        }
        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
        /// <summary>
        /// set length of file as 0
        /// make a file data empty
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
