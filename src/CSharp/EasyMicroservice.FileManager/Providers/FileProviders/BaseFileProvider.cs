using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.FileProviders
{
    public abstract class BaseFileProvider : BasePathProvider, IFileManagerProvider
    {
        public BaseFileProvider(IDirectoryManagerProvider directoryManagerProvider)
        {
            DirectoryManagerProvider = directoryManagerProvider;
            PathProvider = directoryManagerProvider.PathProvider;
        }

        public int BufferSize { get; set; } = 1024 * 512;
        public IDirectoryManagerProvider DirectoryManagerProvider { get; set; }
        public IPathProvider PathProvider { get; set; }

        public async Task CopyToStreamAsync(Stream fromStream, long length, Stream toStream)
        {
            if (fromStream.CanSeek)
                fromStream.Seek(0, SeekOrigin.Begin);
            var readBytes = new byte[BufferSize];
            long writed = 0;
            while (writed < length)
            {
                int readCount;
                if (readBytes.Length > length - writed)
                    readBytes = new byte[length - writed];
                readCount = await fromStream.ReadAsync(readBytes, 0, readBytes.Length);
                if (readCount <= 0)
                    throw new Exception("Client disconnected!");
                await toStream.WriteAsync(readBytes, 0, readCount);
                writed += readCount;
            }
        }

        public Task Copy(byte[] bytes, string toFileName)
        {
            using var stream = new MemoryStream(bytes);
            return Copy(stream, toFileName);
        }

        public async Task<Stream> OpenOrCreateFile(string fileName)
        {
            if (!await IsExistFileAsync(fileName))
                await CreateFileAsync(fileName);
            await TruncateFileAsync(fileName);
            return await OpenFileAsync(fileName);
        }

        public async Task Copy(Stream stream, string toFileName)
        {
            var toFileStream = await OpenOrCreateFile(toFileName);
            await CopyToStreamAsync(stream, stream.Length, toFileStream);
        }

        protected string NormalizePath(string path)
        {
            return NormalizePath(path, DirectoryManagerProvider, PathProvider);
        }

        public virtual Task<FileDetail> GetFileAsync(string path)
        {
            return Task.FromResult(new FileDetail(this)
            {
                DirectoryPath = PathProvider.GetObjectParentPath(path),
                Name = PathProvider.GetObjectName(path),
            });
        }

        public abstract Task<FileDetail> CreateFileAsync(string path);
        public abstract Task<Stream> OpenFileAsync(string path);
        public abstract Task<bool> IsExistFileAsync(string path);
        public abstract Task<bool> DeleteFileAsync(string path);
        public abstract Task TruncateFileAsync(string path);
    }
}
