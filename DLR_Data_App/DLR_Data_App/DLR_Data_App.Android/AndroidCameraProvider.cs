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
using DLR_Data_App.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(com.DLR.DLR_Data_App.Droid.AndroidCameraProvider))]
namespace com.DLR.DLR_Data_App.Droid
{
    class AndroidCameraProvider : ICameraProvider
    {
        public AutoResetEvent CameraClosedHandle = new AutoResetEvent(false);
        public Bitmap CapturedImage;
        public const int REQUEST_CODE = 5;

        public async Task<byte[]> OpenCameraApp()
        {
            Intent intent = new Intent("android.media.action.IMAGE_CAPTURE");
            MainActivity.Instance.StartActivityForResult(intent, REQUEST_CODE);
            await CameraClosedHandle.WaitOneAsync();
            if (CapturedImage == null)
                return null;

            using var stream = new MemoryStream();
            var byteArray = await CapturedImage.CompressAsync(Bitmap.CompressFormat.Png, 0, stream);
            return stream.ToArray();
        }

    }
}