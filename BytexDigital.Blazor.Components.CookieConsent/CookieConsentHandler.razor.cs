using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Threading.Tasks;

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

        public bool IsShowingConsentModal { get; private set; }
        public bool IsShowingSettingsModal { get; private set; }

        private string ConsentModalYPositionCss
        {
            get
            {
                if (new[] { ConsentModalPosition.BottomCenter, ConsentModalPosition.BottomLeft, ConsentModalPosition.BottomRight }.Contains(Options.Value.ConsentModalPosition))
                {
                    return "cc:bottom-0 css:left-0";
                }
                else
                {
                    return "cc:top-0 css:left-0";
                }
            }
        }

        private string ConsentModalXPositionCss
        {
            get
            {
                if (new[] { ConsentModalPosition.BottomLeft, ConsentModalPosition.TopLeft }.Contains(Options.Value.ConsentModalPosition))
                {
                    return "cc:justify-start";
                }

                if (new[] { ConsentModalPosition.BottomRight, ConsentModalPosition.TopRight }.Contains(Options.Value.ConsentModalPosition))
                {
                    return "cc:justify-end";
                }

                return "cc:justify-center";
            }
        }

        private bool OnlyRequiredCategoriesExist => Options.Value.Categories.All(x => x.IsRequired);

        private bool IsConsentModalLayoutBar => Options.Value.ConsentModalLayout == ConsentModalLayout.Bar;
        private string CultureCode => System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        protected override void OnInitialized()
        {
            CookieConsentService.OnShowConsentModal += CookieConsentService_OnShowCookieConsentModal;
            CookieConsentService.OnShowSettingsModal += CookieConsentService_OnShowSettingsModal;
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

        private async Task AcceptAsync(bool all)
        {
            if (all)
            {
                await CookieConsentService.SavePreferencesAcceptAllAsync();
            }
            else
            {
                await CookieConsentService.SavePreferencesNecessaryOnlyAsync();
            }

            IsShowingConsentModal = false;
            StateHasChanged();
        }

        private async void CookieConsentService_OnShowSettingsModal(object sender, EventArgs e)
        {
            await InvokeAsync(() =>
            {
                IsShowingSettingsModal = true;
                StateHasChanged();
            });
        }

        private async void CookieConsentService_OnShowCookieConsentModal(object sender, EventArgs e)
        {
            await InvokeAsync(() =>
            {
                IsShowingConsentModal = true;
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            CookieConsentService.OnShowConsentModal -= CookieConsentService_OnShowCookieConsentModal;
            CookieConsentService.OnShowSettingsModal -= CookieConsentService_OnShowSettingsModal;
        }
    }
}