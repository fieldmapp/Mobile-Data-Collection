using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.SpeechRecognition.Definition;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    public class FieldCartographerModule : ModuleBase<FieldCartographerModule>
    {
        public FieldCartographerModule() : base("FieldCartographer", new List<string> { "SpeechRecognition" }) { }


        public ISpeechRecognizer SpeechRecognizer;
        public override Task OnInitialize()
        {
            ModuleHost.App.FlyoutItem.Items.Add(new ShellContent { Title = "Fahrtansicht", Route = "fieldcartographer", ContentTemplate = new DataTemplate(typeof(DrivingConfigurationSelectionPage)) });
            DependencyService.Get<IUbloxCommunicator>();
            return Task.CompletedTask;
        }

        public override Task OnPostInitialize()
        {
            SpeechRecognizer = DependencyService.Get<ISpeechRecognizerProvider>().Initialize(VoiceCommandCompiler.KeywordStringToSymbol.Keys.ToList());
            return Task.CompletedTask;
        }
    }
}
