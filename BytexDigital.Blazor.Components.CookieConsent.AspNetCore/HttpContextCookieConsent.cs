using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace BytexDigital.Blazor.Components.CookieConsent.AspNetCore
{
    public class HttpContextCookieConsent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCookieConsent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CookiePreferences GetCookieConsentPreferences(
            string cookieName = ".AspNet.CookieConsent",
            CancellationToken cancellationToken = default)
        {
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieName, out var value);

            if (string.IsNullOrEmpty(value))
            {
                return new CookiePreferences();
            }

            var preferences = value.StartsWith("{")
                ? JsonSerializer.Deserialize<CookiePreferences>(value)
                : JsonSerializer.Deserialize<CookiePreferences>(
                    Encoding.UTF8.GetString(Convert.FromBase64String(value)));

            return preferences;
        }
    }
}