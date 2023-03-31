using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Base.Shared.Controls;
using DlrDataApp.Modules.Base.Shared.DependencyServices;
using DlrDataApp.Modules.Base.Shared.TouchGesture;
using DlrDataApp.Modules.FieldCartographer.Shared.VoiceActions;
using DlrDataApp.Modules.SpeechRecognition.Definition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;
using static DlrDataApp.Modules.FieldCartographer.Shared.VoiceCommandCompiler;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrivingPage : ContentPage
    {
        enum DamageType
        {
            Low,
            Medium,
            High
        }
        public const int MaxLaneCountPerSide = 3;
        public const int MaxTotalLaneCount = 2 * MaxLaneCountPerSide + 1;

        readonly static Brush CompletedInfoBrush = Brush.Green;
        readonly static Brush ActiveForInputBrush = Brush.Orange;

        Color InactiveButtonColor => (Color)Resources["InactiveButtonColor"];
        Brush InactiveButtonBrush => new SolidColorBrush(InactiveButtonColor);
        
        Color InactiveButtonColorNight => (Color)Resources["InactiveButtonColorNight"];
        Brush InactiveButtonBrushNight => new SolidColorBrush(InactiveButtonColorNight);

        Color ActiveButtonColor => (Color)Resources["ActiveColor"];
        Brush ActiveButtonBrush => new SolidColorBrush(ActiveButtonColor);
        
        Color ActiveButtonColorNight => (Color)Resources["ActiveColorNight"];
        Brush ActiveButtonBrushNight => new SolidColorBrush(ActiveButtonColorNight);

        readonly int LaneCountPerSide;
        readonly int TotalLaneCount;
        readonly List<Button> LaneBeginButtons = new List<Button>();
        readonly List<Button> LaneMiddleButtons = new List<Button>();
        readonly List<Button> LaneEndButtons = new List<Button>();
        readonly List<BoxView> TypeLaneBackgrounds = new List<BoxView>();
        readonly List<BoxView> CauseLaneBackgrounds = new List<BoxView>();
        readonly List<FormattedButton> DamageTypeButtons = new List<FormattedButton>();
        readonly List<FormattedButton> DamageCauseButtons = new List<FormattedButton>();
        readonly bool[] IsLaneActive;
        readonly bool[] IsLaneSelectedForInput;
        readonly bool[] IsLaneCauseEntered;
        readonly bool[] IsLaneTypeEntered;
        readonly bool[] IsLaneStarted;
        readonly bool[] IsLaneInInitialState;
        readonly string LogFilePath;

        bool _shouldMicrophoneBeRecording = false;
        bool ShouldMicrophoneBeRecording
        {
            get => _shouldMicrophoneBeRecording;
            set
            {
                if (value != _shouldMicrophoneBeRecording)
                {
                    if (value)
                        SpeechRecognizer.StartListening();
                    else
                        SpeechRecognizer.StopListening();
                }
                _shouldMicrophoneBeRecording = value;
            }
        }


        List<(View view, TouchEffect effect)> ActiveTouchEffects = new List<(View view, TouchEffect effect)>();
        protected override void OnAppearing()
        {
            if (ShouldMicrophoneBeRecording)
                SpeechRecognizer.StartListening();

            SpeechRecognizer.ResultRecognized += SpeechRecognizer_ResultRecognized;
            foreach (var child in GetNestedChildren(RelativeLayout))
            {
                var touchEffect = new TouchEffect { Capture = false };
                touchEffect.TouchAction += TouchEffect_TouchAction;
                child.Effects.Add(touchEffect);
                ActiveTouchEffects.Add((child, touchEffect));
            }
        }

        protected override void OnDisappearing()
        {
            SpeechRecognizer.ResultRecognized -= SpeechRecognizer_ResultRecognized;
            SpeechRecognizer.StopListening();
            foreach ((View view, TouchEffect effect) in ActiveTouchEffects)
            {
                view.Effects.Remove(effect);
                effect.TouchAction -= TouchEffect_TouchAction;
            }
            ActiveTouchEffects = new List<(View view, TouchEffect effect)>();
        }

        ISpeechRecognizer SpeechRecognizer;

        public DrivingPageConfiguration Configuration { get; set; }
        public DrivingPage(DrivingPageConfiguration configuration)
        {
            Configuration = configuration;
            LaneCountPerSide = Configuration.LaneCount;
            TotalLaneCount = 2 * LaneCountPerSide;

            IsLaneActive = new bool[TotalLaneCount];
            IsLaneSelectedForInput = new bool[TotalLaneCount];
            IsLaneCauseEntered = new bool[TotalLaneCount];
            IsLaneTypeEntered = new bool[TotalLaneCount];
            IsLaneStarted = new bool[TotalLaneCount];
            IsLaneInInitialState = Enumerable.Repeat(true, TotalLaneCount).ToArray();

            var outputDir = "fieldmapp";
            var logFileIdentifier = "drivingView_" + FieldCartographerModule.Instance.CurrentUser.Username + "_" + LaneCountPerSide + "_" + configuration.LaneWidth + "_" + configuration.GpsAntennaToInputLocationOffset + "_" + DateTime.UtcNow.GetSafeIdentifier() + ".txt";
            LogFilePath = System.IO.Path.Combine(outputDir, logFileIdentifier);

            InitializeComponent();
            WriteUsingCsvWriter(csvWriter => {
                csvWriter.WriteComment(JsonTranslator.GetJson(configuration).Replace("\n", "").Replace("\r", "") + Environment.NewLine);
                csvWriter.WriteHeader<InteractionInfo>();
                csvWriter.NextRecord();
            } );

            CancelButton.Clicked += (a,b) => ResetToInitialState();

            TypeLowButton.Clicked += (a, b) => DamageTypeButtonClicked(DamageType.Low);
            TypeMediumButton.Clicked += (a, b) => DamageTypeButtonClicked(DamageType.Medium);
            TypeHighButton.Clicked += (a, b) => DamageTypeButtonClicked(DamageType.High);
            DamageTypeButtons = new List<FormattedButton>
            {
                TypeLowButton,
                TypeMediumButton,
                TypeHighButton
            };

            var damageCauseIds = new List<string>
            {
                configuration.Cause1Id,
                configuration.Cause2Id,
                configuration.Cause3Id,
                configuration.Cause4Id,
                configuration.Cause5Id,
                configuration.Cause6Id,
                configuration.Cause7Id,
                configuration.Cause8Id,
                configuration.Cause9Id
            };
            var damageCauseFormattedStrings = new List<FormattedString>
            {
                configuration.Cause1,
                configuration.Cause2,
                configuration.Cause3,
                configuration.Cause4,
                configuration.Cause5,
                configuration.Cause6,
                configuration.Cause7,
                configuration.Cause8,
                configuration.Cause9
            };

            float[] causesPerRow = { 1, 2, 3, 2, 3, 3, 3, 3, 3 };
            DamageCauseButtons = new List<FormattedButton>();
            var visibleButtonCount = damageCauseIds.Count(i => !string.IsNullOrWhiteSpace(i));
            for (int i = 0; i < damageCauseIds.Count; i++)
            {
                var formattedButton = new FormattedButton 
                { 
                    FormattedText = damageCauseFormattedStrings[i], 
                    Padding = new Thickness(5, 0), 
                    Margin = new Thickness(0, 5)
                };
                DamageCauseButtons.Add(formattedButton);
                ButtonLayout.Children.Add(formattedButton);
                FlexLayout.SetBasis(formattedButton, new FlexBasis(1 / causesPerRow[visibleButtonCount - 1] - 0.01f, true));

                var damageCauseId = damageCauseIds[i];
                if (string.IsNullOrWhiteSpace(damageCauseId))
                    formattedButton.IsVisible = false;
                else
                    formattedButton.Clicked += (a, b) => DamageCauseButtonClicked(damageCauseId);
            }

            AddSideLanesToLayout();
            ResetToInitialState();
            SpeechRecognizer = DependencyService.Get<ISpeechRecognizerProvider>().Initialize(KeywordStringToSymbol.Keys.ToList());
            SpeechRecognizer.ResultRecognized += SpeechRecognizer_ResultRecognized;
        }

        IEnumerable<View> GetNestedChildren(View view)
        {
            // i think we cant simplify to use Layout here because the specific layouts dont overrwrite the Layout.Children property (they just hide it using the new keyword)
            if (view is RelativeLayout relaiveLayout)
            {
                foreach (var child in relaiveLayout.Children.SelectMany(c => GetNestedChildren(c)))
                    yield return child;
            }
            else if (view is FlexLayout flexLayout)
            {
                foreach (var child in flexLayout.Children.SelectMany(c => GetNestedChildren(c)))
                    yield return child;
            }
            else
                yield return view;
        }

        private void MicButton_Clicked(object sender, EventArgs e)
        {
            ShouldMicrophoneBeRecording = !ShouldMicrophoneBeRecording;
            if (ShouldMicrophoneBeRecording)
                MicButton.Background = Brush.Green;
            else
                MicButton.Background = Brush.Gray;
        }

        private void SpeechRecognizer_ResultRecognized(object sender, SpeechRecognitionResult e)
        {
            var command = Compile(e.Parts.Select(p => p.Word).ToList());
            DependencyService.Get<IToast>().ShortAlert(e.Result);
            try 
            {
                if (command is InvalidAction)
                    return;
                if (command is CancelAction)
                {
                    ResetToInitialState();
                    return;
                }
                if (command is StartZonesAction startZonesAction)
                {
                    foreach (var laneIndex in startZonesAction.LaneIndices)
                    {
                        BeginZone(laneIndex);
                    }
                    return;
                }
                if (command is EndZonesAction endZonesAction)
                {
                    foreach (var laneIndex in endZonesAction.LaneIndices)
                    {
                        EndZone(laneIndex);
                    }
                    return;
                }
                if (command is SetZonesDetailAction setZonesDetailAction)
                {
                    if (setZonesDetailAction.DamageCause != KeywordSymbol.invalid)
                    {
                        SetDamageCauses(setZonesDetailAction.LaneIndices.Where(i => !IsLaneInInitialState[i]).ToList(), keywordSymbolToCause[setZonesDetailAction.DamageCause]);
                    }
                    if (setZonesDetailAction.DamageType != KeywordSymbol.invalid)
                    {
                        SetDamageType(setZonesDetailAction.LaneIndices.Where(i => !IsLaneInInitialState[i]).ToList(), keywordSymbolToDamageType[setZonesDetailAction.DamageType]);
                    }
                    if (setZonesDetailAction.ShouldEndZone)
                    {
                        foreach (var laneIndex in setZonesDetailAction.LaneIndices)
                        {
                            EndZone(laneIndex);
                        }
                    }
                    if (setZonesDetailAction.ShouldStartZone)
                    {
                        foreach (var laneIndex in setZonesDetailAction.LaneIndices)
                        {
                            BeginZone(laneIndex);
                        }
                    }
                    return;
                }
            } catch (Exception ex) {
                Debug.WriteLine("Something went wrong", ex);
                DependencyService.Get<IToast>().ShortAlert("Die Eingabe konnte nicht verstanden werden.");
            }
           
        }

        Dictionary<KeywordSymbol, string> keywordSymbolToCause = new Dictionary<KeywordSymbol, string>
        {
            { KeywordSymbol.maus, "GameMouseDamage" },
            { KeywordSymbol.wild, "GameMouseDamage" },
            { KeywordSymbol.kuppe, "Dome" },
            { KeywordSymbol.nass, "WaterLogging" },
            { KeywordSymbol.sand, "SandLens" },
            { KeywordSymbol.trocken, "DryStress" },
            { KeywordSymbol.verdichtung, "Compaction" },
            { KeywordSymbol.waldrand, "ForestEdge" },
            { KeywordSymbol.wende, "Headland" },
            { KeywordSymbol.hang, "Slope" },
        };
        Dictionary<KeywordSymbol, DamageType> keywordSymbolToDamageType = new Dictionary<KeywordSymbol, DamageType>
        {
            { KeywordSymbol.gering, DamageType.Low },
            { KeywordSymbol.mittel, DamageType.Medium },
            { KeywordSymbol.hoch, DamageType.High }
        };

        private void TouchEffect_TouchAction(object sender, TouchActionEventArgs args)
        {
            if (sender is Button button)
                button.PerformClick();
            if (sender is FormattedButton formattedButton)
                formattedButton.OnTap();
        }

        private void ResetToInitialState()
        {
            PushInteractionToLog(new[] { new InteractionInfo(DateTime.UtcNow, -1, "canceled") });
            foreach (var button in LaneBeginButtons)
            {
                SetActiveIndicator(button, true);
            }
            foreach (var formattedButton in DamageCauseButtons.AsEnumerable<View>().Union(DamageTypeButtons).Union(LaneEndButtons).Union(LaneMiddleButtons))
            {
                SetActiveIndicator(formattedButton, false);
            }
            foreach (var boxView in TypeLaneBackgrounds.Union(CauseLaneBackgrounds))
            {
                boxView.Background = Brush.Transparent;
            }
            for (int i = 0; i < TotalLaneCount; i++)
            {
                IsLaneActive[i] = false;
                IsLaneSelectedForInput[i] = false;
                IsLaneCauseEntered[i] = false;
                IsLaneTypeEntered[i] = false;
                IsLaneStarted[i] = false;
                IsLaneInInitialState[i] = true;
            }
        }

        private void AddSideLanesToLayout()
        {
            double laneWidthFactor = 0.047 * 3 / Configuration.LaneCount;

            const double leftMostBorderXFactor = 0.015;
            const double rightMostBorderXFactor = 0.983;
            const double innerBorderWidthFactor = 0.0015;
            const double outerMostBorderWidthFactor = 0.002;
            const double borderYFactor = 0.054;
            const double borderHeightFactor = 0.842;

            double buttonWidthFactor = 0.043 * 3 / Configuration.LaneCount;
            double leftMostButtonXFactor = leftMostBorderXFactor + (laneWidthFactor - buttonWidthFactor) / 2.0;
            double rightMostButtonXFactor = rightMostBorderXFactor - laneWidthFactor + (laneWidthFactor - buttonWidthFactor) / 2.0;
            const double startButtonYFactor = 0.058;
            const double middleButtonYFactor = 0.455;
            const double endButtonYFactor = 0.821;
            const double buttonHeightFactor = 0.070;

            double laneBackgroundWidthFactor = laneWidthFactor - innerBorderWidthFactor;

            for (int i = 0; i < LaneCountPerSide; i++)
            {
                double laneBackgroundXFactor = leftMostBorderXFactor + (LaneCountPerSide - i) * laneWidthFactor - laneWidthFactor + innerBorderWidthFactor / 2;

                var laneTopBackground = new BoxView { Background = Brush.Transparent };
                TypeLaneBackgrounds.Add(laneTopBackground);
                RelativeLayout.Children.Add(laneTopBackground,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundWidthFactor),
                    yConstraint: Constraint.RelativeToView(CenterLaneTypeBackground, (l, v) => v.Y),
                    heightConstraint: Constraint.RelativeToView(CenterLaneTypeBackground, (l, v) => v.Height));

                var laneBottomBackground = new BoxView { Background = Brush.Transparent };
                CauseLaneBackgrounds.Add(laneBottomBackground);
                RelativeLayout.Children.Add(laneBottomBackground,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundWidthFactor),
                    yConstraint: Constraint.RelativeToView(CenterLaneCauseBackground, (l, v) => v.Y),
                    heightConstraint: Constraint.RelativeToView(CenterLaneCauseBackground, (l, v) => v.Height));

                var laneBorder = new BoxView();
                laneBorder.SetAppThemeColor(BoxView.ColorProperty, (Color)Resources["BorderColor"], (Color)Resources["BorderColorNight"]);
                var borderXFactor = leftMostBorderXFactor + (LaneCountPerSide - i) * laneWidthFactor;
                RelativeLayout.Children.Add(laneBorder,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * borderXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * innerBorderWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * borderYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * borderHeightFactor));

                int laneIndex = i;
                string buttonText = (laneIndex + 1).ToString();
                var buttonXFactor = leftMostButtonXFactor + (LaneCountPerSide - i - 1) * laneWidthFactor;
                var topButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName] };
                LaneBeginButtons.Add(topButton);
                
                topButton.Clicked += (a, b) => BeginZone(laneIndex);
                topButton.FontSize = Device.GetNamedSize(NamedSize.Small, topButton);
                RelativeLayout.Children.Add(topButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * startButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var middleButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName] };
                LaneMiddleButtons.Add(middleButton);
                middleButton.Clicked += (a, b) => ActivateLaneForDetailInput(laneIndex);
                middleButton.FontSize = Device.GetNamedSize(NamedSize.Small, middleButton);
                RelativeLayout.Children.Add(middleButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * middleButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var endButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName] };
                LaneEndButtons.Add(endButton);
                endButton.Clicked += (a, b) => EndZone(laneIndex);
                endButton.FontSize = Device.GetNamedSize(NamedSize.Small, endButton);
                RelativeLayout.Children.Add(endButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * endButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

            }
            var leftMostLaneBorder = new BoxView();
            leftMostLaneBorder.SetAppThemeColor(BoxView.ColorProperty, (Color)Resources["BorderColor"], (Color)Resources["BorderColorNight"]);
            RelativeLayout.Children.Add(leftMostLaneBorder,
                xConstraint: Constraint.RelativeToParent(p => p.Width * leftMostBorderXFactor),
                widthConstraint: Constraint.RelativeToParent(p => p.Width * outerMostBorderWidthFactor),
                yConstraint: Constraint.RelativeToParent(p => p.Height * borderYFactor),
                heightConstraint: Constraint.RelativeToParent(p => p.Height * borderHeightFactor));

            for (int i = 0; i < LaneCountPerSide; i++)
            {
                double laneBackgroundXFactor = rightMostBorderXFactor - (LaneCountPerSide - i) * laneWidthFactor + innerBorderWidthFactor;

                var laneTopBackground = new BoxView { Background = Brush.Transparent, BackgroundColor = Color.Transparent, Color = Color.Transparent };
                TypeLaneBackgrounds.Add(laneTopBackground);
                RelativeLayout.Children.Add(laneTopBackground,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundWidthFactor),
                    yConstraint: Constraint.RelativeToView(CenterLaneTypeBackground, (l, v) => v.Y),
                    heightConstraint: Constraint.RelativeToView(CenterLaneTypeBackground, (l, v) => v.Height));

                var laneBottomBackground = new BoxView { Background = Brush.Transparent };
                CauseLaneBackgrounds.Add(laneBottomBackground);
                RelativeLayout.Children.Add(laneBottomBackground,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundWidthFactor),
                    yConstraint: Constraint.RelativeToView(CenterLaneCauseBackground, (l, v) => v.Y),
                    heightConstraint: Constraint.RelativeToView(CenterLaneCauseBackground, (l, v) => v.Height));


                var laneBorder = new BoxView();
                laneBorder.SetAppThemeColor(BoxView.ColorProperty, (Color)Resources["BorderColor"], (Color)Resources["BorderColorNight"]);
                var borderXFactor = rightMostBorderXFactor - (LaneCountPerSide - i) * laneWidthFactor;
                RelativeLayout.Children.Add(laneBorder,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * borderXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * innerBorderWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * borderYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * borderHeightFactor));

                int laneIndex = i + LaneCountPerSide;
                string buttonText = (laneIndex + 1).ToString();
                var buttonXFactor = rightMostButtonXFactor - (LaneCountPerSide - i - 1) * laneWidthFactor;
                var topButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName] };
                LaneBeginButtons.Add(topButton);
                topButton.Clicked += (a, b) => BeginZone(laneIndex);
                topButton.FontSize = Device.GetNamedSize(NamedSize.Small, topButton);
                RelativeLayout.Children.Add(topButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * startButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var middleButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName] };
                LaneMiddleButtons.Add(middleButton);
                middleButton.Clicked += (a, b) => ActivateLaneForDetailInput(laneIndex);
                middleButton.FontSize = Device.GetNamedSize(NamedSize.Small, middleButton);
                RelativeLayout.Children.Add(middleButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * middleButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var endButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName]};
                LaneEndButtons.Add(endButton);
                endButton.Clicked += (a, b) => EndZone(laneIndex);
                endButton.FontSize = Device.GetNamedSize(NamedSize.Small, endButton);
                RelativeLayout.Children.Add(endButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * endButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));
            }
            var rightMostLaneBorder = new BoxView();
            rightMostLaneBorder.SetAppThemeColor(BoxView.ColorProperty, (Color)Resources["BorderColor"], (Color)Resources["BorderColorNight"]);
            RelativeLayout.Children.Add(rightMostLaneBorder,
                xConstraint: Constraint.RelativeToParent(p => p.Width * rightMostBorderXFactor),
                widthConstraint: Constraint.RelativeToParent(p => p.Width * outerMostBorderWidthFactor),
                yConstraint: Constraint.RelativeToParent(p => p.Height * borderYFactor),
                heightConstraint: Constraint.RelativeToParent(p => p.Height * borderHeightFactor));
        }

        private void BeginZone(int laneIndex)
        {
            IsLaneInInitialState[laneIndex] = false;
            if (!IsLaneActive[laneIndex])
            {
                PushInteractionToLog(new[] { new InteractionInfo(DateTime.UtcNow, laneIndex, "open") });

                SetActiveIndicator(LaneBeginButtons[laneIndex], false);
                SetActiveIndicator(LaneEndButtons[laneIndex], true);
                var middleButton = LaneMiddleButtons[laneIndex];
                SetActiveIndicator(middleButton, true);
                IsLaneStarted[laneIndex] = true;
                IsLaneActive[laneIndex] = true;
                IsLaneCauseEntered[laneIndex] = false;
                IsLaneTypeEntered[laneIndex] = false;
                IsLaneSelectedForInput[laneIndex] = false;
                TypeLaneBackgrounds[laneIndex].Background = Brush.Transparent;
                CauseLaneBackgrounds[laneIndex].Background = Brush.Transparent;
                if (!IsLaneSelectedForInput.Any(l => l))
                    ShowTypeAndCauseButtonsActiveStatus(false);
            }
        }

        void SetActiveIndicator(View view, bool active)
        {
            if (view is Button button)
                button.SetAppThemeColor(BackgroundColorProperty, active ? ActiveButtonColor : InactiveButtonColor, active ? ActiveButtonColorNight : InactiveButtonColorNight);
            else if (view is FormattedButton formattedButton)
                formattedButton.SetAppThemeColor(BackgroundColorProperty, active ? ActiveButtonColor : InactiveButtonColor, active ? ActiveButtonColorNight : InactiveButtonColorNight);
            else if (view is Xamarin.Forms.Shapes.Path shape)
                shape.SetOnAppTheme(Shape.FillProperty, active ? ActiveButtonBrush : InactiveButtonBrush, active ? ActiveButtonBrushNight : InactiveButtonBrushNight);
        }

        public class InteractionInfo
        {
            public InteractionInfo(DateTime utcDateTime, int laneIndex, string action)
            {
                UtcDateTime = utcDateTime;
                LaneIndex = laneIndex;
                Action = action;
                NmeaReceivedUtcDateTime = DependencyService.Get<IUbloxCommunicator>().LatestReceivedNMEADate;
            }
            [Index(0)]
            public DateTime UtcDateTime { get; set; }
            [Index(1)]
            public int LaneIndex { get; set; }
            [Index(2)]
            public string Action { get; set; }
            [Index(3)]
            public DateTime NmeaReceivedUtcDateTime { get; set; }
        }

        public void WriteUsingCsvWriter(Action<CsvWriter> writerAction)
        {
            using (var logFileStream = DependencyService.Get<IStorageAccessProvider>().OpenFileAppendExternal(LogFilePath))
            using (var writer = new StreamWriter(logFileStream))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false }))
            {
                writerAction(csv);
            }
        }

        public void PushInteractionToLog(IEnumerable<InteractionInfo> interactionInfo)
        {
            WriteUsingCsvWriter(csvWriter => csvWriter.WriteRecords(interactionInfo));
        }

        private void ActivateLaneForDetailInput(int laneIndex)
        {
            if (IsLaneStarted[laneIndex] && !IsLaneSelectedForInput[laneIndex])
            {
                ShowTypeAndCauseButtonsActiveStatus(true);

                if (!IsLaneCauseEntered[laneIndex])
                    CauseLaneBackgrounds[laneIndex].Background = ActiveForInputBrush;
                if (!IsLaneTypeEntered[laneIndex])
                    TypeLaneBackgrounds[laneIndex].Background = ActiveForInputBrush;
                IsLaneSelectedForInput[laneIndex] = true;
                SetActiveIndicator(LaneMiddleButtons[laneIndex], false);
            }
        }

        private void EndZone(int laneIndex)
        {
            if (IsLaneActive[laneIndex])
            {
                PushInteractionToLog(new[] { new InteractionInfo(DateTime.UtcNow, laneIndex, "close") });

                SetActiveIndicator(LaneBeginButtons[laneIndex], true);
                SetActiveIndicator(LaneEndButtons[laneIndex], false);
                IsLaneActive[laneIndex] = false;
            }
        }

        void ShowTypeAndCauseButtonsActiveStatus(bool active)
        {
            foreach (var button in DamageCauseButtons.Union(DamageTypeButtons))
            {
                SetActiveIndicator(button, active);
            }
        }

        void SetDamageType(List<int> laneIndices, DamageType type)
        {
            var now = DateTime.UtcNow;
            var interactionInfos = new List<InteractionInfo>();
            foreach (var laneIndex in laneIndices)
            {
                IsLaneTypeEntered[laneIndex] = true;
                if (!IsLaneCauseEntered[laneIndex])
                    CauseLaneBackgrounds[laneIndex].Background = Brush.Transparent;
                TypeLaneBackgrounds[laneIndex].Background = CompletedInfoBrush;
                SetActiveIndicator(LaneMiddleButtons[laneIndex], true);
                interactionInfos.Add(new InteractionInfo(now, laneIndex, "damage=" + type.ToString()));
            }

            if (interactionInfos.Any())
                PushInteractionToLog(interactionInfos);
        }

        void DamageTypeButtonClicked(DamageType type)
        {
            ShowTypeAndCauseButtonsActiveStatus(false);
            var laneIndices = new List<int>();
            for (int i = 0; i < TotalLaneCount; i++)
            {
                if (IsLaneSelectedForInput[i])
                {
                    IsLaneSelectedForInput[i] = false;
                    laneIndices.Add(i);
                }
            }
            SetDamageType(laneIndices, type);
        }

        void SetDamageCauses(List<int> laneIndices, string cause)
        {
            var now = DateTime.UtcNow;
            var interactionInfos = new List<InteractionInfo>();
            foreach (var laneIndex in laneIndices)
            {
                IsLaneSelectedForInput[laneIndex] = false;
                IsLaneCauseEntered[laneIndex] = true;
                if (!IsLaneTypeEntered[laneIndex])
                    TypeLaneBackgrounds[laneIndex].Background = Brush.Transparent;
                CauseLaneBackgrounds[laneIndex].Background = CompletedInfoBrush;
                SetActiveIndicator(LaneMiddleButtons[laneIndex], true);
                interactionInfos.Add(new InteractionInfo(now, laneIndex, "cause=" + cause));
            }

            if (interactionInfos.Any())
                PushInteractionToLog(interactionInfos);
        }
        void DamageCauseButtonClicked(string cause)
        {
            ShowTypeAndCauseButtonsActiveStatus(false);
            var laneIndices = new List<int>();
            for (int i = 0; i < TotalLaneCount; i++)
            {
                if (IsLaneSelectedForInput[i])
                {
                    IsLaneSelectedForInput[i] = false;
                    laneIndices.Add(i);
                }
            }
            SetDamageCauses(laneIndices, cause);
        }
    }
}