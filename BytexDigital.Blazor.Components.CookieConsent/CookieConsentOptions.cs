using System;
using System.Collections.Generic;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentOptions
    {
        /// <summary>
        /// Revision of the cookie policy the user agrees to. If the user's agreed to policy number is any different than the currently configured, the prompt for consent will appear again.
        /// </summary>
        public int Revision { get; set; } = 1;

        /// <summary>
        /// Automatically runs the consent check once the CookieConsentHandler component was rendered. This will also activate any scripts inside the DOM. If user consent is required, the consent modal will appear.
        /// <para>If you disable this option, you must manually call <see cref="CookieConsentService.NotifyPageLoadedAsync"/> once you wish to run the checks.</para>
        /// </summary>
        public bool AutomaticallyShow { get; set; } = true;

        /// <summary>
        /// The default language code the components will look for when localization if the currently configured language does not havea localized message.
        /// </summary>
        public string DefaultLocalizationCode { get; set; } = "en";

        /// <summary>
        /// URL to your cookie policy. Will always be opened in a new tab.
        /// </summary>
        public string PolicyUrl { get; set; } = "#";

        /// <summary>
        /// Cookie specific options.
        /// </summary>
        public CookieConsentCookieOptions CookieOptions { get; set; } = new CookieConsentCookieOptions();

        /// <summary>
        /// Behavior settings specific to <see cref="CookieConsentCheck"/>.
        /// </summary>
        public CookieConsentCheckOptions CheckOptions { get; set; } = new CookieConsentCheckOptions();

        /// <summary>
        /// Positioning of the consent modal.
        /// </summary>
        public ConsentModalPosition ConsentModalPosition { get; set; } = ConsentModalPosition.BottomCenter;

        /// <summary>
        /// Layout of the consent modal.
        /// </summary>
        public ConsentModalLayout ConsentModalLayout { get; set; } = ConsentModalLayout.Cloud;


        /// <summary>
        /// Title text of the consent modal.
        /// </summary>
        public Dictionary<string, string> ConsentTitleText { get; set; } = new()
        {
            ["en"] = "We use cookies",
            ["de"] = "Wir verwenden Cookies"
        };

        /// <summary>
        /// Description text of the consent modal.
        /// </summary>
        public Dictionary<string, string> ConsentDescriptionText { get; set; } = new()
        {
            ["en"] = "This website uses cookies to improve your experience.",
            ["de"] = "Diese Webseite verwendet Cookies, um Ihre Erfahrung zu verbessern."
        };

        /// <summary>
        /// "Accept all" text. Used in multiple places.
        /// </summary>
        public Dictionary<string, string> ConsentAcceptAllText { get; set; } = new()
        {
            ["en"] = "Accept all",
            ["de"] = "Alle akzeptieren"
        };

        /// <summary>
        /// "I understand!" text. Used, if there are no preferences to configure since there is only REQUIRED cookie categories.
        /// </summary>
        public Dictionary<string, string> ConsentAcknowledgeText { get; set; } = new()
        {
            ["en"] = "I understand!",
            ["de"] = "Alles klar!"
        };

        /// <summary>
        /// "Decline" or "Necessary only" text. Indicates that the user will only agree to the minimum amount of cookies necessary.
        /// </summary>
        public Dictionary<string, string> ConsentNecessaryOnlyText { get; set; } = new()
        {
            ["en"] = "Decline",
            ["de"] = "Ablehnen"
        };

        /// <summary>
        /// "Managep preferences" text. Used in multiple places.
        /// </summary>
        public Dictionary<string, string> ConsentOpenPreferencesText { get; set; } = new()
        {
            ["en"] = "Manage preferences",
            ["de"] = "Präferenzen ändern"
        };

        /// <summary>
        /// If there are preferences to manage, a second button will appear inside the consent modal. If this setting is true, then the secondary button will show "Manage preferences". If false, it will display "Decline"/"Necessary only".
        /// </summary>
        public bool ConsentSecondaryActionOpensSettings { get; set; } = false;

        /// <summary>
        /// Title text of the settings modal.
        /// </summary>
        public Dictionary<string, string> SettingsTitleText { get; set; } = new()
        {
            ["en"] = "Your cookie preferences",
            ["de"] = "Ihre Cookie Präferenzen"
        };

        /// <summary>
        /// Description text of the settings modal.
        /// </summary>
        public Dictionary<string, string> SettingsDescriptionText { get; set; } = new()
        {
            ["en"] = "We use cookies to ensure basic functionality of the website and to enhance your online experience. For each category, you may choose to opt-in/out whenever you want.",
            ["de"] = "Wir nutzen Cookies für grundlegende Funktionalitäten unserer Webseite und zum Verbessern Ihrer Nutzererfahrung. Für jede Kategorie können Sie individuell Ihre Zustimmung erteilen."
        };

        /// <summary>
        /// "Save preferences" text inside the settings modal.
        /// </summary>
        public Dictionary<string, string> SettingsContinueWithSelectedPreferencesText { get; set; } = new()
        {
            ["en"] = "Save preferences",
            ["de"] = "Präferenzen anwenden"
        };

        /// <summary>
        /// Header line "Used services" to further detail which services are affected by a specific category.
        /// </summary>
        public Dictionary<string, string> SettingsUsedServicesText { get; set; } = new()
        {
            ["en"] = "Used services",
            ["de"] = "Verwendete Dienste"
        };

        /// <summary>
        /// "Show cookie policy" text. Used in multiple places.
        /// </summary>
        public Dictionary<string, string> ShowPolicyText { get; set; } = new()
        {
            ["en"] = "Show cookie policy",
            ["de"] = "Cookie-Richtline anzeigen"
        };

        /// <summary>
        /// Available cookie categories. This by default only contains a "necessary" category.
        /// </summary>
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
        /// <summary>
        /// Text that describes why the content cannot be displayed.
        /// </summary>
        public Dictionary<string, string> Text { get; set; } = new()
        {
            ["en"] = "This content uses cookies that are disallowed by your settings. To show it, we need to adjust your cookie settings for our website. We require your consent for:",
            ["de"] = "Dieser Inhalt nutzt Cookies, die aktuell nicht erlaubt sind. Um ihn anzuzeigen, müssen Sie Ihre Cookie-Präferenzen auf unserer Webseite aktualisieren. Wir benötigen Ihre Zustimmung für:"
        };

        /// <summary>
        /// "Accept" text.
        /// </summary>
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
        /// <summary>
        /// Identifier of the category. This identifier gets references in multiple places throughout the library. Can be any custom value.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Title of the category.
        /// </summary>
        public Dictionary<string, string> TitleText { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Description of the category.
        /// </summary>
        public Dictionary<string, string> DescriptionText { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// List of services that are included in this category. Not mandatory.
        /// </summary>
        public List<CookieCategoryService> Services { get; set; } = new List<CookieCategoryService>();

        /// <summary>
        /// If true, this category is mandatory and cannot be disabled by the user.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// If true, this category will be preselected as enabled when the user opens the preferences and has not allowed/disallowed this category manually.
        /// </summary>
        public bool IsPreselected { get; set; }

        /// <summary>
        /// Predefined category identifier for the default necessary cookie category.
        /// </summary>
        public const string NecessaryCategoryIdentifier = "necessary";
    }

    public class CookieCategoryService
    {
        /// <summary>
        /// Identifier of the service. Can be any custom value.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// URL to this service's privacy policy.
        /// </summary>
        public string PolicyUrl { get; set; }

        /// <summary>
        /// Title of the service.
        /// </summary>
        public Dictionary<string, string> TitleText { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Text on the "Show policy" button next to the service name.
        /// </summary>
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
