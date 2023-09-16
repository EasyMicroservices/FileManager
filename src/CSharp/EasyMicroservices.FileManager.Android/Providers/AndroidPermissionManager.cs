using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.FileManager.Android.Providers
{
    public class AndroidPermissionManager
    {
        /// <summary>
        /// request id of permission check
        /// </summary>
        public const int RequestId = 9669;
        readonly Activity _activity;
        public AndroidPermissionManager(Activity activity)
        {
            _activity = activity;
            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            ct.Token.Register(() => CompleteGetPermissionTask.TrySetCanceled(), useSynchronizationContext: false);
        }

        /// <summary>
        /// Wait for get permisssion
        /// </summary>
        TaskCompletionSource<bool> CompleteGetPermissionTask { get; set; } = new TaskCompletionSource<bool>();

        public async Task<bool> GetPermission(string path)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                if (_activity.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Denied)
#pragma warning restore CA1416 // Validate platform compatibility
                {
                    ActivityCompat.RequestPermissions(_activity, new string[] { Manifest.Permission.WriteExternalStorage }, RequestId);
                    return await CompleteGetPermissionTask.Task;
                }
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(_activity, Manifest.Permission.WriteExternalStorage) == Permission.Denied)
                {
                    ActivityCompat.RequestPermissions(_activity, new string[] { Manifest.Permission.WriteExternalStorage }, RequestId);
                    return await CompleteGetPermissionTask.Task;
                }
            }
            Complete(true);
            return true;
        }

        public void Complete(bool isComplete)
        {
            CompleteGetPermissionTask.TrySetResult(isComplete);
        }
    }
}
