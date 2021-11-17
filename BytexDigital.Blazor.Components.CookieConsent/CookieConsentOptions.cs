using System;
using System.Collections.Generic;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentOptions
    {
        public int Revision { get; set; } = 1;

        public bool AutomaticallyShow { get; set; } = true;

        public string DefaultLocalizationCode { get; set; } = "en";

        public CookieConsentCookieOptions CookieOptions { get; set; } = new CookieConsentCookieOptions();

        public CookieConsentCheckOptions CheckOptions { get; set; } = new CookieConsentCheckOptions();

        public ConsentModalPosition ConsentModalPosition { get; set; } = ConsentModalPosition.BottomCenter;

        public ConsentModalLayout ConsentModalLayout { get; set; } = ConsentModalLayout.Cloud;

        public Dictionary<string, string> ConsentTitleText { get; set; } = new()
        {
            ["en"] = "We use cookies",
            ["de"] = "Wir verwenden Cookies"
        };

        public Dictionary<string, string> ConsentDescriptionText { get; set; } = new()
        {
            ["en"] = "This website uses cookies to improve your experience.",
            ["de"] = "Diese Webseite verwendet Cookies, um Ihre Erfahrung zu verbessern."
        };

        public Dictionary<string, string> ConsentAcceptAllText { get; set; } = new()
        {
            ["en"] = "Accept all",
            ["de"] = "Alle akzeptieren"
        };

        public Dictionary<string, string> ConsentAcknowledgeText { get; set; } = new()
        {
            ["en"] = "I understand!",
            ["de"] = "Alles klar!"
        };

        public Dictionary<string, string> ConsentNecessaryOnlyText { get; set; } = new()
        {
            ["en"] = "Decline",
            ["de"] = "Ablehnen"
        };

        public Dictionary<string, string> ConsentOpenPreferencesText { get; set; } = new()
        {
            ["en"] = "Manage preferences",
            ["de"] = "Präferenzen ändern"
        };

        public bool ConsentSecondaryActionOpensSettings { get; set; } = false;

        public Dictionary<string, string> SettingsTitleText { get; set; } = new()
        {
            ["en"] = "Your cookie preferences",
            ["de"] = "Ihre Cookie Präferenzen"
        };

        public Dictionary<string, string> SettingsDescriptionText { get; set; } = new()
        {
            ["en"] = "We use cookies to ensure basic functionality of the website and to enhance your online experience. For each category, you may choose to opt-in/out whenever you want.",
            ["de"] = "Wir nutzen Cookies für grundlegende Funktionalitäten unserer Webseite und zum Verbessern Ihrer Nutzererfahrung. Für jede Kategorie können Sie individuell Ihre Zustimmung erteilen."
        };

        public Dictionary<string, string> SettingsContinueWithSelectedPreferencesText { get; set; } = new()
        {
            ["en"] = "Save preferences",
            ["de"] = "Präferenzen anwenden"
        };

        public Dictionary<string, string> SettingsUsedServicesText { get; set; } = new()
        {
            ["en"] = "Used services",
            ["de"] = "Verwendete Dienste"
        };

        public string PolicyUrl { get; set; } = "#";

        public Dictionary<string, string> ShowPolicyText { get; set; } = new()
        {
            ["en"] = "Show cookie policy",
            ["de"] = "Cookie-Richtline anzeigen"
        };

        public List<CookieCategory> Categories { get; set; } = new()
        {
            new CookieCategory()
            {
                Identifier = CookieCategory.NecessaryCategoryIdentifier,
                TitleText = new()
                {
                    ["en"] = "Strictly necessary cookies",
                    ["de"] = "Umbedingt notwendige Cookies"
                },
                DescriptionText = new()
                {
                    ["en"] = "These cookies are essential for the proper functioning of this website. They do not contain personal data and are not used to track you.",
                    ["de"] = "Diese Cookies sind umbedingt notwendig für die Nutzung dieser Webseite. Sie enthalten keine personenbezogenen Daten und werden nicht für Tracking verwendet."
                },
                IsRequired = true
            }
        };
    }

    public class CookieConsentCheckOptions {
        public Dictionary<string, string> Text { get; set; } = new()
        {
            ["en"] = "This content uses cookies that are disallowed by your settings. To show it, we need to adjust your cookie settings for our website. We require your consent for:",
            ["de"] = "Dieser Inhalt nutzt Cookies, die aktuell nicht erlaubt sind. Um ihn anzuzeigen, müssen Sie Ihre Cookie-Präferenzen auf unserer Webseite aktualisieren. Wir benötigen Ihre Zustimmung für:"
        };

        public Dictionary<string, string> AcceptText { get; set; } = new()
        {
            ["en"] = "Accept",
            ["de"] = "Akzeptieren"
        };
    }

    public class CookieConsentCookieOptions
    {
        public string CookieName { get; set; } = ".AspNet.CookieConsent";
        public string CookieDomain { get; set; }
        public bool CookieHttpOnly { get; set; } = false;
        public bool CookieIsEssential { get; set; } = true;
        public TimeSpan? CookieMaxAge { get; set; } = TimeSpan.FromDays(180);
        public DateTime? CookieExpires { get; set; }
        public string CookiePath { get; set; } = "/";
        public string CookieSameSite { get; set; } = "Lax";
        public bool CookieSecure { get; set; } = true;
    }

    public class CookieCategory
    {
        public string Identifier { get; set; }
        public Dictionary<string, string> TitleText { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionText { get; set; } = new Dictionary<string, string>();
        public List<CookieCategoryService> Services { get; set; } = new List<CookieCategoryService>();
        public bool IsRequired { get; set; }
        public bool IsPreselected { get; set; }

        public const string NecessaryCategoryIdentifier = "necessary";
    }

    public class CookieCategoryService
    {
        public string Identifier { get; set; }
        public string PolicyUrl { get; set; }
        public Dictionary<string, string> TitleText { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ShowPolicyText { get; set; } = new Dictionary<string, string>();

    }

    public enum ConsentModalPosition
    {
        TopLeft, TopCenter, TopRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public enum ConsentModalLayout
    {
        Bar, Cloud
    }
}
