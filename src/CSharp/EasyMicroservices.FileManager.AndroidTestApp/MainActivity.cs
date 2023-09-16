using Android.App.Roles;
using Android.Content;
using Android.Runtime;
using EasyMicroservices.FileManager.Android.Providers;
using EasyMicroservices.FileManager.Interfaces;
namespace EasyMicroservices.FileManager.AndroidTestApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        IFileManagerProvider fileManager;
        IDirectoryManagerProvider directoryManager;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            directoryManager = new AndroidDiskDirectoryProvider(this, "/");
            fileManager = new AndroidDiskFileProvider(this, directoryManager);
            CheckCreateDirectory();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }

        public async Task CheckCreateDirectory()
        {
            var create = await directoryManager.CreateDirectoryAsync("AliTest");
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent? data)
        {
            if (requestCode == AndroidPermissionManager.RequestId)
            {
                fileManager.CompleteCheckPermission(resultCode == Result.Ok);
                directoryManager.CompleteCheckPermission(resultCode == Result.Ok);
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}