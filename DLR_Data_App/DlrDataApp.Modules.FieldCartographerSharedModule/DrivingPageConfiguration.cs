using System.Collections.Generic;

using static DlrDataApp.Modules.Base.Shared.Services.FormattedStringSerializerHelper;
using Xamarin.Forms;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    public class DrivingPageConfiguration : BindableObject
    {
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

        public IEnumerable<(string Id, FormattedString cause)> GetCauses()
        {
            yield return (Cause1Id, Cause1);
            yield return (Cause2Id, Cause2);
            yield return (Cause3Id, Cause3);
            yield return (Cause4Id, Cause4);
            yield return (Cause5Id, Cause5);
            yield return (Cause6Id, Cause6);
            yield return (Cause7Id, Cause7);
            yield return (Cause8Id, Cause8);
            yield return (Cause9Id, Cause9);
        }
    }
}