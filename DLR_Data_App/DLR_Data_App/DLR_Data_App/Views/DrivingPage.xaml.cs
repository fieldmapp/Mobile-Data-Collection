using DLR_Data_App.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        enum DamageCause
        {
            SandLens,
            Compaction,
            Headland,
            Dome,
            Slope,
            ForrestEdge,
            DryStress,
            WatterLogging,
            MouseEating
        }
        const int LaneCountPerSide = 3;
        public const int LaneCount = 2 * LaneCountPerSide + 1;

        static Brush CompletedInfoBrush = Brush.Green;
        static Brush ActiveForInputBrush = Brush.Orange;
        static Brush InactiveBrush = Brush.LightGray;
        static Color InactiveButtonColor = Color.WhiteSmoke;
        static Brush InactiveButtonBrush = new SolidColorBrush(InactiveButtonColor);
        static Color ActiveButtonColor = Color.FromHex("#629C44");
        static Brush ActiveButtonBrush = new SolidColorBrush(ActiveButtonColor);
        List<Button> LaneBeginButtons = new List<Button>();
        List<Button> LaneMiddleButtons = new List<Button>();
        List<Button> LaneEndButtons = new List<Button>();
        List<BoxView> TypeLaneBackgrounds = new List<BoxView>();
        List<BoxView> CauseLaneBackgrounds = new List<BoxView>();
        List<FormattedButton> DamageTypeButtons = new List<FormattedButton>();
        List<FormattedButton> DamageCauseButtons = new List<FormattedButton>();

        bool[] IsLaneActive = new bool[LaneCount];
        bool[] IsLaneSelectedForInput = new bool[LaneCount];
        bool[] IsLaneCauseEntered = new bool[LaneCount];
        bool[] IsLaneTypeEntered = new bool[LaneCount];
        bool[] IsLaneStarted = new bool[LaneCount];


        public DrivingPage()
        {
            InitializeComponent();

            var centerLaneStartTapRecognizer = new TapGestureRecognizer();
            centerLaneStartTapRecognizer.Tapped += CenterLaneStartTapRecognizer_Tapped;
            ArrowStartCenterShape.GestureRecognizers.Add(centerLaneStartTapRecognizer);

            var centerLaneEndTapRecognizer = new TapGestureRecognizer();
            centerLaneEndTapRecognizer.Tapped += CenterLaneEndTapRecognizer_Tapped;
            ArrowEndCenterShape.GestureRecognizers.Add(centerLaneEndTapRecognizer);

            CenterLaneMiddleButton.Clicked += (a, b) => CenterButtonClicked(0);

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

            CauseSandLensButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.SandLens);
            CauseCompactionButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.Compaction);
            CauseHeadlandButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.Headland);
            CauseDomeButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.Dome);
            CauseSlopeButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.Slope);
            CauseForrestEdgeButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.ForrestEdge);
            CauseDryStressButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.DryStress);
            CauseWatterLoggingButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.WatterLogging);
            CauseMouseEatingButton.Clicked += (a, b) => DamageCauseButtonClicked(DamageCause.MouseEating);
            DamageCauseButtons = new List<FormattedButton>
            {
                CauseSandLensButton,
                CauseCompactionButton,
                CauseHeadlandButton,
                CauseDomeButton,
                CauseSlopeButton,
                CauseForrestEdgeButton,
                CauseDryStressButton,
                CauseWatterLoggingButton,
                CauseMouseEatingButton
            };

            AddSideLanesToLayout();
            ResetToInitialState();
        }

        private void CenterLaneEndTapRecognizer_Tapped(object sender, EventArgs e)
        {
            EndButtonClicked(0);
        }

        private void CenterLaneStartTapRecognizer_Tapped(object sender, EventArgs e)
        {
            BeginButtonClicked(0);
        }

        private void ResetToInitialState()
        {
            foreach (var button in LaneBeginButtons)
            {
                button.BackgroundColor = ActiveButtonColor;
            }
            foreach (var formattedButton in DamageCauseButtons.AsEnumerable<View>().Union(DamageTypeButtons).Union(LaneEndButtons).Union(LaneMiddleButtons))
            {
                formattedButton.BackgroundColor = InactiveButtonColor;
            }
            CenterLaneMiddleButton.BackgroundColor = InactiveButtonColor;
            ArrowStartCenterShape.Fill = ActiveButtonBrush;
            ArrowEndCenterShape.Fill = InactiveButtonBrush;
            foreach (var boxView in TypeLaneBackgrounds.Union(CauseLaneBackgrounds))
            {
                boxView.Background = Brush.Transparent;
            }
            for (int i = 0; i < LaneCount; i++)
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

            const double buttonWidthFactor = 0.032;
            const double leftMostButtonXFactor = leftMostBorderXFactor + (laneWidthFactor - buttonWidthFactor) / 2.0;
            const double rightMostButtonXFactor = rightMostBorderXFactor - laneWidthFactor + (laneWidthFactor - buttonWidthFactor) / 2.0;
            const double startButtonYFactor = 0.062;
            const double middleButtonYFactor = 0.459;
            const double endButtonYFactor = 0.825;
            const double buttonHeightFactor = 0.062;

            const double laneBackgroundWidthFactor = laneWidthFactor - innerBorderWidthFactor;

            for (int i = 0; i < LaneCountPerSide; i++)
            {
                double laneBackgroundXFactor = leftMostBorderXFactor + (LaneCountPerSide - i) * laneWidthFactor - laneWidthFactor + innerBorderWidthFactor / 2;

                var laneTopBackground = new BoxView { Background = Brush.Transparent };
                TypeLaneBackgrounds.Add(laneTopBackground);
                Layout.Children.Add(laneTopBackground,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundWidthFactor),
                    yConstraint: Constraint.RelativeToView(CenterLaneTypeBackground, (l, v) => v.Y),
                    heightConstraint: Constraint.RelativeToView(CenterLaneTypeBackground, (l, v) => v.Height));

                var laneBottomBackground = new BoxView { Background = Brush.Transparent };
                CauseLaneBackgrounds.Add(laneBottomBackground);
                Layout.Children.Add(laneBottomBackground,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundWidthFactor),
                    yConstraint: Constraint.RelativeToView(CenterLaneCauseBackground, (l, v) => v.Y),
                    heightConstraint: Constraint.RelativeToView(CenterLaneCauseBackground, (l, v) => v.Height));

                var laneBorder = new BoxView { Color = Color.Black };
                var borderXFactor = leftMostBorderXFactor + (LaneCountPerSide - i) * laneWidthFactor;
                Layout.Children.Add(laneBorder,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * borderXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * innerBorderWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * borderYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * borderHeightFactor));

                int laneIndex = i + 1;
                string buttonText = laneIndex.ToString();
                var buttonXFactor = leftMostButtonXFactor + (LaneCountPerSide - i - 1) * laneWidthFactor;
                var topButton = new Button { Text = buttonText, Padding = new Thickness(0), Margin = new Thickness(0), BorderWidth = 0 };
                LaneBeginButtons.Add(topButton);
                topButton.Clicked += (a, b) => BeginButtonClicked(laneIndex);
                topButton.FontSize = Device.GetNamedSize(NamedSize.Small, topButton);
                Layout.Children.Add(topButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * startButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var middleButton = new Button { Text = buttonText, Padding = new Thickness(0), Margin = new Thickness(0), BorderWidth = 0 };
                LaneMiddleButtons.Add(middleButton);
                middleButton.Clicked += (a, b) => CenterButtonClicked(laneIndex);
                middleButton.FontSize = Device.GetNamedSize(NamedSize.Small, middleButton);
                Layout.Children.Add(middleButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * middleButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var endButton = new Button { Text = buttonText, Padding = new Thickness(0), Margin = new Thickness(0), BorderWidth = 0 };
                LaneEndButtons.Add(endButton);
                endButton.Clicked += (a, b) => EndButtonClicked(laneIndex);
                endButton.FontSize = Device.GetNamedSize(NamedSize.Small, endButton);
                Layout.Children.Add(endButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * endButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

            }
            var leftMostLaneBorder = new BoxView { Color = Color.Black };
            Layout.Children.Add(leftMostLaneBorder,
                xConstraint: Constraint.RelativeToParent(p => p.Width * leftMostBorderXFactor),
                widthConstraint: Constraint.RelativeToParent(p => p.Width * outerMostBorderWidthFactor),
                yConstraint: Constraint.RelativeToParent(p => p.Height * borderYFactor),
                heightConstraint: Constraint.RelativeToParent(p => p.Height * borderHeightFactor));

            for (int i = 0; i < LaneCountPerSide; i++)
            {
                double laneBackgroundXFactor = rightMostBorderXFactor - (LaneCountPerSide - i) * laneWidthFactor + innerBorderWidthFactor;

                var laneTopBackground = new BoxView { Background = Brush.Transparent };
                TypeLaneBackgrounds.Add(laneTopBackground);
                Layout.Children.Add(laneTopBackground,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundWidthFactor),
                    yConstraint: Constraint.RelativeToView(CenterLaneTypeBackground, (l, v) => v.Y),
                    heightConstraint: Constraint.RelativeToView(CenterLaneTypeBackground, (l, v) => v.Height));

                var laneBottomBackground = new BoxView { Background = Brush.Transparent };
                CauseLaneBackgrounds.Add(laneBottomBackground);
                Layout.Children.Add(laneBottomBackground,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * laneBackgroundWidthFactor),
                    yConstraint: Constraint.RelativeToView(CenterLaneCauseBackground, (l, v) => v.Y),
                    heightConstraint: Constraint.RelativeToView(CenterLaneCauseBackground, (l, v) => v.Height));


                var laneBorder = new BoxView { Color = Color.Black };
                var borderXFactor = rightMostBorderXFactor - (LaneCountPerSide - i) * laneWidthFactor;
                Layout.Children.Add(laneBorder,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * borderXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * innerBorderWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * borderYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * borderHeightFactor));

                int laneIndex = i + 1 + LaneCountPerSide;
                string buttonText = laneIndex.ToString();
                var buttonXFactor = rightMostButtonXFactor - (LaneCountPerSide - i - 1) * laneWidthFactor;
                var topButton = new Button { Text = buttonText, Padding = new Thickness(0), Margin = new Thickness(0), BorderWidth = 0 };
                LaneBeginButtons.Add(topButton);
                topButton.Clicked += (a, b) => BeginButtonClicked(laneIndex);
                topButton.FontSize = Device.GetNamedSize(NamedSize.Small, topButton);
                Layout.Children.Add(topButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * startButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var middleButton = new Button { Text = buttonText, Padding = new Thickness(0), Margin = new Thickness(0), BorderWidth = 0 };
                LaneMiddleButtons.Add(middleButton);
                middleButton.Clicked += (a, b) => CenterButtonClicked(laneIndex);
                middleButton.FontSize = Device.GetNamedSize(NamedSize.Small, middleButton);
                Layout.Children.Add(middleButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * middleButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));

                var endButton = new Button { Text = buttonText, Padding = new Thickness(0), Margin = new Thickness(0), BorderWidth = 0 };
                LaneEndButtons.Add(endButton);
                endButton.Clicked += (a, b) => EndButtonClicked(laneIndex);
                endButton.FontSize = Device.GetNamedSize(NamedSize.Small, endButton);
                Layout.Children.Add(endButton,
                    xConstraint: Constraint.RelativeToParent(p => p.Width * buttonXFactor),
                    widthConstraint: Constraint.RelativeToParent(p => p.Width * buttonWidthFactor),
                    yConstraint: Constraint.RelativeToParent(p => p.Height * endButtonYFactor),
                    heightConstraint: Constraint.RelativeToParent(p => p.Height * buttonHeightFactor));
            }
            var rightMostLaneBorder = new BoxView { Color = Color.Black };
            Layout.Children.Add(rightMostLaneBorder,
                xConstraint: Constraint.RelativeToParent(p => p.Width * rightMostBorderXFactor),
                widthConstraint: Constraint.RelativeToParent(p => p.Width * outerMostBorderWidthFactor),
                yConstraint: Constraint.RelativeToParent(p => p.Height * borderYFactor),
                heightConstraint: Constraint.RelativeToParent(p => p.Height * borderHeightFactor));
        }

        private void BeginButtonClicked(int laneIndex)
        {
            if (!IsLaneActive[laneIndex])
            {
                if (laneIndex == 0)
                {
                    ArrowStartCenterShape.Fill = InactiveButtonBrush;
                    ArrowEndCenterShape.Fill = ActiveButtonBrush;
                }
                else
                {
                    LaneBeginButtons[laneIndex - 1].Background = InactiveButtonBrush;
                    LaneEndButtons[laneIndex - 1].Background = ActiveButtonBrush;
                }
                var middleButton = GetMiddleButtonWithIndex(laneIndex);
                middleButton.BackgroundColor = ActiveButtonColor;
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

        View GetMiddleButtonWithIndex(int index) => (index == 0 ? (View)CenterLaneMiddleButton : LaneMiddleButtons[index - 1]);

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
                GetMiddleButtonWithIndex(laneIndex).BackgroundColor = InactiveButtonColor;
            }
        }

        private void EndButtonClicked(int laneIndex)
        {
            if (IsLaneActive[laneIndex])
            {
                if (laneIndex == 0)
                {
                    ArrowStartCenterShape.Fill = ActiveButtonBrush;
                    ArrowEndCenterShape.Fill = InactiveButtonBrush;
                }
                else
                {
                    LaneBeginButtons[laneIndex - 1].Background = ActiveButtonBrush;
                    LaneEndButtons[laneIndex - 1].Background = InactiveButtonBrush;
                }
                IsLaneActive[laneIndex] = false;
            }
        }

        void ShowTypeAndCauseButtonsActiveStatus(bool active)
        {
            foreach (var button in DamageCauseButtons.Union(DamageTypeButtons))
            {
                button.BackgroundColor = active ? ActiveButtonColor : InactiveButtonColor;
            }
        }

        void DamageTypeButtonClicked(DamageType type)
        {
            ShowTypeAndCauseButtonsActiveStatus(false);
            for (int i = 0; i < LaneCount; i++)
            {
                if (IsLaneSelectedForInput[i])
                {
                    IsLaneSelectedForInput[i] = false;
                    IsLaneTypeEntered[i] = true;
                    if (!IsLaneCauseEntered[i])
                        CauseLaneBackgrounds[i].Background = Brush.Transparent;
                    TypeLaneBackgrounds[i].Background = CompletedInfoBrush;
                    GetMiddleButtonWithIndex(i).BackgroundColor = ActiveButtonColor;
                }
            }
        }

        void DamageCauseButtonClicked(DamageCause cause)
        {
            ShowTypeAndCauseButtonsActiveStatus(false);
            for (int i = 0; i < LaneCount; i++)
            {
                if (IsLaneSelectedForInput[i])
                {
                    IsLaneSelectedForInput[i] = false;
                    IsLaneCauseEntered[i] = true;
                    if (!IsLaneTypeEntered[i])
                        TypeLaneBackgrounds[i].Background = Brush.Transparent;
                    CauseLaneBackgrounds[i].Background = CompletedInfoBrush;
                    GetMiddleButtonWithIndex(i).BackgroundColor = ActiveButtonColor;
                }
            }
        }
    }
}