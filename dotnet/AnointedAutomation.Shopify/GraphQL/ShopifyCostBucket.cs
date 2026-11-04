// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Client-side model of Shopify's GraphQL cost bucket (leaky bucket: maximumAvailable points refilled at restoreRate
// per second, one bucket per app per shop). Every answer carries extensions.cost.throttleStatus; the bucket remembers
// it and, before the next call, says how long to wait until the bucket has refilled enough for that query's last
// requestedQueryCost. Each call reserves its cost on dispatch so concurrent callers queue behind each other instead of
// all firing into an empty bucket. Share one instance per shop.

using System;
using System.Collections.Concurrent;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>Cost-bucket pacer for ONE shop. Thread safe. Points are telemetry, so double is used.</summary>
    public sealed class ShopifyCostBucket
    {
        /// <summary>Cap on remembered per-query costs.</summary>
        public const int MaxRememberedQueries = 1024;

        private readonly object _gate = new object();
        private readonly ConcurrentDictionary<string, double> _requestedCost = new ConcurrentDictionary<string, double>(StringComparer.Ordinal);
        private bool _known;
        private double _available;
        private double _maximum;
        private double _restoreRate;
        private DateTimeOffset _stamp;
        private double _pending;

        /// <summary>Points available <paramref name="elapsed"/> after holding <paramref name="available"/>. Pure.</summary>
        public static double ProjectAvailable(double available, double restoreRate, double maximum, TimeSpan elapsed)
        {
            double refilled = available + Math.Max(0, elapsed.TotalSeconds) * Math.Max(0, restoreRate);
            return maximum > 0 ? Math.Min(maximum, refilled) : refilled;
        }

        /// <summary>Time until <paramref name="projected"/> points can pay <paramref name="cost"/>, capped. Pure.</summary>
        public static TimeSpan WaitFor(double projected, double cost, double restoreRate, TimeSpan maxWait)
        {
            if (restoreRate <= 0 || projected >= cost)
            {
                return TimeSpan.Zero;
            }
            TimeSpan wait = TimeSpan.FromMilliseconds(Math.Ceiling((cost - projected) / restoreRate * 1000.0));
            return wait > maxWait ? maxWait : wait;
        }

        /// <summary>True once a throttleStatus has been observed.</summary>
        public bool IsKnown
        {
            get
            {
                lock (_gate)
                {
                    return _known;
                }
            }
        }

        /// <summary>Points reserved by callers still waiting for their answer.</summary>
        public double PendingCost
        {
            get
            {
                lock (_gate)
                {
                    return _pending;
                }
            }
        }

        /// <summary>The last requestedQueryCost seen for this query text, or 0.</summary>
        public double EstimateCost(string query) =>
            query != null && _requestedCost.TryGetValue(query, out double cost) ? cost : 0;

        /// <summary>Reserve <paramref name="cost"/> now and return how long to wait before sending (zero until the
        /// bucket is known). Always pair with <see cref="Release"/>.</summary>
        public TimeSpan Reserve(double cost, DateTimeOffset now, TimeSpan maxWait)
        {
            double reserve = Math.Max(0, cost);
            lock (_gate)
            {
                _pending += reserve;
                if (!_known)
                {
                    return TimeSpan.Zero;
                }
                double projected = ProjectAvailable(_available, _restoreRate, _maximum, now - _stamp);
                TimeSpan wait = WaitFor(projected, reserve, _restoreRate, maxWait);
                _available = projected - reserve;
                _stamp = now;
                return wait;
            }
        }

        /// <summary>Release a reservation once its caller has an answer (or failed), before <see cref="Observe"/>.</summary>
        public void Release(double cost)
        {
            if (cost <= 0)
            {
                return;
            }
            lock (_gate)
            {
                _pending = Math.Max(0, _pending - cost);
            }
        }

        /// <summary>Learn from an answer: throttleStatus replaces the model (less outstanding reservations) and the
        /// query's requestedQueryCost becomes its next estimate. A THROTTLED answer without cost marks the bucket empty.</summary>
        public void Observe(string query, ShopifyQueryCost cost, bool throttled, DateTimeOffset now)
        {
            if (cost != null && cost.RequestedQueryCost > 0 && query != null)
            {
                if (_requestedCost.Count >= MaxRememberedQueries)
                {
                    _requestedCost.Clear();
                }
                _requestedCost[query] = cost.RequestedQueryCost;
            }
            lock (_gate)
            {
                if (cost != null && cost.HasThrottleStatus)
                {
                    _known = true;
                    _available = cost.CurrentlyAvailable - _pending;
                    _maximum = cost.MaximumAvailable;
                    _restoreRate = cost.RestoreRate;
                    _stamp = now;
                }
                else if (throttled && _known)
                {
                    _available = Math.Min(_available, -_pending);
                    _stamp = now;
                }
            }
        }

        /// <summary>Points the model holds now (diagnostics); 0 until known.</summary>
        public double Available(DateTimeOffset now)
        {
            lock (_gate)
            {
                return _known ? ProjectAvailable(_available, _restoreRate, _maximum, now - _stamp) : 0;
            }
        }
    }

    /// <summary>One <see cref="ShopifyCostBucket"/> per shop domain (case-insensitive).</summary>
    public sealed class ShopifyCostBucketRegistry
    {
        private readonly ConcurrentDictionary<string, ShopifyCostBucket> _buckets =
            new ConcurrentDictionary<string, ShopifyCostBucket>(StringComparer.OrdinalIgnoreCase);

        /// <summary>The bucket of one shop, created on first use.</summary>
        public ShopifyCostBucket For(string shopDomain)
        {
            if (string.IsNullOrWhiteSpace(shopDomain))
            {
                throw new ArgumentException("A shop domain is required.", nameof(shopDomain));
            }
            return _buckets.GetOrAdd(shopDomain.Trim(), _ => new ShopifyCostBucket());
        }

        /// <summary>How many shops have a bucket.</summary>
        public int Count => _buckets.Count;
    }
}
