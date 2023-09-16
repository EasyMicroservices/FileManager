using EasyMicroservices.FileManager.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EasyMicroservices.FileManager.Tests.Providers.FileProviders
{
    public abstract class BaseFileProviderTest
    {
        protected readonly IFileManagerProvider _fileManagerProvider;
        public BaseFileProviderTest(IFileManagerProvider fileManagerProvider)
        {
            _fileManagerProvider = fileManagerProvider;
        }


        public virtual Task OnInitialize(string methodName)
        {
            return TaskHelper.GetCompletedTask();
        }

        public virtual Task GetException(Exception ex)
        {
            return TaskHelper.GetCompletedTask();
        }

        [Theory]
        [InlineData("Ali.txt")]
        [InlineData("Mahdi.txt")]
        [InlineData("CreateFile\\Ali.txt")]
        [InlineData("CreateFile\\Mahdi.txt")]
        public virtual async Task CreateFile(string name)
        {
            try
            {
                await OnInitialize(nameof(CreateFile));
                if (await _fileManagerProvider.IsExistFileAsync(name))
                    Assert.True(await _fileManagerProvider.DeleteFileAsync(name));
                Assert.False(await _fileManagerProvider.IsExistFileAsync(name));
                await _fileManagerProvider.CreateFileAsync(name);
                Assert.True(await _fileManagerProvider.IsExistFileAsync(name));
                var file = await _fileManagerProvider.GetFileAsync(name);
                Assert.NotEmpty(file.Name);
                Assert.NotEmpty(file.DirectoryPath);
                Assert.True(await _fileManagerProvider.IsExistFileAsync(file.FullPath));
            }
            catch (Exception ex)
            {
                await GetException(ex);
            }
        }

        [Theory]
        [InlineData("Saeed.txt")]
        [InlineData("Reza.txt")]
        [InlineData("DeleteFile\\Saeed.txt")]
        [InlineData("DeleteFile\\Reza.txt")]
        public virtual async Task DeleteFile(string name)
        {
            try
            {
                await OnInitialize(nameof(DeleteFile));
                Assert.False(await _fileManagerProvider.IsExistFileAsync(name));
                await _fileManagerProvider.CreateFileAsync(name);
                Assert.True(await _fileManagerProvider.IsExistFileAsync(name));
                Assert.True(await _fileManagerProvider.DeleteFileAsync(name));
                Assert.False(await _fileManagerProvider.IsExistFileAsync(name));
            }
            catch (Exception ex)
            {
                await GetException(ex);
            }
        }

        [Theory]
        [InlineData("AliTruncate.txt")]
        [InlineData("MahdiTruncate.txt")]
        [InlineData("CreateFile\\AliTruncate.txt")]
        [InlineData("CreateFile\\MahdiTruncate.txt")]
        public virtual async Task TruncateFile(string name)
        {
            await OnInitialize(nameof(TruncateFile));
            if (await _fileManagerProvider.IsExistFileAsync(name))
                Assert.True(await _fileManagerProvider.DeleteFileAsync(name));
            Assert.False(await _fileManagerProvider.IsExistFileAsync(name));
            await _fileManagerProvider.CreateFileAsync(name);
            await _fileManagerProvider.TruncateFileAsync(name);
            var file = await _fileManagerProvider.GetFileAsync(name);
            Assert.True(file.Length == 0);
        }

        [Theory]
        [InlineData("AliStreamWrite.txt")]
        [InlineData("MahdiStreamWrite.txt")]
        [InlineData("CreateFile\\AliStreamWrite.txt")]
        [InlineData("CreateFile\\MahdiStreamWrite.txt")]
        public virtual async Task StreamWriteFile(string name)
        {
            await OnInitialize(nameof(StreamWriteFile));
            if (await _fileManagerProvider.IsExistFileAsync(name))
                Assert.True(await _fileManagerProvider.DeleteFileAsync(name));
            Assert.False(await _fileManagerProvider.IsExistFileAsync(name));
            var fileSetails = await _fileManagerProvider.CreateFileAsync(name);
            await _fileManagerProvider.TruncateFileAsync(name);
            var file = await _fileManagerProvider.GetFileAsync(name);
            Assert.True(file.Length == 0);

            long length = 1024;
            var streamToRead = await TaskRandomStream(length);
            await fileSetails.WriteStreamToFileAsync(streamToRead);
            file = await _fileManagerProvider.GetFileAsync(name);
            Assert.True(file.Length == length);
            var readStream = await fileSetails.OpenFileAsync();

            await CheckReadStream(readStream, length);
        }

        [Theory]
        [InlineData("AliReadWrite.txt")]
        [InlineData("MahdiReadWrite.txt")]
        [InlineData("ReadWriteFile\\Ali.txt")]
        [InlineData("ReadWriteFile\\Mahdi.txt")]
        public virtual async Task WriteAndReadFile(string name)
        {
            await OnInitialize(nameof(WriteAndReadFile));
            if (await _fileManagerProvider.IsExistFileAsync(name))
                await _fileManagerProvider.TruncateFileAsync(name);
            else
                await _fileManagerProvider.CreateFileAsync(name);
            Assert.True(await _fileManagerProvider.IsExistFileAsync(name));
            string[] lines = new string[]
            {
                "line1",
                "line2"
            };
            await _fileManagerProvider.WriteAllLinesAsync(name, lines);
            var readLines = await _fileManagerProvider.ReadAllLinesAsync(name);
            Assert.True(lines.SequenceEqual(readLines));

            await _fileManagerProvider.TruncateFileAsync(name);
            var bytes = new byte[]
            {
                1,
                2,
                5,
                7
            };
            await _fileManagerProvider.WriteAllBytesAsync(name, bytes);
            var allBytes = await _fileManagerProvider.ReadAllBytesAsync(name);
            Assert.True(bytes.SequenceEqual(allBytes));

            await _fileManagerProvider.TruncateFileAsync(name);
            var text = "My name is ali";
            await _fileManagerProvider.WriteAllTextAsync(name, text);
            var readText = await _fileManagerProvider.ReadAllTextAsync(name);
            Assert.Equal(text, readText);
        }

        async Task<Stream> TaskRandomStream(long length)
        {
            MemoryStream stream = new MemoryStream();
            byte[] bytes = new byte[length];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i]++;
            }
            await stream.WriteAsync(bytes, 0, bytes.Length);
            return stream;
        }

        async Task CheckReadStream(Stream stream, long length)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var toStream = new MemoryStream();
            var readBytes = new byte[length];
            long writed = 0;
            while (writed < length)
            {
                int readCount;
                if (readBytes.Length > length - writed)
                    readBytes = new byte[length - writed];
                readCount = await stream.ReadAsync(readBytes, 0, readBytes.Length);
                if (readCount <= 0)
                    throw new Exception("Client disconnected!");
                await toStream.WriteAsync(readBytes, 0, readCount);
                writed += readCount;
            }

            toStream.Seek(0, SeekOrigin.Begin);
            foreach (var item in toStream.ToArray())
            {
                Assert.True(item == 1);
            }
        }
    }
}
