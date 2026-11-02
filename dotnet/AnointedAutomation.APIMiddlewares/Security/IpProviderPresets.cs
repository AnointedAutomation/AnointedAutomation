// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>The format of a provider's published range list.</summary>
    public enum IpProviderFormat
    {
        /// <summary>One CIDR per line (Cloudflare ips-v4 and ips-v6).</summary>
        LineSeparated = 0,

        /// <summary>Fastly public-ip-list JSON: addresses and ipv6_addresses arrays.</summary>
        FastlyJson = 1,

        /// <summary>AWS ip-ranges.json filtered to service CLOUDFRONT.</summary>
        AwsCloudFrontJson = 2
    }

    /// <summary>Where a provider publishes its ranges.</summary>
    public sealed class IpProviderSource
    {
        /// <summary>Initializes a new instance of the <see cref="IpProviderSource"/> class.</summary>
        /// <param name="name">The preset name.</param>
        /// <param name="urls">The official URLs.</param>
        /// <param name="format">The published format.</param>
        public IpProviderSource(string name, IReadOnlyList<string> urls, IpProviderFormat format)
        {
            Name = name;
            Urls = urls;
            Format = format;
        }

        /// <summary>Gets the preset name.</summary>
        public string Name { get; }

        /// <summary>Gets the official URLs the ranges are published at.</summary>
        public IReadOnlyList<string> Urls { get; }

        /// <summary>Gets the published format.</summary>
        public IpProviderFormat Format { get; }
    }

    /// <summary>
    /// Built-in provider presets: the official published ranges shipped with the package (source URL and fetch
    /// date are in <c>IpProviderPresets.Data.cs</c>) and where to refresh them from. Presets only widen what is
    /// matched; keep the shipped lists current by upgrading the package or enabling the scheduled refresh.
    /// </summary>
    public static partial class IpProviderPresets
    {
        /// <summary>The Cloudflare preset name.</summary>
        public const string Cloudflare = "Cloudflare";

        /// <summary>The Fastly preset name.</summary>
        public const string Fastly = "Fastly";

        /// <summary>The AWS CloudFront preset name.</summary>
        public const string CloudFront = "CloudFront";

        private static readonly Dictionary<string, IpProviderSource> Sources = new Dictionary<string, IpProviderSource>(StringComparer.OrdinalIgnoreCase)
        {
            { Cloudflare, new IpProviderSource(Cloudflare, new string[] { "https://www.cloudflare.com/ips-v4", "https://www.cloudflare.com/ips-v6" }, IpProviderFormat.LineSeparated) },
            { Fastly, new IpProviderSource(Fastly, new string[] { "https://api.fastly.com/public-ip-list" }, IpProviderFormat.FastlyJson) },
            { CloudFront, new IpProviderSource(CloudFront, new string[] { "https://ip-ranges.amazonaws.com/ip-ranges.json" }, IpProviderFormat.AwsCloudFrontJson) }
        };

        /// <summary>Gets the names of all presets.</summary>
        public static IReadOnlyList<string> Names
        {
            get { return new List<string>(Sources.Keys); }
        }

        /// <summary>
        /// Gets the source description for a preset.
        /// </summary>
        /// <param name="name">The preset name (case-insensitive).</param>
        /// <returns>The source.</returns>
        /// <exception cref="ArgumentException">Thrown for an unknown preset name.</exception>
        public static IpProviderSource GetSource(string name)
        {
            IpProviderSource source;
            if (name == null || !Sources.TryGetValue(name.Trim(), out source))
            {
                throw new ArgumentException("Unknown IP provider preset '" + name + "'. Known: " + string.Join(", ", Sources.Keys) + ".", nameof(name));
            }
            return source;
        }

        /// <summary>
        /// Gets the ranges shipped with the package for a preset.
        /// </summary>
        /// <param name="name">The preset name (case-insensitive).</param>
        /// <returns>The CIDR strings.</returns>
        /// <exception cref="ArgumentException">Thrown for an unknown preset name.</exception>
        public static IReadOnlyList<string> GetShippedRanges(string name)
        {
            string canonical = GetSource(name).Name;
            if (canonical.Equals(Cloudflare, StringComparison.Ordinal))
            {
                return CloudflareRanges;
            }
            if (canonical.Equals(Fastly, StringComparison.Ordinal))
            {
                return FastlyRanges;
            }
            return CloudFrontRanges;
        }
    }
}
