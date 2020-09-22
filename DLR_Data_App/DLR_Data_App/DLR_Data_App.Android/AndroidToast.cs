using Android.App;
using Android.Widget;
using com.DLR.DLR_Data_App.Droid;
using DLR_Data_App.Services;

// see https://stackoverflow.com/a/44126899

[assembly: Xamarin.Forms.Dependency(typeof(AndroidToast))]
namespace com.DLR.DLR_Data_App.Droid
{
    class AndroidToast : IToast
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}