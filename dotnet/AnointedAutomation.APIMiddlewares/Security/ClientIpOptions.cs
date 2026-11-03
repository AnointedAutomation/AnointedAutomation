// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// How to find the real client IP behind proxies. Forwarded headers are honored only when the direct peer
    /// is a trusted proxy; with nothing configured they are never honored.
    /// </summary>
    public class ClientIpOptions
    {
        /// <summary>Gets or sets proxy addresses or CIDR ranges whose forwarded headers are trusted.</summary>
        public List<string> TrustedProxies { get; set; } = new List<string>();

        /// <summary>Gets or sets preset names (for example "Cloudflare") whose shipped ranges count as trusted proxies.</summary>
        public List<string> TrustedProxyProviders { get; set; } = new List<string>();

        /// <summary>Gets or sets whether CF-Connecting-IP is used before X-Forwarded-For. It is believed only when the direct peer is both a trusted proxy and a Cloudflare edge address.</summary>
        public bool HonorCfConnectingIp { get; set; }

        /// <summary>
        /// Builds the trusted proxy list.
        /// </summary>
        /// <returns>The list.</returns>
        /// <exception cref="FormatException">Thrown for an invalid entry.</exception>
        /// <exception cref="ArgumentException">Thrown for an unknown preset name.</exception>
        public IpNetworkList BuildTrustedProxies()
        {
            List<string> entries = new List<string>();
            if (TrustedProxies != null)
            {
                entries.AddRange(TrustedProxies);
            }
            if (TrustedProxyProviders != null)
            {
                foreach (string provider in TrustedProxyProviders)
                {
                    entries.AddRange(IpProviderPresets.GetShippedRanges(provider));
                }
            }
            return IpNetworkList.Parse(entries);
        }
    }
}
