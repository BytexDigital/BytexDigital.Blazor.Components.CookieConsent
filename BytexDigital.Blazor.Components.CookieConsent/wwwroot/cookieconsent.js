export let CookieConsent =
{
    ReadCookie: function (name) {
        return document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)')?.pop() || '';
    },

    RemoveCookie: function (name) {
        document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    },

    SetCookie: function (cookieString) {
        document.cookie = cookieString;
    },

    ApplyPreferences: function (categories, services) {
        let activatableScriptTags = document.querySelectorAll("script[type='text/plain']");

        activatableScriptTags.forEach(element => {
            let requiredCategory = element.getAttribute("data-consent-category");

            if (!requiredCategory) return;

            if (categories.includes(requiredCategory)) {
                let clonedElement = element.cloneNode(true);

                let targetType = element.getAttribute("data-consent-targetType");

                if (!targetType) {
                    clonedElement.setAttribute("type", "text/javascript");
                } else {
                    clonedElement.setAttribute("type", targetType);
                }

                element.parentElement.insertBefore(clonedElement, element.nextSibling);
                element.remove();
            }
        });
    }
};