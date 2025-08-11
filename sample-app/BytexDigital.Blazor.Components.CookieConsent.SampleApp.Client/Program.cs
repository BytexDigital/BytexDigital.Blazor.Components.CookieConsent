using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BytexDigital.Blazor.Components.CookieConsent.SampleApp.Client;

public static class Program
{
    public static readonly Action<CookieConsentOptions> CookieConsentConfigurationAction = (CookieConsentOptions o) =>
    {
        o.Revision = 1;
        o.PolicyUrl = "/cookie-policy";
    
        // Call optional
        o.UseDefaultConsentPrompt(prompt =>
        {
            prompt.Position = ConsentModalPosition.BottomRight;
            prompt.Layout = ConsentModalLayout.Bar;
            prompt.SecondaryActionOpensSettings = false;
            prompt.AcceptAllButtonDisplaysFirst = false;
            
            // Set this property to overwrite the default setting.
            //prompt.Theme = ThemeOptions.Light;
        });

        o.UseDefaultSettingsModal(modal =>
        {
            // Set this property to overwrite the default setting.
            //modal.Theme = ThemeOptions.Dark;
        });
    
        o.Categories.Add(new CookieCategory
        {
            TitleText = new()
            {
                ["en"] = "Google Services",
                ["de"] = "Google Dienste"
            },
            DescriptionText = new()
            {
                ["en"] = "Allows the integration and usage of Google services.",
                ["de"] = "Erlaubt die Verwendung von Google Diensten."
            },
            Identifier = "google",
            IsPreselected = true,

            Services = new()
            {
                new CookieCategoryService
                {
                    Identifier = "google-analytics",
                    PolicyUrl = "https://policies.google.com/privacy",
                    TitleText = new()
                    {
                        ["en"] = "Google Analytics",
                        ["de"] = "Google Analytics"
                    },
                    ShowPolicyText = new()
                    {
                        ["en"] = "Display policies",
                        ["de"] = "Richtlinien anzeigen"
                    }
                }
            }
        });
    };

    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        

        builder.Services.AddCookieConsent(CookieConsentConfigurationAction);

        await builder.Build().RunAsync();
    }
}