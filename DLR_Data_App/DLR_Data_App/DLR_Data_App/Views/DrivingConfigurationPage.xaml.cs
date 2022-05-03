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

            [Newtonsoft.Json.JsonProperty()]
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
        public List<Action<string>> SetCauseIdActions;
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
            Configuration.CopyProperties(Input);
            var c = new DrivingPageConfigurationDTO(Input);
            Database.Update(ref c);
            Input.Id = c.Id;
            Save?.Invoke();
            Navigation.PopModalAsync();
        }
    }
}