// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System.Net;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// Maps an IP address to an ISO 3166-1 alpha-2 country code. Implement this to plug in any data source
    /// (for example a MaxMind GeoLite2 reader); the package ships a keyless one built on the public RIR files.
    /// </summary>
    public interface ICountryLookup
    {
        /// <summary>
        /// Looks up the country of an address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>The upper-case two-letter code, or null when the country is unknown.</returns>
        string Lookup(IPAddress address);
    }
}
