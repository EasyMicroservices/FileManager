using EasyMicroservices.FileManager.Providers.PathProviders;
using System.IO;

namespace EasyMicroservices.FileManager.AmazonS3.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class AmazonS3PathProvider : BasePathProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public override string Combine(params string[] paths)
        {
            return Path.Combine(paths).Replace("\\", "/").Trim('/');
        }
    }
}
