using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<CookieConsentService> _logger;
        private readonly IOptions<CookieConsentOptions> _options;
        private CookiePreferences _cookiePreferencesCached;

        private Task<IJSObjectReference> _module;

        private Task<IJSObjectReference> Module => _module ??= _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                new[] { "./_content/BytexDigital.Blazor.Components.CookieConsent/cookieconsent.js" })
            .AsTask();

        public CookieConsentService(
            IOptions<CookieConsentOptions> options,
            IJSRuntime jsRuntime,
            ILogger<CookieConsentService> logger)
        {
            _options = options;
            _jsRuntime = jsRuntime;
            _logger = logger;

            // Create a default cookie preferences object that is returned when Javascript turns out to be unavailable
            // and no call to SavePreferences has been made yet.
            _cookiePreferencesCached = new CookiePreferences
            {
                AcceptedRevision = -1,
                AllowedCategories = new[] { CookieCategory.NecessaryCategoryIdentifier }
            };
        }

        public event EventHandler<CookiePreferences> CookiePreferencesChanged;
        public event EventHandler<EventArgs> OnShowConsentModal;
        public event EventHandler<EventArgs> OnShowSettingsModal;

        public async Task ShowConsentModalAsync(bool showOnlyIfNecessary)
        {
            if (showOnlyIfNecessary && await IsCurrentRevisionAcceptedAsync()) return;

            await Task.Run(() => OnShowConsentModal?.Invoke(this, EventArgs.Empty));
        }

        public async Task ShowSettingsModalAsync()
        {
            await Task.Run(() => OnShowSettingsModal?.Invoke(this, EventArgs.Empty));
        }

        public async Task SavePreferencesAsync(CookiePreferences cookiePreferences)
        {
            // Fetch the currently valid settings.
            // If JS is unavailable, this will return default settings or the last written settings from memory cache.
            var existingPreferences = await GetPreferencesAsync();

            // Attempt to write the new settings object to our cookie if possible.
            try
            {
                var module = await Module;

                await module.InvokeVoidAsync(
                    "CookieConsent.SetCookie",
                    CreateCookieString(JsonSerializer.Serialize(cookiePreferences)));

                await module.InvokeVoidAsync(
                    "CookieConsent.ApplyPreferences",
                    cookiePreferences.AllowedCategories,
                    cookiePreferences.AllowedServices);
            }
            catch (JSException ex)
            {
                // Ignore exceptions on purpose.
                // We might get here because JS is blocked or disabled.
                _logger.LogTrace(ex, "Exception raised attempting to call into JavaScript");
            }

            // In either case, we always save the instance that is supposed to be valid in our memory cache.
            // We don't want to "forget" written things even if JS was unavailable to actually permanently save them!
            // This solution will at least allow us to remember written settings until our tab is closed.
            _cookiePreferencesCached = cookiePreferences;

            try
            {
                if (!existingPreferences.Equals(cookiePreferences))
                    await Task.Run(() => CookiePreferencesChanged?.Invoke(this, cookiePreferences));
            }
            catch (Exception ex)
            {
                // Ignore most likely user caused exception and log it as we don't want to interrupt program flow.
                _logger.LogError(ex, "Exception raised trying to run CookiePreferencesChanged event handler");
            }
        }

        public async Task SavePreferencesNecessaryOnlyAsync()
        {
            await SavePreferencesAsync(
                new CookiePreferences
                {
                    AcceptedRevision = _options.Value.Revision,
                    AllowedCategories = _options.Value.Categories.Where(x => x.IsRequired)
                        .Select(x => x.Identifier)
                        .ToArray(),
                    AllowedServices = _options.Value.Categories.Where(x => x.IsRequired)
                        .SelectMany(x => x.Services)
                        .Select(x => x.Identifier)
                        .ToArray()
                });
        }

        public async Task SavePreferencesAcceptAllAsync()
        {
            await SavePreferencesAsync(
                new CookiePreferences
                {
                    AcceptedRevision = _options.Value.Revision,
                    AllowedCategories = _options.Value.Categories.Select(x => x.Identifier).ToArray(),
                    AllowedServices = _options.Value.Categories.SelectMany(x => x.Services)
                        .Select(x => x.Identifier)
                        .ToArray()
                });
        }

        /// <summary>
        ///     Returns the currently valid <see cref="CookiePreferences" /> instance. If JavaScript is unavailable, will return
        ///     either an instance with only the necessary category enabled or the last written settings object given to
        ///     <see cref="SavePreferencesAsync" />.
        /// </summary>
        /// <returns></returns>
        public async Task<CookiePreferences> GetPreferencesAsync()
        {
            try
            {
                var module = await Module;
                var cookieValue = await module.InvokeAsync<string>(
                    "CookieConsent.ReadCookie",
                    _options.Value.CookieOptions.CookieName);

                // If the cookie value is empty, no cookie is set yet. In this case
                // return default data.
                return string.IsNullOrWhiteSpace(cookieValue)
                    ? _cookiePreferencesCached
                    : JsonSerializer.Deserialize<CookiePreferences>(cookieValue);
            }
            catch (JSException ex)
            {
                // We might get here because JS is unavailable (blocked, prerendering, etc.).
                // Return default/cached data instead.
                _logger.LogTrace(ex, "Exception raised attempting to call into JavaScript");

                return _cookiePreferencesCached;
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

            preferences.AcceptedRevision = _options.Value.Revision;
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

            preferences.AcceptedRevision = _options.Value.Revision;
            preferences.AllowedCategories = newCategories.ToArray();
            preferences.AllowedServices = newServices.ToArray();

            await SavePreferencesAsync(preferences);
        }

        public async Task<bool> IsCurrentRevisionAcceptedAsync()
        {
            return (await GetPreferencesAsync()).AcceptedRevision == _options.Value.Revision;
        }

        public async Task NotifyPageLoadedAsync()
        {
            var preferences = await GetPreferencesAsync();

            try
            {
                if (await IsCurrentRevisionAcceptedAsync())
                {
                    var module = await Module;

                    await module.InvokeVoidAsync(
                        "CookieConsent.ApplyPreferences",
                        preferences.AllowedCategories,
                        preferences.AllowedServices);
                }
            }
            catch (JSException ex)
            {
                // Ignore exceptions due to blocked or unavailable JS.
                // In this case, we aren't able to activate JS tags, but there seems to be nothing
                // we can do in this situation!
                _logger.LogTrace(ex, "Exception raised attempting to call into JavaScript");
            }

            try
            {
                await Task.Run(() => CookiePreferencesChanged?.Invoke(this, preferences));
            }
            catch (Exception ex)
            {
                // Ignore most likely user caused exception and log it as we don't want to interrupt program flow.
                _logger.LogError(ex, "Exception raised trying to run CookiePreferencesChanged event handler");
            }
        }

        protected virtual string CreateCookieString(string value)
        {
            var cookieString = $"{_options.Value.CookieOptions.CookieName}={value}";
            cookieString += $"; samesite={_options.Value.CookieOptions.CookieSameSite}";

            if (_options.Value.CookieOptions.CookieMaxAge != default)
                cookieString += $"; max-age={(int) _options.Value.CookieOptions.CookieMaxAge.Value.TotalSeconds}";

            if (!string.IsNullOrEmpty(_options.Value.CookieOptions.CookieDomain))
                cookieString += $"; domain={_options.Value.CookieOptions.CookieDomain}";

            if (!string.IsNullOrEmpty(_options.Value.CookieOptions.CookiePath))
                cookieString += $"; path={_options.Value.CookieOptions.CookiePath}";

            if (_options.Value.CookieOptions.CookieHttpOnly) cookieString += "; HttpOnly";

            if (_options.Value.CookieOptions.CookieSecure) cookieString += "; Secure";

            if (_options.Value.CookieOptions.CookieExpires != default)
                cookieString += $"; expires={_options.Value.CookieOptions.CookieExpires.Value:r}";

            return cookieString;
        }
    }
}