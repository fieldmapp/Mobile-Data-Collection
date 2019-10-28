using System;
using System.IO;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using DLR_Data_App;
using DLR_Data_App.Services;
using Xamarin.Forms;

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

            var storageProvider = new JsonStorageProvider(new AndroidStorageAccessProvider(this));
            LoadApplication(new App(folderPath, fullPath, storageProvider));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            // SetSupportActionBar(toolbar);
            // 
            // MessagingCenter.Subscribe<object, bool>(this, "ReloadToolbar", (sender, reload) => {
            //     if (reload)
            //     {
            //         ReloadToolbar();
            //     }
            // });

            CheckAppPermissions();
        }

        private void ReloadToolbar()
        {
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
            }
        }

        public void CheckAppPermissions()
        {
            if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
                && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted)
            {
                var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                RequestPermissions(permissions, 1);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // check if the current item id 
            // is equals to the back button id
            if (item.ItemId == 16908332)
            {
                // retrieve the current xamarin forms page instance
                var currentpage = (Xamarin.Forms.Application.Current as App).Navigation.CurrentPage;
                if (!currentpage.SendBackButtonPressed())
                {
                    if (base.OnOptionsItemSelected(item))
                        return true;
                    else
                        //Hack:
                        //Assume the NavigationStack of MainPage.Detail is of height 1 (only root in stack)
                        //Use MessagingCenter to open the Master menu
                        //We need to override OnBackButtonPressed in every non-root Page to return true though
                        MessagingCenter.Send(EventArgs.Empty, "OpenMasterMenu");
                }
                return false;
            }
            else
            {
                //since its not the back button 
                //click, pass the event to the base
                return base.OnOptionsItemSelected(item);
            }
        }
    }
}