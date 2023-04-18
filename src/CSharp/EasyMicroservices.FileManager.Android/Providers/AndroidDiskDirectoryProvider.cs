using Android.App;
using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Providers.DirectoryProviders;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Android.Providers
{
    /// <summary>
    /// Disk directory provider for android
    /// </summary>
    public class AndroidDiskDirectoryProvider : DiskDirectoryProvider
    {
        AndroidPermissionManager androidPermissionManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pathProvider"></param>
        public AndroidDiskDirectoryProvider(Activity activity, string root, IPathProvider pathProvider) : base(root, pathProvider)
        {
            androidPermissionManager = new AndroidPermissionManager(activity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public AndroidDiskDirectoryProvider(Activity activity, string root) : base(root)
        {
            androidPermissionManager = new AndroidPermissionManager(activity);
        }

        /// <summary>
        /// check permission
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override async Task<bool> CheckPermissionAsync(string path)
        {
            return await androidPermissionManager.GetPermission(path);
        }

        /// <summary>
        /// complete check permission
        /// </summary>
        /// <param name="isComplete"></param>
        public void CompleteCheckPermission(bool isComplete)
        {
            androidPermissionManager.Complete(isComplete);
        }
    }
}
