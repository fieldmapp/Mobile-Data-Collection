using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace DlrDataApp.Modules.SharedModule.Localization
{
    public static class ResourcesCollector
    {
        private static List<ResourceManager> ResourceManagers = new List<ResourceManager> { GetResourceManager<AppResources>() };
        public static void AddResource<T>()
        {
            ResourceManagers.Add(GetResourceManager<T>());
        }

        private static ResourceManager GetResourceManager<T>()
        {
            var type = typeof(T);
            return new ResourceManager(type.FullName, type.Assembly);
        }

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
