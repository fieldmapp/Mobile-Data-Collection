using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using DLR_Data_App;
using DLR_Data_App.Services;
using Java.Lang;
using Xamarin.Forms;

namespace com.DLR.DLR_Data_App.Droid
{
    [Activity(Label = "FieldMApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.FullUser)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Xamarin.Essentials.Platform.Init(Application);

            JavaSystem.LoadLibrary("kaldi_jni");

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            const string dbName = "DLRdata.sqlite";
            var folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var fullPath = System.IO.Path.Combine(folderPath, dbName);

            var storageProvider = new JsonStorageProvider(new AndroidStorageAccessProvider(this));
            LoadApplication(new App(folderPath, fullPath, storageProvider));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            MessagingCenter.Subscribe<object, bool>(this, "ReloadToolbar", async (sender, reload) =>
            {
                await ToolbarLock.WaitAsync();
                {
                    if (reload)
                    {
                        ReloadToolbar();
                    }
                }
                ToolbarLock.Release();
            });

            EnsureAppPermission(Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage, Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation, Manifest.Permission.RecordAudio);
        }
        
        Page LastPage;
        Semaphore ToolbarItemsLock = new Semaphore(1, 1);
        SemaphoreSlim ToolbarLock = new SemaphoreSlim(1);

        private void ResetToolbarItems(Page page, List<ToolbarItem> toolbarItems)
        {
            //When you turn your smartphone on and off (just the screen), the ToolbarItems will disappear
            //Probably has something to do with TabbedPage calling OnAppearing multiple times
            //TODO: Fix issue with tabbed page:
            Device.BeginInvokeOnMainThread(() =>
            {
                ToolbarItemsLock.WaitOne();
                {
                    page.ToolbarItems.Clear();
                    foreach (var item in toolbarItems)
                    {
                        page.ToolbarItems.Add(item);
                    }
                }
                ToolbarItemsLock.Release();
            });
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            // Workaround for ToolbarItems not apearing on android (https://github.com/xamarin/Xamarin.Forms/issues/2118)
            Task.Run(() =>
            {
                var currentPage = (App.Current as App)?.CurrentPage;
                if (currentPage == null)
                    return;

                var items = currentPage.ToolbarItems.ToList();

                if (LastPage != null)
                    LastPage.Appearing -= currentPage_Appearing;

                LastPage = currentPage;
                currentPage.Appearing += currentPage_Appearing;

                ResetToolbarItems(currentPage, items);
                
                void currentPage_Appearing(object sender, EventArgs e)
                {
                    ResetToolbarItems((Page)sender, items);
                }
            });
        }

        private void ReloadToolbar()
        {
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
            }
        }

        public void EnsureAppPermission(params string[] perms)
        {
            List<string> neededPerms = new List<string>();
            foreach (var perm in perms)
            {
                if (PackageManager.CheckPermission(perm, PackageName) != Permission.Granted)
                {
                    neededPerms.Add(perm);
                }
            }
            if (neededPerms.Any())
                RequestPermissions(neededPerms.ToArray(), 1);
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

        const int BackButtonId = 16908332;

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == BackButtonId)
            {
                if (!BackButtonPress())
                    return base.OnOptionsItemSelected(item);
                return false;
            }
            else
            {
                //since its not the back button 
                //click, pass the event to the base
                return base.OnOptionsItemSelected(item);
            }
        }

        bool BackButtonPress()
        {
            var navigationPage = (Xamarin.Forms.Application.Current as App).Navigation;
            var currentPage = navigationPage.CurrentPage;
            var mainPage = (Xamarin.Forms.Application.Current as App).MainPage;

            bool onBackButtonPressedOverwritten = currentPage.GetType().Overrides(typeof(Page).GetMethod("OnBackButtonPressed", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.Instance));
            if (onBackButtonPressedOverwritten)
                return currentPage.SendBackButtonPressed();
            else if (navigationPage.Navigation.ModalStack.Count > 0)
                navigationPage.Navigation.PopModalAsync();
            else if (navigationPage.Navigation.NavigationStack.Count > 1)
                navigationPage.Navigation.PopAsync();
            else
                return mainPage.SendBackButtonPressed();
            return true;
        }

        public override void OnBackPressed()
        {
            if ((Xamarin.Forms.Application.Current as App).MainPage == null)
                base.OnBackPressed();
            else
                BackButtonPress();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == AndroidCameraProvider.REQUEST_CODE)
            {
                var cameraProvider = (AndroidCameraProvider)DependencyService.Get<ICameraProvider>();
                if (data != null)
                {
                    Bitmap bitmap = (Bitmap)data.Extras.Get("data");
                    cameraProvider.CapturedImage = bitmap;
                }
                else
                    cameraProvider.CapturedImage = null;

                cameraProvider.CameraClosedHandle.Set();
            }
            else
                base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}