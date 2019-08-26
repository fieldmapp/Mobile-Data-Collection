using DLR_Data_App.Localizations;

namespace DLR_Data_App.ViewModels.CurrentProject
{
  /**
   * Class for presenting current project
   */
  public class ProjectViewModel : BaseViewModel
  {
    public ProjectViewModel()
    {
      Title = AppResources.currentproject;
    }
  }
}
