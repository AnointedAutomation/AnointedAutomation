// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// The live allowlist: configured entries plus each provider's current ranges. Provider lists start as the
    /// shipped ranges and are swapped atomically on a successful refresh; nothing ever empties them.
    /// <para>
    /// Two different jobs use provider presets, and they are matched differently. <c>Providers</c> means "accept
    /// traffic that arrives through this CDN" and is matched against the DIRECT peer. <c>Allowlist</c> entries are
    /// matched against the resolved client IP. <c>ClientIp.TrustedProxyProviders</c> decides whose forwarded headers
    /// are believed, and those ranges are refreshed too.
    /// </para>
    /// </summary>
    public sealed class IpAllowlistState
    {
        private readonly IpNetworkList _configured;
        private readonly IpNetworkList _configuredProxies;
        private readonly HashSet<string> _allowProviders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _proxyProviders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, IpNetworkList> _lists = new ConcurrentDictionary<string, IpNetworkList>(StringComparer.OrdinalIgnoreCase);
        private readonly object _gate = new object();
        private IpNetworkList _trusted;

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
            _configuredProxies = IpNetworkList.Parse(options.ClientIp.TrustedProxies);
            Exemptions = new PathExemptions(options.ExemptPaths);
            AddProviders(options.Providers, _allowProviders);
            AddProviders(options.ClientIp.TrustedProxyProviders, _proxyProviders);
            RebuildTrusted();
        }

        /// <summary>Gets the options.</summary>
        public IpAllowlistOptions Options { get; }

        /// <summary>Gets the path exemptions.</summary>
        public PathExemptions Exemptions { get; }

        /// <summary>Gets the current trusted proxies (configured entries plus the current trusted provider ranges).</summary>
        public IpNetworkList TrustedProxies
        {
            get { return Volatile.Read(ref _trusted); }
        }

        /// <summary>
        /// Gets the Cloudflare edge ranges used to decide whether CF-Connecting-IP is believed: the refreshed
        /// Cloudflare list when the Cloudflare preset is in use here, otherwise the ranges shipped with the package.
        /// </summary>
        public IpNetworkList CloudflareEdge
        {
            get
            {
                IpNetworkList refreshed;
                if (_lists.TryGetValue(IpProviderPresets.Cloudflare, out refreshed))
                {
                    return refreshed;
                }
                return ClientIpResolver.CloudflareEdge;
            }
        }

        /// <summary>Gets the names of every provider preset in use (allowed or trusted as proxy), the ones a refresh covers.</summary>
        public IReadOnlyCollection<string> ProviderNames
        {
            get { return new List<string>(_lists.Keys); }
        }

        /// <summary>
        /// Finds the rule that allows the request.
        /// </summary>
        /// <param name="client">The resolved client address (matched against <c>Allowlist</c>).</param>
        /// <param name="peer">The direct peer address (matched against <c>Providers</c>).</param>
        /// <returns>The matched rule text ("203.0.113.0/24" or "Cloudflare:173.245.48.0/20"), or null when none allows it.</returns>
        public string FindAllowingRule(IPAddress client, IPAddress peer)
        {
            IpNetwork configured = _configured.FindMatch(client);
            if (configured != null)
            {
                return configured.Text;
            }
            foreach (string name in _allowProviders)
            {
                IpNetwork match = _lists[name].FindMatch(peer);
                if (match != null)
                {
                    return name + ":" + match.Text;
                }
            }
            return null;
        }

        /// <summary>
        /// Replaces a provider's ranges with a refreshed, fully validated list.
        /// </summary>
        /// <param name="providerName">The preset name.</param>
        /// <param name="ranges">The new ranges (must be non-empty).</param>
        /// <exception cref="ArgumentException">Thrown when the list is empty or the provider is not in use.</exception>
        public void ReplaceProvider(string providerName, IpNetworkList ranges)
        {
            if (ranges == null || ranges.Count == 0)
            {
                throw new ArgumentException("A refreshed provider list must not be empty.", nameof(ranges));
            }
            string name = IpProviderPresets.GetSource(providerName).Name;
            lock (_gate)
            {
                if (!_lists.ContainsKey(name))
                {
                    throw new ArgumentException("Provider '" + name + "' is not in use.", nameof(providerName));
                }
                _lists[name] = ranges;
                RebuildTrusted();
            }
        }

        /// <summary>
        /// Gets how many ranges a provider currently holds.
        /// </summary>
        /// <param name="providerName">The preset name.</param>
        /// <returns>The count, or 0 when the provider is not in use.</returns>
        public int GetProviderRangeCount(string providerName)
        {
            IpNetworkList list;
            return _lists.TryGetValue(providerName, out list) ? list.Count : 0;
        }

        private void AddProviders(List<string> providers, HashSet<string> target)
        {
            if (providers == null)
            {
                return;
            }
            foreach (string provider in providers)
            {
                string name = IpProviderPresets.GetSource(provider).Name;
                target.Add(name);
                _lists.GetOrAdd(name, key => IpNetworkList.Parse(IpProviderPresets.GetShippedRanges(key)));
            }
        }

        private void RebuildTrusted()
        {
            List<IpNetwork> all = new List<IpNetwork>();
            all.AddRange(_configuredProxies.All);
            foreach (string name in _proxyProviders)
            {
                all.AddRange(_lists[name].All);
            }
            Volatile.Write(ref _trusted, new IpNetworkList(all));
        }
    }
}
