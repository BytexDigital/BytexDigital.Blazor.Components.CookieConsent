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
        const activatableScriptTags = document.querySelectorAll("script[type='text/plain']");

        activatableScriptTags.forEach(originalScriptElement => {
            const requiredCategory = originalScriptElement.getAttribute("data-consent-category");

            if (!requiredCategory) return;

            if (categories.includes(requiredCategory)) {
                originalScriptElement.type = "text/javascript";
                originalScriptElement.removeAttribute("data-consent-category");

                const sourceUri = originalScriptElement.getAttribute("data-src");

                if (sourceUri) {
                    originalScriptElement.removeAttribute("data-src");
                }

                const newScriptElement = document.createElement("script");
                newScriptElement.textContent = originalScriptElement.innerHTML;

                const sourceAttributes = originalScriptElement.attributes;

                for (let i = 0; i < sourceAttributes.length; i++) {
                    const attributeName = sourceAttributes[i].nodeName;

                    newScriptElement.setAttribute(
                        attributeName,
                        originalScriptElement[attributeName] || originalScriptElement.getAttribute(attributeName));
                }

                if (sourceUri) {
                    newScriptElement.src = sourceUri;
                }

                originalScriptElement.parentNode.replaceChild(newScriptElement, originalScriptElement);
            }
        });
    }
};