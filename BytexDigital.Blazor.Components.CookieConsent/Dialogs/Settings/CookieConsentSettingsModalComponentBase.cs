using Microsoft.AspNetCore.Components;

namespace BytexDigital.Blazor.Components.CookieConsent.Dialogs.Settings
{
    public abstract class CookieConsentSettingsModalComponentBase : ComponentBase
    {
        /// <summary>
        ///     When raised, requests the parent component to close this dialog. If true is passed, the consent prompt will be
        ///     dismissed too if shown.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OnClosePreferences { get; set; }
    }
}