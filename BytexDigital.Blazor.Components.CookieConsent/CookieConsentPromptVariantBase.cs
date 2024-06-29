using System;
using System.Diagnostics.CodeAnalysis;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public abstract class CookieConsentPromptVariantBase
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public abstract Type ComponentType { get; set; }
    }
}