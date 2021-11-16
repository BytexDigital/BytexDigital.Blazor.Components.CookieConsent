using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentService
    {
        private readonly IOptions<CookieConsentOptions> _options;
        private readonly IJSRuntime _jsRuntime;

        public event EventHandler<CookiePreferences> CookiePreferencesChanged;
        public event EventHandler<EventArgs> OnShowConsentModal;
        public event EventHandler<EventArgs> OnShowSettingsModal;

        private Task<IJSObjectReference> _module;
        private Task<IJSObjectReference> Module => _module ??= _jsRuntime.InvokeAsync<IJSObjectReference>("import", new[] { "./_content/BytexDigital.Blazor.Components.CookieConsent/cookieconsent.js" }).AsTask();

        public CookieConsentService(IOptions<CookieConsentOptions> options, IJSRuntime jsRuntime)
        {
            _options = options;
            _jsRuntime = jsRuntime;
        }

        public async Task ShowConsentModalAsync(bool showOnlyIfNecessary)
        {
            if (showOnlyIfNecessary && await IsCurrentRevisionAcceptedAsync()) return;

            await Task.Run(() => OnShowConsentModal?.Invoke(this, new EventArgs()));
        }

        public async Task ShowSettingsModalAsync()
        {
            await Task.Run(() => OnShowSettingsModal?.Invoke(this, new EventArgs()));
        }

        public async Task SavePreferencesAsync(CookiePreferences cookiePreferences)
        {
            var existingPreferences = await GetPreferencesAsync();

            var module = await Module;
            await module.InvokeVoidAsync("CookieConsent.SetCookie", CreateCookieString(JsonSerializer.Serialize(cookiePreferences)));
            await module.InvokeVoidAsync("CookieConsent.ApplyPreferences", cookiePreferences.AllowedCategories, cookiePreferences.AllowedServices);

            if (!existingPreferences.Equals(cookiePreferences))
            {
                await Task.Run(() => CookiePreferencesChanged?.Invoke(this, cookiePreferences));
            }
        }

        public async Task SavePreferencesNecessaryOnlyAsync()
        {
            await SavePreferencesAsync(new CookiePreferences
            {
                AcceptedRevision = _options.Value.Revision,
                AllowedCategories = _options.Value.Categories.Where(x => x.IsRequired).Select(x => x.Identifier).ToArray(),
                AllowedServices = _options.Value.Categories.Where(x => x.IsRequired).SelectMany(x => x.Services).Select(x => x.Identifier).ToArray()
            });
        }

        public async Task SavePreferencesAcceptAllAsync()
        {
            await SavePreferencesAsync(new CookiePreferences
            {
                AcceptedRevision = _options.Value.Revision,
                AllowedCategories = _options.Value.Categories.Select(x => x.Identifier).ToArray(),
                AllowedServices = _options.Value.Categories.SelectMany(x => x.Services).Select(x => x.Identifier).ToArray()
            });
        }

        public async Task<CookiePreferences> GetPreferencesAsync()
        {
            try
            {
                var module = await Module;
                var cookieValue = await module.InvokeAsync<string>("CookieConsent.ReadCookie", _options.Value.CookieOptions.CookieName);

                return JsonSerializer.Deserialize<CookiePreferences>(cookieValue);
            }
            catch (Exception)
            {
                // During prerendering, we will not be able to access JS interop. Thus we must assume we have no preferences set except the necessary ones.
                return new CookiePreferences
                {
                    AcceptedRevision = -1,
                    AllowedCategories = new[] { CookieCategory.NecessaryCategoryIdentifier }
                };
            }
        }

        public async Task AllowCategoryAsync(string categoryIdentifier)
        {
            var category = _options.Value.Categories.First(x => x.Identifier == categoryIdentifier);
            var preferences = await GetPreferencesAsync();

            var newCategories = preferences.AllowedCategories.ToList();
            var newServices = preferences.AllowedServices.ToList();

            newCategories.Add(category.Identifier);
            newServices.AddRange(category.Services.Select(x => x.Identifier));

            preferences.AllowedCategories = newCategories.Distinct().ToArray();
            preferences.AllowedServices = newServices.Distinct().ToArray();

            await SavePreferencesAsync(preferences);
        }

        public async Task ForbidCategoryAsync(string categoryIdentifier)
        {
            var category = _options.Value.Categories.First(x => x.Identifier == categoryIdentifier);
            var preferences = await GetPreferencesAsync();

            var newCategories = preferences.AllowedCategories.ToList();
            var newServices = preferences.AllowedServices.ToList();

            newCategories.RemoveAll(x => x == category.Identifier);
            newServices.RemoveAll(x => category.Services.Select(x => x.Identifier).Contains(x));

            preferences.AllowedCategories = newCategories.ToArray();
            preferences.AllowedServices = newServices.ToArray();

            await SavePreferencesAsync(preferences);
        }

        public async Task<bool> IsCurrentRevisionAcceptedAsync() => (await GetPreferencesAsync()).AcceptedRevision == _options.Value.Revision;

        public async Task NotifyPageLoadedAsync()
        {
            if (await IsCurrentRevisionAcceptedAsync())
            {
                var preferences = await GetPreferencesAsync();

                await Task.Run(() => CookiePreferencesChanged?.Invoke(this, preferences));

                var module = await Module;
                await module.InvokeVoidAsync("CookieConsent.ApplyPreferences", preferences.AllowedCategories, preferences.AllowedServices);
            }
        }

        protected virtual string CreateCookieString(string value)
        {
            string cookieString = $"{_options.Value.CookieOptions.CookieName}={value}";
            cookieString += $"; samesite={_options.Value.CookieOptions.CookieSameSite}";

            if (_options.Value.CookieOptions.CookieMaxAge != default) cookieString += $"; max-age={(int)_options.Value.CookieOptions.CookieMaxAge.Value.TotalSeconds}";
            if (!string.IsNullOrEmpty(_options.Value.CookieOptions.CookieDomain)) cookieString += $"; domain={_options.Value.CookieOptions.CookieDomain}";
            if (!string.IsNullOrEmpty(_options.Value.CookieOptions.CookiePath)) cookieString += $"; path={_options.Value.CookieOptions.CookiePath}";
            if (_options.Value.CookieOptions.CookieHttpOnly) cookieString += "; HttpOnly";
            if (_options.Value.CookieOptions.CookieSecure) cookieString += "; Secure";
            if (_options.Value.CookieOptions.CookieExpires != default) cookieString += $"; expires={_options.Value.CookieOptions.CookieExpires.Value:r}";

            return cookieString;
        }
    }
}
