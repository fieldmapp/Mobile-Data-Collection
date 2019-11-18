using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DLR_Data_App.Localizations
{
  [ContentProperty("Text")]
  public class TranslateExtension : IMarkupExtension
  {
    private const string ResourceId = "DLR_Data_App.Localizations.AppResources";
    public string Text { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
      if (Text == null)
        return null;

      var resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
      return resourceManager.GetString(Text, CultureInfo.CurrentCulture);
    }
  }
}
