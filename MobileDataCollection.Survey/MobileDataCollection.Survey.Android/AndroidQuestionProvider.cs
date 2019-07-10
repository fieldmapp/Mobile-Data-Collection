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
    /// <summary>
    /// Class that can use an AssetManager to read the txt files present in the folder "Assetes"
    /// </summary>
    class AndroidQuestionProvider : IQuestionsProvider
    {
        Context Context;

        /// <summary>
        /// Constructor needs a Context, so the AssetManager knows which Assets to use
        /// </summary>
        public AndroidQuestionProvider(Context context)
        {
            Context = context;
        }

        /// <summary>
        /// Reads a txt file with the given source. File should be in the "Assets" folder and sourceTxt must be <name>.txt
        /// </summary>
        /// <param name="sourceTxt"></param>
        /// <returns>returns all of the text from the txt file</returns>
        public String LoadTextFromTxt(String sourceTxt)
        {
            AssetManager assetManager = Context.Assets;
            string content;
            using (StreamReader sr = new StreamReader(assetManager.Open(sourceTxt)))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }
    }
}