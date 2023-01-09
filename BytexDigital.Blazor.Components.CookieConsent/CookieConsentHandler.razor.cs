using System;
using System.Linq;
using System.Threading.Tasks;
using BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Options;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public partial class CookieConsentHandler : IDisposable
    {
        [Inject]
        protected IOptions<CookieConsentOptions> Options { get; set; }

        [Inject]
        protected CookieConsentService CookieConsentService { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public RenderFragment PromptFragment { get; set; }
        
        public bool IsShowingConsentModal { get; private set; }
        public bool IsShowingSettingsModal { get; private set; }
        

        private string CultureCode => System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        public void Dispose()
        {
            CookieConsentService.OnShowConsentModal -= CookieConsentService_OnShowCookieConsentModal;
            CookieConsentService.OnShowSettingsModal -= CookieConsentService_OnShowSettingsModal;
        }

        protected override void OnInitialized()
        {
            CookieConsentService.OnShowConsentModal += CookieConsentService_OnShowCookieConsentModal;
            CookieConsentService.OnShowSettingsModal += CookieConsentService_OnShowSettingsModal;
        }

        protected override void OnParametersSet()
        {
            if (!Options.Value.PromptComponentType.IsAssignableTo(typeof(CookieConsentPromptBase)))
            {
                throw new InvalidOperationException(
                    $"{nameof(CookieConsentOptions)}.{nameof(CookieConsentOptions.PromptComponentType)} must inherit from {nameof(CookieConsentPromptBase)}");
            }

            PromptFragment = builder =>
            {
                int seq = 0;

                builder.OpenComponent(seq++, Options.Value.PromptComponentType);
                builder.AddAttribute(seq++,
                    nameof(CookieConsentPromptBase.OnClosePrompt),
                    EventCallback.Factory.Create(this, OnPromptClose));
                builder.CloseComponent();
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CookieConsentService.NotifyPageLoadedAsync();

                if (Options.Value.AutomaticallyShow)
                {
                    if (!await CookieConsentService.IsCurrentRevisionAcceptedAsync())
                    {
                        IsShowingConsentModal = true;
                        StateHasChanged();
                    }
                }
            }
        }

        private void OpenSettings()
        {
            IsShowingSettingsModal = true;
            StateHasChanged();
        }

        private void SettingsClosed(bool selectionMade)
        {
            IsShowingSettingsModal = false;

            // If a selection has been made inside the preferences dialog, we want to also hide the consent dialog in case it was showing, since the users preferences have been set
            if (selectionMade)
            {
                IsShowingConsentModal = false;
            }

            StateHasChanged();
        }

        private async Task OnPromptClose()
        {
            IsShowingConsentModal = false;
            StateHasChanged();
        }
        
        
        private async void CookieConsentService_OnShowSettingsModal(object sender, EventArgs e)
        {
            await InvokeAsync(
                () =>
                {
                    IsShowingSettingsModal = true;
                    StateHasChanged();
                });
        }

        private async void CookieConsentService_OnShowCookieConsentModal(object sender, EventArgs e)
        {
            await InvokeAsync(
                () =>
                {
                    IsShowingConsentModal = true;
                    StateHasChanged();
                });
        }
    }
}