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
using DlrDataApp.Modules.SharedModule;
using DlrDataApp.Modules.AndroidModule;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidStorageAccessProvider))]
namespace DlrDataApp.Modules.AndroidModule
{
    /// <summary>
    /// Implements the IStorageAccessProvider interface for android
    /// </summary>
    public class AndroidStorageAccessProvider : IStorageAccessProvider
    {
        Context Context;
        
        public AndroidStorageAccessProvider()
        {
            Context = Android.App.Application.Context;
        }

        public Stream OpenAsset(string path) => Context.Assets.Open(path);

        public FileStream OpenFileRead(string path) => File.Exists(path) ? File.OpenRead(path) : null;

        public FileStream OpenFileWrite(string path) => File.OpenWrite(path);

        public FileStream OpenFileAppend(string path) => File.Exists(path) ? File.Open(path, FileMode.Append, FileAccess.Write) : File.Create(path);

        public FileStream OpenFileWriteExternal(string path)
        {
            path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, path);
            return OpenFileWrite(path);
        }

        public FileStream OpenFileAppendExternal(string path)
        {
            path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, path);
            return OpenFileAppend(path);
        }
    }
}