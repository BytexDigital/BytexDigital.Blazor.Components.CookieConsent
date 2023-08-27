using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Options;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentLocalizer
    {
        private readonly IOptions<CookieConsentOptions> _options;

        public CookieConsentLocalizer(IOptions<CookieConsentOptions> options)
        {
            _options = options;
        }

        public string GetLocalization(IDictionary<string, string> dictionary, string currentCode, string defaultCode)
        {
            if (dictionary.TryGetValue(currentCode, out var localization)) return localization;
            if (dictionary.TryGetValue(defaultCode, out var localization1)) return localization1;
            if (dictionary.Count > 0) return dictionary[dictionary.Keys.ToArray().First()];

            throw new Exception(
                $"Could not localize cookie consent text for language '{currentCode}' because no localization are present.");
        }

        public string GetLocalization(IDictionary<string, string> dictionary)
        {
            return GetLocalization(dictionary,
                CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                _options.Value.DefaultLocalizationCode);
        }
    }
}