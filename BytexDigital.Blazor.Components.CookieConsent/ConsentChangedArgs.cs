using System;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class ConsentChangedArgs : EventArgs
    {
        /// <summary>
        /// Identifier of the category that changed. Corresponds to a <see cref="CookieCategory.Identifier"/>.
        /// </summary>
        public string CategoryIdentifier { get; init; }
        
        /// <summary>
        /// Indicates to what status the consent changed.
        /// </summary>
        public ConsentChangeType ChangedTo { get; init; }
        
        /// <summary>
        /// Indicates whether this category consent change is the first to ever happen during the lifetime of the
        /// app. This would usually be true if the event is broadcast as part of the app startup.
        /// </summary>
        public bool IsInitialChange { get; init; }
        
        public enum ConsentChangeType
        {
            Granted,
            Revoked
        }
    }
}