using DLR_Data_App.Localizations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.ViewModels
{
  class ProjectViewModel : BaseViewModel
  {
    public ProjectViewModel()
    {
      Title = AppResources.currentproject;
    }
  }
}
