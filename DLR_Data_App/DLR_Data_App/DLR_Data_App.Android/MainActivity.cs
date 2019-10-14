using System.IO;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using DLR_Data_App;

namespace com.DLR.DLR_Data_App.Droid
{
    [Activity(Label = "DLR Fieldmapp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Xamarin.Essentials.Platform.Init(Application);

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            const string dbName = "DLRdata.sqlite";
            var folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var fullPath = Path.Combine(folderPath, dbName);

            LoadApplication(new App(folderPath, fullPath, new FileManager()));

            if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
                && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted)
            {
              var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
              RequestPermissions(permissions, 1);
            }
        }

        public void CheckAppPermissions()
        {
          if ((int)Build.VERSION.SdkInt < 23)
          {
            return;
          }

          if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
              && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted)
          {
            var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
            RequestPermissions(permissions, 1);
          }
        }
  }
}