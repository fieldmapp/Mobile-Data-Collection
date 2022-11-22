using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace DlrDataApp.Modules.Base.Shared.Localization
{
    /// <summary>
    /// Static Collection of all localization resources. Used by <see cref="TranslateExtension"/>
    /// </summary>
    public static class ResourcesCollector
    {
        private static List<ResourceManager> ResourceManagers = new List<ResourceManager> { GetResourceManager<SharedResources>() };
        /// <summary>
        /// Add resource to global collection.
        /// </summary>
        /// <typeparam name="T">Type of localization resource class.</typeparam>
        public static void AddResource<T>()
        {
            ResourceManagers.Add(GetResourceManager<T>());
        }

        private static ResourceManager GetResourceManager<T>()
        {
            var type = typeof(T);
            return new ResourceManager(type.FullName, type.Assembly);
        }

        /// <summary>
        /// Fetches the localization for a given key from a specified culture
        /// </summary>
        /// <param name="key">Localization key (as used in .resx files)</param>
        /// <param name="culture">Culture containing eg information about the used language. Defaults to <see cref="CultureInfo.CurrentCulture"/></param>
        /// <returns>The value of the resource localized for the specified culture, or null if name cannot be found in a resource set</returns>
        public static string GetFromAnyResource(string key, CultureInfo culture = null)
        {
            if (key == null)
                return null;
            culture = culture ?? CultureInfo.CurrentCulture;
            foreach (var resource in ResourceManagers)
            {
                var val = resource.GetString(key, culture);
                if (val != null)
                    return val;
            }
            return null;
        }
    }
}
