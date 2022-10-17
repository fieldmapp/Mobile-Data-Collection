using DlrDataApp.Modules.Base.Shared;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    public class FieldCartographerModule : ModuleBase<FieldCartographerModule>
    {
        public override Task OnInitialize()
        {
            ModuleHost.App.FlyoutItem.Items.Add(new ShellContent { Title = "Feldkartierer", Route = "fieldcartographer", ContentTemplate = new DataTemplate(typeof(DrivingPage)) });
            DependencyService.Get<IUbloxCommunicator>();
            return Task.CompletedTask;
        }
    }
}
