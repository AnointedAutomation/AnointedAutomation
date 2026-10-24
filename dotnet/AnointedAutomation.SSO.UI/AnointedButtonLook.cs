// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;
using System.Text;

namespace AnointedAutomation.SSO.UI
{
    /// <summary>
    /// The shared look and URL rules of the Anointed Automation buttons, identical to the
    /// <c>@anointedautomation/sso-react</c> button so every front end renders the same brand.
    /// </summary>
    public static class AnointedButtonLook
    {
        /// <summary>The default sign-in label.</summary>
        public const string DefaultSignInLabel = "Sign in with Anointed Automation";

        /// <summary>The default sign-out label.</summary>
        public const string DefaultSignOutLabel = "Sign out";

        /// <summary>The Anointed Automation logo shown in the button (decorative, hidden from screen readers).</summary>
        public const string LogoUrl = "https://www.anointedautomation.net/favicon.svg";

        /// <summary>The default query (or form) parameter that carries the return target.</summary>
        public const string DefaultReturnToParam = "returnTo";

        /// <summary>The built-in inline button style. Dropped when you pass your own CSS class.</summary>
        public const string ButtonStyle = "display:inline-flex;align-items:center;justify-content:center;gap:10px;padding:10px 18px;border:1px solid #1f2a44;border-radius:8px;background:#1f2a44;color:#ffffff;font:600 15px/1.2 system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif;cursor:pointer;text-decoration:none;";

        /// <summary>The inline style of the logo image.</summary>
        public const string LogoStyle = "width:20px;height:20px;display:block;";

        /// <summary>
        /// Throws when the start route is missing. There is no default start route (fail fast).
        /// </summary>
        /// <param name="startUrl">The path or URL of your server's start route.</param>
        /// <returns>The same value.</returns>
        public static string RequireStartUrl(string startUrl)
        {
            if (string.IsNullOrWhiteSpace(startUrl))
            {
                throw new InvalidOperationException("AnointedAutomation.SSO.UI: a start URL is required (the path of your start route, for example /auth/anointed/start).");
            }
            return startUrl;
        }

        /// <summary>
        /// The URL the sign-in button points at: the start route plus <c>?returnTo=&lt;target&gt;</c> when a target is
        /// given. Existing query parameters and any fragment are kept. The server validates the target again.
        /// </summary>
        /// <param name="startUrl">The start route.</param>
        /// <param name="returnTo">Where to land after sign-in, or null for none.</param>
        /// <param name="returnToParam">The parameter the server reads.</param>
        /// <returns>The link target.</returns>
        public static string BuildStartUrl(string startUrl, string returnTo, string returnToParam)
        {
            RequireStartUrl(startUrl);
            if (returnTo == null)
            {
                return startUrl;
            }
            if (string.IsNullOrWhiteSpace(returnToParam))
            {
                throw new InvalidOperationException("AnointedAutomation.SSO.UI: returnToParam must be a non-empty string.");
            }
            int hashAt = startUrl.IndexOf('#');
            string baseUrl = hashAt < 0 ? startUrl : startUrl.Substring(0, hashAt);
            string hash = hashAt < 0 ? string.Empty : startUrl.Substring(hashAt);
            char separator = baseUrl.Contains('?') ? '&' : '?';
            StringBuilder url = new StringBuilder(baseUrl);
            url.Append(separator).Append(Uri.EscapeDataString(returnToParam)).Append('=').Append(Uri.EscapeDataString(returnTo)).Append(hash);
            return url.ToString();
        }
    }
}
