using DlrDataApp.Modules.ProfilingSharedModule.Models;
using DlrDataApp.Modules.SharedModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.ProfilingSharedModule
{
    static class Helpers
    {
        /// <summary>
        /// Performs a lookup for the english translation for a given translationKey. Used by profilings.
        /// </summary>
        /// <param name="translations">Dictionary containing translations for keys</param>
        /// <param name="translationKey">Key to lookup</param>
        /// <returns>Translation or the string "translation missing" if there is no english translation</returns>
        public static string GetEnglishTranslation(Dictionary<string, string> translations, string translationKey)
        {
            const string englishLanguageExtension = "English";
            translationKey = translationKey + englishLanguageExtension;
            if (!translations.TryGetValue(translationKey, out string translation))
            {
                translation = string.Empty;
            }
            return translation;
        }

        /// <summary>
        /// Performs a lookup for the system languages translation for a given translationKey. 
        /// Falls back to <see cref="GetEnglishTranslation"/> if there is no translation in the current language. Used by profilings.
        /// </summary>
        /// <param name="translations">Dictionary containing translations for keys</param>
        /// <param name="translationKey">Key to lookup</param>
        /// <returns>Translation or the string "translation missing" if there is neither a translation in the current language nor in english</returns>
        public static string GetCurrentLanguageTranslation(Dictionary<string, string> translations, string translationKey)
        {
            string currentLanguageExtension = CultureInfo.CurrentUICulture.EnglishName;
            int firstSpaceInCurrentLanguageExtension = currentLanguageExtension.IndexOf(' ');
            if (firstSpaceInCurrentLanguageExtension != -1)
            {
                currentLanguageExtension = currentLanguageExtension.Substring(0, firstSpaceInCurrentLanguageExtension);
            }
            var currentLanguageTranslationKey = translationKey + currentLanguageExtension;
            if (!translations.TryGetValue(currentLanguageTranslationKey, out string translation))
            {
                translation = GetEnglishTranslation(translations, translationKey);
            }
            return translation;
        }

        public static bool SetCurrentProfiling(this Database db, ProfilingData profiling)
        {
            var previousProfilingInfo = db.ReadWithChildren<ActiveProfilingInfo>(true).FirstOrDefault() ?? new ActiveProfilingInfo();
            previousProfilingInfo.ActiveProfiling = profiling;
            return db.InsertOrUpdateWithChildren(previousProfilingInfo, true);
        }
    }
}
