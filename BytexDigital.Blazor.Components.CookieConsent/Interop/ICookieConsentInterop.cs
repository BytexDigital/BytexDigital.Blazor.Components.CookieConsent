using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.Blazor.Components.CookieConsent.Interop
{
    public interface ICookieConsentInterop
    {
        Task RegisterBroadcastReceiverAsync<T>(DotNetObjectReference<T> dotNetObjectReference, bool isCallerWasm) where T : class;

        Task BroadcastEventAsync(bool isDirectedTowardsWasm, string name, string data);

        Task<string> ReadLoadedScriptsAsync(CancellationToken cancellationToken = default);

        Task<string> ReadCookieAsync(string cookieName);

        Task SetCookieAsync(string cookieString);

        Task ApplyPreferencesAsync(string[] allowedCategories, string[] allowedServices);
    }
}