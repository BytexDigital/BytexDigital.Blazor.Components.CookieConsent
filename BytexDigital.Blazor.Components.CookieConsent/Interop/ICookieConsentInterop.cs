using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.Blazor.Components.CookieConsent.Interop
{
    public interface ICookieConsentInterop
    {
        Task RegisterBroadcastReceiverAsync<T>(DotNetObjectReference<T> dotNetObjectReference, bool isOsPlatform) where T : class;

        Task BroadcastEventAsync(bool isOsPlatform, string name, string data);

        Task<string> ReadLoadedScriptsAsync(CancellationToken cancellationToken = default);

        Task<string> ReadCookiesAsync(string cookieName);

        Task SetCookieAsync(string cookieString);

        Task ApplyPreferencesAsync(string[] allowedCategories, string[] allowedServices);
    }
}