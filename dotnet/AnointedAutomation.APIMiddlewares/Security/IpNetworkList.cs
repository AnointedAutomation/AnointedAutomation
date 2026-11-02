// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Net;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// An immutable list of IP networks with first-match lookup.
    /// </summary>
    public sealed class IpNetworkList
    {
        private readonly IpNetwork[] _networks;

        /// <summary>An empty list (matches nothing).</summary>
        public static readonly IpNetworkList Empty = new IpNetworkList(new List<IpNetwork>());

        /// <summary>
        /// Initializes a new instance of the <see cref="IpNetworkList"/> class.
        /// </summary>
        /// <param name="networks">The networks.</param>
        public IpNetworkList(IEnumerable<IpNetwork> networks)
        {
            if (networks == null)
            {
                throw new ArgumentNullException(nameof(networks));
            }
            _networks = new List<IpNetwork>(networks).ToArray();
        }

        /// <summary>Gets the number of networks.</summary>
        public int Count
        {
            get { return _networks.Length; }
        }

        /// <summary>
        /// Parses every entry (address or CIDR) and throws on the first invalid one.
        /// </summary>
        /// <param name="entries">The configured entries.</param>
        /// <returns>The list.</returns>
        /// <exception cref="FormatException">Thrown when an entry is invalid.</exception>
        public static IpNetworkList Parse(IEnumerable<string> entries)
        {
            List<IpNetwork> parsed = new List<IpNetwork>();
            if (entries != null)
            {
                foreach (string entry in entries)
                {
                    parsed.Add(IpNetwork.Parse(entry));
                }
            }
            return new IpNetworkList(parsed);
        }

        /// <summary>
        /// Finds the first network containing the address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>The matching network, or null when none matches.</returns>
        public IpNetwork FindMatch(IPAddress address)
        {
            for (int i = 0; i < _networks.Length; i++)
            {
                if (_networks[i].Contains(address))
                {
                    return _networks[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Whether any network contains the address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>True when one does.</returns>
        public bool Contains(IPAddress address)
        {
            return FindMatch(address) != null;
        }
    }
}
