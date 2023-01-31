using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.Base.Shared.Localization
{
    /// <summary>
    /// Markup extension class allowing localization being used in XAML.
    /// </summary>
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        /// <summary>
        /// Text containing the localization key
        /// </summary>
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return ResourcesCollector.GetFromAnyResource(Text);
        }
    }
}