using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BytexDigital.Blazor.Components.CookieConsent.Interop
{
    internal class CookieConsentInterop : ICookieConsentInterop
    {
        protected const string JsInteropRegisterReceiver = "CookieConsent.RegisterBroadcastReceiver";
        protected const string JsInteropBroadcast = "CookieConsent.BroadcastEvent";
        protected const string JsInteropReadLoadedScripts = "CookieConsent.ReadLoadedScripts";
        protected const string JsInteropReadCookie = "CookieConsent.ReadCookie";
        protected const string JsInteropSetCookie = "CookieConsent.SetCookie";
        protected const string JsInteropApplyPreferences = "CookieConsent.ApplyPreferences";

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

        public async Task RegisterBroadcastReceiverAsync<T>(DotNetObjectReference<T> dotNetObjectReference, bool isOsPlatform) where T : class
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                await module.InvokeVoidAsync(JsInteropRegisterReceiver,
                    dotNetObjectReference,
                    isOsPlatform);
            }
            else
            {

                await _jsRuntime.InvokeVoidAsync(JsInteropRegisterReceiver,
                    dotNetObjectReference,
                    isOsPlatform);
            }
        }

        public async Task BroadcastEventAsync(bool isOsPlatform, string name, string data)
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                await module.InvokeVoidAsync(JsInteropBroadcast,
                    !isOsPlatform, // Directed towards WASM?
                    name, // Event name
                    data); // Event data
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync(JsInteropBroadcast,
                    !isOsPlatform, // Directed towards WASM?
                    name, // Event name
                    data); // Event data
            }
        }

        public async Task<string> ReadLoadedScriptsAsync(CancellationToken cancellationToken)
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                var activatedScriptsJson = await module.InvokeAsync<string>(
                    JsInteropReadLoadedScripts,
                    cancellationToken);

                return activatedScriptsJson;
            }
            else
            {
                var activatedScriptsJson = await _jsRuntime.InvokeAsync<string>(
                    JsInteropReadLoadedScripts,
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
                    JsInteropReadCookie, cookieName);

                return cookieValue;
            }
            else
            {
                var cookieValue = await _jsRuntime.InvokeAsync<string>(
                    JsInteropReadCookie, cookieName);

                return cookieValue;
            }
        }

        public async Task SetCookieAsync(string cookieString)
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                await module.InvokeVoidAsync(
                    JsInteropSetCookie, cookieString);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync(
                    JsInteropSetCookie, cookieString);
            }
        }

        public async Task ApplyPreferencesAsync(string[] allowedCategories, string[] allowedServices)
        {
            if (_options.Value.ImportJsAutomatically)
            {
                var module = await Module;

                await module.InvokeVoidAsync(
                    JsInteropApplyPreferences,
                    allowedCategories,
                    allowedServices);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync(
                    JsInteropApplyPreferences,
                    allowedCategories,
                    allowedServices);
            }
        }
    }
}
