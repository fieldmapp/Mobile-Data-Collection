using DLR_Data_App.Localizations;
using DLR_Data_App.Models;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views.Settings
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SettingsPage : TabbedPage
  {
    public SettingsPage()
    {
      InitializeComponent();
    }
  }
}