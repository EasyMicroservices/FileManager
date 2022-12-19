using EasyMicroservice.FileManager.Interfaces;
using EasyMicroservice.FileManager.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager.Providers.FileProviders
{
    /// <summary>
    /// Base of File provider
    /// </summary>
    public abstract class BaseFileProvider : BasePathProvider, IFileManagerProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryManagerProvider"></param>
        public BaseFileProvider(IDirectoryManagerProvider directoryManagerProvider)
        {
            DirectoryManagerProvider = directoryManagerProvider;
            PathProvider = directoryManagerProvider.PathProvider;
        }

        /// <summary>
        /// Buffer size to read stream
        /// </summary>
        public int BufferSize { get; set; } = 1024 * 512;
        /// <summary>
        /// Directory provider to manage folders
        /// </summary>
        public IDirectoryManagerProvider DirectoryManagerProvider { get; set; }
        /// <summary>
        /// Path provider to manage paths
        /// </summary>
        public IPathProvider PathProvider { get; set; }

        /// <summary>
        /// copy a stream to another stream
        /// </summary>
        /// <param name="fromStream"></param>
        /// <param name="length"></param>
        /// <param name="toStream"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// copy byte array to a file
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="toFileName"></param>
        /// <returns></returns>
        public Task Copy(byte[] bytes, string toFileName)
        {
            using var stream = new MemoryStream(bytes);
            return Copy(stream, toFileName);
        }

        /// <summary>
        /// open or create filename
        /// it creates file when it is not exist
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Stream> OpenOrCreateFile(string fileName)
        {
            if (!await IsExistFileAsync(fileName))
                await CreateFileAsync(fileName);
            await TruncateFileAsync(fileName);
            return await OpenFileAsync(fileName);
        }

        /// <summary>
        /// copy a stream to a file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="toFileName"></param>
        /// <returns></returns>
        public async Task Copy(Stream stream, string toFileName)
        {
            var toFileStream = await OpenOrCreateFile(toFileName);
            await CopyToStreamAsync(stream, stream.Length, toFileStream);
        }

        /// <summary>
        /// normalize path to fix path problems
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string NormalizePath(string path)
        {
            return NormalizePath(path, DirectoryManagerProvider, PathProvider);
        }

        /// <summary>
        /// get file details
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual Task<FileDetail> GetFileAsync(string path)
        {
            path = NormalizePath(path);
            var details = new FileDetail(this)
            {
                DirectoryPath = PathProvider.GetObjectParentPath(path),
                Name = PathProvider.GetObjectName(path),
            };
            return Task.FromResult(details);
        }

        /// <summary>
        /// Create a directory if it's not exist
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<DirectoryDetail> CreateDirectoryIfNotExist(FileDetail file)
        {
            var directory = await DirectoryManagerProvider.GetDirectoryAsync(file.DirectoryPath);
            if (!await directory.IsExistAsync())
                return await directory.CreateDirectory();
            return directory;
        }

        /// <summary>
        /// write stream to a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual async Task WriteStreamToFileAsync(string path, Stream stream)
        {
            using var streamToWrite = await OpenFileAsync(path);
            await CopyToStreamAsync(stream, stream.Length, streamToWrite);
        }
        /// <summary>
        /// Create new file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract Task<FileDetail> CreateFileAsync(string path);
        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract Task<Stream> OpenFileAsync(string path);
        /// <summary>
        /// check if file is exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract Task<bool> IsExistFileAsync(string path);
        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract Task<bool> DeleteFileAsync(string path);
        /// <summary>
        /// set length of file as 0
        /// make a file data empty
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract Task TruncateFileAsync(string path);
    }
}
