using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.DLR.DLR_Data_App.Droid;
using DLR_Data_App.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidAppCloser))]
namespace com.DLR.DLR_Data_App.Droid
{
    class AndroidAppCloser : IAppCloser
    {
        public void CloseApp()
        {
            var activity = (Activity)MainActivity.Instance;
            activity.FinishAffinity();
        }
    }
}