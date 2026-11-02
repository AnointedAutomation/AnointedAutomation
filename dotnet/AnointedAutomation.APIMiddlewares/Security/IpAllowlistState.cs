// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// The live allowlist: configured entries plus each provider's current ranges. Provider lists start as the
    /// shipped ranges and are swapped atomically on a successful refresh; nothing ever empties them.
    /// </summary>
    public sealed class IpAllowlistState
    {
        private readonly IpNetworkList _configured;
        private readonly ConcurrentDictionary<string, IpNetworkList> _providers = new ConcurrentDictionary<string, IpNetworkList>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes the state from validated options.
        /// </summary>
        /// <param name="options">The options.</param>
        public IpAllowlistState(IpAllowlistOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            options.Validate();
            Options = options;
            _configured = IpNetworkList.Parse(options.Allowlist);
            Exemptions = new PathExemptions(options.ExemptPaths);
            TrustedProxies = options.ClientIp.BuildTrustedProxies();
            if (options.Providers != null)
            {
                foreach (string provider in options.Providers)
                {
                    string name = IpProviderPresets.GetSource(provider).Name;
                    _providers[name] = IpNetworkList.Parse(IpProviderPresets.GetShippedRanges(name));
                }
            }
        }

        /// <summary>Gets the options.</summary>
        public IpAllowlistOptions Options { get; }

        /// <summary>Gets the path exemptions.</summary>
        public PathExemptions Exemptions { get; }

        /// <summary>Gets the trusted proxies.</summary>
        public IpNetworkList TrustedProxies { get; }

        /// <summary>Gets the names of the active provider presets.</summary>
        public IReadOnlyCollection<string> ProviderNames
        {
            get { return new List<string>(_providers.Keys); }
        }

        /// <summary>
        /// Finds the rule that allows the address.
        /// </summary>
        /// <param name="address">The client address.</param>
        /// <returns>The matched rule text ("203.0.113.0/24" or "Cloudflare:173.245.48.0/20"), or null when none allows it.</returns>
        public string FindAllowingRule(IPAddress address)
        {
            IpNetwork configured = _configured.FindMatch(address);
            if (configured != null)
            {
                return configured.Text;
            }
            foreach (KeyValuePair<string, IpNetworkList> provider in _providers)
            {
                IpNetwork match = provider.Value.FindMatch(address);
                if (match != null)
                {
                    return provider.Key + ":" + match.Text;
                }
            }
            return null;
        }

        /// <summary>
        /// Replaces a provider's ranges with a refreshed, fully validated list.
        /// </summary>
        /// <param name="providerName">The preset name.</param>
        /// <param name="ranges">The new ranges (must be non-empty).</param>
        /// <exception cref="ArgumentException">Thrown when the list is empty or the provider is not active.</exception>
        public void ReplaceProvider(string providerName, IpNetworkList ranges)
        {
            if (ranges == null || ranges.Count == 0)
            {
                throw new ArgumentException("A refreshed provider list must not be empty.", nameof(ranges));
            }
            string name = IpProviderPresets.GetSource(providerName).Name;
            if (!_providers.ContainsKey(name))
            {
                throw new ArgumentException("Provider '" + name + "' is not active.", nameof(providerName));
            }
            _providers[name] = ranges;
        }

        /// <summary>
        /// Gets how many ranges a provider currently holds.
        /// </summary>
        /// <param name="providerName">The preset name.</param>
        /// <returns>The count, or 0 when the provider is not active.</returns>
        public int GetProviderRangeCount(string providerName)
        {
            IpNetworkList list;
            return _providers.TryGetValue(providerName, out list) ? list.Count : 0;
        }
    }
}
