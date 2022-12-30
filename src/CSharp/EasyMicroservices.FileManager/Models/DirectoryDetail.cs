using EasyMicroservices.FileManager.Interfaces;

namespace EasyMicroservices.FileManager.Models
{
    /// <summary>
    /// Details of directory
    /// </summary>
    public class DirectoryDetail
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public DirectoryDetail(IDirectoryManagerProvider provider)
        {
            Provider = provider;
        }

        /// <summary>
        /// Provider of this directory
        /// </summary>
        internal IDirectoryManagerProvider Provider { get; set; }
        /// <summary>
        /// Name of directory
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Path of this directory
        /// </summary>
        public string DirectoryPath { get; set; }
        /// <summary>
        /// Full directory's path
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