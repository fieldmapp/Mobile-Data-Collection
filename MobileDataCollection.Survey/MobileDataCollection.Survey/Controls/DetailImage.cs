using MobileDataCollection.Survey.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Controls
{
    class DetailImage : ImageButton
    {
        Stopwatch Stopwatch = new Stopwatch();

        public DetailImage()
        {
            Released += ReleasePicture;
            Pressed += PressPicture;
        }

        private void PressPicture(object sender, EventArgs e)
        {
            Stopwatch.Reset();
            Stopwatch.Start();
        }

        private void ReleasePicture(object sender, EventArgs e)
        {
            Stopwatch.Stop();
            ImageButton imageButton = (ImageButton)sender;
            if (Stopwatch.ElapsedMilliseconds > 1000)
            {
                (Application.Current as App).Navigation.PushAsync(new ImageDetailPage(Source));
            }
        }
    }
}
