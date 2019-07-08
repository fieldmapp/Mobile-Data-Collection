using DLR_Data_App.Localizations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.ViewModels.Settings
{
  class SettingsViewModel : BaseViewModel
  {
    public SettingsViewModel()
    {
      Title = AppResources.settings;
    }
  }
}
