using System;
using System.Threading.Tasks;
using BytexDigital.Blazor.Components.CookieConsent.Broadcasting;
using BytexDigital.Blazor.Components.CookieConsent.Interop;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentServiceAuthority : CookieConsentService
    {
        public CookieConsentServiceAuthority(
            IOptions<CookieConsentOptions> options,
            ICookieConsentInterop cookieConsentInterop,
            CookieConsentEventHandler eventHandler,
            CookieConsentRuntimeContext runtimeContext,
            ILogger<CookieConsentService> logger) : base(options, cookieConsentInterop, eventHandler, runtimeContext, logger)
        {
        }

        public override async Task NotifyApplicationLoadedAsync()
        {
            var preferences = await GetPreferencesAsync();

            try
            {
                if (await IsCurrentRevisionAcceptedAsync())
                {
                    await _cookieConsentInterop.ApplyPreferencesAsync(
                        preferences.AllowedCategories,
                        preferences.AllowedServices);
                }
            }
            catch (JSException ex)
            {
                // Ignore exceptions due to blocked or unavailable JS.
                // In this case, we aren't able to activate JS tags, but there seems to be nothing
                // we can do in this situation!
                _logger.LogTrace(ex, "Exception raised attempting to call into JavaScript");
            }

            try
            {
                await PublishCookiePreferencesChanged(preferences);
            }
            catch (Exception ex)
            {
                // Ignore most likely user caused exception and log it as we don't want to interrupt program flow.
                _logger.LogError(ex, "Exception raised trying to run CookiePreferencesChanged event handler");
            }
        }
    }
}