using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Android.Widget;
using DLR_Data_App.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(com.DLR.DLR_Data_App.Droid.AndroidCameraProvider))]
namespace com.DLR.DLR_Data_App.Droid
{
    class AndroidCameraProvider : ICameraProvider
    {
        public AutoResetEvent CameraClosedHandle = new AutoResetEvent(false);
        public const int REQUEST_CODE = 5;
        string PhotoPath;

        public async Task<byte[]> OpenCameraApp()
        {
            Intent intent = new Intent("android.media.action.IMAGE_CAPTURE");
            var dir = Android.App.Application.Context.GetExternalFilesDirs(Android.OS.Environment.DirectoryPictures)[0];
            var fileName = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileExtension = ".jpg";
            var file = Java.IO.File.CreateTempFile(fileName, fileExtension, dir);
            PhotoPath = file.AbsolutePath;

            var uri = Android.Support.V4.Content.FileProvider.GetUriForFile(MainActivity.Instance.ApplicationContext, MainActivity.Instance.PackageName + ".fileProvider", file);
            intent.PutExtra(MediaStore.ExtraOutput, uri);
            MainActivity.Instance.StartActivityForResult(intent, REQUEST_CODE);
            await CameraClosedHandle.WaitOneAsync();
            if (file.Length() == 0)
                return null;

            var bytes = await File.ReadAllBytesAsync(PhotoPath);
            return bytes;
        }

    }
}