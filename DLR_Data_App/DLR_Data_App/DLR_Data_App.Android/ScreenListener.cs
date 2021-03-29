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

namespace com.DLR.DLR_Data_App.Droid
{
    /// <summary>
    /// Altered from https://stackoverflow.com/a/55014763/8512719
    /// </summary>
    public class ScreenListener
    {
        private Context Context;
        private ScreenBroadcastReceiver mScreenReceiver;
        private static IScreenStateListener ScreenStateListener;

        public ScreenListener(Context context)
        {
            Context = context;
            mScreenReceiver = new ScreenBroadcastReceiver();
        }

        /**
         * screen BroadcastReceiver
         */
        private class ScreenBroadcastReceiver : BroadcastReceiver
        {
            private string action = null;

            public override void OnReceive(Context context, Intent intent)
            {
                action = intent.Action;
                if (Intent.ActionScreenOn == action)
                { // screen on
                    ScreenStateListener.OnScreenOn();
                }
                else if (Intent.ActionScreenOff == action)
                { // screen off
                    ScreenStateListener.OnScreenOff();
                }
                else if (Intent.ActionUserPresent == action)
                { // unlock
                    ScreenStateListener.OnUserPresent();
                }
            }
        }

        /**
         * begin to listen screen state
         *
         * @param listener
         */
        public void Begin(IScreenStateListener listener, bool callRelevantEnventsNow = false)
        {
            ScreenStateListener = listener;
            RegisterListener();
            if (callRelevantEnventsNow)
                GetScreenState();
        }

        /**
         * get screen state
         */
        private void GetScreenState()
        {
            PowerManager manager = (PowerManager)Context
                    .GetSystemService(Context.PowerService);
            if (manager.IsScreenOn)
            {
                if (ScreenStateListener != null)
                {
                    ScreenStateListener.OnScreenOn();
                }
            }
            else
            {
                if (ScreenStateListener != null)
                {
                    ScreenStateListener.OnScreenOff();
                }
            }
        }

        /**
         * stop listen screen state
         */
        public void UnregisterListener()
        {
            Context.UnregisterReceiver(mScreenReceiver);
        }

        /**
         * regist screen state broadcast
         */
        private void RegisterListener()
        {
            IntentFilter filter = new IntentFilter();
            filter.AddAction(Intent.ActionScreenOn);
            filter.AddAction(Intent.ActionScreenOff);
            filter.AddAction(Intent.ActionUserPresent);
            Context.RegisterReceiver(mScreenReceiver, filter);
        }

        /// <summary>
        /// Returns screen status information to the caller
        /// </summary>
        public interface IScreenStateListener
        {
            void OnScreenOn();

            void OnScreenOff();

            void OnUserPresent();
        }
    }
}