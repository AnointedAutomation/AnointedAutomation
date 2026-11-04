// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>Collaborators of <see cref="ShopifyGraphQlClient"/>. Every property is required (validated by the client).</summary>
    public sealed class ShopifyGraphQlClientOptions
    {
        /// <summary>Retry limits and backoff.</summary>
        public ShopifyRetryPolicy RetryPolicy { get; init; }

        /// <summary>The shop's cost bucket (share one per shop, see <see cref="ShopifyCostBucketRegistry"/>).</summary>
        public ShopifyCostBucket CostBucket { get; init; }

        /// <summary>Hook awaited around each send.</summary>
        public IShopifyRequestScheduler Scheduler { get; init; }

        /// <summary>How the client waits (Task.Delay in production, a recorder in tests).</summary>
        public Func<TimeSpan, CancellationToken, Task> Delay { get; init; }

        /// <summary>Clock for the cost bucket and Retry-After dates.</summary>
        public TimeProvider Clock { get; init; }

        /// <summary>Cap on one pre-throttle wait.</summary>
        public TimeSpan MaxPreThrottleWait { get; init; }

        /// <summary>Standard collaborators: <see cref="ShopifyRetryPolicy.Default"/>, the given bucket, immediate
        /// scheduling, Task.Delay, the system clock and a 30 s pre-throttle cap.</summary>
        public static ShopifyGraphQlClientOptions Standard(ShopifyCostBucket costBucket) => new ShopifyGraphQlClientOptions
        {
            RetryPolicy = ShopifyRetryPolicy.Default,
            CostBucket = costBucket ?? throw new ArgumentNullException(nameof(costBucket)),
            Scheduler = ImmediateShopifyRequestScheduler.Instance,
            Delay = (wait, token) => Task.Delay(wait, token),
            Clock = TimeProvider.System,
            MaxPreThrottleWait = TimeSpan.FromSeconds(30),
        };
    }
}
