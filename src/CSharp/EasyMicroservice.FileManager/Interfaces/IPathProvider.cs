namespace EasyMicroservice.FileManager.Interfaces
{
    public interface IPathProvider
    {
        string Combine(params string[] paths);
        string GetObjectName(string path);
        string GetObjectParentPath(string path);
    }
}
