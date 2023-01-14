using Microsoft.AspNetCore.Components;

namespace BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompts
{
    public abstract class CookieConsentPromptComponentBase : ComponentBase
    {
        [Parameter]
        public EventCallback OnClosePrompt { get; set; }
    }
}