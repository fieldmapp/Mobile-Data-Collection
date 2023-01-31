using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Localization;

namespace DLR_Data_App.ViewModels.Settings
{
    class SensorListViewModel : BaseViewModel
    {
        public SensorListViewModel()
        {
            Title = SharedResources.sensors;
        }
    }
}
