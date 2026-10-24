// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;

namespace AnointedAutomation.SSO.UI
{
    /// <summary>
    /// Open-redirect safe return targets: only same-site absolute paths such as <c>/account</c> are kept.
    /// Same rule as <c>safeRelativePath</c> in <c>@anointedautomation/sso</c>.
    /// </summary>
    public static class AnointedReturnTo
    {
        /// <summary>The longest return path that is kept.</summary>
        public const int MaxLength = 2048;

        /// <summary>
        /// Keeps a return path only when it is a same-site absolute path. Blocks <c>//host</c>, <c>/\host</c>,
        /// absolute URLs, control characters and backslashes.
        /// </summary>
        /// <param name="value">The requested target.</param>
        /// <returns>The value, or null when it is unsafe or empty.</returns>
        public static string SafeRelativePath(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > MaxLength)
            {
                return null;
            }
            if (value[0] != '/' || value.StartsWith("//", StringComparison.Ordinal) || value.StartsWith("/\\", StringComparison.Ordinal))
            {
                return null;
            }
            foreach (char c in value)
            {
                if (c < ' ' || c == '\u007f' || c == '\\')
                {
                    return null;
                }
            }
            return value;
        }
    }
}
