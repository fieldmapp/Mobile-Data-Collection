using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DlrDataApp.Modules.Base.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(com.DLR.DLR_Data_App.Droid.AndroidMainActivityProvider))]
namespace com.DLR.DLR_Data_App.Droid
{
    public class AndroidMainActivityProvider : IMainActivityProvider
    {
        public FormsAppCompatActivity MainActivity => Droid.MainActivity.Instance;
    }
}