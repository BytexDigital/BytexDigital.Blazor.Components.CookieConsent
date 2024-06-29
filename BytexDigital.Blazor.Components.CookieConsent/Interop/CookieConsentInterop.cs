using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BytexDigital.Blazor.Components.CookieConsent.Interop
{
    internal class CookieConsentInterop : ICookieConsentInterop
    {
        private readonly IJSRuntime _jsRuntime;
        private Task<IJSObjectReference> _module;
        private readonly IOptions<CookieConsentOptions> _options;

        private Task<IJSObjectReference> Module => _module ??= _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                new object[] { "./_content/BytexDigital.Blazor.Components.CookieConsent/cookieconsent.js" })
            .AsTask();

        public CookieConsentInterop(
            IOptions<CookieConsentOptions> options,
            IJSRuntime jsRuntime)
        {
            _options = options;
            _jsRuntime = jsRuntime;
        }

        public async Task<string> ReadLoadedScriptsAsync(CancellationToken cancellationToken)
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                var activatedScriptsJson = await module.InvokeAsync<string>(
                    "CookieConsent.ReadLoadedScripts",
                    cancellationToken);

                return activatedScriptsJson;
            }
            else
            {
                var activatedScriptsJson = await _jsRuntime.InvokeAsync<string>(
                    "CookieConsent.ReadLoadedScripts",
                    cancellationToken);

                return activatedScriptsJson;
            }
        }
        public async Task<string> ReadCookiesAsync(string cookieName)
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                var cookieValue = await module.InvokeAsync<string>(
                    "CookieConsent.ReadCookie", cookieName);

                return cookieValue;
            }
            else
            {
                var cookieValue = await _jsRuntime.InvokeAsync<string>(
                    "CookieConsent.ReadCookie", cookieName);

                return cookieValue; ;
            }
        }

        public async Task SetCookieAsync(string cookieString)
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                await module.InvokeVoidAsync(
                    "CookieConsent.SetCookie", cookieString);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync(
                    "CookieConsent.SetCookie", cookieString);
            }
        }

        public async Task ApplyPreferencesAsync(string[] allowedCategories, string[] allowedServices)
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                await module.InvokeVoidAsync(
                    "CookieConsent.ApplyPreferences",
                    allowedCategories,
                    allowedServices);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync(
                    "CookieConsent.ApplyPreferences",
                    allowedCategories,
                    allowedServices);
            }
        }
    }
}
