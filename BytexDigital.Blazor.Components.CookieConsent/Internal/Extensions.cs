using System;
using System.Collections.Generic;
using System.Linq;

namespace BytexDigital.Blazor.Components.CookieConsent.Internal
{
    public static class Extensions
    {
        public static string GetLocalization(this Dictionary<string, string> dictionary, string currentCode, string defaultCode)
        {
            if (dictionary.ContainsKey(currentCode)) return dictionary[currentCode];
            if (dictionary.ContainsKey(defaultCode)) return dictionary[defaultCode];
            if (dictionary.Count > 0) return dictionary[dictionary.Keys.ToArray().First()];

            throw new Exception($"Could not localize cookie consent text for language '{currentCode}' because no localization are present.");
        }
    }
}
