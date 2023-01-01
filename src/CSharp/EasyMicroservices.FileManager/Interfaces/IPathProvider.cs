namespace EasyMicroservices.FileManager.Interfaces
{
    /// <summary>
    /// Manage path of files and folders
    /// </summary>
    public interface IPathProvider
    {
        /// <summary>
        /// Combine some paths
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        string Combine(params string[] paths);
        /// <summary>
        /// Get name of object
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetObjectName(string path);
        /// <summary>
        /// Get object parent path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetObjectParentPath(string path);
    }
}
