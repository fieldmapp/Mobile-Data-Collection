//Main contributors: Henning Woydt
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using DLR_Data_App.Services;
using com.DLR.DLR_Data_App.Droid;

[assembly: Dependency(typeof(AndroidStorageAccessProvider))]
namespace com.DLR.DLR_Data_App.Droid
{
    /// <summary>
    /// Implements the IStorageAccessProvider interface for android
    /// </summary>
    public class AndroidStorageAccessProvider : IStorageAccessProvider
    {
        Context Context;
        
        public AndroidStorageAccessProvider()
        {
            Context = MainActivity.Instance;
        }

        public Stream OpenAsset(string path) => Context.Assets.Open(path);

        public FileStream OpenFileRead(string path) => File.Exists(path) ? File.OpenRead(path) : null;

        public FileStream OpenFileWrite(string path) => File.OpenWrite(path);

        public FileStream OpenFileAppend(string path) => File.Exists(path) ? File.Open(path, FileMode.Append, FileAccess.Write) : File.Create(path);

        public FileStream OpenFileWriteExternal(string path)
        {
            // prevent compilation when targeting android 30 or above because android:requestLegacyExternalStorage would be ignored, breaking this code
#if !__ANDROID_30__
            path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, path);
            return OpenFileWrite(path);
#endif
        }

        public FileStream OpenFileAppendExternal(string path)
        {
            // prevent compilation when targeting android 30 or above because android:requestLegacyExternalStorage would be ignored, breaking this code
#if !__ANDROID_30__
            path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, path);
            return OpenFileAppend(path);
#endif
        }
    }
}