using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.SharedModule.Localization
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return ResourcesCollector.GetFromAnyResource(Text);
        }
    }
}
