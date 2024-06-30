using System;
using System.Diagnostics.CodeAnalysis;

namespace BytexDigital.Blazor.Components.CookieConsent.Dialogs.Settings.Default
{
    public class CookieConsentDefaultSettingsModalVariant : CookieConsentPreferencesVariantBase
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public override Type ComponentType { get; set; } = typeof(CookieConsentDefaultSettingsModal);
    }
}