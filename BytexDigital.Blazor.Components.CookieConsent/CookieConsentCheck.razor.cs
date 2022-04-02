using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public partial class CookieConsentCheck : IDisposable
    {
        /// <summary>
        /// Content that gets rendered if the requirement is not met.
        /// </summary>
        [Parameter]
        public RenderFragment<CookieConsentCheck> NotAllowed { get; set; }

        /// <summary>
        /// Content that gets rendered if the requirement is met.
        /// </summary>
        [Parameter]
        public RenderFragment Allowed { get; set; }

        [Inject]
        public CookieConsentService CookieConsentService { get; set; }

        [Inject]
        public IOptions<CookieConsentOptions> Options { get; set; }

        /// <summary>
        /// Identifier of the required category. Must be set OR <see cref="RequiredService"/>.
        /// </summary>
        [Parameter]
        public string RequiredCategory { get; set; }

        /// <summary>
        /// Identifier of the required service. Must be set OR <see cref="RequiredCategory"/>.
        /// </summary>
        [Parameter]
        public string RequiredService { get; set; }

        /// <summary>
        /// <see cref="CookieCategory"/> object.
        /// </summary>
        public CookieCategory Category { get; private set; }

        /// <summary>
        /// <see cref="CookieCategoryService"/> object if met.
        /// </summary>
        public CookieCategoryService Service { get; private set; }

        /// <summary>
        /// Indicates if the content is currently showing.
        /// </summary>
        public bool IsAllowed { get; private set; }

        public void Dispose()
        {
            CookieConsentService.CookiePreferencesChanged -= CookieConsentService_CookiePreferencesChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            if (Allowed == null)
            {
                throw new InvalidOperationException($"The '{nameof(Allowed)}' parameter has to be specified.");
            }

            if (RequiredCategory == null && RequiredService == null)
            {
                throw new InvalidOperationException(
                    $"Either '{nameof(RequiredCategory)}' or '{nameof(RequiredService)}' has to be specified.");
            }

            CookieConsentService.CookiePreferencesChanged += CookieConsentService_CookiePreferencesChanged;

            if (RequiredService != null)
            {
                Category = Options.Value.Categories.FirstOrDefault(
                    x => x.Services.Any(x => x.Identifier == RequiredService));
                Service = Category?.Services.First(x => x.Identifier == RequiredService);
            }
            else if (RequiredCategory != null)
            {
                Category = Options.Value.Categories.FirstOrDefault(x => x.Identifier == RequiredCategory);
            }

            if (Category == null)
            {
                throw new Exception(
                    $"The required service or category '{RequiredService ?? RequiredCategory}' was not configured.");
            }

            await EvaluateStateAsync();
        }

        /// <summary>
        /// Accepts the required <see cref="RequiredCategory"/> and displays the content.
        /// </summary>
        /// <returns></returns>
        public async Task AcceptRequiredAsync()
        {
            await CookieConsentService.AllowCategoryAsync(Category.Identifier);
        }

        private async void CookieConsentService_CookiePreferencesChanged(object sender, CookiePreferences e)
        {
            await InvokeAsync(async () => await EvaluateStateAsync(e));
        }

        private async Task EvaluateStateAsync(CookiePreferences preferences = null)
        {
            // If the user hasn't given permission the the newest cookie policy, we should not display anything.
            if (!await CookieConsentService.IsCurrentRevisionAcceptedAsync())
            {
                IsAllowed = false;
                return;
            }

            preferences ??= await CookieConsentService.GetPreferencesAsync();

            IsAllowed = preferences.AllowedCategories.Contains(RequiredCategory);

            StateHasChanged();
        }
    }
}