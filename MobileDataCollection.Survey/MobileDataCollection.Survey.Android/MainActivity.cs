using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MobileDataCollection.Survey.Models;
using System.Linq;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Droid
{
    [Activity(Label = "MobileDataCollection.Survey", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidQuestionProvider androidQuestionProvider = new AndroidQuestionProvider(this);
            DatabankCommunication databankCommunication = new DatabankCommunication(androidQuestionProvider);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(databankCommunication));
            Android.Support.V7.Widget.Toolbar toolbar = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // check if the current item id 
            // is equals to the back button id
            if (item.ItemId == 16908332)
            {
                // retrieve the current xamarin forms page instance
                var currentpage = (ContentPage)Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
                currentpage.SendBackButtonPressed();
                return false;
            }
            else
            {
                // since its not the back button 
                //click, pass the event to the base
                return base.OnOptionsItemSelected(item);
            }
        }
    }
}