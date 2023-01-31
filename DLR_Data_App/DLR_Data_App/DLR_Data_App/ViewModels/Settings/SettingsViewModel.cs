using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;

namespace DLR_Data_App.ViewModels.Settings
{
    class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            Title = SharedResources.settings;
        }
    }
}
