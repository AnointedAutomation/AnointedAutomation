// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace AnointedAutomation.SSO.Owin
{
    /// <summary>What the "Sign in with Anointed Automation" button links to and how it reads.</summary>
    public class AnointedSignInButtonOptions
    {
        /// <summary>
        /// Required. Your server's start route (the action that challenges the Anointed scheme), as a root-relative
        /// path (<c>/auth/anointed/start</c>) or an absolute http(s) URL. The MVC and Web Forms helpers also resolve <c>~/</c>.
        /// </summary>
        public string StartUrl { get; set; }

        /// <summary>Optional local path (<c>/account</c>) to land on after sign-in. Your start route must check it again.</summary>
        public string ReturnTo { get; set; }

        /// <summary>The query parameter that carries <see cref="ReturnTo"/>. Default <c>returnTo</c>.</summary>
        public string ReturnToParam { get; set; } = AnointedAutomationDefaults.ReturnToParam;

        /// <summary>The button text. Default "Sign in with Anointed Automation".</summary>
        public string Label { get; set; } = AnointedAutomationDefaults.SignInLabel;

        /// <summary>When set, the built-in inline look is dropped so your CSS owns the button.</summary>
        public string CssClass { get; set; }
    }

    /// <summary>
    /// Renders the brand sign-in button as a plain link styled like a button (works without JavaScript, no
    /// secrets, no network calls of its own). The MVC <c>Html.AnointedSignInButton(...)</c> helper and the Web
    /// Forms <c>AnointedSignInButton</c> control both use this.
    /// </summary>
    public static class AnointedSignInButtonMarkup
    {
        private const string ButtonStyle = "display:inline-flex;align-items:center;justify-content:center;gap:10px;padding:10px 18px;border:1px solid #1f2a44;border-radius:8px;background:#1f2a44;color:#ffffff;font:600 15px/1.2 system-ui,-apple-system,'Segoe UI',Roboto,sans-serif;text-decoration:none;cursor:pointer";
        private const string LogoStyle = "width:20px;height:20px;display:block";

        /// <summary>Builds the button HTML.</summary>
        /// <param name="options">The button settings; <see cref="AnointedSignInButtonOptions.StartUrl"/> must already be resolved (no <c>~/</c>).</param>
        /// <returns>HTML-encoded markup, safe to write as-is.</returns>
        /// <exception cref="ArgumentException">StartUrl or ReturnTo is missing or not allowed.</exception>
        public static string Render(AnointedSignInButtonOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            string href = BuildHref(options);
            if (string.IsNullOrWhiteSpace(options.Label))
            {
                throw new ArgumentException("AnointedSignInButtonOptions.Label must not be empty.", nameof(options));
            }
            string label = options.Label;

            StringBuilder html = new StringBuilder();
            html.Append("<a href=\"").Append(WebUtility.HtmlEncode(href)).Append('"');
            if (string.IsNullOrWhiteSpace(options.CssClass))
            {
                html.Append(" style=\"").Append(WebUtility.HtmlEncode(ButtonStyle)).Append('"');
            }
            else
            {
                html.Append(" class=\"").Append(WebUtility.HtmlEncode(options.CssClass)).Append('"');
            }
            html.Append(" data-anointed-sign-in=\"\">");
            html.Append("<img src=\"").Append(AnointedAutomationDefaults.LogoUrl).Append("\" alt=\"\" aria-hidden=\"true\" width=\"20\" height=\"20\" style=\"").Append(LogoStyle).Append("\" />");
            html.Append("<span>").Append(WebUtility.HtmlEncode(label)).Append("</span></a>");
            return html.ToString();
        }

        /// <summary>The link target: the start URL plus the return path when one is given.</summary>
        /// <param name="options">The button settings.</param>
        /// <returns>The unencoded URL.</returns>
        internal static string BuildHref(AnointedSignInButtonOptions options)
        {
            string start = options.StartUrl;
            if (string.IsNullOrWhiteSpace(start))
            {
                throw new ArgumentException("AnointedSignInButtonOptions.StartUrl is required (your server's start route).", nameof(options));
            }
            if (!IsRootRelative(start) && !IsAbsoluteHttp(start))
            {
                throw new ArgumentException("AnointedSignInButtonOptions.StartUrl must be a root-relative path or an absolute http(s) URL.", nameof(options));
            }
            if (string.IsNullOrEmpty(options.ReturnTo))
            {
                return start;
            }
            if (!IsRootRelative(options.ReturnTo))
            {
                throw new ArgumentException("AnointedSignInButtonOptions.ReturnTo must be a local path starting with a single /.", nameof(options));
            }
            if (string.IsNullOrWhiteSpace(options.ReturnToParam))
            {
                throw new ArgumentException("AnointedSignInButtonOptions.ReturnToParam is required when ReturnTo is set.", nameof(options));
            }
            string separator = start.IndexOf('?') >= 0 ? "&" : "?";
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}={3}", start, separator, Uri.EscapeDataString(options.ReturnToParam), Uri.EscapeDataString(options.ReturnTo));
        }

        /// <summary>Whether the value is a path on this site (one leading slash, no backslash tricks).</summary>
        /// <param name="value">The value.</param>
        /// <returns>True for <c>/path</c>; false for <c>//host</c>, <c>/\host</c> and everything else.</returns>
        internal static bool IsRootRelative(string value)
        {
            return value.Length >= 1
                && value[0] == '/'
                && (value.Length == 1 || (value[1] != '/' && value[1] != '\\'));
        }

        /// <summary>Whether the value is an absolute http or https URL.</summary>
        /// <param name="value">The value.</param>
        /// <returns>True for http(s) URLs.</returns>
        private static bool IsAbsoluteHttp(string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out Uri uri)
                && (string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal) || string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.Ordinal));
        }
    }
}
