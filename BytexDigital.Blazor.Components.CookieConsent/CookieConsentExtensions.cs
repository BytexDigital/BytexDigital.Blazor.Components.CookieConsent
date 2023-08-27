using BytexDigital.Blazor.Components.CookieConsent;
using BytexDigital.Blazor.Components.CookieConsent.Broadcasting;

using System;
using BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompt.Default;
using BytexDigital.Blazor.Components.CookieConsent.Dialogs.Settings.Default;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CookieConsentExtensions
    {
        public static IServiceCollection AddCookieConsent(this IServiceCollection services, Action<CookieConsentOptions> configure = default, bool withUserInterface = true)
        {
            if (configure != null) services.Configure(configure);

            var runtimeContext = new CookieConsentRuntimeContext
            {
                RendersUserInterface = withUserInterface
            };

            services.AddScoped(_ => runtimeContext);

            if (withUserInterface)
            {
                services.AddScoped<CookieConsentService, CookieConsentServiceAuthority>();
            }
            else
            {
                services.AddScoped<CookieConsentService, CookieConsentServiceProxy>();
            }

            services.AddScoped<CookieConsentEventHandler>();
            services.AddScoped<CookieConsentLocalizer>();

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
        
        public static void UseDefaultSettingsModal(
            this CookieConsentOptions options,
            Action<CookieConsentDefaultSettingsModalVariant> modalOptions = default)
        {
            var variant = new CookieConsentDefaultSettingsModalVariant();
            options.SettingsModalVariant = variant;

            modalOptions?.Invoke(variant);
        }
    }
}
