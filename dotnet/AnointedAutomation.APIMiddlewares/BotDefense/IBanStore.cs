// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>One temporary ban.</summary>
    public sealed class BanRecord
    {
        /// <summary>Gets or sets the banned IP text.</summary>
        public string Ip { get; set; }

        /// <summary>Gets or sets why the IP was banned.</summary>
        public string Reason { get; set; }

        /// <summary>Gets or sets when the ban ends.</summary>
        public DateTimeOffset ExpiresAt { get; set; }
    }

    /// <summary>
    /// Where temporary bans live. The package ships <see cref="InMemoryBanStore"/>; implement this interface over
    /// Mongo, Redis or any shared store to keep bans across restarts and instances. Implementations may throw on
    /// failure: the middleware logs it at Error and applies <see cref="BotDefenseOptions.BlockWhenBanStoreFails"/>.
    /// </summary>
    public interface IBanStore
    {
        /// <summary>
        /// Gets the active ban for an IP.
        /// </summary>
        /// <param name="ip">The IP text.</param>
        /// <param name="cancellationToken">Cancellation.</param>
        /// <returns>The active ban, or null when the IP is not banned (expired bans must read as null).</returns>
        Task<BanRecord> GetBanAsync(string ip, CancellationToken cancellationToken);

        /// <summary>
        /// Bans an IP until the duration passes. Banning an already banned IP extends the ban.
        /// </summary>
        /// <param name="ip">The IP text.</param>
        /// <param name="reason">Why.</param>
        /// <param name="duration">How long.</param>
        /// <param name="cancellationToken">Cancellation.</param>
        /// <returns>A task.</returns>
        Task BanAsync(string ip, string reason, TimeSpan duration, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a ban.
        /// </summary>
        /// <param name="ip">The IP text.</param>
        /// <param name="cancellationToken">Cancellation.</param>
        /// <returns>A task.</returns>
        Task UnbanAsync(string ip, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Per-process ban store with expiry and a size cap (oldest-expiring bans are dropped first when full).
    /// </summary>
    public sealed class InMemoryBanStore : IBanStore
    {
        /// <summary>The most bans held at once.</summary>
        public const int MaxBans = 100000;

        private readonly ConcurrentDictionary<string, BanRecord> _bans = new ConcurrentDictionary<string, BanRecord>(StringComparer.Ordinal);
        private readonly Func<DateTimeOffset> _clock;

        /// <summary>Initializes a store that uses the system clock.</summary>
        public InMemoryBanStore() : this(() => DateTimeOffset.UtcNow)
        {
        }

        /// <summary>Initializes a store with a custom clock (tests).</summary>
        /// <param name="clock">The clock.</param>
        public InMemoryBanStore(Func<DateTimeOffset> clock)
        {
            _clock = clock;
        }

        /// <summary>Gets the number of stored bans, expired ones included until read or swept.</summary>
        public int Count
        {
            get { return _bans.Count; }
        }

        /// <inheritdoc />
        public Task<BanRecord> GetBanAsync(string ip, CancellationToken cancellationToken)
        {
            BanRecord record;
            if (_bans.TryGetValue(ip, out record))
            {
                if (record.ExpiresAt > _clock())
                {
                    return Task.FromResult(record);
                }
                BanRecord removed;
                _bans.TryRemove(ip, out removed);
            }
            return Task.FromResult<BanRecord>(null);
        }

        /// <inheritdoc />
        public Task BanAsync(string ip, string reason, TimeSpan duration, CancellationToken cancellationToken)
        {
            DateTimeOffset now = _clock();
            if (_bans.Count >= MaxBans && !_bans.ContainsKey(ip))
            {
                Evict(now);
            }
            DateTimeOffset expires = now + duration;
            _bans.AddOrUpdate(
                ip,
                new BanRecord { Ip = ip, Reason = reason, ExpiresAt = expires },
                (key, existing) => new BanRecord { Ip = ip, Reason = reason, ExpiresAt = existing.ExpiresAt > expires ? existing.ExpiresAt : expires });
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task UnbanAsync(string ip, CancellationToken cancellationToken)
        {
            BanRecord removed;
            _bans.TryRemove(ip, out removed);
            return Task.CompletedTask;
        }

        private void Evict(DateTimeOffset now)
        {
            foreach (KeyValuePair<string, BanRecord> pair in _bans)
            {
                if (pair.Value.ExpiresAt <= now)
                {
                    BanRecord removed;
                    _bans.TryRemove(pair.Key, out removed);
                }
            }
            if (_bans.Count >= MaxBans)
            {
                string oldest = null;
                DateTimeOffset oldestExpiry = DateTimeOffset.MaxValue;
                foreach (KeyValuePair<string, BanRecord> pair in _bans)
                {
                    if (pair.Value.ExpiresAt < oldestExpiry)
                    {
                        oldestExpiry = pair.Value.ExpiresAt;
                        oldest = pair.Key;
                    }
                }
                if (oldest != null)
                {
                    BanRecord removed;
                    _bans.TryRemove(oldest, out removed);
                }
            }
        }
    }
}
