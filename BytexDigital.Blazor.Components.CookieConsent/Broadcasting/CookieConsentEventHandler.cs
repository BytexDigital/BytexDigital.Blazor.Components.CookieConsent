using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using BytexDigital.Blazor.Components.CookieConsent.Interop;
using Microsoft.JSInterop;

namespace BytexDigital.Blazor.Components.CookieConsent.Broadcasting
{
    public class CookieConsentEventHandler
    {
        protected const string JsBroadcastEventCookiePreferencesChanged =
            nameof(JsBroadcastEventCookiePreferencesChanged);

        protected const string JsBroadcastEventShowConsentModalRequested =
            nameof(JsBroadcastEventShowConsentModalRequested);

        protected const string JsBroadcastEventShowPreferencesModalRequested =
            nameof(JsBroadcastEventShowPreferencesModalRequested);

        protected const string JsBroadcastEventScriptLoaded =
            nameof(JsBroadcastEventScriptLoaded);

        protected readonly ICookieConsentInterop _cookieConsentInterop;
        protected readonly CookieConsentRuntimeContext _runtimeContext;

        [DynamicDependency(nameof(OnReceivedBroadcastAsync))]
        public CookieConsentEventHandler(CookieConsentRuntimeContext runtimeContext, ICookieConsentInterop cookieConsentInterop)
        {
            _runtimeContext = runtimeContext;
            _cookieConsentInterop = cookieConsentInterop;
        }

        public event EventHandler<CookiePreferences> CookiePreferencesChanged;
        public event EventHandler<EventArgs> ShowConsentModalRequested;
        public event EventHandler<EventArgs> ShowPreferencesModalRequested;
        public event EventHandler<CookieConsentScriptLoadedArgs> ScriptLoaded;

        public async Task InitializeAsync()
        {
            try
            {
                await _cookieConsentInterop.RegisterBroadcastReceiverAsync(
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

                JsBroadcastEventScriptLoaded => Task.Run(()
                    => ScriptLoaded?.Invoke(this,
                        JsonSerializer.Deserialize<CookieConsentScriptLoadedArgs>(data))),

                _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Event name not known.")
            };

            return Task.CompletedTask;
        }

        protected async Task PublishToJsAsync(string name, string data)
        {
            await _cookieConsentInterop.BroadcastEventAsync(
                !RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")), // Directed towards WASM?
                name, // Event name
                data); // Event data
        }
    }
}