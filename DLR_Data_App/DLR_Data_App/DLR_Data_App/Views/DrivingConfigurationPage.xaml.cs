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
            public int LaneCount { get; set; }
        }

        public static readonly DrivingPageConfiguration DefaultConfiguration = new DrivingPageConfiguration
        {
            Cause1 = StringWithAnnotationsToFormattedString("*Sand*linse"),
            Cause2 = StringWithAnnotationsToFormattedString("*Verdichtung*"),
            Cause3 = StringWithAnnotationsToFormattedString("Vorge*wende*"),
            Cause4 = StringWithAnnotationsToFormattedString("*Kuppe*"),
            Cause5 = StringWithAnnotationsToFormattedString("*Hang*"),
            Cause6 = StringWithAnnotationsToFormattedString("*Wald*rand"),
            Cause7 = StringWithAnnotationsToFormattedString("*Trocken*stress"),
            Cause8 = StringWithAnnotationsToFormattedString("*Nass*stelle"),
            Cause9 = StringWithAnnotationsToFormattedString("*Mäuse*fraß" + Environment.NewLine + "*Wild*schaden"),
            LaneCount = 3
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

        public DrivingPageConfiguration Configuration { get; set; }
        public DrivingConfigurationPage(DrivingPageConfiguration input)
        {
            Configuration = input;
            InitializeComponent();
        }

        private async void CauseButton_Clicked(object sender, EventArgs e)
        {
            var button = (FormattedButton)sender;
            string newCause = await DisplayPromptAsync("Ursache anpassen", "Neues Minderertragsursache angeben. Leer lassen um die Schaltfläche zu verstecken.");
            if (newCause == null)
                return;
            if (string.IsNullOrWhiteSpace(newCause))
            {
                button.FormattedText = new FormattedString();
                return;
            }
            while (true)
            {
                string voiceRecogKeywords = await DisplayPromptAsync("Spracherkennung anpassen",
                    "Neues Spracherkennungs-Schlüsselwort angeben. Wenn mehrere Schlüsselwörter gewünscht sind, bitte mit Komma (,) trennen. Muss ein Teilwort der Ursache sein. Leer lassen um die Spracherkennung zu deaktivieren.",
                    initialValue: newCause);
                if (voiceRecogKeywords == null)
                    return;

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
                if (annotatedString != null)
                {
                    button.FormattedText = StringWithAnnotationsToFormattedString(annotatedString);
                    return;
                }
            }
        }
    }
}