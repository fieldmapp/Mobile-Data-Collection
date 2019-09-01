﻿//Main contributors: Henning Woydt
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

using MobileDataCollection.Survey.Models;

namespace MobileDataCollection.Survey.Droid
{
    /// <summary>
    /// Implements the IStorageAccessProvider interface for android
    /// </summary>
    class AndroidStorageAccessProvider : IStorageAccessProvider
    {
        Context Context;

        public AndroidStorageAccessProvider(Context context)
        {
            Context = context;
        }

        public Stream OpenAsset(string path) => Context.Assets.Open(path);

        public Stream OpenFileRead(string path) => File.Exists(path) ? File.OpenRead(path) : Stream.Null;

        public Stream OpenFileWrite(string path) => File.OpenWrite(path);
    }
}