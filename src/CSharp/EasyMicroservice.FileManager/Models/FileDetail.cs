using EasyMicroservice.FileManager.Interfaces;

namespace EasyMicroservice.FileManager.Models
{
    /// <summary>
    /// Details of file
    /// </summary>
    public class FileDetail
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public FileDetail(IFileManagerProvider provider)
        {
            Provider = provider;
        }

        /// <summary>
        /// Provider of file
        /// </summary>
        internal IFileManagerProvider Provider { get; set; }
        /// <summary>
        /// Name of file
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Directory path of this file
        /// </summary>
        public string DirectoryPath { get; set; }
        /// <summary>
        /// Length of this file
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// Full directory's and File's name path
        /// </summary>
        public string FullPath
        {
            get
            {
                return Provider.PathProvider.Combine(DirectoryPath, Name);
            }
        }
    }
}
