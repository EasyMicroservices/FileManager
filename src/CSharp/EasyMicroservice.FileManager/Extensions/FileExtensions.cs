using EasyMicroservice.FileManager.Models;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager
{
    public static class FileExtensions
    {
        public static Task<bool> IsExistAsync(this FileDetail file)
        {
            return file.Provider.IsExistFileAsync(file.FullPath);
        }

        public static Task<FileDetail> CreateDirectory(this FileDetail file)
        {
            return file.Provider.CreateFileAsync(file.FullPath);
        }

        public static Task<Stream> OpenFileAsync(this FileDetail file)
        {
            return file.Provider.OpenFileAsync(file.FullPath);
        }

        public static async Task WriteStreamToFileAsync(this FileDetail file, Stream stream)
        {
            await file.Provider.WriteStreamToFileAsync(file.FullPath, stream);
        }
    }
}
