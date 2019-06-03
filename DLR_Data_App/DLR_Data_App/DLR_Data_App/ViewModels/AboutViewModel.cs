using DLR_Data_App.Localizations;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace DLR_Data_App.ViewModels
{
  public class AboutViewModel : BaseViewModel
  {
    public AboutViewModel()
    {
      Title = AppResources.about;

      OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://www.dlr.de/dlr/desktopdefault.aspx/tabid-10002/")));
    }

    public ICommand OpenWebCommand { get; }
  }
}