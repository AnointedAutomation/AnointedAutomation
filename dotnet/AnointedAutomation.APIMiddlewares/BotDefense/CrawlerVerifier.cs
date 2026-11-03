// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Optimization.Logging;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>A crawler that is allowed when it proves who it is.</summary>
    public sealed class GoodCrawler
    {
        /// <summary>Gets or sets a label for logs.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the User-Agent fragment that claims to be this crawler (case-insensitive).</summary>
        public string UserAgentFragment { get; set; }

        /// <summary>Gets or sets the host name suffixes the crawler's reverse DNS must end with (for example ".googlebot.com").</summary>
        public List<string> HostSuffixes { get; set; } = new List<string>();
    }

    /// <summary>Confirms that an IP really belongs to a crawler.</summary>
    public interface ICrawlerVerifier
    {
        /// <summary>
        /// Verifies by reverse DNS plus forward confirmation.
        /// </summary>
        /// <param name="address">The client address.</param>
        /// <param name="hostSuffixes">The accepted host name suffixes.</param>
        /// <param name="cancellationToken">Cancellation.</param>
        /// <returns>True only when the address verifiably belongs to the crawler.</returns>
        Task<bool> VerifyAsync(IPAddress address, IReadOnlyList<string> hostSuffixes, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Reverse-DNS crawler verification: the PTR name must end with an accepted suffix and resolve forward back to
    /// the same address (the method Google, Bing and others document). Results are cached; a DNS failure is logged
    /// at Warning with the exception and counts as unverified (never as verified).
    /// </summary>
    public sealed class ReverseDnsCrawlerVerifier : ICrawlerVerifier
    {
        private readonly ConcurrentDictionary<string, KeyValuePair<DateTimeOffset, bool>> _cache = new ConcurrentDictionary<string, KeyValuePair<DateTimeOffset, bool>>(StringComparer.Ordinal);
        private readonly TimeSpan _cacheFor;
        private readonly Func<DateTimeOffset> _clock;
        private readonly Func<string, CancellationToken, Task<IPHostEntry>> _resolve;

        /// <summary>Initializes a verifier that caches results for 12 hours.</summary>
        public ReverseDnsCrawlerVerifier() : this(TimeSpan.FromHours(12), () => DateTimeOffset.UtcNow, (name, token) => Dns.GetHostEntryAsync(name, token))
        {
        }

        /// <summary>Initializes a verifier with a custom clock and DNS resolver (tests).</summary>
        /// <param name="cacheFor">How long a result is reused.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="resolve">Resolves an address or host name to a host entry.</param>
        public ReverseDnsCrawlerVerifier(TimeSpan cacheFor, Func<DateTimeOffset> clock, Func<string, CancellationToken, Task<IPHostEntry>> resolve)
        {
            _cacheFor = cacheFor;
            _clock = clock;
            _resolve = resolve;
        }

        /// <inheritdoc />
        public async Task<bool> VerifyAsync(IPAddress address, IReadOnlyList<string> hostSuffixes, CancellationToken cancellationToken)
        {
            if (address.IsIPv4MappedToIPv6)
            {
                address = address.MapToIPv4();
            }
            string key = address + "|" + string.Join(",", hostSuffixes);
            KeyValuePair<DateTimeOffset, bool> cached;
            if (_cache.TryGetValue(key, out cached) && cached.Key > _clock())
            {
                return cached.Value;
            }
            bool verified;
            try
            {
                verified = await ResolveAndConfirmAsync(address, hostSuffixes, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                IPBlacklistMiddleware.AddLog(LogMessage.Warning("BotDefense reverse DNS check failed for " + address + "; treating as unverified.", ex));
                return false;
            }
            Remember(key, verified);
            return verified;
        }

        /// <summary>The most results cached; beyond it new results are simply not cached.</summary>
        public const int MaxCacheEntries = 10000;

        private DateTimeOffset _nextSweep = DateTimeOffset.MinValue;

        /// <summary>Gets the number of cached results.</summary>
        public int CacheCount
        {
            get { return _cache.Count; }
        }

        private void Remember(string key, bool verified)
        {
            DateTimeOffset now = _clock();
            if (_cache.Count >= MaxCacheEntries && !_cache.ContainsKey(key))
            {
                // Sweep expired entries at most once a minute so a flood of fresh addresses cannot make every lookup scan the table.
                lock (_cache)
                {
                    if (now >= _nextSweep)
                    {
                        _nextSweep = now.AddMinutes(1);
                        foreach (KeyValuePair<string, KeyValuePair<DateTimeOffset, bool>> pair in _cache)
                        {
                            if (pair.Value.Key <= now)
                            {
                                KeyValuePair<DateTimeOffset, bool> removed;
                                _cache.TryRemove(pair.Key, out removed);
                            }
                        }
                    }
                }
                if (_cache.Count >= MaxCacheEntries)
                {
                    return;
                }
            }
            _cache[key] = new KeyValuePair<DateTimeOffset, bool>(now + _cacheFor, verified);
        }

        private async Task<bool> ResolveAndConfirmAsync(IPAddress address, IReadOnlyList<string> hostSuffixes, CancellationToken cancellationToken)
        {
            IPHostEntry reverse = await _resolve(address.ToString(), cancellationToken).ConfigureAwait(false);
            string host = reverse.HostName == null ? null : reverse.HostName.TrimEnd('.');
            if (string.IsNullOrEmpty(host) || !HostMatches(host, hostSuffixes))
            {
                return false;
            }
            IPHostEntry forward = await _resolve(host, cancellationToken).ConfigureAwait(false);
            return forward.AddressList.Any(candidate => (candidate.IsIPv4MappedToIPv6 ? candidate.MapToIPv4() : candidate).Equals(address));
        }

        /// <summary>
        /// Whether a host name ends with one of the suffixes on a label boundary.
        /// </summary>
        /// <param name="host">The PTR name.</param>
        /// <param name="suffixes">Suffixes such as ".googlebot.com".</param>
        /// <returns>True on a match.</returns>
        public static bool HostMatches(string host, IReadOnlyList<string> suffixes)
        {
            foreach (string suffix in suffixes)
            {
                string normalized = suffix.StartsWith(".", StringComparison.Ordinal) ? suffix : "." + suffix;
                if (host.EndsWith(normalized, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
