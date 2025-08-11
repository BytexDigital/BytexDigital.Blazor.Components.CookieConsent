using System;
using System.Diagnostics.CodeAnalysis;

namespace BytexDigital.Blazor.Components.CookieConsent.Dialogs.Settings.Default
{
    public class CookieConsentDefaultSettingsModalVariant : CookieConsentPreferencesVariantBase
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public override Type ComponentType { get; set; } = typeof(CookieConsentDefaultSettingsModal);

        /// <summary>
        ///     Theme configuration for this component. Supports automatic detection of user's system preference,
        ///     or manual selection of light or dark theme. Overwrites the same setting in <see cref="CookieConsentOptions.Theme"/>.
        /// </summary>
        public ThemeOptions? Theme { get; set; } = null;
    }
}