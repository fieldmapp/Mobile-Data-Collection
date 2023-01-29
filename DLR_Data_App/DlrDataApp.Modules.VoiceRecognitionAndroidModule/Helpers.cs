using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DlrDataApp.Modules.Base.Android;
using Java.Util.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.SpeechRecognition.Android
{
    static class Helpers
    {
        public static void UnzipAssets(string assetName, string destPath)
        {
            // taken from https://stackoverflow.com/a/49884459
            if (Directory.Exists(destPath))
                Directory.Delete(destPath, true);

            byte[] buffer = new byte[1024];
            int byteCount;

            var destPathDir = new Java.IO.File(destPath);
            destPathDir.Mkdirs();

            using (var assetStream = DependencyService.Get<IMainActivityProvider>().MainActivity.Assets.Open(assetName, Access.Streaming))
            using (var zipStream = new ZipInputStream(assetStream))
            {
                ZipEntry zipEntry;
                while ((zipEntry = zipStream.NextEntry) != null)
                {
                    if (zipEntry.IsDirectory)
                    {
                        var zipDir = new Java.IO.File(Path.Combine(destPath, zipEntry.Name));
                        zipDir.Mkdirs();
                        continue;
                    }

                    using (var fileStream = new FileStream(Path.Combine(destPath, zipEntry.Name), FileMode.CreateNew))
                    {
                        while ((byteCount = zipStream.Read(buffer)) != -1)
                        {
                            fileStream.Write(buffer, 0, byteCount);
                        }
                    }
                    zipEntry.Dispose();
                }
            }
        }
    }
}