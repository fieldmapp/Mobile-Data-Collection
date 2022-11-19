using Android.App;
using Android.Widget;
using DlrDataApp.Modules.Base.Android;
using DlrDataApp.Modules.Base.Shared.DependencyServices;

// see https://stackoverflow.com/a/44126899

[assembly: Xamarin.Forms.Dependency(typeof(AndroidToast))]
namespace DlrDataApp.Modules.Base.Android
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