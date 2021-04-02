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
        private readonly Context Context;
        private readonly ScreenBroadcastReceiver ScreenReceiver;
        private static IScreenStateListener ScreenStateListener;

        public ScreenListener(Context context)
        {
            Context = context;
            ScreenReceiver = new ScreenBroadcastReceiver();
        }

        /// <summary>
        /// Screen BroadcastReceiver
        /// </summary>
        private class ScreenBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                var action = intent.Action;
                if (action == Intent.ActionScreenOn)
                { // screen on
                    ScreenStateListener.OnScreenOn();
                }
                else if (action == Intent.ActionScreenOff)
                {
                    ScreenStateListener.OnScreenOff();
                }
                else if (action == Intent.ActionUserPresent)
                { // unlock
                    ScreenStateListener.OnUserPresent();
                }
            }
        }

        /// <summary>
        /// Begin to listen screen state
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="callRelevantEnventsNow"></param>
        public void Begin(IScreenStateListener listener, bool callRelevantEnventsNow = false)
        {
            ScreenStateListener = listener;
            RegisterListener();
            if (callRelevantEnventsNow)
                GetScreenState();
        }

        /// <summary>
        /// Get screen state
        /// </summary>
        private void GetScreenState()
        {
            PowerManager manager = (PowerManager)Context
                    .GetSystemService(Context.PowerService);

            if (manager.IsScreenOn)
            {
                ScreenStateListener?.OnScreenOn();
            }
            else
            {
                ScreenStateListener?.OnScreenOff();
            }
        }

        /// <summary>
        /// Stop listen screen state listening
        /// </summary>
        public void UnregisterListener()
        {
            Context.UnregisterReceiver(ScreenReceiver);
        }


        /// <summary>
        /// Register screen state broadcast
        /// </summary>
        private void RegisterListener()
        {
            IntentFilter filter = new IntentFilter();
            filter.AddAction(Intent.ActionScreenOn);
            filter.AddAction(Intent.ActionScreenOff);
            filter.AddAction(Intent.ActionUserPresent);
            Context.RegisterReceiver(ScreenReceiver, filter);
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