//Main contributors: Maximilian Enderling
using DLR_Data_App.Services;
using DLR_Data_App.Views.Survey;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Controls
{
    class DetailImage : ImageButton
    {
        public event EventHandler ShortPress;

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
            if (Stopwatch.ElapsedMilliseconds > 500)
            {
                _ = this.PushPage(new ImageDetailPage(Source));
            }
            else
                ShortPress?.Invoke(this, null);
        }
    }
}
