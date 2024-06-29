using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.Blazor.Components.CookieConsent.Interop
{
    public interface ICookieConsentInterop
    {
        Task<string> ReadLoadedScriptsAsync(CancellationToken cancellationToken = default);

        Task<string> ReadCookiesAsync(string cookieName);

        Task SetCookieAsync(string cookieString);

        Task ApplyPreferencesAsync(string[] allowedCategories, string[] allowedServices);
    }
}