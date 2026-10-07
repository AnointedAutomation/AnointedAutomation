// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.SSO
{
    /// <summary>
    /// Remembers Logout Token ids (<c>jti</c>) so a retried logout is answered 200 without running your handler
    /// twice. Register your own implementation (for example over Redis) before <c>AddAnointedAutomation</c> when
    /// the app runs on more than one instance.
    /// </summary>
    public interface IAnointedLogoutReplayStore
    {
        /// <summary>Records a jti atomically.</summary>
        /// <param name="jti">The token id.</param>
        /// <param name="rememberUntil">When the entry may be forgotten.</param>
        /// <param name="cancellationToken">Cancels the call.</param>
        /// <returns>True when the jti was new; false when it was already recorded.</returns>
        Task<bool> TryAddAsync(string jti, DateTimeOffset rememberUntil, CancellationToken cancellationToken);

        /// <summary>Forgets a jti, so a retry after a failed handler runs again.</summary>
        /// <param name="jti">The token id.</param>
        /// <param name="cancellationToken">Cancels the call.</param>
        /// <returns>A task that completes when the entry is gone.</returns>
        Task RemoveAsync(string jti, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The default single-instance replay store: an in-memory map pruned of expired entries on every write.
    /// </summary>
    public class InMemoryAnointedLogoutReplayStore : IAnointedLogoutReplayStore
    {
        private readonly ConcurrentDictionary<string, DateTimeOffset> _seen = new ConcurrentDictionary<string, DateTimeOffset>(StringComparer.Ordinal);
        private readonly TimeProvider _timeProvider;

        /// <summary>Creates the store.</summary>
        /// <param name="timeProvider">The clock used to expire entries.</param>
        public InMemoryAnointedLogoutReplayStore(TimeProvider timeProvider)
        {
            if (timeProvider == null)
            {
                throw new ArgumentNullException(nameof(timeProvider));
            }
            _timeProvider = timeProvider;
        }

        /// <summary>Records a jti atomically, replacing an entry that has already expired.</summary>
        /// <param name="jti">The token id.</param>
        /// <param name="rememberUntil">When the entry may be forgotten.</param>
        /// <param name="cancellationToken">Unused; the store is synchronous.</param>
        /// <returns>True when the jti was new or expired; false when it is still remembered.</returns>
        public Task<bool> TryAddAsync(string jti, DateTimeOffset rememberUntil, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(jti))
            {
                throw new ArgumentException("A jti is required.", nameof(jti));
            }
            DateTimeOffset now = _timeProvider.GetUtcNow();
            Prune(now);
            if (_seen.TryAdd(jti, rememberUntil))
            {
                return Task.FromResult(true);
            }
            if (_seen.TryGetValue(jti, out DateTimeOffset existing) && existing <= now && _seen.TryUpdate(jti, rememberUntil, existing))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>Forgets a jti.</summary>
        /// <param name="jti">The token id.</param>
        /// <param name="cancellationToken">Unused; the store is synchronous.</param>
        /// <returns>A completed task.</returns>
        public Task RemoveAsync(string jti, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(jti))
            {
                throw new ArgumentException("A jti is required.", nameof(jti));
            }
            _seen.TryRemove(jti, out DateTimeOffset _);
            return Task.CompletedTask;
        }

        /// <summary>Drops every entry whose time has passed.</summary>
        /// <param name="now">The current time.</param>
        private void Prune(DateTimeOffset now)
        {
            foreach (KeyValuePair<string, DateTimeOffset> entry in _seen)
            {
                if (entry.Value <= now)
                {
                    _seen.TryRemove(entry);
                }
            }
        }
    }
}
