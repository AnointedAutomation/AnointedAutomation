// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// Path prefixes (health checks, webhooks) that a security middleware lets through untouched.
    /// </summary>
    public sealed class PathExemptions
    {
        private readonly PathString[] _prefixes;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathExemptions"/> class.
        /// </summary>
        /// <param name="prefixes">Prefixes such as "/health"; each must start with "/". Matching is by path segment, case-insensitive.</param>
        /// <exception cref="ArgumentException">Thrown when a prefix does not start with "/".</exception>
        public PathExemptions(IEnumerable<string> prefixes)
        {
            List<PathString> list = new List<PathString>();
            if (prefixes != null)
            {
                foreach (string prefix in prefixes)
                {
                    if (string.IsNullOrWhiteSpace(prefix) || !prefix.StartsWith("/", StringComparison.Ordinal))
                    {
                        throw new ArgumentException("An exempt path must start with '/': '" + prefix + "'.", nameof(prefixes));
                    }
                    string trimmed = prefix.TrimEnd('/');
                    if (trimmed.Length == 0)
                    {
                        throw new ArgumentException("Exempting '/' would exempt every path; name a prefix.", nameof(prefixes));
                    }
                    list.Add(new PathString(trimmed));
                }
            }
            _prefixes = list.ToArray();
        }

        /// <summary>
        /// Whether the request path is exempt.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <returns>True when it equals or sits under an exempt prefix.</returns>
        public bool IsExempt(PathString path)
        {
            for (int i = 0; i < _prefixes.Length; i++)
            {
                if (path.StartsWithSegments(_prefixes[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
