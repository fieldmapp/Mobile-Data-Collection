using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.DLR.DLR_Data_App.Droid;
using DLR_Data_App.Services;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Util;
using Java.Util.Concurrent;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidUbloxComm))]
namespace com.DLR.DLR_Data_App.Droid
{
    [BroadcastReceiver]
    class AndroidUbloxComm : IUbloxCommunicator
    {
        public const int REQUEST_CODE = 6;
        private static readonly string ACTION_USB_PERMISSION = MainActivity.Instance.PackageName + ".USB_PERMISSION";
        public Task LoadTask { get; }

        UsbBroadcastReciever UsbReceiver;
        public IUsbSerialDriver UbloxDriver;
        public UsbManager UsbManager;

        public AndroidUbloxComm()
        {
            LoadTask = Initialize();
        }



        public async Task Initialize()
        {
            // see https://github.com/851265601/Xamarin.Android_ListviewSelect/blob/master/GetUSBPermission for tutorial on xamarin usb host connection
            // see https://github.com/anotherlab/UsbSerialForAndroid/blob/master/UsbSerialExampleApp/SerialConsoleActivity.cs for tutorial on used serial usb lib
            UsbManager = (UsbManager)MainActivity.Instance.ApplicationContext.GetSystemService(Context.UsbService);

            UsbReceiver = new UsbBroadcastReciever(this);
            var filter = new IntentFilter(ACTION_USB_PERMISSION);
            MainActivity.Instance.RegisterReceiver(UsbReceiver, filter);

            var table = UsbSerialProber.DefaultProbeTable;
            const int ubloxVendorId = 5446;
            const int ubloxSensorProductId = 425;
            table.AddProduct(ubloxVendorId, ubloxSensorProductId, Java.Lang.Class.FromType(typeof(CdcAcmSerialDriver)));
            var prober = new UsbSerialProber(table);
            var drivers = prober.FindAllDrivers(UsbManager);
            var driversList = drivers.ToList();
            UbloxDriver = driversList.FirstOrDefault();

            if (UsbManager.HasPermission(UbloxDriver.Device))
            {
                UsbReceiver.OnHasUsbPermission();
            }
            else
            {
                var connection = UsbManager.OpenDevice(UbloxDriver.Device);
                var intent = PendingIntent.GetBroadcast(MainActivity.Instance.ApplicationContext, REQUEST_CODE, new Intent(ACTION_USB_PERMISSION), PendingIntentFlags.UpdateCurrent);
                //todo: popup will be overwritten by other permissions popup (MainActivity.EnsureAppPermission)
                UsbManager.RequestPermission(UbloxDriver.Device, intent);
            }

        }

        class UsbBroadcastReciever : BroadcastReceiver
        {
            AndroidUbloxComm AndroidUbloxComm;
            SerialInputOutputManager SerialIOManager;

            public UsbBroadcastReciever(AndroidUbloxComm androidUbloxComm)
            {
                AndroidUbloxComm = androidUbloxComm;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action == ACTION_USB_PERMISSION)
                {
                    var userAllowedAccess = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
                    if (userAllowedAccess)
                    {
                        OnHasUsbPermission();
                    }
                    else
                    {
                        DependencyService.Get<IAppCloser>().CloseApp();
                    }
                }
            }

            public void OnHasUsbPermission()
            {
                var port = AndroidUbloxComm.UbloxDriver.Ports.Single();
                SerialIOManager = new SerialInputOutputManager(port)
                {
                    BaudRate = 38400,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None
                };
               
                SerialIOManager.DataReceived += (a, b) =>
                {
                    var message = UTF8Encoding.UTF8.GetString(b.Data);
                    Log.Verbose("ublox", message);
                };
                SerialIOManager.Open(AndroidUbloxComm.UsbManager);
            }
        }
    }
}