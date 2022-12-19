using EasyMicroservice.FileManager.Models;
using System.IO;
using System.Threading.Tasks;

namespace EasyMicroservice.FileManager
{
    /// <summary>
    /// Extensions of file for easy of use
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// check if the files is exist
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Task<bool> IsExistAsync(this FileDetail file)
        {
            return file.Provider.IsExistFileAsync(file.FullPath);
        }

        /// <summary>
        /// create new directory
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Task<FileDetail> CreateDirectory(this FileDetail file)
        {
            return file.Provider.CreateFileAsync(file.FullPath);
        }

        /// <summary>
        /// open file stream
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Task<Stream> OpenFileAsync(this FileDetail file)
        {
            return file.Provider.OpenFileAsync(file.FullPath);
        }

        /// <summary>
        /// write stream to file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task WriteStreamToFileAsync(this FileDetail file, Stream stream)
        {
            await file.Provider.WriteStreamToFileAsync(file.FullPath, stream);
        }
    }
}
