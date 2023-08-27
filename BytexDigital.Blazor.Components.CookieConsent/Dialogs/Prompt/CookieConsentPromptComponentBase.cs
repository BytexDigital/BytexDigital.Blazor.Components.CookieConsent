using Microsoft.AspNetCore.Components;

namespace BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompt
{
    public abstract class CookieConsentPromptComponentBase : ComponentBase
    {
        /// <summary>
        /// When raised, requests the parent component to close this dialog.
        /// </summary>
        [Parameter]
        public EventCallback OnClosePrompt { get; set; }
    }
}