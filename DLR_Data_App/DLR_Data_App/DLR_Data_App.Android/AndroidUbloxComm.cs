﻿using System;
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
            const int READ_WAIT_MILLIS = 200;
            const int WRITE_WAIT_MILLIS = 200;

            AndroidUbloxComm AndroidUbloxComm;
            IUsbSerialPort SerialPort;
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
                SerialPort = AndroidUbloxComm.UbloxDriver.Ports.Single();
                SerialIOManager = new SerialInputOutputManager(SerialPort)
                {
                    BaudRate = 38400,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None
                };
               
                SerialIOManager.DataReceived += (a, b) =>
                {
                    var message = BitConverter.ToString(b.Data);
                    Log.Verbose("ublox", message);
                };
                SerialIOManager.Open(AndroidUbloxComm.UsbManager);
                WriteData(UbloxConfigurationMessageGenerator.StardardUbloxConfiguration());
            }

            void WriteData(byte[] data)
            {
                if (SerialIOManager?.IsOpen == true)
                {
                    SerialPort.Write(data, WRITE_WAIT_MILLIS);
                }
            }
        }

        static class UbloxConfigurationMessageGenerator
        {
            // see https://www.u-blox.com/en/docs/UBX-18010854 3 UBX protocol (uses little endian)

            static byte[] GenerateConfigurationItem(int key, byte[] value)
            {
                var result = new List<byte>(sizeof(int) + value.Length);
                result.AddRange(GetAsLittleEndian(BitConverter.GetBytes(key)));
                result.AddRange(GetAsLittleEndian(value));
                return result.ToArray();
            }

            public static byte[] StardardUbloxConfiguration()
            {
                // see https://www.u-blox.com/en/docs/UBX-18010854 5.9.11 CFG-MSGOUT for message ids
                // see https://www.use-snip.com/kb/knowledge-base/rtcm-3-message-list/ and https://www.use-snip.com/kb/knowledge-base/an-rtcm-message-cheat-sheet/ for message description
                const byte rate = 1;
                byte[] rateValue = new[] { rate };
                byte[] falseValue = new byte[] { 0 };
                byte[] trueValue = new byte[] { 0xFF };

                // Stationary RTK Reference Station ARP
                const int type1005UsbOutputRate = 0x209102c0;

                // GPS  (1077 is the best to use.  Setting up uBlox with RTKLIB? – use this)
                const int type1074UsbOutputRate = 0x20910361;
                const int type1077UsbOutputRate = 0x209102cf;

                // GLONASS  (and 1087 would be the one to use here)
                const int type1084UsbOutputRate = 0x20910366;
                const int type1087UsbOutputRate = 0x209102d4;

                // Galileo
                const int type1094UsbOutputRate = 0x2091036b;
                const int type1097UsbOutputRate = 0x2091031b;

                // BeiDou
                const int type1124UsbOutputRate = 0x20910370;
                const int type1127UsbOutputRate = 0x209102d9;

                // GLONASS L1 and L2 Code-Phase Biases
                const int type1230UsbOutputRate = 0x20910306;

                // ublox proprietary ??
                const int type4072_0UsbOutputRate = 0x20910301;
                const int type4072_1UsbOutputRate = 0x20910384;

                // See https://www.u-blox.com/en/docs/UBX-18010854 5.9.36 CFG - USBOUTPROT
                const int usbOutputProtocolNmea = 0x10780002;
                const int usbOutputProtocolUbx = 0x10780001;

                return SetConfigurationItems(
                    GenerateConfigurationItem(type1077UsbOutputRate, rateValue),
                    GenerateConfigurationItem(type1087UsbOutputRate, rateValue),
                    GenerateConfigurationItem(type1097UsbOutputRate, rateValue),
                    GenerateConfigurationItem(type1127UsbOutputRate, rateValue),
                    GenerateConfigurationItem(type1230UsbOutputRate, rateValue),
                    GenerateConfigurationItem(usbOutputProtocolNmea, falseValue)
                    );
            }

            static byte[] SetConfigurationItems(params byte[][] configurationItems)
            {
                // see https://www.u-blox.com/en/docs/UBX-18010854 3.10.26 UBX-CFG-VALSET

                const int maxConfigurationItemCount = 64;
                if (configurationItems.Length > maxConfigurationItemCount)
                    throw new ArgumentException($"No more than {maxConfigurationItemCount} can be set in one operation");

                const byte messageClass = 0x06;
                const byte messageId = 0x8a;

                const byte version = 0;
                const byte applyOnlyToRamLayer = 0b1;
                const byte reservedFiller0 = 0;
                const byte reservedFiller1 = 0;

                var payload = new List<byte> { version, applyOnlyToRamLayer, reservedFiller0, reservedFiller1 };

                foreach (var configurationItem in configurationItems)
                {
                    payload.AddRange(configurationItem);
                }

                return CreateUbloxWrapper(messageClass, messageId, payload);
            }

            /// <summary>
            /// Creates the wrapped message out of the supplied payload
            /// </summary>
            static byte[] CreateUbloxWrapper(byte messageClass, byte messageId, IEnumerable<byte> payload)
            {
                // steps:
                // 1. m = class + id + length of payload + payload

                var payloadSize = payload.Count();

                if (payloadSize > ushort.MaxValue)
                    throw new ArgumentException("Argument may not have more then ushort.max elements.");

                byte[] payloadLengthBytes = BitConverter.GetBytes((ushort)payloadSize);

                List<byte> message = new List<byte>(8 + payloadSize) { messageClass, messageId };
                message.AddRange(GetAsLittleEndian(payloadLengthBytes));

                message.AddRange(payload);

                // 2. m = m + ubx checksum of m (see https://www.u-blox.com/en/docs/UBX-18010854) 3.4 UBX checksum )

                byte checksum_a = 0;
                byte checksum_b = 0;

                for (int i = 0; i < message.Count; i++)
                {
                    // arithmetic overflow should be ignored
                    checksum_a += message[i];
                    checksum_b += checksum_a;
                }

                message.Add(checksum_a);
                message.Add(checksum_b);


                // 3. m = preamble + m

                const byte Preamble0 = 0xb5;
                const byte Preamble1 = 0x62;

                message.InsertRange(0, new[] { Preamble0, Preamble1 });

                return message.ToArray();
            }

            static IEnumerable<byte> GetAsLittleEndian(IEnumerable<byte> input) => BitConverter.IsLittleEndian ? input : input.Reverse();
        }
    }
}