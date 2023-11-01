using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BytexDigital.Blazor.Components.CookieConsent;

/// <summary>
///     Provides a simple way to render content only if specific script tags have been enabled by the cookie consent
///     manager
///     and have fully loaded/executed.
/// </summary>
public partial class CookieConsentScriptsLoadedCheck
{
    /// <summary>
    ///     Content to render if all specified <see cref="Scripts" /> are loaded.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    ///     Content to render if not all specified <see cref="Scripts" /> are loaded.
    /// </summary>
    [Parameter]
    public RenderFragment NotLoaded { get; set; }

    /// <summary>
    ///     IDs of scripts that are required to be loaded for the content to show.
    /// </summary>
    [Parameter]
    public IEnumerable<string> Scripts { get; set; }

    /// <summary>
    ///     ID of the category that the user has to give consent to. If left empty or null, it will not be checked for.
    /// </summary>
    [Parameter]
    public string Category { get; set; }

    [Inject]
    public CookieConsentService CookieConsentService { get; set; }

    /// <summary>
    ///     Fires when the component goes from not rendering to rendering or vice versa.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OnRenderStateChanged { get; set; }

    /// <summary>
    ///     Marks whether the conditions have been met for this content to display.
    /// </summary>
    public bool IsRendering { get; protected set; }

    public void Dispose()
    {
        CookieConsentService.ScriptLoaded -= CookieConsentServiceOnScriptLoaded;
        CookieConsentService.CategoryConsentChanged -= CookieConsentServiceOnCategoryConsentChanged;
    }

    protected override void OnInitialized()
    {
        if (Scripts == null || !Scripts.Any())
        {
            throw new InvalidOperationException($"{nameof(Scripts)} needs to be non empty and not null.");
        }

        CookieConsentService.ScriptLoaded += CookieConsentServiceOnScriptLoaded;
        CookieConsentService.CategoryConsentChanged += CookieConsentServiceOnCategoryConsentChanged;
    }

    private async void CookieConsentServiceOnCategoryConsentChanged(object sender, ConsentChangedArgs e)
    {
        await CheckAsync();
    }

    private async void CookieConsentServiceOnScriptLoaded(object sender, CookieConsentScriptLoadedArgs e)
    {
        await CheckAsync(e.AllLoadedScripts);
    }

    /// <summary>
    ///     Forces a check if the content should be shown.
    /// </summary>
    /// <param name="activatedScripts">Optional list of activated scripts to check against.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CheckAsync(
        List<CookieConsentLoadedScript> activatedScripts = default,
        CancellationToken cancellationToken = default)
    {
        activatedScripts ??= await CookieConsentService.GetLoadedScriptsAsync(cancellationToken) ??
            new List<CookieConsentLoadedScript>();

        var wasRendering = IsRendering;

        var scriptsReady = Scripts.All(requiredScriptId
            => activatedScripts.Any(activeScript => activeScript.Id == requiredScriptId));

        if (scriptsReady)
        {
            if (string.IsNullOrEmpty(Category))
            {
                IsRendering = true;
            }
            else
            {
                var preferences = await CookieConsentService.GetPreferencesAsync();

                IsRendering = preferences.IsCategoryAllowed(Category);
            }
        }
        else
        {
            IsRendering = false;
        }

        await InvokeAsync(async () =>
        {
            StateHasChanged();

            if (wasRendering != IsRendering)
            {
                await OnRenderStateChanged.InvokeAsync(IsRendering);
            }
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || IsRendering) return;

        await CheckAsync();
    }
}