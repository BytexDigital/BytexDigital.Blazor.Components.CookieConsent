<div align="center">
    This library offers a simple way to handle GDPR compliant cookie consent concerns in your Blazor WASM/Serverside app.
</div>

<br />

<div align="center" style="padding-top: 3rem; padding-bottom: 5rem;">
<img src=".github/resources/1.png">
<img src=".github/resources/2.png">
</div>

<br />

![NuGet Version](https://img.shields.io/nuget/v/BytexDigital.Blazor.Components.CookieConsent?style=flat-square&labelColor=%23172554&color=%233b82f6&label=Stable)
![NuGet Version](https://img.shields.io/nuget/vpre/BytexDigital.Blazor.Components.CookieConsent?style=flat-square&labelColor=%23022c22&color=%23059669&label=Preview)

## How to install

<br />

> **Note**
>
> The following installation instructions have been split up depending on how you use Blazor.
>
> - If you use .NET 8 (or higher) Blazor WebApps AND have your `<Router>` statically rendered (this means the router's or it's parent component rendermode **is not explicitly set** for example through `[RenderModeWebAssembly]` or `[RenderModeServer]`), then follow the first guide.
>
>   
> - If you use .NET 8 (or higher) Blazor WebApps AND have your `<Router>` dynamically rendered (this means the router's or it's parent component rendermode **is explicitly set** for example through `[RenderModeWebAssembly]` or `[RenderModeServer]`), then follow the second guide.
>
>   
> - If you use .NET 7 or prior, then follow the third guide.

<br />

<details>
  <summary> 🔧 Installation .NET 8 and higher, Blazor Web App</summary>

<br>

> **Note**
>
> This library is compatible with dynamic Blazor WebAssembly and Server components within one Blazor Web App project.
> This means that the UI can be rendered in WebAssembly or Server and changes will propagate to all other interactive components regardless of whether they are running on WebAssembly or the Server. Usage of the `CookieConsentService` is also possible in all interactive components regardless of whether they are running on the server or in WebAssembly. Interactive means they are explicitly rendered either through Blazor Server or Blazor WebAssembly (e.g. with `[RenderModeWebAssembly]` or `[RenderModeServer]`) and NOT statically rendered Blazor components.

```ps1
Install-Package BytexDigital.Blazor.Components.CookieConsent
```

<br />

### Requirements

- Library version 1.1.0 or higher
- .NET >= 8.0
- You're using Blazor Web App and your `<Router>` is rendered statically without a render mode set (no RenderMode attribute set)

<br />

### Configure in your project

#### 1. Configure your App.razor

First you will have to determine which Blazor implementation should display the Cookie Consent user interface. It can either be rendered with Blazor WebAssembly or Blazor Server.

> **Note**
>
> If you, for example, choose to render in Blazor WebAssembly, the main `CookieConsentHandler` will need to be configured to render in WebAssembly.
> 
> If you're running interactive Blazor Server components in the same project too and wish to be able to interact with the library there as well, for example to perform a `CookieConsentCheck`, you'll need to add a `CookieConsentInitializer` to render on the server which will hook everything up to communicate with the client project. If you're not running Blazor Server components or do not need to interact with them there, you can omit this initializer.
>
> The same applies the other way around if you're rendering the UI in Blazor Server and have WebAssembly components interacting with the cookie library, then the handler must be on the server and the initializer on the client.

##### 🅰️ If you choose to render it with Blazor WebAssembly, add the following beneath your router:

```html
<Router AppAssembly="@typeof(App).Assembly">
    ...
</Router>

<!-- Add this -->
<BytexDigital.Blazor.Components.CookieConsent.CookieConsentHandler @rendermode="@RenderMode.InteractiveWebAssembly" />

<!-- Add this additionally, if you use interactive Blazor Server components aswell and wish to interact with the library on the server too. -->
<BytexDigital.Blazor.Components.CookieConsent.CookieConsentInitializer @rendermode="@RenderMode.InteractiveServer" />
```

##### 🅱️ If you choose to render it with Blazor server, add the following instead:

```html
<Router AppAssembly="@typeof(App).Assembly">
    ...
</Router>

<!-- Add this -->
<BytexDigital.Blazor.Components.CookieConsent.CookieConsentHandler @rendermode="@RenderMode.InteractiveServer" />

<!-- Add this additionally, if you use WebAssembly components aswell and wish to interact with the library on the client too. -->
<BytexDigital.Blazor.Components.CookieConsent.CookieConsentInitializer @rendermode="@RenderMode.InteractiveWebAssembly" />
```

<br />

#### 2. Add the required CSS

Add the following css include to your App.razor/Host.razor file.

```html
<link rel="stylesheet" href="_content/BytexDigital.Blazor.Components.CookieConsent/styles.min.css" />
```

<br />

#### 3. (Optional) Add the default font used

**Installing the font**  
By default, the components use the following order of fonts

```css
Inter var, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"
```

`Inter` is the font used in the screenshots. If you wish the components to use this font, import the inter font by
additionally adding the following CSS link:

```html
<link rel="stylesheet" href="https://rsms.me/inter/inter.css" />
```

<br />

#### 4. Register and configure the services in your dependency container

Add the required services in your Program.cs/Startup.cs and configure cookie categories present in your application.

The library implicitly adds a `necessary` (value of constant `CookieCategory.NecessaryCategoryIdentifier`) category.

<br>

##### 🅰️ If you're rendering the UI with Blazor WebAssembly, the call will have to be made as follows:

*In the WebAssembly client project*
```csharp
builder.Services.AddCookieConsent(o =>
{
    // Your configuration
});
```

*Add this additionally in the server project, if you use Blazor Server aswell and wish to interact with the library on the server.*
```csharp
builder.Services.AddCookieConsent(o =>
{
    // The same configuration as on the client! Best to put this lambda in a shared project to reuse to reduce duplication.
}, withUserInterface: false);
```

<br>

##### 🅱️️ If you're rendering the UI with Blazor Server, the call will have to be made as follows:

*In the server project*
```csharp
builder.Services.AddCookieConsent(o =>
{
    // Your configuration
});
```

*Add this additionally in the client project, if you use Blazor WebAssembly aswell and wish to interact with the library on the client.*
```csharp
builder.Services.AddCookieConsent(o =>
{
    // The same configuration as on the server! Best to put this lambda in a shared project to reuse to reduce duplication.
}, withUserInterface: false);
```

<br>

##### Example configuration

```cs
builder.Services.AddCookieConsent(o =>
{
    o.Revision = 1;
    o.PolicyUrl = "/cookie-policy";
    
    // Call optional
    o.UseDefaultConsentPrompt(prompt =>
    {
        prompt.Position = ConsentModalPosition.BottomRight;
        prompt.Layout = ConsentModalLayout.Bar;
        prompt.SecondaryActionOpensSettings = false;
        prompt.AcceptAllButtonDisplaysFirst = false;
    });

    o.Categories.Add(new CookieCategory
    {
        TitleText = new()
        {
            ["en"] = "Google Services",
            ["de"] = "Google Dienste"
        },
        DescriptionText = new()
        {
            ["en"] = "Allows the integration and usage of Google services.",
            ["de"] = "Erlaubt die Verwendung von Google Diensten."
        },
        Identifier = "google",
        IsPreselected = true,

        Services = new()
        {
            new CookieCategoryService
            {
                Identifier = "google-maps",
                PolicyUrl = "https://policies.google.com/privacy",
                TitleText = new()
                {
                    ["en"] = "Google Maps",
                    ["de"] = "Google Maps"
                },
                ShowPolicyText = new()
                {
                    ["en"] = "Display policies",
                    ["de"] = "Richtlinien anzeigen"
                }
            },
            new CookieCategoryService
            {
                Identifier = "google-analytics",
                PolicyUrl = "https://policies.google.com/privacy",
                TitleText = new()
                {
                    ["en"] = "Google Analytics",
                    ["de"] = "Google Analytics"
                },
                ShowPolicyText = new()
                {
                    ["en"] = "Display policies",
                    ["de"] = "Richtlinien anzeigen"
                }
            }
        }
    });
});
```

</details>

<details>
  <summary> 🔧 Installation .NET 8 and higher, Full WebAssembly or Full Server</summary>

<br>

```ps1
Install-Package BytexDigital.Blazor.Components.CookieConsent
```

<br />

### Requirements

- .NET >= 8.0
- You're using Blazor Web App and your `<Router>` is dynamically rendered within a Blazor Server or WASM component (This means your `<Router>` is inside a component that is fulled rendered either with Blazor Server or Blazor WASM (That is the case if there is a  `[RenderModeWebAssembly]` or `[RenderModeServer]` attribute on the component containing the router or if the component containing the router is rendered with a `@rendermode="@RenderMode.InteractiveWebAssembly"` or `@rendermode="@RenderMode.InteractiveServer"` attribute))

<br />

### Configure in your project

#### 1. Configure your App.razor

Add the `CookieConsentHandler` your App.razor, beneath the `Router` component, like so:

```html
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(EmptyLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(EmptyLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>

<BytexDigital.Blazor.Components.CookieConsent.CookieConsentHandler />
```

<br />

#### 2. Add the required CSS

Add the following css include to your App.razor/Host.razor file.

```html
<link rel="stylesheet" href="_content/BytexDigital.Blazor.Components.CookieConsent/styles.min.css" />
```

<br />

#### 3. (Optional) Add the default font used

**Installing the font**  
By default, the components use the following order of fonts

```css
Inter var, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"
```

`Inter` is the font used in the screenshots. If you wish the components to use this font, import the inter font by
additionally adding the following CSS link:

```html
<link rel="stylesheet" href="https://rsms.me/inter/inter.css" />
```

<br />

#### 4. Register and configure the services in your dependency container

Add the required services in your Program.cs/Startup.cs and configure cookie categories present in your application. The
library implicitly adds a `necessary` (value of constant `CookieCategory.NecessaryCategoryIdentifier`) category. For
example:

```cs
builder.Services.AddCookieConsent(o =>
{
    o.Revision = 1;
    o.PolicyUrl = "/cookie-policy";
    
    // Call optional
    o.UseDefaultConsentPrompt(prompt =>
    {
        prompt.Position = ConsentModalPosition.BottomRight;
        prompt.Layout = ConsentModalLayout.Bar;
        prompt.SecondaryActionOpensSettings = false;
        prompt.AcceptAllButtonDisplaysFirst = false;
    });

    o.Categories.Add(new CookieCategory
    {
        TitleText = new()
        {
            ["en"] = "Google Services",
            ["de"] = "Google Dienste"
        },
        DescriptionText = new()
        {
            ["en"] = "Allows the integration and usage of Google services.",
            ["de"] = "Erlaubt die Verwendung von Google Diensten."
        },
        Identifier = "google",
        IsPreselected = true,

        Services = new()
        {
            new CookieCategoryService
            {
                Identifier = "google-maps",
                PolicyUrl = "https://policies.google.com/privacy",
                TitleText = new()
                {
                    ["en"] = "Google Maps",
                    ["de"] = "Google Maps"
                },
                ShowPolicyText = new()
                {
                    ["en"] = "Display policies",
                    ["de"] = "Richtlinien anzeigen"
                }
            },
            new CookieCategoryService
            {
                Identifier = "google-analytics",
                PolicyUrl = "https://policies.google.com/privacy",
                TitleText = new()
                {
                    ["en"] = "Google Analytics",
                    ["de"] = "Google Analytics"
                },
                ShowPolicyText = new()
                {
                    ["en"] = "Display policies",
                    ["de"] = "Richtlinien anzeigen"
                }
            }
        }
    });
});
```

#### 5. The library is ready to be used!

Scroll further down to see how you can use the library to conditionally enable/disable Javascript tags in your HTML or
show/hide specific content.

</details>

<details>
  <summary> 🔧 Installation .NET 7 and prior</summary>

<br>

```ps1
Install-Package BytexDigital.Blazor.Components.CookieConsent
```

<br />

### Requirements

.NET >= 5.0 and < 8.0

<br />

### Configure in your project

#### 1. Configure your App.razor

Add the `CookieConsentHandler` your App.razor, beneath the `Router` component, like so:

```html
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(EmptyLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(EmptyLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>

<BytexDigital.Blazor.Components.CookieConsent.CookieConsentHandler />
```

<br />

#### 2. Add the required CSS

Add the following css include to your index.html/_Host.cshtml file.

```html
<link rel="stylesheet" href="_content/BytexDigital.Blazor.Components.CookieConsent/styles.min.css" />
```

<br />

#### 3. (Optional) Add the default font used

**Installing the font**  
By default, the components use the following order of fonts

```css
Inter var, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"
```

`Inter` is the font used in the screenshots. If you wish the components to use this font, import the inter font by
additionally adding the following CSS link:

```html
<link rel="stylesheet" href="https://rsms.me/inter/inter.css" />
```

<br />

#### 4. Register and configure the services in your dependency container

Add the required services in your Program.cs/Startup.cs and configure cookie categories present in your application. The
library implicitly adds a `necessary` (value of constant `CookieCategory.NecessaryCategoryIdentifier`) category. For
example:

```cs
builder.Services.AddCookieConsent(o =>
{
    o.Revision = 1;
    o.PolicyUrl = "/cookie-policy";
    
    // Call optional
    o.UseDefaultConsentPrompt(prompt =>
    {
        prompt.Position = ConsentModalPosition.BottomRight;
        prompt.Layout = ConsentModalLayout.Bar;
        prompt.SecondaryActionOpensSettings = false;
        prompt.AcceptAllButtonDisplaysFirst = false;
    });

    o.Categories.Add(new CookieCategory
    {
        TitleText = new()
        {
            ["en"] = "Google Services",
            ["de"] = "Google Dienste"
        },
        DescriptionText = new()
        {
            ["en"] = "Allows the integration and usage of Google services.",
            ["de"] = "Erlaubt die Verwendung von Google Diensten."
        },
        Identifier = "google",
        IsPreselected = true,

        Services = new()
        {
            new CookieCategoryService
            {
                Identifier = "google-maps",
                PolicyUrl = "https://policies.google.com/privacy",
                TitleText = new()
                {
                    ["en"] = "Google Maps",
                    ["de"] = "Google Maps"
                },
                ShowPolicyText = new()
                {
                    ["en"] = "Display policies",
                    ["de"] = "Richtlinien anzeigen"
                }
            },
            new CookieCategoryService
            {
                Identifier = "google-analytics",
                PolicyUrl = "https://policies.google.com/privacy",
                TitleText = new()
                {
                    ["en"] = "Google Analytics",
                    ["de"] = "Google Analytics"
                },
                ShowPolicyText = new()
                {
                    ["en"] = "Display policies",
                    ["de"] = "Richtlinien anzeigen"
                }
            }
        }
    });
});
```

#### 5. The library is ready to be used!

Scroll further down to see how you can use the library to conditionally enable/disable Javascript tags in your HTML or
show/hide specific content.

</details>


<br />

## Localization

For now, localization is done entirely inside the configuration of the services as seen in the example above. The
library ships with default texts in English, German Dutch, French and Spanish.

The library uses the current `CurrentCulture` by default. Blazor's `.AddLocalization(..)` will automatically set the
current culture. We aim at adding proper support for `IStringLocalizer` aswell, so that all localization can be done
inside resource files instead.

<br />

## Disabled or blocked JavaScript

The library depends on JavaScript to save and load preferences and to enable HTML script tags. If JavaScript is blocked
or not enabled by a browser, the library will **not be able to dynamically enable JavaScript tags
like `<script type="text/plain" data-consent-category="myCategoryName">`**; They will remain disabled even if given
permission by the user. **Saving and loading preferences will also not be possible**, which means any permissions the
user has given will be forgotten if the browser tab is closed and are only valid within the browser tab they were given
in.

<br />

## Customizing colors and font

The default consent prompt and settings modal are customizable to some degree with CSS variables.

Use a CSS rule as follows to overwrite colors and font used.
The values shown are the current default values as shown in the screenshots using Tailwind's theme function.

```css
.cc-isolation-container * {
    /* Font used */
    --cc-font-family: Inter var, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji";

    /* Accent color for primary button */
    --cc-color-accent: theme(colors.slate.800);

    /* Accent color for primary button when hovered */
    --cc-color-accent-dark: theme(colors.slate.900);

    /* Color for secondary button */
    --cc-color-secondary: theme(colors.gray.200);

    /* Color for secondary button when hovered */
    --cc-color-secondary-dark: theme(colors.gray.300);

    /* Color for links ("Display policy" links within preferences dialogue) */
    --cc-color-link: theme(colors.slate.400);
    
    /* Color for link when hovered */
    --cc-color-link-highlight: theme(colors.blue.500);

    /* Default color for text */
    --cc-color-text: theme(colors.slate.700);

    /* Background color for active category switch */
    --cc-color-switch-active: theme(colors.slate.800);

    /* (Transparent) Color for background when any modal is opened */
    --cc-color-modal-background: theme(colors.gray.800 / 75%);
}
```

<br />

## Custom consent prompt and settings modal

You can entirely replace the components that display to ask the user for consent and the settings modal.

First, create a component that inherits from `CookieConsentSettingsModalComponentBase`/`CookieConsentPromptComponentBase`.

Then, you must register this component type in the cookie library by creating a custom variant that inherits from `CookieConsentPromptVariantBase`/`CookieConsentPreferencesVariantBase`.
For example

```csharp
public class CustomSettingsModalVariant : CookieConsentDefaultSettingsModalVariant
{
    public override Type ComponentType { get; set; } = typeof(YOUR_CUSTOM_COMPONENT);
}
```

Lastly, you need to register this variant in the `AddCookieConsent` call.

```csharp
builder.Services.AddCookieConsent(options =>
{
    // To replace the consent prompt
    options.ConsentPromptVariant = new CustomConsentPromptVariant();
        
    // To replace the settings modal
    options.SettingsModalVariant = new CustomSettingsModalVariant();
    
    // ...
});
```

<br />

## Available ways to hide/show content based on cookie preferences

### JavaScript tags

If you wish to use services like Google Analytics, you can integrate them with this library the following way. This will
make it so the script tags do not get run unless allowed to do so by the user.

1. Change the script tags type attribute from `type="text/javascript"` to `type"text/plain"`.
2. Add the attribute `data-consent-category="IDENTIFIER"`.
3. Replace `IDENTIFIER` with the Identifier given to a configured category. In the example given earlier, this could
   be `google`.

> ***Blazor Server* Important note:**
> It appears as though when using Blazor Server, Javascript-tags require the `defer="true"` attribute to be set so that
> the script tag is not removed by Blazor upon
> load ([view issue](https://github.com/BytexDigital/BytexDigital.Blazor.Components.CookieConsent/issues/9#issue-1269307707)).

The result should look like so:

```html
<script type="text/plain" data-consent-category="google">
    (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
    (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
    m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
    })(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

    ga('create', 'UA-XXXXX-Y', 'auto');
    ga('send', 'pageview');
</script>
```

This Google Analytics script will only load if given permission.

<br />

### `CookieConsentCheck` component

You can use the prebuilt component to show content only if given permission in a category. This can be useful for
displaying iframes, for example Google Maps or YouTube videos:

```html
<CookieConsentCheck RequiredCategory="google">
    <Allowed>
        <iframe loading="lazy" allowfullscreen src="https://www.google.com/maps/embed/v1/place?q=place_id:ChIJAVkDPzdOqEcRcDteW0YgIQQ&key=..."></iframe>
    </Allowed>
</CookieConsentCheck>
```

You can customize what this component will render when the given `RequiredCategory` is not allowed by defining
a `NotAllowed` tag. By default, the component will render this:

<br />

<div align="center">
<img src=".github/resources/3.png">
</div>

<br />

Defining something custom to render can be done the following way. It's a good idea to set the `Context` parameter on
the `CookieConsentCheck` component so you can easily access it's properties inside your custom `NotAllowed` block (for
example the `Component.Category` property to access the display name of the required category).

```html
<CookieConsentCheck RequiredCategory="google" Context="Component">
    <Allowed>
        <h1>It works!</h1>
    </Allowed>
    <NotAllowed>
        <button @onclick="async () => await Component.AcceptRequiredAsync()">Show it!</button>
    </NotAllowed>
</CookieConsentCheck>
```

<br/>

## Manually open the preferences modal

Call the following method to show the preferences menu. This could be done from an element inside your footer for example.

ℹ️ *In Blazor Web App, this can be done from both WASM and the Server.*

```csharp
CookieConsentService.ShowSettingsModalAsync();
```

<br />

## Stop scripts (like Google Analytics) from running if consent is revoked

If you integrate services such as Google Analytics and the user grants consent, scripts might start running in the background. To stop these scripts from executing once the user revokes consent, it is necessary to refresh the page.

To achieve this, you can subscribe to the following event and evaluate whether a specific category consent has been
revoked that requires action such as refreshing the page to stop aforementioned scripts:

```csharp~~~~
CookieConsentService.CategoryConsentChanged += (sender, args) =>
{
    if (args.CategoryIdentifier == "google" &&
        args.ChangedTo == ConsentChangedArgs.ConsentChangeType.Revoked &&
        !args.IsInitialChange)
    {
        // Reload the current page with a hard refresh (restart Blazor app).
        NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
    }
};
```

<br />

## Check for the cookie consent state in non-interactive Blazor components such as statically rendered Blazor components, Razor pages or controllers

To accomplish this, you need to read the contents of the cookie which contains the cookie consent preferences encoded in Base64.

The easiest way to achieve this is to use the helper package:
```ps1
Install-Package BytexDigital.Blazor.Components.CookieConsent.AspNetCore
```

After installation, add the following service registration:
```csharp
builder.Services.AddCookieConsentHttpContextServices();
```

Then request the service from the service container, e.g. in a static Blazor component:
```csharp
[Inject]
public HttpContextCookieConsent CookieConsent { get; set; }
```

You can then use the service to fetch the `CookiePreferences` object.
```csharp
var preferences = CookieConsent.GetCookieConsentPreferences();

bool isAllowed = preferences.IsCategoryAllowed("google");
```

<br />

## Checking for scripts to be loaded before interacting with them or rendering content

Sometimes your code may depend on `<script>` tags having run, which might be dependant on whether their consent category has been enabled by the user ([see #javascript-tags](#javascript-tags)).
To make your code run when these script tags are enabled, you might try to listen to the `CookieConsentService.CategoryConsentChanged` and execute your code if the category of these scripts is enabled.
However, inside this event handler, it is not guaranteed that the scripts, enabled by the category you're waiting for, have already run and are ready for usage.

To avoid this race condition, you should additionally use `CookieConsentService.ScriptLoaded` and `CookieConsentService.GetLoadedScriptsAsync` to determine if a script you need has already been fully loaded and executed in the DOM.

As an example, let's use the Google Maps API. This example assumes you've setup a cookie consent category with the ID `google`.

First, you need to add the JS script tag to the Google Maps API library into your host file as follows.
Pay attention to how we're giving this script tag a special ID to recognize it by, in this case it's `google-maps-api`.

```html
<script data-consent-category="google"
        data-consent-script-id="google-maps-api"
        src="https://maps.googleapis.com/maps/api/js?key=YOURKEY&v=3" type="text/plain">
</script>
```

Now, there are two ways you can go about checking if this script is loaded in the browser. One utilizes the `CookieConsentService.ScriptLoaded` and `CookieConsentService.GetLoadedScriptsAsync` methods to achieve this, the other utilizes the ready-built component `CookieConsentScriptsLoadedCheck` which may be usable for you in some scenarios.

<details>
  <summary> 🅰️ Manually check for a loaded script</summary>

<br>

With this script tag added, we can now use the following setup inside a component to only render a map once we both know the category has been accepted by the user and the script has successfully loaded:

```csharp
// Flag whether we've called into JS to render the map. This flag is used 
// inside our component to conditionally add the map div to the DOM or not.
public bool RenderMap { get; set; }

protected override void OnInitialized()
{
    // Notify us when scripts are loaded by the cookie consent manager
    CookieConsentService.ScriptLoaded += CookieConsentServiceOnScriptLoaded;
    
    // Notify us if a category consent changes
    CookieConsentService.CategoryConsentChanged += CookieConsentServiceOnCategoryConsentChanged;
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (!firstRender) return;

    // Attempt to render the map after first render
    await RenderMapAsync();
}

private async void CookieConsentServiceOnCategoryConsentChanged(object sender, ConsentChangedArgs e)
{
    // Attempt to render it if a category consent changed
    await RenderMapAsync();
}

private async void CookieConsentServiceOnScriptLoaded(object sender, CookieConsentScriptLoadedArgs e)
{
    // Attempt to render it if a script tag was loaded.
    await RenderMapAsync();
}

private async Task RenderMapAsync()
{
    // Get the list of loaded scripts and preferences from the browser.
    var loadedScripts = await CookieConsentService.GetLoadedScriptsAsync();
    var preferences = await CookieConsentService.GetPreferencesAsync();

    // If our google category isn't allowed, don't render the map.
    if (!preferences.IsCategoryAllowed("google"))
    {
        RenderMap = false;
        return;
    }

    // If the google-maps-api script hasn't loaded yet, don't render the map, as a call to the Google Maps JS API will fail!
    if (loadedScripts.All(x => x.Id != "google-maps-api"))
    {
        RenderMap = false;
        return;
    }
    
    // Note for the above: It's important you check both the scripts AND the category consent, as the
    // latter may have been revoked by the user but the scripts are still loaded until the browser tab
    // is refreshed!

    // If all conditions are met but the map is already rendered, don't render it twice.
    if (RenderMap) return;

    // Call into the UI thread to...
    await InvokeAsync(async () =>
    {
        // ...call into JavaScript to render the map since we know we're both allowed to and the Google Maps API script tag has loaded!
        var module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./app.js");
        await module.InvokeVoidAsync("renderMap");
    });
}
```
</details>

<details>
  <summary> 🅱️ Use the *CookieConsentScriptsLoadedCheck* component to conditionally render other components</summary>

<br>

**Another way** to achieve the same result is utilizing the `CookieConsentScriptsLoadedCheck` component.

In this case, the above example would look as follows:

```html
<BytexDigital.Blazor.Components.CookieConsent.CookieConsentScriptsLoadedCheck Category="google" Scripts='new [] { "google-maps-api" }' OnRenderStateChanged="RenderMapAsync">
    <div>
        <!-- my map div -->
    </div>
</BytexDigital.Blazor.Components.CookieConsent.CookieConsentScriptsLoadedCheck>
```

```csharp
// Will run every time the div went from hidden to shown or vice versa.
async Task RenderMapAsync(bool isShown)
{
    if (isShown) {
        // ...call into JavaScript to render the map since we know we're both allowed to and the Google Maps API script tag has loaded!
        var module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./app.js");
        await module.InvokeVoidAsync("renderMap");
    }
}
```
</details>

<br />

# Changelog

### 1.2.0

- Adds event `CookieConsentService.ScriptLoaded` to execute code when script tags have been loaded dynamically by the cookie consent manager
- Adds component `CookieConsentScriptsLoadedCheck` that can be used to conditionally render content depending on whether scripts have been loaded by the cookie consent manager 

### 1.1.0

- Support for .NET 8 Blazor Web Apps with mixed WebAssembly and Server usage
- The settings/preferences modal is now replaceable just like the consent prompt is
- Various internal improvements

### 1.0.18

<details>
  <summary>Click to expand!</summary>

   <br /> 

- Bug fixes

</details>

### 1.0.17

<details>
  <summary>Click to expand!</summary>

   <br /> 

- Implemented way to use custom consent prompts components instead of the default one
- Improved default consent prompt behavior on mobile devices
- Overall css improvements

</details>

### 1.0.16

<details>
  <summary>Click to expand!</summary>

   <br /> 

- Implemented way to customize some colors aswell as the font using CSS variables

</details>

### 1.0.15

<details>
  <summary>Click to expand!</summary>

   <br /> 

- Fixed crashes related to JavaScript being not enabled or blocked by browsers (
  see https://github.com/BytexDigital/BytexDigital.Blazor.Components.CookieConsent/issues/12)

</details>

### 1.0.13

<details>
  <summary>Click to expand!</summary>

   <br /> 

- (https://github.com/BytexDigital/BytexDigital.Blazor.Components.CookieConsent/pull/11) Added languages ES, FR

</details>

### 1.0.12

<details>
  <summary>Click to expand!</summary>

   <br /> 

- (https://github.com/BytexDigital/BytexDigital.Blazor.Components.CookieConsent/pull/10) Added language NL

</details>

### 1.0.11

<details>
  <summary>Click to expand!</summary>

   <br /> 

- Fixed conditional script tags not being executed after activation in Firefox (
  see https://github.com/BytexDigital/BytexDigital.Blazor.Components.CookieConsent/issues/9)

</details>

### 1.0.10

<details>
  <summary>Click to expand!</summary>

   <br /> 

- (https://github.com/BytexDigital/BytexDigital.Blazor.Components.CookieConsent/issues/8) Fixed preferences being saved
  with revision set to -1

</details>

### 1.0.9

<details>
  <summary>Click to expand!</summary>

   <br /> 

- Implemented CSS reset to isolate the components of this library from any other CSS influence

</details>

### 1.0.6

<details>
  <summary>Click to expand!</summary>

   <br /> 

- Improved support for overwriting of font used

</details>

