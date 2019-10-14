using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AboutPage
  {
    public AboutPage()
    {
      InitializeComponent();
    }

    /**
     * Override hardware back button on Android devices to return to project list
     */
    protected override bool OnBackButtonPressed()
    {
      Device.BeginInvokeOnMainThread(async () => {
        base.OnBackButtonPressed();
        if (Application.Current.MainPage is MainPage mainPage)
          await mainPage.NavigateFromMenu(1);
      });

      return true;
    }

    /**
     * License Button clicked
     */
    private async void LicenseButton_OnClicked(object sender, EventArgs e)
    {
      await Navigation.PushAsync(new LicensesPage());
    }
  }
}