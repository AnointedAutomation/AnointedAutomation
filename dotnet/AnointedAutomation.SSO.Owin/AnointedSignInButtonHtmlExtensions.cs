// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;
using System.Web;
using System.Web.Mvc;

namespace AnointedAutomation.SSO.Owin
{
    /// <summary>Razor (MVC 5) helpers for the "Sign in with Anointed Automation" button.</summary>
    public static class AnointedSignInButtonHtmlExtensions
    {
        /// <summary>Renders the brand button linking to your start route.</summary>
        /// <param name="html">The Razor HTML helper.</param>
        /// <param name="startUrl">Your start route, for example <c>~/auth/anointed/start</c> or <c>/auth/anointed/start</c>.</param>
        /// <returns>The button markup.</returns>
        public static MvcHtmlString AnointedSignInButton(this HtmlHelper html, string startUrl)
        {
            return AnointedSignInButton(html, new AnointedSignInButtonOptions { StartUrl = startUrl });
        }

        /// <summary>Renders the brand button linking to your start route with a return path.</summary>
        /// <param name="html">The Razor HTML helper.</param>
        /// <param name="startUrl">Your start route.</param>
        /// <param name="returnTo">The local path to land on after sign-in.</param>
        /// <returns>The button markup.</returns>
        public static MvcHtmlString AnointedSignInButton(this HtmlHelper html, string startUrl, string returnTo)
        {
            return AnointedSignInButton(html, new AnointedSignInButtonOptions { StartUrl = startUrl, ReturnTo = returnTo });
        }

        /// <summary>Renders the brand button from full settings.</summary>
        /// <param name="html">The Razor HTML helper.</param>
        /// <param name="options">The button settings; a <c>~/</c> start URL is resolved against the app root.</param>
        /// <returns>The button markup.</returns>
        public static MvcHtmlString AnointedSignInButton(this HtmlHelper html, AnointedSignInButtonOptions options)
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            AnointedSignInButtonOptions resolved = new AnointedSignInButtonOptions
            {
                StartUrl = ResolveAppRelative(options.StartUrl),
                ReturnTo = options.ReturnTo,
                ReturnToParam = options.ReturnToParam,
                Label = options.Label,
                CssClass = options.CssClass,
            };
            return MvcHtmlString.Create(AnointedSignInButtonMarkup.Render(resolved));
        }

        /// <summary>Turns <c>~/path</c> into an absolute path under the app's virtual directory.</summary>
        /// <param name="url">The configured URL.</param>
        /// <returns>The resolved URL, or the input when it does not start with <c>~/</c>.</returns>
        private static string ResolveAppRelative(string url)
        {
            if (url != null && url.StartsWith("~/", StringComparison.Ordinal))
            {
                return VirtualPathUtility.ToAbsolute(url);
            }
            return url;
        }
    }
}
