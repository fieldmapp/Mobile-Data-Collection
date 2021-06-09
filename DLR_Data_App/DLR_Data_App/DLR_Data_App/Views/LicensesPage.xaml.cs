using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LicensesPage : ContentPage
	{
		public LicensesPage()
		{
			InitializeComponent();
			AddLicenesesToBody();
		}

        private void AddLicenesesToBody()
        {
			var licenses = UsedLicenesService.UsedLicenses;
			LicensesStackLayout.BatchBegin();
			foreach (var license in licenses)
            {
				LicensesStackLayout.Children.Add(new Label { Text = license.PackageName, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) });
				LicensesStackLayout.Children.Add(new Label { Text = license.LicenseText, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) });
            }
			LicensesStackLayout.BatchCommit();
        }
    }
}