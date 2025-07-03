using System;
using System.Linq;

namespace BytexDigital.Blazor.Components.CookieConsent
{
    public class CookiePreferences : IEquatable<CookiePreferences>
    {
        public int AcceptedRevision { get; set; } = 0;
        public string[] AllowedCategories { get; set; } = Array.Empty<string>();
        public string[] AllowedServices { get; set; } = Array.Empty<string>();

        public bool IsCategoryAllowed(string category)
        {
            return AllowedCategories != null && AllowedCategories.Contains(category);
        }
        
        public bool IsServiceAllowed(string service)
        {
            return AllowedServices != null && AllowedServices.Contains(service);
        }
        
        public bool Equals(CookiePreferences other)
        {
            if (AcceptedRevision != other.AcceptedRevision)
            {
                return false;
            }

            if (AllowedCategories.Intersect(other.AllowedCategories).Count() != AllowedCategories.Length)
            {
                return false;
            }

            if (AllowedServices.Intersect(other.AllowedServices).Count() != AllowedServices.Length)
            {
                return false;
            }

            return true;
        }
    }
}
