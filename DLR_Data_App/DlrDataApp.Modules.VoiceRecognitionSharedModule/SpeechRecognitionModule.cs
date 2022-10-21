using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.SpeechRecognition.Definition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.SpeechRecognition.Shared
{
    public class SpeechRecognitionModule : ModuleBase<SpeechRecognitionModule>
    {
        public SpeechRecognitionModule() : base("SpeechRecognition", new List<string>()) { }

        public override Task OnInitialize()
        {
            var speechRecognizer = DependencyService.Get<ISpeechRecognizerProvider>();
            ModuleHost.App.FlyoutItem.Items.Add(new ShellContent
            {
                Title = "Spracherkennung-Demo",
                Route = "profilingcurrent",
                ContentTemplate = new DataTemplate(typeof(SpeechRecognitionDemoPage))
            });

            return speechRecognizer.LoadTask;
        }
    }
}
