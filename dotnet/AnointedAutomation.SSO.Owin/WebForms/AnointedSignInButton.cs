// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;
using System.ComponentModel;
using System.Web.UI;

namespace AnointedAutomation.SSO.Owin.WebForms
{
    /// <summary>
    /// The "Sign in with Anointed Automation" button for Web Forms. Register it with
    /// <c>&lt;%@ Register Assembly="AnointedAutomation.SSO.Owin" Namespace="AnointedAutomation.SSO.Owin.WebForms" TagPrefix="anointed" %&gt;</c>
    /// and use <c>&lt;anointed:AnointedSignInButton runat="server" StartUrl="~/auth/anointed/start" /&gt;</c>.
    /// </summary>
    [ToolboxData("<{0}:AnointedSignInButton runat=\"server\" StartUrl=\"~/auth/anointed/start\" />")]
    public class AnointedSignInButton : Control
    {
        /// <summary>Required. Your start route; <c>~/</c> is resolved against the app root.</summary>
        [Bindable(true)]
        public string StartUrl { get; set; }

        /// <summary>Optional local path to land on after sign-in.</summary>
        [Bindable(true)]
        public string ReturnTo { get; set; }

        /// <summary>The query parameter that carries <see cref="ReturnTo"/>. Default <c>returnTo</c>.</summary>
        public string ReturnToParam { get; set; } = AnointedAutomationDefaults.ReturnToParam;

        /// <summary>The button text. Default "Sign in with Anointed Automation".</summary>
        public string Label { get; set; } = AnointedAutomationDefaults.SignInLabel;

        /// <summary>When set, the built-in inline look is dropped so your CSS owns the button.</summary>
        public string CssClass { get; set; }

        /// <summary>Writes the button markup.</summary>
        /// <param name="writer">The page writer.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            string start = StartUrl;
            if (start != null && start.StartsWith("~/", StringComparison.Ordinal))
            {
                start = ResolveUrl(start);
            }
            writer.Write(AnointedSignInButtonMarkup.Render(new AnointedSignInButtonOptions
            {
                StartUrl = start,
                ReturnTo = ReturnTo,
                ReturnToParam = ReturnToParam,
                Label = Label,
                CssClass = CssClass,
            }));
        }
    }
}
