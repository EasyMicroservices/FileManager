using EasyMicroservices.FileManager.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasyMicroservices.FileManager.Providers.PathProviders
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BasePathProvider : IPathProvider
    {
        /// <summary>
        /// Combine multiple paths
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public virtual string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        /// Get object's name
        /// it's like File name or Directory name
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetObjectName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Get object parent's path
        /// it's file's directory path
        /// or directory's parent's directory path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetObjectParentPath(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}