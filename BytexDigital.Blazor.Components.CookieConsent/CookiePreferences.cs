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
            if (other == null)
            {
                return false;
            }

            if (AcceptedRevision != other.AcceptedRevision)
            {
                return false;
            }

            if (AllowedCategories?.Length != other.AllowedCategories?.Length ||
                AllowedCategories.Intersect(other.AllowedCategories).Count() != AllowedCategories.Length)
            {
                return false;
            }

            if (AllowedServices?.Length != other.AllowedServices?.Length ||
                AllowedServices.Intersect(other.AllowedServices).Count() != AllowedServices.Length)
            {
                return false;
            }

            return true;
        }
    }
}
