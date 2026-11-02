// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using AnointedAutomation.APIMiddleware.Security;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>Honeypot route rules: a hit bans the IP.</summary>
    public class HoneypotPathOptions
    {
        /// <summary>Gets or sets whether the honeypot routes are active. Off by default.</summary>
        public bool Enabled { get; set; }

        /// <summary>The honeypot path prefixes used when <see cref="Paths"/> is left empty.</summary>
        public static readonly string[] DefaultPaths = new string[] { "/wp-admin", "/wp-login.php", "/xmlrpc.php", "/phpmyadmin", "/.git", "/cgi-bin" };

        /// <summary>The honeypot file names used when <see cref="FileNames"/> is left empty.</summary>
        public static readonly string[] DefaultFileNames = new string[] { ".env", ".git", ".aws" };

        /// <summary>
        /// Gets or sets path prefixes nobody legitimate requests (matched by segment, case-insensitive). Leave empty
        /// for <see cref="DefaultPaths"/>; anything listed REPLACES the defaults (the property is not pre-populated
        /// because configuration binding would append to a pre-populated list).
        /// </summary>
        public List<string> Paths { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets file names that are honeypots at any depth (for example ".env" matches /app/.env). Leave empty
        /// for <see cref="DefaultFileNames"/>; anything listed REPLACES the defaults.
        /// </summary>
        public List<string> FileNames { get; set; } = new List<string>();

        /// <summary>Gets the paths in effect.</summary>
        /// <returns>The configured paths, or the defaults.</returns>
        public List<string> ResolvePaths()
        {
            return Paths == null || Paths.Count == 0 ? new List<string>(DefaultPaths) : new List<string>(Paths);
        }

        /// <summary>Gets the file names in effect.</summary>
        /// <returns>The configured file names, or the defaults.</returns>
        public List<string> ResolveFileNames()
        {
            return FileNames == null || FileNames.Count == 0 ? new List<string>(DefaultFileNames) : new List<string>(FileNames);
        }

        /// <summary>Gets or sets how long a tripped IP stays banned.</summary>
        public TimeSpan BanDuration { get; set; } = TimeSpan.FromHours(24);

        /// <summary>Gets or sets the status code the bot sees on the honeypot request.</summary>
        public int ResponseStatus { get; set; } = 404;
    }

    /// <summary>User-Agent rules.</summary>
    public class UserAgentOptions
    {
        /// <summary>Gets or sets whether User-Agent rules are active. Off by default.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets whether the built-in list of scanner tools (<see cref="BotDefenseOptions.KnownBadUserAgents"/>) is blocked.</summary>
        public bool BlockKnownBad { get; set; } = true;

        /// <summary>Gets or sets extra User-Agent fragments to block (case-insensitive).</summary>
        public List<string> BlockedFragments { get; set; } = new List<string>();

        /// <summary>Gets or sets whether a request with no User-Agent is blocked.</summary>
        public bool BlockEmpty { get; set; }

        /// <summary>
        /// Gets or sets crawlers that pass every rule when reverse DNS confirms them; a claimed crawler that fails is
        /// blocked as spoofed. Leave empty for <see cref="BotDefenseOptions.DefaultGoodCrawlers"/>; anything listed REPLACES the defaults.
        /// </summary>
        public List<GoodCrawler> GoodCrawlers { get; set; } = new List<GoodCrawler>();

        /// <summary>Gets the crawlers in effect.</summary>
        /// <returns>The configured crawlers, or the defaults.</returns>
        public List<GoodCrawler> ResolveGoodCrawlers()
        {
            return GoodCrawlers == null || GoodCrawlers.Count == 0 ? BotDefenseOptions.DefaultGoodCrawlers() : new List<GoodCrawler>(GoodCrawlers);
        }
    }

    /// <summary>One path-specific rate limit.</summary>
    public class RouteRateLimit
    {
        /// <summary>Gets or sets the path prefix.</summary>
        public string PathPrefix { get; set; }

        /// <summary>Gets or sets the requests allowed per window per IP.</summary>
        public int Limit { get; set; }

        /// <summary>Gets or sets the window length.</summary>
        public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
    }

    /// <summary>Rate limiting.</summary>
    public class RateLimitOptions
    {
        /// <summary>Gets or sets whether rate limiting is active. Off by default.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets requests per IP per window across all paths (0 disables the global limit).</summary>
        public int PerIpLimit { get; set; }

        /// <summary>Gets or sets the global window length.</summary>
        public TimeSpan PerIpWindow { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>Gets or sets per-route limits; the longest matching prefix applies.</summary>
        public List<RouteRateLimit> Routes { get; set; } = new List<RouteRateLimit>();
    }

    /// <summary>
    /// Options for <see cref="BotDefenseMiddleware"/>. Off by default and each rule is opt in.
    /// </summary>
    public class BotDefenseOptions
    {
        /// <summary>The configuration section name.</summary>
        public const string SectionName = "BotDefense";

        /// <summary>User-Agent fragments of well-known scanner and exploit tools. Generic HTTP clients (curl, python-requests) are deliberately not listed because API clients use them.</summary>
        public static readonly string[] KnownBadUserAgents = new string[]
        {
            "sqlmap", "nikto", "nmap", "masscan", "zgrab", "acunetix", "nessus", "dirbuster", "gobuster",
            "wpscan", "havij", "netsparker", "openvas", "nuclei", "w3af", "arachni", "burpcollaborator"
        };

        /// <summary>Gets or sets whether bot defense is active. Off by default.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets path prefixes that skip every rule (health checks, webhooks).</summary>
        public List<string> ExemptPaths { get; set; } = new List<string>();

        /// <summary>Gets or sets how the client IP is found behind proxies.</summary>
        public ClientIpOptions ClientIp { get; set; } = new ClientIpOptions();

        /// <summary>Gets or sets the honeypot route rules.</summary>
        public HoneypotPathOptions HoneypotPaths { get; set; } = new HoneypotPathOptions();

        /// <summary>Gets or sets the User-Agent rules.</summary>
        public UserAgentOptions UserAgent { get; set; } = new UserAgentOptions();

        /// <summary>Gets or sets the rate limits.</summary>
        public RateLimitOptions RateLimit { get; set; } = new RateLimitOptions();

        /// <summary>Gets or sets whether a ban store failure blocks the request (true) or lets it through (false). Either way the failure is logged at Error.</summary>
        public bool BlockWhenBanStoreFails { get; set; }

        /// <summary>
        /// The crawlers allowed by default, verified by reverse DNS.
        /// </summary>
        /// <returns>A fresh list.</returns>
        public static List<GoodCrawler> DefaultGoodCrawlers()
        {
            return new List<GoodCrawler>
            {
                new GoodCrawler { Name = "Googlebot", UserAgentFragment = "Googlebot", HostSuffixes = new List<string> { ".googlebot.com", ".google.com" } },
                new GoodCrawler { Name = "Bingbot", UserAgentFragment = "bingbot", HostSuffixes = new List<string> { ".search.msn.com" } },
                new GoodCrawler { Name = "Applebot", UserAgentFragment = "Applebot", HostSuffixes = new List<string> { ".applebot.apple.com" } },
                new GoodCrawler { Name = "YandexBot", UserAgentFragment = "YandexBot", HostSuffixes = new List<string> { ".yandex.ru", ".yandex.net", ".yandex.com" } },
                new GoodCrawler { Name = "Baiduspider", UserAgentFragment = "Baiduspider", HostSuffixes = new List<string> { ".baidu.com", ".baidu.jp" } }
            };
        }

        /// <summary>
        /// Throws when the options cannot work, so a bad configuration crashes at startup.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown for missing or non-positive values.</exception>
        /// <exception cref="ArgumentException">Thrown for a bad path.</exception>
        public void Validate()
        {
            if (!Enabled)
            {
                return;
            }
            new PathExemptions(ExemptPaths);
            if (ClientIp == null || HoneypotPaths == null || UserAgent == null || RateLimit == null)
            {
                throw new InvalidOperationException("BotDefense option groups must not be null.");
            }
            ClientIp.BuildTrustedProxies();
            if (HoneypotPaths.Enabled)
            {
                new PathExemptions(HoneypotPaths.ResolvePaths());
                if (HoneypotPaths.BanDuration <= TimeSpan.Zero)
                {
                    throw new InvalidOperationException("BotDefense HoneypotPaths.BanDuration must be positive.");
                }
                if (HoneypotPaths.ResponseStatus < 100 || HoneypotPaths.ResponseStatus > 599)
                {
                    throw new InvalidOperationException("BotDefense HoneypotPaths.ResponseStatus must be an HTTP status code.");
                }
            }
            if (UserAgent.Enabled)
            {
                foreach (GoodCrawler crawler in UserAgent.ResolveGoodCrawlers())
                {
                    if (string.IsNullOrWhiteSpace(crawler.UserAgentFragment) || crawler.HostSuffixes == null || crawler.HostSuffixes.Count == 0)
                    {
                        throw new InvalidOperationException("BotDefense good crawler '" + crawler.Name + "' needs a UserAgentFragment and HostSuffixes.");
                    }
                }
            }
            if (RateLimit.Enabled)
            {
                if (RateLimit.PerIpLimit < 0 || (RateLimit.PerIpLimit > 0 && RateLimit.PerIpWindow <= TimeSpan.Zero))
                {
                    throw new InvalidOperationException("BotDefense RateLimit.PerIpLimit/PerIpWindow are invalid.");
                }
                if (RateLimit.PerIpLimit == 0 && (RateLimit.Routes == null || RateLimit.Routes.Count == 0))
                {
                    throw new InvalidOperationException("BotDefense RateLimit is enabled but has no PerIpLimit and no Routes.");
                }
                if (RateLimit.Routes != null)
                {
                    foreach (RouteRateLimit route in RateLimit.Routes)
                    {
                        new PathExemptions(new string[] { route.PathPrefix });
                        if (route.Limit <= 0 || route.Window <= TimeSpan.Zero)
                        {
                            throw new InvalidOperationException("BotDefense route limit for '" + route.PathPrefix + "' needs a positive Limit and Window.");
                        }
                    }
                }
            }
        }
    }
}
