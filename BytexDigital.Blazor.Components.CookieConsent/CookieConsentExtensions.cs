using BytexDigital.Blazor.Components.CookieConsent;

using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CookieConsentExtensions
    {
        public static IServiceCollection AddCookieConsent(this IServiceCollection services, Action<CookieConsentOptions> configure = default)
        {
            services.AddScoped<CookieConsentService>();
            
            if (configure != null) services.Configure(configure);

            return services;
        }

        public static void UseDefaultConsentPrompt(
            this CookieConsentOptions options,
            Action<CookieConsentDefaultPromptVariant> promptOptions = default)
        {
            var variant = new CookieConsentDefaultPromptVariant();
            options.ConsentPromptVariant = variant;

            promptOptions?.Invoke(variant);
        }
    }
}
