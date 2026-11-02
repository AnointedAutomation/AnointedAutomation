// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// Resolves the client IP, trusting forwarded headers only from trusted proxies.
    /// </summary>
    public static class ClientIpResolver
    {
        /// <summary>
        /// Resolves the client IP. The direct peer is the answer unless it is a trusted proxy; then
        /// CF-Connecting-IP (when enabled) or the right-most untrusted X-Forwarded-For entry is used.
        /// </summary>
        /// <param name="context">The request.</param>
        /// <param name="trustedProxies">The trusted proxies.</param>
        /// <param name="honorCfConnectingIp">Whether CF-Connecting-IP may be used.</param>
        /// <returns>The client IP, or null when the request has no peer address.</returns>
        public static IPAddress Resolve(HttpContext context, IpNetworkList trustedProxies, bool honorCfConnectingIp)
        {
            IPAddress peer = context.Connection.RemoteIpAddress;
            if (peer == null || trustedProxies == null || !trustedProxies.Contains(peer))
            {
                return peer;
            }
            if (honorCfConnectingIp)
            {
                string cf = context.Request.Headers["CF-Connecting-IP"].ToString();
                IPAddress cfAddress;
                if (TryParseEntry(cf, out cfAddress))
                {
                    return cfAddress;
                }
            }
            string forwarded = context.Request.Headers["X-Forwarded-For"].ToString();
            if (string.IsNullOrWhiteSpace(forwarded))
            {
                return peer;
            }
            string[] entries = forwarded.Split(',');
            IPAddress leftMost = null;
            for (int i = entries.Length - 1; i >= 0; i--)
            {
                IPAddress parsed;
                if (!TryParseEntry(entries[i], out parsed))
                {
                    // A garbled hop cannot be trusted past; stop at the last good address we know of.
                    return leftMost != null ? leftMost : peer;
                }
                if (!trustedProxies.Contains(parsed))
                {
                    return parsed;
                }
                leftMost = parsed;
            }
            return leftMost != null ? leftMost : peer;
        }

        /// <summary>
        /// Whether the direct peer is a trusted proxy.
        /// </summary>
        /// <param name="context">The request.</param>
        /// <param name="trustedProxies">The trusted proxies.</param>
        /// <returns>True when the peer is trusted.</returns>
        public static bool IsPeerTrusted(HttpContext context, IpNetworkList trustedProxies)
        {
            IPAddress peer = context.Connection.RemoteIpAddress;
            return peer != null && trustedProxies != null && trustedProxies.Contains(peer);
        }

        /// <summary>
        /// Parses one header entry, tolerating a port ("1.2.3.4:80", "[2001:db8::1]:80").
        /// </summary>
        /// <param name="text">The entry.</param>
        /// <param name="address">The parsed address.</param>
        /// <returns>True when parsed.</returns>
        internal static bool TryParseEntry(string text, out IPAddress address)
        {
            address = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            string value = text.Trim();
            if (value.StartsWith("[", StringComparison.Ordinal))
            {
                int close = value.IndexOf(']');
                if (close < 0)
                {
                    return false;
                }
                value = value.Substring(1, close - 1);
            }
            else
            {
                int firstColon = value.IndexOf(':');
                if (firstColon >= 0 && firstColon == value.LastIndexOf(':'))
                {
                    value = value.Substring(0, firstColon);
                }
            }
            return IPAddress.TryParse(value, out address);
        }
    }
}
