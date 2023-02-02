﻿using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Controls;
using DlrDataApp.Modules.Base.Shared.DependencyServices;
using DlrDataApp.Modules.Base.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static DlrDataApp.Modules.Base.Shared.Services.FormattedStringSerializerHelper;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrivingConfigurationPage : ContentPage
    {
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


        int CauseBeingChangedIndex;
        List<(string Id, FormattedString cause)> KnownCauses;
        private void CauseButton_Clicked(object sender, EventArgs e)
        {
            var button = (FormattedButton)sender;
            CauseBeingChangedIndex = KeywordButtons.IndexOf(button);

            KnownCauses = FieldCartographerModule.Instance.Database.Read<DrivingPageConfigurationDTO>()
                .Select(dto => dto.DrivingPageConfiguration)
                .SelectMany(c => c.GetCauses())
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .Where(x => !string.IsNullOrWhiteSpace(x.Id))
                .ToList();
            HelperPicker.Items.Clear();
            HelperPicker.Items.Add("Schaltfläche verstecken");
            foreach (var item in KnownCauses.Select(c => c.cause.ToString()))
            {
                HelperPicker.Items.Add(item);
            }
            HelperPicker.Items.Add("Neu erstellen");
            HelperPicker.IsVisible = true;
            HelperPicker.Focus();
        }

        private async void HelperPicker_Unfocused(object sender, FocusEventArgs e)
        {
            HelperPicker.IsVisible = false;
            void setCause(FormattedString formattedString)
            {
                switch (CauseBeingChangedIndex)
                {
                    case 0: Configuration.Cause1 = formattedString; break;
                    case 1: Configuration.Cause2 = formattedString; break;
                    case 2: Configuration.Cause3 = formattedString; break;
                    case 3: Configuration.Cause4 = formattedString; break;
                    case 4: Configuration.Cause5 = formattedString; break;
                    case 5: Configuration.Cause6 = formattedString; break;
                    case 6: Configuration.Cause7 = formattedString; break;
                    case 7: Configuration.Cause8 = formattedString; break;
                    case 8: Configuration.Cause9 = formattedString; break;
                }
            }
            void setCauseId(string causeId)
            {
                switch (CauseBeingChangedIndex)
                {
                    case 0: Configuration.Cause1Id = causeId; break;
                    case 1: Configuration.Cause2Id = causeId; break;
                    case 2: Configuration.Cause3Id = causeId; break;
                    case 3: Configuration.Cause4Id = causeId; break;
                    case 4: Configuration.Cause5Id = causeId; break;
                    case 5: Configuration.Cause6Id = causeId; break;
                    case 6: Configuration.Cause7Id = causeId; break;
                    case 7: Configuration.Cause8Id = causeId; break;
                    case 8: Configuration.Cause9Id = causeId; break;
                }
            }

            var selectedIndex = HelperPicker.SelectedIndex;
            if (selectedIndex == -1)
                return;

            if (selectedIndex == 0)
            {
                setCause(new FormattedString());
                setCauseId("");
                OnPropertyChanged(nameof(Configuration));
                return;
            }
            selectedIndex--;
            if (selectedIndex < KnownCauses.Count)
            {
                setCause(KnownCauses[selectedIndex].cause);
                setCauseId(KnownCauses[selectedIndex].Id);
                OnPropertyChanged(nameof(Configuration));
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
                        accept: SharedResources.ok, cancel: SharedResources.cancel);
                    if (voiceRecogKeywordInput == null)
                        return;
                    if (!Regex.IsMatch(voiceRecogKeywordInput, @"^[a-zA-Z, ]*$"))
                    {
                        if (!await DisplayAlert("Warnung",
                            "Schlüsselwörter der Spracherkennung können nur aus lateinischen Buchstaben bestehen. " +
                            "Trotzdem fortfahren?",
                            accept: SharedResources.yes, cancel: "Korrigieren"))
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

            setCause(StringWithAnnotationsToFormattedString(annotatedString));
            setCauseId(Guid.NewGuid().ToString());
            OnPropertyChanged(nameof(Configuration));
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Configuration.CopyProperties(Input);
            var c = new DrivingPageConfigurationDTO(Input);
            FieldCartographerModule.Instance.Database.InsertOrUpdate(c);
            Input.Id = c.Id;
            Save?.Invoke();
            Navigation.PopModalAsync();
        }
    }
}