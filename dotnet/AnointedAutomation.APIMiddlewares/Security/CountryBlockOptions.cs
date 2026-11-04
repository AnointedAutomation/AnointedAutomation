// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>Whether the country list is a block list or the only allowed countries.</summary>
    public enum CountryBlockMode
    {
        /// <summary>Requests from a listed country are blocked.</summary>
        Block = 1,

        /// <summary>Only requests from a listed country are allowed.</summary>
        AllowOnly = 2
    }

    /// <summary>What to do when the country of a request cannot be determined.</summary>
    public enum UnknownCountryAction
    {
        /// <summary>Block the request.</summary>
        Block = 1,

        /// <summary>Let the request through.</summary>
        Allow = 2
    }

    /// <summary>
    /// Options for <see cref="CountryBlockMiddleware"/>. Off by default. When enabled, <see cref="Mode"/> and
    /// <see cref="UnknownCountry"/> must be set explicitly: there is no silent default for either.
    /// </summary>
    public class CountryBlockOptions
    {
        /// <summary>The configuration section name.</summary>
        public const string SectionName = "CountryBlock";

        /// <summary>The five RIR delegated extended files (public, no key).</summary>
        public static readonly string[] RirDataUrls = new string[]
        {
            "https://ftp.arin.net/pub/stats/arin/delegated-arin-extended-latest",
            "https://ftp.ripe.net/pub/stats/ripencc/delegated-ripencc-extended-latest",
            "https://ftp.apnic.net/stats/apnic/delegated-apnic-extended-latest",
            "https://ftp.lacnic.net/pub/stats/lacnic/delegated-lacnic-extended-latest",
            "https://ftp.afrinic.net/pub/stats/afrinic/delegated-afrinic-extended-latest"
        };

        /// <summary>Gets or sets whether country blocking is enforced. Off by default.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets Block or AllowOnly. Required when enabled.</summary>
        public CountryBlockMode? Mode { get; set; }

        /// <summary>Gets or sets the ISO 3166-1 alpha-2 codes the mode applies to.</summary>
        public List<string> Countries { get; set; } = new List<string>();

        /// <summary>Gets or sets what happens to a request whose country is unknown. Required when enabled.</summary>
        public UnknownCountryAction? UnknownCountry { get; set; }

        /// <summary>Gets or sets whether loopback and private-network clients skip the check (they have no country).</summary>
        public bool AllowPrivateAddresses { get; set; } = true;

        /// <summary>Gets or sets whether the CF-IPCountry header is used when the direct peer is a trusted proxy that is also a Cloudflare edge address.</summary>
        public bool UseCfIpCountryHeader { get; set; }

        /// <summary>Gets or sets path prefixes (health checks, webhooks) that skip the check.</summary>
        public List<string> ExemptPaths { get; set; } = new List<string>();

        /// <summary>Gets or sets how the client IP is found behind proxies.</summary>
        public ClientIpOptions ClientIp { get; set; } = new ClientIpOptions();

        /// <summary>Gets or sets whether the RIR files are downloaded and refreshed. Set false when you register your own <see cref="ICountryLookup"/>.</summary>
        public bool DownloadRirData { get; set; } = true;

        /// <summary>
        /// Gets or sets the data URLs (RIR delegated extended format). Leave empty to use <see cref="RirDataUrls"/>;
        /// anything configured here REPLACES the defaults (the default list is not pre-populated because
        /// configuration binding would append to it).
        /// </summary>
        public List<string> DataUrls { get; set; } = new List<string>();

        /// <summary>
        /// Gets the URLs in effect: the configured ones, or the five RIR files when none are configured.
        /// </summary>
        /// <returns>The URLs.</returns>
        public List<string> ResolveDataUrls()
        {
            if (DataUrls == null || DataUrls.Count == 0)
            {
                return new List<string>(RirDataUrls);
            }
            return new List<string>(DataUrls);
        }

        /// <summary>Gets or sets how often the data is refreshed once loaded.</summary>
        public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromDays(7);

        /// <summary>Gets or sets how soon to retry while no data has loaded yet.</summary>
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>Gets or sets the timeout of one download (the files are several MB each).</summary>
        public TimeSpan RefreshTimeout { get; set; } = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Normalizes and returns the configured country codes.
        /// </summary>
        /// <returns>Upper-case codes.</returns>
        /// <exception cref="FormatException">Thrown for a code that is not two letters.</exception>
        public HashSet<string> BuildCountrySet()
        {
            HashSet<string> set = new HashSet<string>(StringComparer.Ordinal);
            if (Countries != null)
            {
                foreach (string code in Countries)
                {
                    if (code == null || !Regex.IsMatch(code.Trim(), "^[A-Za-z]{2}$"))
                    {
                        throw new FormatException("Not an ISO 3166-1 alpha-2 country code: '" + code + "'.");
                    }
                    set.Add(code.Trim().ToUpperInvariant());
                }
            }
            return set;
        }

        /// <summary>
        /// Throws when the options cannot work, so a bad configuration crashes at startup.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when a required setting is missing.</exception>
        /// <exception cref="FormatException">Thrown for an invalid country code, IP or CIDR entry.</exception>
        /// <exception cref="ArgumentException">Thrown for a bad exempt path or preset name.</exception>
        public void Validate()
        {
            if (!Enabled)
            {
                return;
            }
            if (!Mode.HasValue)
            {
                throw new InvalidOperationException("CountryBlock is enabled but Mode is not set (Block or AllowOnly).");
            }
            if (!UnknownCountry.HasValue)
            {
                throw new InvalidOperationException("CountryBlock is enabled but UnknownCountry is not set (Block or Allow).");
            }
            if (BuildCountrySet().Count == 0)
            {
                throw new InvalidOperationException("CountryBlock is enabled but Countries is empty.");
            }
            new PathExemptions(ExemptPaths);
            if (ClientIp == null)
            {
                throw new InvalidOperationException("CountryBlock ClientIp options must not be null.");
            }
            ClientIp.BuildTrustedProxies();
            if (DownloadRirData)
            {
                foreach (string url in ResolveDataUrls())
                {
                    Uri parsed;
                    if (!Uri.TryCreate(url, UriKind.Absolute, out parsed))
                    {
                        throw new FormatException("DataUrls entry is not an absolute URL: '" + url + "'.");
                    }
                }
                if (RefreshInterval <= TimeSpan.Zero || RetryInterval <= TimeSpan.Zero || RefreshTimeout <= TimeSpan.Zero)
                {
                    throw new InvalidOperationException("CountryBlock RefreshInterval, RetryInterval and RefreshTimeout must be positive.");
                }
            }
        }
    }
}
