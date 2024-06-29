using System;
using System.Diagnostics.CodeAnalysis;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public abstract class CookieConsentPreferencesVariantBase
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public abstract Type ComponentType { get; set; }
    }
}