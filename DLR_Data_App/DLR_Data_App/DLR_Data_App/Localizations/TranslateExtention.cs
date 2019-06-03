using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Localizations
{
  [ContentProperty("Text")]
  /**
   * Class used to translate strings
   */
  class TranslateExtention : IMarkupExtension
  {
    const string ResourceId = "DLR_Data_App.Localizations.AppResources";
    public string Text { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
      if (Text == null)
        return null;

      ResourceManager resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtention).GetTypeInfo().Assembly);
      return resourceManager.GetString(Text, CultureInfo.CurrentCulture);
    }
  }
}
