using System;
using System.Globalization;
using System.Threading.Tasks;
using BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompt;
using BytexDigital.Blazor.Components.CookieConsent.Dialogs.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public partial class CookieConsentInterface : IDisposable
    {
        [Inject]
        protected IOptions<CookieConsentOptions> Options { get; set; }

        [Inject]
        protected CookieConsentService CookieConsentService { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public RenderFragment PromptFragment { get; set; }

        public RenderFragment SettingsFragment { get; set; }

        public bool IsShowingConsentModal { get; private set; }
        public bool IsShowingSettingsModal { get; private set; }


        private string CultureCode => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        public void Dispose()
        {
            CookieConsentService.ShowConsentModalRequested -= CookieConsentServiceShowCookieConsentModalRequested;
            CookieConsentService.ShowPreferencesModalRequested -= CookieConsentServiceShowPreferencesModalRequested;
        }

        protected override void OnInitialized()
        {
            CookieConsentService.ShowConsentModalRequested += CookieConsentServiceShowCookieConsentModalRequested;
            CookieConsentService.ShowPreferencesModalRequested += CookieConsentServiceShowPreferencesModalRequested;
        }

        protected override void OnParametersSet()
        {
            if (!Options.Value.ConsentPromptVariant.ComponentType.IsAssignableTo(
                    typeof(CookieConsentPromptComponentBase)))
            {
                throw new InvalidOperationException(
                    $"{nameof(CookieConsentOptions)}.{nameof(CookieConsentOptions.ConsentPromptVariant)}.{nameof(CookieConsentPromptVariantBase.ComponentType)} must inherit from {nameof(CookieConsentPromptComponentBase)}");
            }

            if (!Options.Value.SettingsModalVariant.ComponentType.IsAssignableTo(
                    typeof(CookieConsentSettingsModalComponentBase)))
            {
                throw new InvalidOperationException(
                    $"{nameof(CookieConsentOptions)}.{nameof(CookieConsentOptions.SettingsModalVariant)}.{nameof(CookieConsentPromptVariantBase.ComponentType)} must inherit from {nameof(CookieConsentSettingsModalComponentBase)}");
            }

            PromptFragment = builder =>
            {
                var seq = 0;

                builder.OpenComponent(seq++, Options.Value.ConsentPromptVariant.ComponentType);
                builder.AddAttribute(seq++,
                    nameof(CookieConsentPromptComponentBase.OnClosePrompt),
                    EventCallback.Factory.Create(this, OnPromptClose));
                builder.CloseComponent();
            };

            SettingsFragment = builder =>
            {
                var seq = 0;

                builder.OpenComponent(seq++, Options.Value.SettingsModalVariant.ComponentType);
                builder.AddAttribute(seq++,
                    nameof(CookieConsentSettingsModalComponentBase.OnClosePreferences),
                    EventCallback.Factory.Create<bool>(this, OnPreferencesClose));
                builder.CloseComponent();
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
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
        
        private async Task OnPromptClose()
        {
            IsShowingConsentModal = false;
            StateHasChanged();
        }

        private async Task OnPreferencesClose(bool dismissPrompt)
        {
            IsShowingSettingsModal = false;

            // If a selection has been made inside the preferences dialog, we want to also hide the consent dialog in case it was showing, since the users preferences have been set
            if (dismissPrompt)
            {
                IsShowingConsentModal = false;
            }

            StateHasChanged();
        }

        private async void CookieConsentServiceShowPreferencesModalRequested(object sender, EventArgs e)
        {
            await InvokeAsync(
                () =>
                {
                    IsShowingSettingsModal = true;
                    StateHasChanged();
                });
        }

        private async void CookieConsentServiceShowCookieConsentModalRequested(object sender, EventArgs e)
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