using BytexDigital.Blazor.Components.CookieConsent.Broadcasting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentServiceProxy : CookieConsentService
    {
        public CookieConsentServiceProxy(
            IOptions<CookieConsentOptions> options,
            IJSRuntime jsRuntime,
            CookieConsentEventHandler eventHandler,
            CookieConsentRuntimeContext runtimeContext,
            ILogger<CookieConsentService> logger) : base(options, jsRuntime, eventHandler, runtimeContext, logger)
        {
        }
    }
}