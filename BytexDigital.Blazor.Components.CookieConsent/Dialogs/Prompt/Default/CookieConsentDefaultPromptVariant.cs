using System;

namespace BytexDigital.Blazor.Components.CookieConsent.Dialogs.Prompt.Default
{
    public class CookieConsentDefaultPromptVariant : CookieConsentPromptVariantBase
    {
        /// <summary>
        /// Specifies the component type to use. Do not change unless you know what you are doing.
        /// </summary>
        public override Type ComponentType { get; set; } = typeof(CookieConsentDefaultPrompt);

        /// <summary>
        ///     Positioning of the consent modal.
        /// </summary>
        public ConsentModalPosition Position { get; set; } = ConsentModalPosition.BottomCenter;

        /// <summary>
        ///     Layout of the consent modal.
        /// </summary>
        public ConsentModalLayout Layout { get; set; } = ConsentModalLayout.Cloud;

        /// <summary>
        ///     If there are preferences to manage, a second button will appear inside the consent modal. If this setting is true,
        ///     then the secondary button will show "Manage preferences". If false, it will display "Decline"/"Necessary only".
        /// </summary>
        public bool SecondaryActionOpensSettings { get; set; } = false;

        /// <summary>
        ///     If this is set to <see cref="true" />, the "Accept All" button will display at the top, with the secondary button
        ///     second.
        ///     <remarks>Defaults to <see cref="false" />.</remarks>
        /// </summary>
        public bool AcceptAllButtonDisplaysFirst { get; set; } = false;
    }
}