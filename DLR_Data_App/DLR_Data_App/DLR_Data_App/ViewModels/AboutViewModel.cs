using DLR_Data_App.Localizations;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace DLR_Data_App.ViewModels
{
  /**
   * ViewModel for the about page
   */
  public class AboutViewModel : BaseViewModel
  {
    /**
     * Constructor
     */
    public AboutViewModel()
    {
      Title = AppResources.about;

      OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://www.dlr.de/dlr/desktopdefault.aspx/tabid-10002/")));
    }

    /**
     * Interface for opening a webpage in the local browser
     */
    public ICommand OpenWebCommand { get; }
  }
}