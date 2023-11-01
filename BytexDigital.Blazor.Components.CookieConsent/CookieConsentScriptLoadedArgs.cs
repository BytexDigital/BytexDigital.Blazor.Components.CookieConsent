using System.Collections.Generic;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookieConsentScriptLoadedArgs
    {
        public List<CookieConsentLoadedScript> AllLoadedScripts { get; set; }
        public CookieConsentLoadedScript Script { get; set; }
    }
}