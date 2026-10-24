// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Encodings.Web;

namespace AnointedAutomation.SSO.UI.TagHelpers
{
    /// <summary>
    /// <c>&lt;anointed-sign-out action="/auth/anointed/sign-out" /&gt;</c> renders a POST form with an antiforgery
    /// token and a sign-out button. Pass <c>class</c> to drop the built-in inline look.
    /// </summary>
    [HtmlTargetElement("anointed-sign-out", TagStructure = TagStructure.WithoutEndTag)]
    public class AnointedSignOutTagHelper : TagHelper
    {
        /// <summary>Required. The sign-out route, for example <c>/auth/anointed/sign-out</c>.</summary>
        [HtmlAttributeName("action")]
        public string Action { get; set; }

        /// <summary>Where to land after sign-out. Omit for the server's default.</summary>
        [HtmlAttributeName("return-to")]
        public string ReturnTo { get; set; }

        /// <summary>The form field your server reads. Default <c>returnTo</c>.</summary>
        [HtmlAttributeName("return-to-param")]
        public string ReturnToParam { get; set; } = AnointedButtonLook.DefaultReturnToParam;

        /// <summary>Button text. Default "Sign out".</summary>
        [HtmlAttributeName("label")]
        public string Label { get; set; } = AnointedButtonLook.DefaultSignOutLabel;

        /// <summary>The current view context (set by Razor).</summary>
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (string.IsNullOrWhiteSpace(Action))
            {
                throw new InvalidOperationException("AnointedAutomation.SSO.UI: <anointed-sign-out> needs action, the path of your sign-out route.");
            }
            if (ViewContext == null)
            {
                throw new InvalidOperationException("AnointedAutomation.SSO.UI: <anointed-sign-out> must render inside a Razor view or page.");
            }
            HttpContext http = ViewContext.HttpContext;
            IAntiforgery antiforgery = http.RequestServices.GetRequiredService<IAntiforgery>();
            AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(http);
            Render(output, Action, ReturnTo, ReturnToParam, Label, tokens.FormFieldName, tokens.RequestToken);
        }

        /// <summary>Writes the form markup.</summary>
        /// <param name="output">The output.</param>
        /// <param name="action">The sign-out route.</param>
        /// <param name="returnTo">The return target or null.</param>
        /// <param name="returnToParam">The return field name.</param>
        /// <param name="label">The button text.</param>
        /// <param name="tokenField">The antiforgery form field name.</param>
        /// <param name="token">The antiforgery request token.</param>
        internal static void Render(TagHelperOutput output, string action, string returnTo, string returnToParam, string label, string tokenField, string token)
        {
            HtmlEncoder encoder = HtmlEncoder.Default;
            bool styled = !output.Attributes.ContainsName("class");
            string buttonClass = styled ? string.Empty : " class=\"" + encoder.Encode(output.Attributes["class"].Value.ToString()) + "\"";
            if (!styled)
            {
                output.Attributes.RemoveAll("class");
            }
            output.TagName = "form";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("method", "post");
            output.Attributes.SetAttribute("action", action);
            output.Attributes.SetAttribute("data-anointed-sign-out", string.Empty);
            output.Attributes.SetAttribute("style", "display:inline;margin:0;");
            string html = "<input type=\"hidden\" name=\"" + encoder.Encode(tokenField) + "\" value=\"" + encoder.Encode(token) + "\" />";
            if (returnTo != null)
            {
                html += "<input type=\"hidden\" name=\"" + encoder.Encode(returnToParam) + "\" value=\"" + encoder.Encode(returnTo) + "\" />";
            }
            string style = styled ? " style=\"" + encoder.Encode(AnointedButtonLook.ButtonStyle) + "\"" : string.Empty;
            html += "<button type=\"submit\"" + buttonClass + style + ">" + AnointedTagMarkup.LogoAndLabel(label) + "</button>";
            output.Content.SetHtmlContent(html);
        }
    }
}
