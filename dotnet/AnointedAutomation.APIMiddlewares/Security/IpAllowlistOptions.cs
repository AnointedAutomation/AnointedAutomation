// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// Options for <see cref="IpAllowlistMiddleware"/>. Off by default.
    /// </summary>
    public class IpAllowlistOptions
    {
        /// <summary>The configuration section name used by the configuration overload of AddIpAllowlist.</summary>
        public const string SectionName = "IpAllowlist";

        /// <summary>Gets or sets whether the allowlist is enforced. Off by default.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets allowed single IPs and CIDR ranges, IPv4 and IPv6.</summary>
        public List<string> Allowlist { get; set; } = new List<string>();

        /// <summary>Gets or sets provider preset names whose published ranges are allowed (Cloudflare, Fastly, CloudFront).</summary>
        public List<string> Providers { get; set; } = new List<string>();

        /// <summary>Gets or sets path prefixes (health checks, webhooks) that skip the allowlist.</summary>
        public List<string> ExemptPaths { get; set; } = new List<string>();

        /// <summary>Gets or sets how the client IP is found behind proxies.</summary>
        public ClientIpOptions ClientIp { get; set; } = new ClientIpOptions();

        /// <summary>Gets or sets whether provider ranges are refreshed from the provider's published list on a schedule.</summary>
        public bool RefreshEnabled { get; set; }

        /// <summary>Gets or sets how often to refresh. The shipped ranges stay in use until the first success.</summary>
        public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromHours(24);

        /// <summary>Gets or sets the timeout of one provider download.</summary>
        public TimeSpan RefreshTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Throws when the options cannot work, so a bad configuration crashes at startup instead of failing open.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when enabled with nothing allowed or a bad interval.</exception>
        /// <exception cref="FormatException">Thrown for an invalid IP or CIDR entry.</exception>
        /// <exception cref="ArgumentException">Thrown for an unknown provider or bad exempt path.</exception>
        public void Validate()
        {
            if (!Enabled)
            {
                return;
            }
            if ((Allowlist == null || Allowlist.Count == 0) && (Providers == null || Providers.Count == 0))
            {
                throw new InvalidOperationException("IpAllowlist is enabled but Allowlist and Providers are both empty; that would block every request. Add an entry or disable it.");
            }
            IpNetworkList.Parse(Allowlist);
            if (Providers != null)
            {
                foreach (string provider in Providers)
                {
                    IpProviderPresets.GetSource(provider);
                }
            }
            new PathExemptions(ExemptPaths);
            if (ClientIp == null)
            {
                throw new InvalidOperationException("IpAllowlist ClientIp options must not be null.");
            }
            ClientIp.BuildTrustedProxies();
            if (RefreshEnabled && (RefreshInterval <= TimeSpan.Zero || RefreshTimeout <= TimeSpan.Zero))
            {
                throw new InvalidOperationException("IpAllowlist RefreshInterval and RefreshTimeout must be positive.");
            }
        }
    }
}
