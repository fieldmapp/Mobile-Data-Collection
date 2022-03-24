using DLR_Data_App.Controls;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrivingConfigurationPage : ContentPage
    {
        const string BoldMarker = "*";
        public class DrivingPageConfiguration
        {
            public FormattedString Cause1 { get; set; }
            public FormattedString Cause2 { get; set; }
            public FormattedString Cause3 { get; set; }
            public FormattedString Cause4 { get; set; }
            public FormattedString Cause5 { get; set; }
            public FormattedString Cause6 { get; set; }
            public FormattedString Cause7 { get; set; }
            public FormattedString Cause8 { get; set; }
            public FormattedString Cause9 { get; set; }
            public string Cause1Id { get; set; }
            public string Cause2Id { get; set; }
            public string Cause3Id { get; set; }
            public string Cause4Id { get; set; }
            public string Cause5Id { get; set; }
            public string Cause6Id { get; set; }
            public string Cause7Id { get; set; }
            public string Cause8Id { get; set; }
            public string Cause9Id { get; set; }
            public int LaneCount { get; set; }
            public string Name { get; set; }
        }

        public static readonly DrivingPageConfiguration DefaultConfiguration = new DrivingPageConfiguration
        {
            Cause1 = StringWithAnnotationsToFormattedString("*Sand*linse"),
            Cause1Id = "SandLens",
            Cause2 = StringWithAnnotationsToFormattedString("*Verdichtung*"),
            Cause2Id = "Compaction",
            Cause3 = StringWithAnnotationsToFormattedString("Vorge*wende*"),
            Cause3Id = "Headland",
            Cause4 = StringWithAnnotationsToFormattedString("*Kuppe*"),
            Cause4Id = "Dome",
            Cause5 = StringWithAnnotationsToFormattedString("*Hang*"),
            Cause5Id = "Slope",
            Cause6 = StringWithAnnotationsToFormattedString("*Wald*rand"),
            Cause6Id = "ForestEdge",
            Cause7 = StringWithAnnotationsToFormattedString("*Trocken*stress"),
            Cause7Id = "DryStress",
            Cause8 = StringWithAnnotationsToFormattedString("*Nass*stelle"),
            Cause8Id = "WaterLogging",
            Cause9 = StringWithAnnotationsToFormattedString("*Mäuse*fraß\\n*Wild*schaden"),
            Cause9Id = "GameMouseDamage",
            LaneCount = 3,
            Name = "Standard"
        };

        public static FormattedString StringWithAnnotationsToFormattedString(string annotatedInput)
        {
            annotatedInput = annotatedInput.Replace("\\n", Environment.NewLine);
            var markerIndices = annotatedInput.AllIndicesOf(BoldMarker);
            int prevPartEndIndex = 0;
            var result = new FormattedString();

            bool bold = false;
            foreach (var markerIndex in markerIndices.Concat(new int[] { annotatedInput.Length }))
            {
                var part = annotatedInput.Substring(prevPartEndIndex, markerIndex - prevPartEndIndex);
                if (!string.IsNullOrWhiteSpace(part))
                {
                    var span = new Span { Text = part };
                    if (bold)
                        span.FontAttributes = FontAttributes.Bold;

                    result.Spans.Add(span);
                }

                bold = !bold;
                prevPartEndIndex = markerIndex + 1;
            }

            return result;
        }

        public List<FormattedButton> KeywordButtons;
        public List<Action<string>> SetCauseIdActions;
        public Action Save { get; set; }
        public DrivingPageConfiguration Configuration { get; set; }
        public DrivingConfigurationPage(DrivingPageConfiguration input)
        {
            Configuration = input;
            InitializeComponent();
            KeywordButtons = new List<FormattedButton>
            {
                Button1,
                Button2,
                Button3,
                Button4,
                Button5,
                Button6,
                Button7,
                Button8,
                Button9
            };
            SetCauseIdActions = new List<Action<string>>
            {
                id => input.Cause1Id = id,
                id => input.Cause2Id = id,
                id => input.Cause3Id = id,
                id => input.Cause4Id = id,
                id => input.Cause5Id = id,
                id => input.Cause6Id = id,
                id => input.Cause7Id = id,
                id => input.Cause8Id = id,
                id => input.Cause9Id = id
            };
        }

        private async void CauseButton_Clicked(object sender, EventArgs e)
        {
            var button = (FormattedButton)sender;
            var causeIndex = KeywordButtons.IndexOf(button);

            string newCauseId = await DisplayPromptAsync("Ursachen-ID angeben", "Id der neuen Minderertragsursache angeben. Leer lassen um die Schaltfläche zu verstecken.");
            if (newCauseId == null)
                return;
            if (string.IsNullOrWhiteSpace(newCauseId))
            {
                button.FormattedText = new FormattedString();
                SetCauseIdActions[causeIndex](string.Empty);
                OnPropertyChanged(nameof(Configuration));
                return;
            }

            string newCause = null;
            while (string.IsNullOrWhiteSpace(newCause))
            {
                newCause = await DisplayPromptAsync("Ursache anpassen", "Neues Minderertragsursache angeben.");
                if (newCause == null)
                    return;
            }
            string voiceRecogKeywords = null;
            while (string.IsNullOrWhiteSpace(voiceRecogKeywords))
            {
                voiceRecogKeywords = await DisplayPromptAsync("Spracherkennung anpassen",
                    "Neues Spracherkennungs-Schlüsselwort angeben. Wenn mehrere Schlüsselwörter gewünscht sind, bitte mit Komma (,) trennen. Muss ein Teilwort der Ursache sein. Leer lassen um die Spracherkennung zu deaktivieren.",
                    initialValue: newCause);
                if (voiceRecogKeywords == null)
                    return;
            }

            var annotatedString = newCause;
            foreach (var keyword in voiceRecogKeywords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var substrPos = annotatedString.ToLower().IndexOf(keyword.ToLower());
                if (substrPos == -1)
                {
                    annotatedString = null;
                    break;
                }
                annotatedString = annotatedString.Substring(0, substrPos) + BoldMarker + annotatedString.Substring(substrPos, keyword.Length) + BoldMarker + annotatedString.Substring(substrPos + keyword.Length);
            }
            
            button.FormattedText = StringWithAnnotationsToFormattedString(annotatedString);
            SetCauseIdActions[causeIndex](newCauseId);
            OnPropertyChanged(nameof(Configuration));
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
            Save?.Invoke();
        }
    }
}