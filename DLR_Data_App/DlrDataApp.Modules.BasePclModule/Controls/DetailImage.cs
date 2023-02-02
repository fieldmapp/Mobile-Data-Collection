﻿//Main contributors: Maximilian Enderling
using DlrDataApp.Modules.Base.Shared.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared.Controls
{
    /// <summary>
    /// An ImageButton which, when hold long enough, will open an <see cref="ImageDetailPage"/> .
    /// </summary>
    public class DetailImage : ImageButton
    {
        static readonly TimeSpan LongPressThreshold = TimeSpan.FromMilliseconds(500);
        public event EventHandler ShortPress;

        Stopwatch Stopwatch = new Stopwatch();
        Guid LastPressGuid = Guid.Empty;

        public DetailImage()
        {
            Released += ReleasePicture;
            Pressed += PressPicture;
        }

        private void PressPicture(object sender, EventArgs e)
        {
            var pressGuid = LastPressGuid = Guid.NewGuid();
            Stopwatch.Restart();
            _ = AutoOpenDetailPage(pressGuid);
        }

        private async Task AutoOpenDetailPage(Guid pressGuid)
        {
            await Task.Delay(LongPressThreshold);
            if (LastPressGuid == pressGuid)
                Device.BeginInvokeOnMainThread(() => _ = Shell.Current.Navigation.PushAsync(new ImageDetailPage(Source)));
        }

        private void ReleasePicture(object sender, EventArgs e)
        {
            LastPressGuid = Guid.Empty;
            Stopwatch.Stop();
            ImageButton imageButton = (ImageButton)sender;
            if (Stopwatch.Elapsed < LongPressThreshold)
                ShortPress?.Invoke(this, null);
        }
    }
}