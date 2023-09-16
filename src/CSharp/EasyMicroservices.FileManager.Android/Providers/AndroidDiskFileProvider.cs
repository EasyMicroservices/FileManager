using Android.App;
using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Providers.FileProviders;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Android.Providers
{
    public class AndroidDiskFileProvider : DiskFileProvider
    {
        AndroidPermissionManager androidPermissionManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryManagerProvider"></param>
        public AndroidDiskFileProvider(Activity activity, IDirectoryManagerProvider directoryManagerProvider) : base(directoryManagerProvider)
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
        public override void CompleteCheckPermission(bool isComplete)
        {
            androidPermissionManager.Complete(isComplete);
        }
    }
}
