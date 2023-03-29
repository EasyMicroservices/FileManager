using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Providers.FileProviders
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task CopyToStreamAsync(Stream fromStream, long length, Stream toStream, CancellationToken cancellationToken = default)
        {
            var stream = await StreamToBytesAsync(fromStream, length, cancellationToken);
            await stream.CopyToAsync(toStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromStream"></param>
        /// <param name="length"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<MemoryStream> StreamToBytesAsync(Stream fromStream, long length, CancellationToken cancellationToken = default)
        {
            MemoryStream toStream = new MemoryStream();
            if (length == 0)
                return toStream;
            if (fromStream.CanSeek)
                fromStream.Seek(0, SeekOrigin.Begin);
            var readBytes = new byte[BufferSize];
            long writed = 0;
            while (writed < length)
            {
                int readCount;
                if (readBytes.Length > length - writed)
                    readBytes = new byte[length - writed];
                readCount = await fromStream.ReadAsync(readBytes, 0, readBytes.Length, cancellationToken);
                if (readCount <= 0)
                    throw new Exception("Client disconnected!");
                await toStream.WriteAsync(readBytes, 0, readCount, cancellationToken);
                writed += readCount;
            }
            toStream.Seek(0, SeekOrigin.Begin);
            return toStream;
        }

        /// <summary>
        /// copy byte array to a file
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="toFileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Copy(byte[] bytes, string toFileName, CancellationToken cancellationToken = default)
        {
            using var stream = new MemoryStream(bytes);
            return Copy(stream, toFileName, cancellationToken);
        }

        /// <summary>
        /// open or create filename
        /// it creates file when it is not exist
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Stream> OpenOrCreateFile(string fileName, CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Copy(Stream stream, string toFileName, CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<FileDetail> GetFileAsync(string path, CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DirectoryDetail> CreateDirectoryIfNotExist(FileDetail file, CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task WriteStreamToFileAsync(string path, Stream stream, CancellationToken cancellationToken = default)
        {
            using var streamToWrite = await OpenOrCreateFile(path, cancellationToken);
            await CopyToStreamAsync(stream, stream.Length, streamToWrite);
        }

        /// <summary>
        /// Create new file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<FileDetail> CreateFileAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// open file to read or write stream
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<Stream> OpenFileAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// check if file is exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<bool> IsExistFileAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// set length of file as 0
        /// make a file data empty
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task TruncateFileAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream(bytes);
            memoryStream.Seek(0, SeekOrigin.Begin);
            await WriteStreamToFileAsync(path, memoryStream, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task WriteAllTextAsync(string path, string text, Encoding encoding, CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream();
            using var memoryWriterStream = new StreamWriter(memoryStream, encoding);
            await memoryWriterStream.WriteAsync(text);
            await memoryWriterStream.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            await WriteStreamToFileAsync(path, memoryStream, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task WriteAllTextAsync(string path, string text, CancellationToken cancellationToken = default)
        {
            return WriteAllTextAsync(path, text, Encoding.UTF8, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lines"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task WriteAllLinesAsync(string path, string[] lines, Encoding encoding, CancellationToken cancellationToken = default)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (i == lines.Length - 1)
                    builder.Append(line);
                else
                    builder.AppendLine(line);
            }
            return WriteAllTextAsync(path, builder.ToString(), encoding, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lines"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task WriteAllLinesAsync(string path, string[] lines, CancellationToken cancellationToken = default)
        {
            return WriteAllLinesAsync(path, lines, Encoding.UTF8, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            using var fileStream = await OpenFileAsync(path, cancellationToken);
            using var memoryStream = await StreamToBytesAsync(fileStream, fileStream.Length, cancellationToken);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
        {
            using var fileStream = await OpenFileAsync(path, cancellationToken);
            using var readerStream = new StreamReader(await StreamToBytesAsync(fileStream, fileStream.Length, cancellationToken), encoding);
            List<string> lines = new List<string>();
            string line;
            while ((line = await readerStream.ReadLineAsync()) != null)
            {
                lines.Add(line);
            }
            return lines.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default)
        {
            return ReadAllLinesAsync(path, Encoding.UTF8, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
        {
            using var fileStream = await OpenFileAsync(path, cancellationToken);
            using var readerStream = new StreamReader(await StreamToBytesAsync(fileStream, fileStream.Length, cancellationToken), encoding);
            return await readerStream.ReadToEndAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        {
            return ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);
        }
    }
}
