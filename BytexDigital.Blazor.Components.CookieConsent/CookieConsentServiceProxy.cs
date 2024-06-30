using BytexDigital.Blazor.Components.CookieConsent.Broadcasting;
using BytexDigital.Blazor.Components.CookieConsent.Interop;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentServiceProxy : CookieConsentService
    {
        public CookieConsentServiceProxy(
            IOptions<CookieConsentOptions> options,
            ICookieConsentInterop cookieConsentInterop,
            CookieConsentEventHandler eventHandler,
            CookieConsentRuntimeContext runtimeContext,
            ILogger<CookieConsentService> logger) : base(options, cookieConsentInterop, eventHandler, runtimeContext, logger)
        {
        }
    }
}