// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-28 Jesus is King ✝️
using System;

namespace AnointedAutomation.APIMiddleware.Utility
{
    /// <summary>
    /// Lets one log line through per time window and counts the ones it held back, so a per-request condition
    /// is recorded once (with how often it happened) instead of once per request.
    /// </summary>
    internal sealed class LogThrottle
    {
        private readonly object _gate = new object();
        private readonly TimeSpan _window;
        private DateTime _windowStart = DateTime.MinValue;
        private int _suppressed;

        /// <summary>Creates a throttle.</summary>
        /// <param name="window">How long one logged line covers.</param>
        internal LogThrottle(TimeSpan window)
        {
            _window = window;
        }

        /// <summary>The window one logged line covers.</summary>
        internal TimeSpan Window => _window;

        /// <summary>
        /// True when this occurrence should be logged (the first in a new window); false when it is suppressed.
        /// </summary>
        /// <param name="utcNow">The current UTC time.</param>
        /// <param name="suppressedBefore">On true: how many occurrences were suppressed in the previous window.</param>
        /// <returns>True to log.</returns>
        internal bool TryEnter(DateTime utcNow, out int suppressedBefore)
        {
            lock (_gate)
            {
                if (utcNow - _windowStart >= _window)
                {
                    suppressedBefore = _suppressed;
                    _suppressed = 0;
                    _windowStart = utcNow;
                    return true;
                }
                _suppressed++;
                suppressedBefore = 0;
                return false;
            }
        }

        /// <summary>Starts over (tests).</summary>
        internal void Reset()
        {
            lock (_gate)
            {
                _windowStart = DateTime.MinValue;
                _suppressed = 0;
            }
        }
    }
}
