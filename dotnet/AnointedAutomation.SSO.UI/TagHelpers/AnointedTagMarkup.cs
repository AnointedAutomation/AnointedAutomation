// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System.Text.Encodings.Web;

namespace AnointedAutomation.SSO.UI.TagHelpers
{
    /// <summary>Shared inner markup of the tag helper buttons.</summary>
    internal static class AnointedTagMarkup
    {
        /// <summary>The decorative logo followed by the encoded label.</summary>
        /// <param name="label">The button text.</param>
        /// <returns>HTML.</returns>
        internal static string LogoAndLabel(string label)
        {
            HtmlEncoder encoder = HtmlEncoder.Default;
            return "<img src=\"" + AnointedButtonLook.LogoUrl + "\" alt=\"\" aria-hidden=\"true\" width=\"20\" height=\"20\" style=\"" + encoder.Encode(AnointedButtonLook.LogoStyle) + "\" />"
                + "<span>" + encoder.Encode(label) + "</span>";
        }
    }
}
