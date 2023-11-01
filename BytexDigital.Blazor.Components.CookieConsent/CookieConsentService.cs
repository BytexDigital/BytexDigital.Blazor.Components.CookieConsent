using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BytexDigital.Blazor.Components.CookieConsent.Broadcasting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public abstract class CookieConsentService
    {
        protected readonly CookieConsentEventHandler _eventHandler;
        protected readonly IJSRuntime _jsRuntime;
        protected readonly ILogger<CookieConsentService> _logger;
        protected readonly IOptions<CookieConsentOptions> _options;
        private readonly CookieConsentRuntimeContext _runtimeContext;
        protected CookiePreferences _cookiePreferencesCached;
        protected Task<IJSObjectReference> _module;
        protected CookiePreferences _previousCookiePreferences;

        protected Task<IJSObjectReference> Module => _module ??= _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                new object[] { "./_content/BytexDigital.Blazor.Components.CookieConsent/cookieconsent.js" })
            .AsTask();

        public CookieConsentService(
            IOptions<CookieConsentOptions> options,
            IJSRuntime jsRuntime,
            CookieConsentEventHandler eventHandler,
            CookieConsentRuntimeContext runtimeContext,
            ILogger<CookieConsentService> logger)
        {
            _options = options;
            _jsRuntime = jsRuntime;
            _eventHandler = eventHandler;
            _runtimeContext = runtimeContext;
            _logger = logger;

            // Create a default cookie preferences object that is returned when Javascript turns out to be unavailable
            // and no call to SavePreferences has been made yet.
            _cookiePreferencesCached = new CookiePreferences
            {
                AcceptedRevision = -1,
                AllowedCategories = new[] { CookieCategory.NecessaryCategoryIdentifier }
            };

            _eventHandler.CookiePreferencesChanged += EventHandlerOnCookiePreferencesChanged;
            _eventHandler.ShowConsentModalRequested += EventHandlerOnShowConsentModalRequested;
            _eventHandler.ShowPreferencesModalRequested += EventHandlerOnShowPreferencesModalRequested;
            _eventHandler.ScriptLoaded += EventHandlerOnScriptLoaded;
        }

        public event EventHandler<CookiePreferences> CookiePreferencesChanged;
        public event EventHandler<ConsentChangedArgs> CategoryConsentChanged;
        public event EventHandler<EventArgs> ShowConsentModalRequested;
        public event EventHandler<EventArgs> ShowPreferencesModalRequested;
        public event EventHandler<CookieConsentScriptLoadedArgs> ScriptLoaded;

        private void EventHandlerOnScriptLoaded(object sender, CookieConsentScriptLoadedArgs e)
        {
            _ = Task.Run(() => ScriptLoaded?.Invoke(this, e));
        }

        private void EventHandlerOnShowPreferencesModalRequested(object sender, EventArgs e)
        {
            _ = Task.Run(() => ShowPreferencesModalRequested?.Invoke(this, EventArgs.Empty));
        }

        private void EventHandlerOnShowConsentModalRequested(object sender, EventArgs e)
        {
            _ = Task.Run(() => ShowConsentModalRequested?.Invoke(this, EventArgs.Empty));
        }

        protected virtual async Task PublishCookiePreferencesChanged(CookiePreferences preferences)
        {
            await _eventHandler.BroadcastCookiePreferencesChangedAsync(preferences);
        }

        protected virtual async Task PublishShowConsentModalRequested()
        {
            await _eventHandler.BroadcastShowConsentModalRequestedAsync();
        }

        protected virtual async Task PublishShowPreferencesModalRequested()
        {
            await _eventHandler.BroadcastShowPreferencesModalRequestedAsync();
        }

        public virtual async Task ShowConsentModalAsync(bool showOnlyIfNecessary)
        {
            if (showOnlyIfNecessary && await IsCurrentRevisionAcceptedAsync()) return;

            await PublishShowConsentModalRequested();
        }

        public virtual async Task ShowPreferencesModalAsync()
        {
            await PublishShowPreferencesModalRequested();
        }

        public virtual async Task SavePreferencesAsync(CookiePreferences cookiePreferences)
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
            catch (Exception ex)
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
                {
                    await PublishCookiePreferencesChanged(cookiePreferences);
                }
            }
            catch (Exception ex)
            {
                // Ignore most likely user caused exception and log it as we don't want to interrupt program flow.
                _logger.LogError(ex, "Exception raised trying to run CookiePreferencesChanged event handler");
            }
        }

        public virtual async Task SavePreferencesNecessaryOnlyAsync()
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

        public virtual async Task SavePreferencesAcceptAllAsync()
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
        public virtual async Task<CookiePreferences> GetPreferencesAsync()
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
                    : cookieValue.StartsWith("{")
                        ? JsonSerializer.Deserialize<CookiePreferences>(cookieValue)
                        : JsonSerializer.Deserialize<CookiePreferences>(
                            Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue)));
            }
            catch (Exception ex)
            {
                // We might get here because JS is unavailable (blocked, prerendering, etc.).
                // Return default/cached data instead.
                _logger.LogTrace(ex, "Exception raised attempting to call into JavaScript");

                return _cookiePreferencesCached;
            }
        }

        /// <summary>
        /// Returns the JS script tags that were loaded and have already executed. If JS isn't available, this will return an empty list.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<List<CookieConsentLoadedScript>> GetLoadedScriptsAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var module = await Module;
                var activatedScriptsJson = await module.InvokeAsync<string>(
                    "CookieConsent.ReadLoadedScripts",
                    cancellationToken);

                return JsonSerializer.Deserialize<List<CookieConsentLoadedScript>>(activatedScriptsJson);
            }
            catch (Exception ex)
            {
                // We might get here because JS is unavailable (blocked, prerendering, etc.).
                // Return default/cached data instead.
                _logger.LogTrace(ex, "Exception raised attempting to call into JavaScript");

                return new List<CookieConsentLoadedScript>();
            }
        }

        public virtual async Task AllowCategoryAsync(string categoryIdentifier)
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

        public virtual async Task ForbidCategoryAsync(string categoryIdentifier)
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

        public virtual async Task<bool> IsCurrentRevisionAcceptedAsync()
        {
            return (await GetPreferencesAsync()).AcceptedRevision == _options.Value.Revision;
        }

        public virtual async Task NotifyApplicationLoadedAsync()
        {
        }

        protected virtual string CreateCookieString(string value)
        {
            var cookieString =
                $"{_options.Value.CookieOptions.CookieName}={Convert.ToBase64String(Encoding.UTF8.GetBytes(value))}";
            cookieString += $"; samesite={_options.Value.CookieOptions.CookieSameSite}";

            if (_options.Value.CookieOptions.CookieMaxAge != default)
            {
                cookieString += $"; max-age={(int) _options.Value.CookieOptions.CookieMaxAge.Value.TotalSeconds}";
            }

            if (!string.IsNullOrEmpty(_options.Value.CookieOptions.CookieDomain))
            {
                cookieString += $"; domain={_options.Value.CookieOptions.CookieDomain}";
            }

            if (!string.IsNullOrEmpty(_options.Value.CookieOptions.CookiePath))
            {
                cookieString += $"; path={_options.Value.CookieOptions.CookiePath}";
            }

            if (_options.Value.CookieOptions.CookieHttpOnly) cookieString += "; HttpOnly";

            if (_options.Value.CookieOptions.CookieSecure) cookieString += "; Secure";

            if (_options.Value.CookieOptions.CookieExpires != default)
            {
                cookieString += $"; expires={_options.Value.CookieOptions.CookieExpires.Value:r}";
            }

            return cookieString;
        }

        protected virtual void EventHandlerOnCookiePreferencesChanged(object sender, CookiePreferences newPreferences)
        {
            var isInitialChange = _previousCookiePreferences == default;

            try
            {
                Task.Run(() => CookiePreferencesChanged?.Invoke(this, newPreferences));
            }
            catch (Exception ex)
            {
            }

            try
            {
                foreach (var category in _options.Value.Categories)
                {
                    // Revoked -> Granted
                    if (isInitialChange
                            ? newPreferences.IsCategoryAllowed(category.Identifier)
                            : !_previousCookiePreferences.IsCategoryAllowed(category.Identifier) &&
                            newPreferences.IsCategoryAllowed(category.Identifier))
                    {
                        BroadcastGranted(category.Identifier, ConsentChangedArgs.ConsentChangeType.Granted);
                    }
                    else
                        // Granted -> Revoked
                    if (isInitialChange
                            ? !newPreferences.IsCategoryAllowed(category.Identifier)
                            : _previousCookiePreferences.IsCategoryAllowed(category.Identifier) &&
                            !newPreferences.IsCategoryAllowed(category.Identifier))
                    {
                        BroadcastGranted(category.Identifier, ConsentChangedArgs.ConsentChangeType.Revoked);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore most likely user caused exception and log it as we don't want to interrupt program flow.
                _logger.LogError(ex, "Exception raised trying to run CategoryConsentChanged event handler");
            }

            _previousCookiePreferences = newPreferences;
            return;

            void BroadcastGranted(string identifier, ConsentChangedArgs.ConsentChangeType type)
            {
                _ = Task.Run(() => CategoryConsentChanged?.Invoke(this,
                    new ConsentChangedArgs
                    {
                        CategoryIdentifier = identifier,
                        ChangedTo = type,
                        IsInitialChange = isInitialChange
                    }));
            }
        }
    }
}