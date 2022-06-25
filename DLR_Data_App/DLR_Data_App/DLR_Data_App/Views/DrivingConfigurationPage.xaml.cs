using DLR_Data_App.Controls;
using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrivingConfigurationPage : ContentPage
    {
        const string BoldMarker = "*";

        public class DrivingPageConfigurationDTO
        {
            public DrivingPageConfigurationDTO()
            {

            }
            public DrivingPageConfigurationDTO(DrivingPageConfiguration configuration)
            {
                Configuration = JsonTranslator.GetJson(configuration);
                Id = configuration.Id;
            }
            [SQLite.PrimaryKey, SQLite.AutoIncrement]
            public int? Id { get; set; }
            public string Configuration { get; set; }
            [SQLite.Ignore]
            public DrivingPageConfiguration DrivingPageConfiguration
            {
                get
                {
                    var conf = JsonTranslator.GetFromJson<DrivingPageConfiguration>(Configuration);
                    conf.Id = Id;
                    return conf;
                }
            }
        }
        public class DrivingPageConfiguration : BindableObject
        {
            public int? Id { get; set; }
            public static BindableProperty Cause1Property = BindableProperty.Create(nameof(Cause1), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause2Property = BindableProperty.Create(nameof(Cause2), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause3Property = BindableProperty.Create(nameof(Cause3), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause4Property = BindableProperty.Create(nameof(Cause4), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause5Property = BindableProperty.Create(nameof(Cause5), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause6Property = BindableProperty.Create(nameof(Cause6), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause7Property = BindableProperty.Create(nameof(Cause7), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause8Property = BindableProperty.Create(nameof(Cause8), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause9Property = BindableProperty.Create(nameof(Cause9), typeof(FormattedString), typeof(DrivingPageConfiguration), default(FormattedString));
            public static BindableProperty Cause1IdProperty = BindableProperty.Create(nameof(Cause1Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty Cause2IdProperty = BindableProperty.Create(nameof(Cause2Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty Cause3IdProperty = BindableProperty.Create(nameof(Cause3Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty Cause4IdProperty = BindableProperty.Create(nameof(Cause4Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty Cause5IdProperty = BindableProperty.Create(nameof(Cause5Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty Cause6IdProperty = BindableProperty.Create(nameof(Cause6Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty Cause7IdProperty = BindableProperty.Create(nameof(Cause7Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty Cause8IdProperty = BindableProperty.Create(nameof(Cause8Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty Cause9IdProperty = BindableProperty.Create(nameof(Cause9Id), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public static BindableProperty LaneCountProperty = BindableProperty.Create(nameof(LaneCount), typeof(int), typeof(DrivingPageConfiguration), default(int));
            public static BindableProperty LaneWidthProperty = BindableProperty.Create(nameof(LaneWidth), typeof(string), typeof(DrivingPageConfiguration), default(string));
            public string LaneWidth
            {
                get { return (string)GetValue(LaneWidthProperty); }
                set { SetValue(LaneWidthProperty, value); }
            }

            public FormattedString Cause1
            {
                get { return (FormattedString)GetValue(Cause1Property); }
                set { SetValue(Cause1Property, value); }
            }
            public FormattedString Cause2
            {
                get { return (FormattedString)GetValue(Cause2Property); }
                set { SetValue(Cause2Property, value); }
            }
            public FormattedString Cause3
            {
                get { return (FormattedString)GetValue(Cause3Property); }
                set { SetValue(Cause3Property, value); }
            }
            public FormattedString Cause4
            {
                get { return (FormattedString)GetValue(Cause4Property); }
                set { SetValue(Cause4Property, value); }
            }
            public FormattedString Cause5
            {
                get { return (FormattedString)GetValue(Cause5Property); }
                set { SetValue(Cause5Property, value); }
            }
            public FormattedString Cause6
            {
                get { return (FormattedString)GetValue(Cause6Property); }
                set { SetValue(Cause6Property, value); }
            }
            public FormattedString Cause7
            {
                get { return (FormattedString)GetValue(Cause7Property); }
                set { SetValue(Cause7Property, value); }
            }
            public FormattedString Cause8
            {
                get { return (FormattedString)GetValue(Cause8Property); }
                set { SetValue(Cause8Property, value); }
            }
            public FormattedString Cause9
            {
                get { return (FormattedString)GetValue(Cause9Property); }
                set { SetValue(Cause9Property, value); }
            }
            public string Cause1Id
            {
                get { return (string)GetValue(Cause1IdProperty); }
                set { SetValue(Cause1IdProperty, value); }
            }
            public string Cause2Id
            {
                get { return (string)GetValue(Cause2IdProperty); }
                set { SetValue(Cause2IdProperty, value); }
            }
            public string Cause3Id
            {
                get { return (string)GetValue(Cause3IdProperty); }
                set { SetValue(Cause3IdProperty, value); }
            }
            public string Cause4Id
            {
                get { return (string)GetValue(Cause4IdProperty); }
                set { SetValue(Cause4IdProperty, value); }
            }
            public string Cause5Id
            {
                get { return (string)GetValue(Cause5IdProperty); }
                set { SetValue(Cause5IdProperty, value); }
            }
            public string Cause6Id
            {
                get { return (string)GetValue(Cause6IdProperty); }
                set { SetValue(Cause6IdProperty, value); }
            }
            public string Cause7Id
            {
                get { return (string)GetValue(Cause7IdProperty); }
                set { SetValue(Cause7IdProperty, value); }
            }
            public string Cause8Id
            {
                get { return (string)GetValue(Cause8IdProperty); }
                set { SetValue(Cause8IdProperty, value); }
            }
            public string Cause9Id
            {
                get { return (string)GetValue(Cause9IdProperty); }
                set { SetValue(Cause9IdProperty, value); }
            }
            public int LaneCount
            {
                get { return (int)GetValue(LaneCountProperty); }
                set { SetValue(LaneCountProperty, value); }
            }
            public string Name
            {
                get { return (string)GetValue(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            public DrivingPageConfiguration Clone()
            {
                return (DrivingPageConfiguration)this.MemberwiseClone();
            }
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
            if (string.IsNullOrWhiteSpace(annotatedInput))
                return new FormattedString();

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

        public static string FormattedStringToAnnotatedString(FormattedString formattedString)
        {
            if (formattedString == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            foreach (var span in formattedString.Spans)
            {
                if (span.FontAttributes.HasFlag(FontAttributes.Bold))
                    builder.Append('*');
                builder.Append(span.Text);
                if (span.FontAttributes.HasFlag(FontAttributes.Bold))
                    builder.Append('*');
            }
            return builder.ToString().Replace(Environment.NewLine, "\\n");
        }

        public List<FormattedButton> KeywordButtons;
        public List<Label> CauseIdLabels;
        public Action Save { get; set; }
        public DrivingPageConfiguration Input { get; set; }
        public DrivingPageConfiguration Configuration { get; set; }
        public DrivingConfigurationPage(DrivingPageConfiguration input)
        {
            Input = input;
            Configuration = input.Clone();
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
            CauseIdLabels = new List<Label>
            {
                Cause1IdLabel, 
                Cause2IdLabel, 
                Cause3IdLabel, 
                Cause4IdLabel,
                Cause5IdLabel, 
                Cause6IdLabel, 
                Cause7IdLabel,
                Cause8IdLabel,
                Cause9IdLabel
            };
        }

        private async void CauseButton_Clicked(object sender, EventArgs e)
        {
            var button = (FormattedButton)sender;
            var causeIndex = KeywordButtons.IndexOf(button);

            string newCauseId = await DisplayPromptAsync("Ursachen-ID angeben", 
                "ID der neuen Minderertragsursache festlegen. Wird keine ID eingegeben, bleibt die Schaltfläche verborgen.", 
                accept:AppResources.ok, cancel:AppResources.cancel);
            if (newCauseId == null)
                return;
            if (string.IsNullOrWhiteSpace(newCauseId))
            {
                button.FormattedText = new FormattedString();
                CauseIdLabels[causeIndex].Text = string.Empty;
                OnPropertyChanged(nameof(Configuration));
                return;
            }

            if (CauseIdLabels.Where(l => l != CauseIdLabels[causeIndex]).Any(l => l.Text == newCauseId))
            {
                if (!await DisplayAlert("Warnung", "Es gibt bereits eine Minderertragsursache mit dieser ID. Trotzdem fortfahren?",
                    accept: AppResources.yes, cancel: AppResources.cancel))
                    return;
            }

            string newCause = null;
            while (string.IsNullOrWhiteSpace(newCause))
            {
                newCause = await DisplayPromptAsync("Minderertragsursache anpassen", "Neue Minderertragsursache angeben. Zwei oder mehr Worte können durch einfügen eines Zeilenumbruchs (\\n) auf mehrere Zeilen innerhalb des Buttons verteilt werden.");
                if (newCause == null)
                    return;
                if (newCause.Contains(BoldMarker))
                {
                    DependencyService.Get<IToast>().LongAlert($@"""{BoldMarker}"" kann nicht benutzt werden.");
                    newCause = null;
                }
            }

            string annotatedString = null;
            while (annotatedString == null)
            {
                string voiceRecogKeywordInput = null;
                while (string.IsNullOrWhiteSpace(voiceRecogKeywordInput))
                {
                    voiceRecogKeywordInput = await DisplayPromptAsync("Spracherkennung anpassen",
                        "Neue Schlüsselworte für die Spracherkennung angeben. " +
                        "Wenn mehrere Schlüsselworte gewünscht sind, diese bitte durch Komma (,) getrennt auflisten. " +
                        "Diese müssen als Teilwort in der angegebenen Ursache enthalten sein. " +
                        "Zudem darf ein Schlüsselwort nicht in anderen vorkommen. " +
                        "Durch das Auslassen von Angaben wird die Spracherkennung deaktiviert.",
                        initialValue: newCause,
                        accept: AppResources.ok, cancel: AppResources.cancel);
                    if (voiceRecogKeywordInput == null)
                        return;
                    if (!Regex.IsMatch(voiceRecogKeywordInput, @"^[a-zA-Z, ]*$"))
                    {
                        if (!await DisplayAlert("Warnung",
                            "Schlüsselwörter der Spracherkennung können nur aus lateinischen Buchstaben bestehen. " +
                            "Trotzdem fortfahren?",
                            accept: AppResources.yes, cancel: "Korrigieren"))
                            voiceRecogKeywordInput = null;
                    }
                }

                annotatedString = newCause;
                var voiceRecogKeywords = voiceRecogKeywordInput.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in voiceRecogKeywords)
                {
                    var substrPos = annotatedString.ToLower().IndexOf(keyword.ToLower());
                    if (substrPos == -1)
                    {
                        DependencyService.Get<IToast>().LongAlert("Die Schlüsselwörter müssen Teile der Minderertragsursache sein.");
                        annotatedString = null;
                        voiceRecogKeywordInput = null;
                        break;
                    }
                    annotatedString = annotatedString.Substring(0, substrPos) + BoldMarker + annotatedString.Substring(substrPos, keyword.Length) + BoldMarker + annotatedString.Substring(substrPos + keyword.Length);
                }
            }

            button.FormattedText = StringWithAnnotationsToFormattedString(annotatedString);
            CauseIdLabels[causeIndex].Text = newCauseId;
            OnPropertyChanged(nameof(Configuration));
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Configuration.CopyProperties(Input);
            var c = new DrivingPageConfigurationDTO(Input);
            Database.Update(ref c);
            Input.Id = c.Id;
            Save?.Invoke();
            Navigation.PopModalAsync();
        }
    }
}