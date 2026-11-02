// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AnointedAutomation.APIMiddleware.BotDefense
{
    /// <summary>
    /// Fixed-window request counter per key (for example "ip|route"). Memory is hard-bounded at
    /// <see cref="MaxKeys"/>: when the table is full, expired windows are swept (at most once per
    /// <see cref="SweepInterval"/>, so a flood of fresh keys cannot make every request scan the table) and, if it is
    /// still full, a request with a new key is admitted WITHOUT being counted and a Warning is logged. Concurrent in-flight requests can overshoot the cap by a few entries, no more.
    /// </summary>
    public sealed class FixedWindowRateLimiter
    {
        /// <summary>The most live keys held at once.</summary>
        public const int MaxKeys = 50000;

        /// <summary>The least time between two sweeps of expired windows.</summary>
        public static readonly TimeSpan SweepInterval = TimeSpan.FromSeconds(30);

        private readonly object _sweepGate = new object();
        private DateTimeOffset _nextSweep = DateTimeOffset.MinValue;

        /// <summary>Gets the number of tracked keys.</summary>
        public int Count
        {
            get { return _windows.Count; }
        }

        private readonly ConcurrentDictionary<string, Window> _windows = new ConcurrentDictionary<string, Window>(StringComparer.Ordinal);
        private readonly Func<DateTimeOffset> _clock;

        private sealed class Window
        {
            public readonly object Gate = new object();
            public DateTimeOffset Start;
            public TimeSpan Length;
            public int Count;
        }

        /// <summary>Initializes a limiter that uses the system clock.</summary>
        public FixedWindowRateLimiter() : this(() => DateTimeOffset.UtcNow)
        {
        }

        /// <summary>Initializes a limiter with a custom clock (tests).</summary>
        /// <param name="clock">The clock.</param>
        public FixedWindowRateLimiter(Func<DateTimeOffset> clock)
        {
            _clock = clock;
        }

        /// <summary>
        /// Counts one request against the key.
        /// </summary>
        /// <param name="key">The counter key.</param>
        /// <param name="limit">Requests allowed per window.</param>
        /// <param name="window">The window length.</param>
        /// <param name="retryAfter">When the request is rejected, how long until the window resets.</param>
        /// <returns>True when the request is within the limit.</returns>
        public bool TryAcquire(string key, int limit, TimeSpan window, out TimeSpan retryAfter)
        {
            DateTimeOffset now = _clock();
            Window entry;
            if (!_windows.TryGetValue(key, out entry))
            {
                if (_windows.Count >= MaxKeys)
                {
                    Sweep(now);
                }
                if (_windows.Count >= MaxKeys)
                {
                    // Full of live windows: do not grow. Admit uncounted rather than block unknown visitors.
                    retryAfter = TimeSpan.Zero;
                    return true;
                }
            }
            entry = _windows.GetOrAdd(key, k => new Window { Start = now, Length = window });
            lock (entry.Gate)
            {
                if (now - entry.Start >= entry.Length || entry.Length != window)
                {
                    entry.Start = now;
                    entry.Length = window;
                    entry.Count = 0;
                }
                if (entry.Count >= limit)
                {
                    retryAfter = entry.Start + entry.Length - now;
                    return false;
                }
                entry.Count++;
                retryAfter = TimeSpan.Zero;
                return true;
            }
        }

        private void Sweep(DateTimeOffset now)
        {
            lock (_sweepGate)
            {
                if (now < _nextSweep)
                {
                    return;
                }
                _nextSweep = now + SweepInterval;
                IPBlacklistMiddleware.AddLog(AnointedAutomation.Optimization.Logging.LogMessage.Warning(
                    "BotDefense rate limiter table is full (" + MaxKeys + " keys); sweeping expired windows and admitting new keys uncounted while it stays full."));
            }
            foreach (KeyValuePair<string, Window> pair in _windows)
            {
                bool expired;
                lock (pair.Value.Gate)
                {
                    expired = now - pair.Value.Start >= pair.Value.Length;
                }
                if (expired)
                {
                    Window removed;
                    _windows.TryRemove(pair.Key, out removed);
                }
            }
        }
    }
}
