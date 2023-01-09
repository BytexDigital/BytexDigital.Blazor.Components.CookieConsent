using Microsoft.AspNetCore.Components;

namespace BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompts
{
    public abstract class CookieConsentPromptBase : ComponentBase
    {
        [Parameter]
        public EventCallback OnClosePrompt { get; set; }
    }
}