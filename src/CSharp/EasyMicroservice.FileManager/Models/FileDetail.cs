using EasyMicroservice.FileManager.Interfaces;

namespace EasyMicroservice.FileManager.Models
{
    public class FileDetail
    {
        public FileDetail(IFileManagerProvider provider)
        {
            Provider = provider;
        }

        IFileManagerProvider Provider { get; set; }

        public string Name { get; set; }
        public string DirectoryPath { get; set; }
        public string FullPath
        {
            get
            {
                return Provider.PathProvider.Combine(DirectoryPath, Name);
            }
        }
    }
}
