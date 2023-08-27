using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BytexDigital.Blazor.Components.CookieConsent.Broadcasting
{
    public class CookieConsentEventHandler
    {
        protected const string JsInteropRegisterReceiver = "CookieConsent.RegisterBroadcastReceiver";
        protected const string JsInteropBroadcast = "CookieConsent.BroadcastEvent";

        protected const string JsBroadcastEventCookiePreferencesChanged =
            nameof(JsBroadcastEventCookiePreferencesChanged);

        protected const string JsBroadcastEventShowConsentModalRequested =
            nameof(JsBroadcastEventShowConsentModalRequested);

        protected const string JsBroadcastEventShowPreferencesModalRequested =
            nameof(JsBroadcastEventShowPreferencesModalRequested);

        protected readonly Lazy<Task<IJSObjectReference>> _importModule;
        protected readonly IJSRuntime _jsRuntime;
        protected readonly CookieConsentRuntimeContext _runtimeContext;

        public CookieConsentEventHandler(CookieConsentRuntimeContext runtimeContext, IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _runtimeContext = runtimeContext;
            _importModule = new Lazy<Task<IJSObjectReference>>(() => _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    new object[] { "./_content/BytexDigital.Blazor.Components.CookieConsent/cookieconsent.js" })
                .AsTask());
        }

        public event EventHandler<CookiePreferences> CookiePreferencesChanged;
        public event EventHandler<EventArgs> ShowConsentModalRequested;
        public event EventHandler<EventArgs> ShowPreferencesModalRequested;

        public async Task InitializeAsync()
        {
            try
            {
                var module = await _importModule.Value;

                await module.InvokeVoidAsync(JsInteropRegisterReceiver,
                    DotNetObjectReference.Create(this),
                    RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")));
            }
            catch
            {
                // ignore, no JS available. Probably in prerender.   
            }
        }

        public async Task BroadcastCookiePreferencesChangedAsync(CookiePreferences cookiePreferences)
        {
            _ = Task.Run(() => CookiePreferencesChanged?.Invoke(this, cookiePreferences));

            await PublishToJsAsync(JsBroadcastEventCookiePreferencesChanged,
                JsonSerializer.Serialize(cookiePreferences));
        }

        public async Task BroadcastShowConsentModalRequestedAsync()
        {
            if (_runtimeContext.RendersUserInterface)
            {
                _ = Task.Run(() => ShowConsentModalRequested?.Invoke(this, EventArgs.Empty));
            }
            else
            {
                await PublishToJsAsync(JsBroadcastEventShowConsentModalRequested, string.Empty);
            }
        }

        public async Task BroadcastShowPreferencesModalRequestedAsync()
        {
            if (_runtimeContext.RendersUserInterface)
            {
                _ = Task.Run(() => ShowPreferencesModalRequested?.Invoke(this, EventArgs.Empty));
            }
            else
            {
                await PublishToJsAsync(JsBroadcastEventShowPreferencesModalRequested, string.Empty);
            }
        }

        [JSInvokable("OnReceivedBroadcastAsync")]
        public Task OnReceivedBroadcastAsync(string name, string data)
        {
            _ = name switch
            {
                JsBroadcastEventCookiePreferencesChanged => Task.Run(()
                    => CookiePreferencesChanged?.Invoke(this, JsonSerializer.Deserialize<CookiePreferences>(data))),
                
                JsBroadcastEventShowConsentModalRequested => Task.Run(()
                    => ShowConsentModalRequested?.Invoke(this, EventArgs.Empty)),
                
                JsBroadcastEventShowPreferencesModalRequested => Task.Run(()
                    => ShowPreferencesModalRequested?.Invoke(this, EventArgs.Empty)),

                _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Event name not known.")
            };

            return Task.CompletedTask;
        }

        protected async Task PublishToJsAsync(string name, string data)
        {
            var module = await _importModule.Value;

            await module.InvokeVoidAsync(JsInteropBroadcast,
                !RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")), // Directed towards WASM?
                name, // Event name
                data); // Event data
        }
    }
}