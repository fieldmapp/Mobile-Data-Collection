using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.OS;
using Android.Views;
using DLR_Data_App;
using DLR_Data_App.Services;
using Java.Lang;
using Xamarin.Forms;
using AndroidX.AppCompat.Widget;
using static com.DLR.DLR_Data_App.Droid.ScreenListener;
using DlrDataApp.Modules.Base.Shared;

[assembly: UsesFeature("android.hardware.usb.host")]
namespace com.DLR.DLR_Data_App.Droid
{
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]
    [Activity(Label = "FieldMApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.FullUser)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IScreenStateListener
    {
        public static MainActivity Instance { get; private set; }

        ScreenListener ScreenListener;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Xamarin.Essentials.Platform.Init(Application);

            // TODO
            //JavaSystem.LoadLibrary("vosk_jni");

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            const string dbName = "DLRdata.sqlite";
            var folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var fullPath = System.IO.Path.Combine(folderPath, dbName);

            LoadApplication(new App(folderPath, fullPath, new List<ISharedModule> { 
                new DlrDataApp.Modules.Profiling.Shared.ProfilingModule()
            }));
            ReloadToolbar();
            EnsureAppPermission(Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage, Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation, Manifest.Permission.RecordAudio);

            ScreenListener = new ScreenListener(this);

            MessagingCenter.Subscribe<object, bool>(this, "ReloadToolbar", (sender, reload) =>
            {
                if (reload)
                {
                    ReloadToolbar();
                }
            });

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new System.Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new System.Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as System.Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(System.Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var libraryPath = @""; // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = System.IO.Path.Combine(libraryPath, errorFileName);
                var errorMessage = string.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
                DateTime.Now, exception.ToString());
                File.WriteAllText(errorFilePath, errorMessage);

                // Log to Android Device Logging.
                Android.Util.Log.Error("Crash Report", errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        /// <summary>
        // If there is an unhandled exception, the exception information is diplayed 
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        [Conditional("DEBUG")]
        private void DisplayCrashReport()
        {
            const string errorFilename = "Fatal.log";
            var libraryPath = @"";
            var errorFilePath = System.IO.Path.Combine(libraryPath, errorFilename);

            if (!File.Exists(errorFilePath))
            {
                return;
            }

            var errorText = File.ReadAllText(errorFilePath);
            new AlertDialog.Builder(this)
                .SetPositiveButton("Clear", (sender, args) =>
                {
                    File.Delete(errorFilePath);
                })
                .SetNegativeButton("Close", (sender, args) =>
                {
                    // User pressed Close.
                })
                .SetMessage(errorText)
                .SetTitle("Crash Report")
                .Show();
        }

        private async Task ResetToolbarItems(Page page, List<ToolbarItem> toolbarItems)
        {
            await Task.Delay(100);
            Device.BeginInvokeOnMainThread(() =>
            {
                page.ToolbarItems.Clear();
                foreach (var item in toolbarItems)
                {
                    page.ToolbarItems.Add(item);
                }
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

                _ = ResetToolbarItems(currentPage, items);
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
                if (base.PackageManager.CheckPermission(perm, base.PackageName) != Permission.Granted)
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

            ScreenListener.Begin(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ScreenListener.UnregisterListener();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            const int BackButtonId = 16908332;
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
            var navigation = AppShell.Current.Navigation;
            var currentPage = (Xamarin.Forms.Application.Current as App).CurrentPage;
            var mainPage = (Xamarin.Forms.Application.Current as App).MainPage;

            var currentPageType = currentPage.GetType();
            bool onBackButtonPressedOverwritten = currentPageType.Overrides(typeof(Page).GetMethod("OnBackButtonPressed", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.Instance));
            
            if (onBackButtonPressedOverwritten)
            {
                if (!currentPageType.IsSubclassOf(typeof(MultiPage<Page>))
                    || currentPageType.Overrides(typeof(MultiPage<Page>).GetMethod("OnBackButtonPressed", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.Instance)))
                return currentPage.SendBackButtonPressed();
            }
            if (navigation.ModalStack.Count > 0)
                navigation.PopModalAsync();
            else if (navigation.NavigationStack.Count > 1)
                navigation.PopAsync();
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
            // TODO ?
            if (requestCode == AndroidCameraProvider.REQUEST_CODE)
            {
                var cameraProvider = (AndroidCameraProvider)DependencyService.Get<ICameraProvider>();
                cameraProvider.CameraClosedHandle.Set();
            }
            else
                base.OnActivityResult(requestCode, resultCode, data);
        }

        public void OnScreenOn()
        {

        }

        public void OnScreenOff()
        {

        }

        public void OnUserPresent()
        {
            ReloadToolbar();
        }
    }
}