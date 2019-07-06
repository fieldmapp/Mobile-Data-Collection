using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Droid
{
    class AssetReader
    {
        public String readAssetTxt(string sourceTxt)
        {
            AssetManager assetManager = Forms.Context.Assets;
            String Text;
            using (StreamReader sr = new StreamReader(assetManager.Open("ImageCheckerQuestions.txt")))
            {
                Text = sr.ReadToEnd();
            }
            return Text;
        }
    }
}