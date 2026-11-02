// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AnointedAutomation.APIMiddleware.Security;
using AnointedAutomation.APIMiddleware.Utility;
using AnointedAutomation.Optimization.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>
    /// Opt-in bot defense: temporary bans, honeypot routes, User-Agent rules with reverse-DNS verified good
    /// crawlers, and per-IP / per-route rate limits. Every block is logged at Warning with the IP, rule and path.
    /// Order: exempt paths, verified crawlers, ban check, honeypot routes, User-Agent rules, rate limits.
    /// </summary>
    public class BotDefenseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BotDefenseOptions _options;
        private readonly IBanStore _bans;
        private readonly ICrawlerVerifier _crawlers;
        private readonly FixedWindowRateLimiter _limiter;
        private readonly PathExemptions _exemptions;
        private readonly IpNetworkList _trustedProxies;
        private readonly PathExemptions _honeypotPaths;
        private readonly HashSet<string> _honeypotFileNames;
        private readonly List<string> _badFragments = new List<string>();
        private readonly List<GoodCrawler> _goodCrawlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotDefenseMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="options">The options (validated here).</param>
        /// <param name="bans">The ban store.</param>
        /// <param name="crawlers">The crawler verifier.</param>
        /// <param name="limiter">The rate limiter.</param>
        public BotDefenseMiddleware(RequestDelegate next, IOptions<BotDefenseOptions> options, IBanStore bans, ICrawlerVerifier crawlers, FixedWindowRateLimiter limiter)
        {
            _next = next;
            _options = options.Value;
            _bans = bans;
            _crawlers = crawlers;
            _limiter = limiter;
            _options.Validate();
            _exemptions = new PathExemptions(_options.ExemptPaths);
            _trustedProxies = _options.ClientIp.BuildTrustedProxies();
            _honeypotPaths = new PathExemptions(_options.HoneypotPaths.ResolvePaths());
            _honeypotFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string fileName in _options.HoneypotPaths.ResolveFileNames())
            {
                _honeypotFileNames.Add(fileName);
            }
            _goodCrawlers = _options.UserAgent.ResolveGoodCrawlers();
            if (_options.UserAgent.BlockKnownBad)
            {
                _badFragments.AddRange(BotDefenseOptions.KnownBadUserAgents);
            }
            if (_options.UserAgent.BlockedFragments != null)
            {
                _badFragments.AddRange(_options.UserAgent.BlockedFragments);
            }
        }

        /// <summary>
        /// Applies the rules to the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!_options.Enabled || _exemptions.IsExempt(context.Request.Path))
            {
                await _next(context);
                return;
            }
            IPAddress client = ClientIpResolver.Resolve(context, _trustedProxies, _options.ClientIp.HonorCfConnectingIp);
            if (client != null && client.IsIPv4MappedToIPv6)
            {
                // Dual-mode sockets report an IPv4 peer as ::ffff:a.b.c.d; one canonical form keeps ban and rate-limit keys unified.
                client = client.MapToIPv4();
            }
            string ip = client == null ? null : client.ToString();
            string userAgent = context.Request.Headers["User-Agent"].ToString();

            if (_options.UserAgent.Enabled)
            {
                GoodCrawler claimed = FindClaimedCrawler(userAgent);
                if (claimed != null)
                {
                    bool verified = client != null && await _crawlers.VerifyAsync(client, claimed.HostSuffixes, context.RequestAborted);
                    if (verified)
                    {
                        await _next(context);
                        return;
                    }
                    Block(context, StatusCodes.Status403Forbidden, ip, "spoofed crawler: claims " + claimed.Name + " but reverse DNS does not confirm it");
                    return;
                }
            }

            if (ip != null && await IsBannedAsync(context, ip))
            {
                return;
            }

            if (_options.HoneypotPaths.Enabled && IsHoneypot(context.Request.Path))
            {
                await BanAsync(ip, "honeypot path " + context.Request.Path.Value);
                Block(context, _options.HoneypotPaths.ResponseStatus, ip, "honeypot path hit, IP banned for " + _options.HoneypotPaths.BanDuration);
                return;
            }

            if (_options.UserAgent.Enabled)
            {
                string reason = CheckUserAgent(userAgent);
                if (reason != null)
                {
                    Block(context, StatusCodes.Status403Forbidden, ip, reason);
                    return;
                }
            }

            if (_options.RateLimit.Enabled && ip != null && CheckRateLimit(context, ip))
            {
                return;
            }

            await _next(context);
        }

        private GoodCrawler FindClaimedCrawler(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return null;
            }
            foreach (GoodCrawler crawler in _goodCrawlers)
            {
                if (userAgent.Contains(crawler.UserAgentFragment, StringComparison.OrdinalIgnoreCase))
                {
                    return crawler;
                }
            }
            return null;
        }

        private string CheckUserAgent(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return _options.UserAgent.BlockEmpty ? "empty User-Agent" : null;
            }
            foreach (string fragment in _badFragments)
            {
                if (userAgent.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                {
                    return "User-Agent matches bad bot '" + fragment + "'";
                }
            }
            return null;
        }

        private bool IsHoneypot(PathString path)
        {
            if (_honeypotPaths.IsExempt(path))
            {
                return true;
            }
            string value = path.Value;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            string last = value.TrimEnd('/');
            int slash = last.LastIndexOf('/');
            string name = slash >= 0 ? last.Substring(slash + 1) : last;
            return name.Length > 0 && _honeypotFileNames.Contains(name);
        }

        private async Task<bool> IsBannedAsync(HttpContext context, string ip)
        {
            BanRecord ban;
            try
            {
                ban = await _bans.GetBanAsync(ip, context.RequestAborted);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                IPBlacklistMiddleware.AddLog(LogMessage.Error(
                    "BotDefense ban store read failed for " + ip + " " + APIUtility.DescribeRequest(context) + "; "
                    + (_options.BlockWhenBanStoreFails ? "blocking (BlockWhenBanStoreFails)." : "letting the request through (BlockWhenBanStoreFails is off)."), ex));
                if (_options.BlockWhenBanStoreFails)
                {
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    return true;
                }
                return false;
            }
            if (ban == null)
            {
                return false;
            }
            Block(context, StatusCodes.Status403Forbidden, ip, "IP is banned until " + ban.ExpiresAt.ToString("u", CultureInfo.InvariantCulture) + " (" + ban.Reason + ")");
            return true;
        }

        private async Task BanAsync(string ip, string reason)
        {
            if (ip == null)
            {
                return;
            }
            try
            {
                await _bans.BanAsync(ip, reason, _options.HoneypotPaths.BanDuration, System.Threading.CancellationToken.None);
            }
            catch (Exception ex)
            {
                IPBlacklistMiddleware.AddLog(LogMessage.Error("BotDefense ban store write failed for " + ip + " (" + reason + "); the IP is not banned.", ex));
            }
        }

        private bool CheckRateLimit(HttpContext context, string ip)
        {
            TimeSpan retryAfter;
            RouteRateLimit route = FindRoute(context.Request.Path);
            if (route != null && !_limiter.TryAcquire(ip + "|" + route.PathPrefix, route.Limit, route.Window, out retryAfter))
            {
                Throttle(context, ip, "rate limit " + route.Limit + " per " + route.Window + " on " + route.PathPrefix, retryAfter);
                return true;
            }
            if (_options.RateLimit.PerIpLimit > 0 && !_limiter.TryAcquire(ip + "|*", _options.RateLimit.PerIpLimit, _options.RateLimit.PerIpWindow, out retryAfter))
            {
                Throttle(context, ip, "rate limit " + _options.RateLimit.PerIpLimit + " per " + _options.RateLimit.PerIpWindow + " per IP", retryAfter);
                return true;
            }
            return false;
        }

        private RouteRateLimit FindRoute(PathString path)
        {
            RouteRateLimit best = null;
            if (_options.RateLimit.Routes == null)
            {
                return null;
            }
            foreach (RouteRateLimit route in _options.RateLimit.Routes)
            {
                if (path.StartsWithSegments(new PathString(route.PathPrefix.TrimEnd('/')), StringComparison.OrdinalIgnoreCase)
                    && (best == null || route.PathPrefix.Length > best.PathPrefix.Length))
                {
                    best = route;
                }
            }
            return best;
        }

        private static void Throttle(HttpContext context, string ip, string rule, TimeSpan retryAfter)
        {
            int seconds = (int)Math.Max(1, Math.Ceiling(retryAfter.TotalSeconds));
            context.Response.Headers["Retry-After"] = seconds.ToString(CultureInfo.InvariantCulture);
            Block(context, StatusCodes.Status429TooManyRequests, ip, rule);
        }

        private static void Block(HttpContext context, int status, string ip, string rule)
        {
            context.Response.StatusCode = status;
            IPBlacklistMiddleware.AddLog(LogMessage.Warning(
                "BotDefense blocked " + (ip == null ? "(no client address)" : ip) + " " + APIUtility.DescribeRequest(context) + ": " + rule + "."));
        }
    }
}
