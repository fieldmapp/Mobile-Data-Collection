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

using MobileDataCollection.Survey.Models;

namespace MobileDataCollection.Survey.Droid
{
    class AndroidQuestionProvider : IQuestionsProvider
    {
        public String LoadQuestionsForImageCheckerFromTXT()
        {
            AssetManager assetManager = Forms.Context.Assets;
            string content;
            using (StreamReader sr = new StreamReader(assetManager.Open("AboutAssets.txt")))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }
    }
}