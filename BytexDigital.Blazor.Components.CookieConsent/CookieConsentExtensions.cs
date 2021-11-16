using BytexDigital.Blazor.Components.CookieConsent;

using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CookieConsentExtensions
    {
        public static IServiceCollection AddCookieConsent(this IServiceCollection services, Action<CookieConsentOptions> configure)
        {
            services.AddScoped<CookieConsentService>();
            services.Configure<CookieConsentOptions>(configure);

            return services;
        }
    }
}
