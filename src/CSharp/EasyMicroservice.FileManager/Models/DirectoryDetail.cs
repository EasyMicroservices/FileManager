using EasyMicroservice.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMicroservice.FileManager.Models
{
    public class DirectoryDetail
    {
        public DirectoryDetail(IDirectoryManagerProvider provider)
        {
            Provider = provider;
        }

        IDirectoryManagerProvider Provider { get; set; }

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