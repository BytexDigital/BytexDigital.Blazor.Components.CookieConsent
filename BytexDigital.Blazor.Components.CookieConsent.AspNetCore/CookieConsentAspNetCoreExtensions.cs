using BytexDigital.Blazor.Components.CookieConsent.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CookieConsentAspNetCoreExtensions
    {
        public static IServiceCollection AddCookieConsentHttpContextServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextCookieConsent>();

            return services;
        }
    }
}