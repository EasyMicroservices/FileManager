using EasyMicroservice.FileManager.Interfaces;

namespace EasyMicroservice.FileManager.Models
{
    public class FileDetail
    {
        public FileDetail(IFileManagerProvider provider)
        {
            Provider = provider;
        }

        internal IFileManagerProvider Provider { get; set; }

        public string Name { get; set; }
        public string DirectoryPath { get; set; }
        public long Length { get; set; }
        public string FullPath
        {
            get
            {
                return Provider.PathProvider.Combine(DirectoryPath, Name);
            }
        }
    }
}
