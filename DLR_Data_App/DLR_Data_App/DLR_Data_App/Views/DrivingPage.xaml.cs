using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using DLR_Data_App.Controls;
using DLR_Data_App.Services;
using DLR_Data_App.Services.TouchGesture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Views
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
        readonly string LogFileIdentifier;


        public DrivingConfigurationPage.DrivingPageConfiguration Configuration { get; set; }
        public DrivingPage(DrivingConfigurationPage.DrivingPageConfiguration configuration)
        {
            Configuration = configuration;
            LaneCountPerSide = Configuration.LaneCount;
            TotalLaneCount = 2 * LaneCountPerSide + 1;

            IsLaneActive = new bool[TotalLaneCount];
            IsLaneSelectedForInput = new bool[TotalLaneCount];
            IsLaneCauseEntered = new bool[TotalLaneCount];
            IsLaneTypeEntered = new bool[TotalLaneCount];
            IsLaneStarted = new bool[TotalLaneCount];

            LogFileIdentifier = "drivingView_" + App.CurrentUser.Username + "_" + LaneCountPerSide + DateTime.UtcNow.GetSafeIdentifier() + ".txt";

            InitializeComponent();
            WriteUsingCsvWriter(csvWriter => csvWriter.WriteHeader<InteractionInfo>());

            var backgroundTouchEffect = new TouchEffect();
            backgroundTouchEffect.TouchAction += BackgroundTouchEffect_TouchAction;
            RelativeLayout.Effects.Add(backgroundTouchEffect);

            TypeLaneBackgrounds.Add(CenterLaneTypeBackground);
            CauseLaneBackgrounds.Add(CenterLaneCauseBackground);

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

            Cause1Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause1Button.FormattedText.Spans.Select(s => s.Text)));
            Cause2Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause2Button.FormattedText.Spans.Select(s => s.Text)));
            Cause3Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause3Button.FormattedText.Spans.Select(s => s.Text)));
            Cause4Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause4Button.FormattedText.Spans.Select(s => s.Text)));
            Cause5Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause5Button.FormattedText.Spans.Select(s => s.Text)));
            Cause6Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause6Button.FormattedText.Spans.Select(s => s.Text)));
            Cause7Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause7Button.FormattedText.Spans.Select(s => s.Text)));
            Cause8Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause8Button.FormattedText.Spans.Select(s => s.Text)));
            Cause9Button.Clicked += (a, b) => DamageCauseButtonClicked(string.Join("", Cause9Button.FormattedText.Spans.Select(s => s.Text)));
            DamageCauseButtons = new List<FormattedButton>
            {
                Cause1Button,
                Cause2Button,
                Cause3Button,
                Cause4Button,
                Cause5Button,
                Cause6Button,
                Cause7Button,
                Cause8Button,
                Cause9Button
            };

            AddSideLanesToLayout();
            ResetToInitialState();
        }

        private void BackgroundTouchEffect_TouchAction(object sender, TouchActionEventArgs args)
        {
            var pos = args.Location;
            double normalizedX = pos.X / RelativeLayout.Width;
            double normalizedY = pos.Y / RelativeLayout.Height;
            PushInteractionToLog(new[] { new InteractionInfo(DateTime.UtcNow, -1, $"Miss: {normalizedX.ToString(CultureInfo.InvariantCulture)}, {normalizedY.ToString(CultureInfo.InvariantCulture)}") });
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
            }
        }

        private void AddSideLanesToLayout()
        {
            const double laneWidthFactor = 0.047;

            const double leftMostBorderXFactor = 0.015;
            const double rightMostBorderXFactor = 0.983;
            const double innerBorderWidthFactor = 0.0015;
            const double outerMostBorderWidthFactor = 0.002;
            const double borderYFactor = 0.054;
            const double borderHeightFactor = 0.842;

            const double buttonWidthFactor = 0.043;
            const double leftMostButtonXFactor = leftMostBorderXFactor + (laneWidthFactor - buttonWidthFactor) / 2.0;
            const double rightMostButtonXFactor = rightMostBorderXFactor - laneWidthFactor + (laneWidthFactor - buttonWidthFactor) / 2.0;
            const double startButtonYFactor = 0.058;
            const double middleButtonYFactor = 0.455;
            const double endButtonYFactor = 0.821;
            const double buttonHeightFactor = 0.070;

            const double laneBackgroundWidthFactor = laneWidthFactor - innerBorderWidthFactor;

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
                topButton.Clicked += (a, b) => BeginButtonClicked(laneIndex);
                topButton.FontSize = Device.GetNamedSize(NamedSize.Small, topButton);
                RelativeLayout.Children.Add(topButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * startButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var middleButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName] };
                LaneMiddleButtons.Add(middleButton);
                middleButton.Clicked += (a, b) => CenterButtonClicked(laneIndex);
                middleButton.FontSize = Device.GetNamedSize(NamedSize.Small, middleButton);
                RelativeLayout.Children.Add(middleButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * middleButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var endButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName] };
                LaneEndButtons.Add(endButton);
                endButton.Clicked += (a, b) => EndButtonClicked(laneIndex);
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
                topButton.Clicked += (a, b) => BeginButtonClicked(laneIndex);
                topButton.FontSize = Device.GetNamedSize(NamedSize.Small, topButton);
                RelativeLayout.Children.Add(topButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * startButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var middleButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName] };
                LaneMiddleButtons.Add(middleButton);
                middleButton.Clicked += (a, b) => CenterButtonClicked(laneIndex);
                middleButton.FontSize = Device.GetNamedSize(NamedSize.Small, middleButton);
                RelativeLayout.Children.Add(middleButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * middleButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var endButton = new Button { Text = buttonText, FontAttributes = FontAttributes.Bold, Padding = new Thickness(0), Margin = new Thickness(0), Style = (Style)Application.Current.Resources[typeof(Button).FullName]};
                LaneEndButtons.Add(endButton);
                endButton.Clicked += (a, b) => EndButtonClicked(laneIndex);
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

        private void BeginButtonClicked(int laneIndex)
        {
            if (!IsLaneActive[laneIndex])
            {
                PushInteractionToLog(new[] { new InteractionInfo(DateTime.UtcNow, laneIndex, "open") });

                SetActiveIndicator(LaneBeginButtons[laneIndex], false);
                SetActiveIndicator(LaneEndButtons[laneIndex], true);
                var middleButton = GetMiddleButtonWithIndex(laneIndex);
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

        View GetMiddleButtonWithIndex(int index) => LaneMiddleButtons[index];

        public class InteractionInfo
        {
            public InteractionInfo(DateTime utcDateTime, int laneIndex, string action)
            {
                UtcDateTime = utcDateTime;
                LaneIndex = laneIndex;
                Action = action;
            }
            [Index(0)]
            public DateTime UtcDateTime { get; set; }
            [Index(1)]
            public int LaneIndex { get; set; }
            [Index(2)]
            public string Action { get; set; }
        }

        public void WriteUsingCsvWriter(Action<CsvWriter> writerAction)
        {
            using (var logFileStream = DependencyService.Get<IStorageAccessProvider>().OpenFileAppendExternal(LogFileIdentifier))
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

        private void CenterButtonClicked(int laneIndex)
        {
            if (IsLaneStarted[laneIndex] && !IsLaneSelectedForInput[laneIndex])
            {
                ShowTypeAndCauseButtonsActiveStatus(true);

                if (!IsLaneCauseEntered[laneIndex])
                    CauseLaneBackgrounds[laneIndex].Background = ActiveForInputBrush;
                if (!IsLaneTypeEntered[laneIndex])
                    TypeLaneBackgrounds[laneIndex].Background = ActiveForInputBrush;
                IsLaneSelectedForInput[laneIndex] = true;
                SetActiveIndicator(GetMiddleButtonWithIndex(laneIndex), false);
            }
        }

        private void EndButtonClicked(int laneIndex)
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

        void DamageTypeButtonClicked(DamageType type)
        {
            ShowTypeAndCauseButtonsActiveStatus(false);
            var now = DateTime.UtcNow;
            var interactionInfos = new List<InteractionInfo>();
            for (int i = 0; i < TotalLaneCount; i++)
            {
                if (IsLaneSelectedForInput[i])
                {

                    IsLaneSelectedForInput[i] = false;
                    IsLaneTypeEntered[i] = true;
                    if (!IsLaneCauseEntered[i])
                        CauseLaneBackgrounds[i].Background = Brush.Transparent;
                    TypeLaneBackgrounds[i].Background = CompletedInfoBrush;
                    SetActiveIndicator(GetMiddleButtonWithIndex(i), true);
                    interactionInfos.Add(new InteractionInfo(now, i, "damage=" + type.ToString()));
                }
            }
            if (interactionInfos.Any())
                PushInteractionToLog(interactionInfos);
        }

        void DamageCauseButtonClicked(string cause)
        {
            ShowTypeAndCauseButtonsActiveStatus(false);
            var now = DateTime.UtcNow;
            var interactionInfos = new List<InteractionInfo>();
            for (int i = 0; i < TotalLaneCount; i++)
            {
                if (IsLaneSelectedForInput[i])
                {
                    IsLaneSelectedForInput[i] = false;
                    IsLaneCauseEntered[i] = true;
                    if (!IsLaneTypeEntered[i])
                        TypeLaneBackgrounds[i].Background = Brush.Transparent;
                    CauseLaneBackgrounds[i].Background = CompletedInfoBrush;
                    SetActiveIndicator(GetMiddleButtonWithIndex(i), true);
                    interactionInfos.Add(new InteractionInfo(now, i, "cause=" + cause));
                }
            }
            if (interactionInfos.Any())
                PushInteractionToLog(interactionInfos);
        }

        private void ConfigurePressed(object sender, EventArgs e)
        {
            var confPage = new DrivingConfigurationPage(new DrivingConfigurationPage.DrivingPageConfiguration());
            Navigation.PushModalAsync(confPage);
        }
    }
}