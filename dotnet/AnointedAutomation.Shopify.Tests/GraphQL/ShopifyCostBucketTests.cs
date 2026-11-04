// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using AnointedAutomation.Shopify.GraphQL;
using Xunit;

namespace AnointedAutomation.Shopify.Tests.GraphQL
{
    public class ShopifyCostBucketTests
    {
        private static readonly DateTimeOffset T0 = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private static readonly TimeSpan Cap = TimeSpan.FromSeconds(30);

        private static ShopifyQueryCost Status(double available, double requested = 100) => new ShopifyQueryCost
        {
            RequestedQueryCost = requested, HasThrottleStatus = true, CurrentlyAvailable = available, MaximumAvailable = 1000, RestoreRate = 50,
        };

        [Fact]
        public void PureMath()
        {
            Assert.Equal(150, ShopifyCostBucket.ProjectAvailable(50, 50, 1000, TimeSpan.FromSeconds(2)));
            Assert.Equal(1000, ShopifyCostBucket.ProjectAvailable(990, 50, 1000, TimeSpan.FromSeconds(10)));
            Assert.Equal(50, ShopifyCostBucket.ProjectAvailable(50, 50, 0, TimeSpan.FromSeconds(-1)));
            Assert.Equal(TimeSpan.Zero, ShopifyCostBucket.WaitFor(100, 50, 50, Cap));
            Assert.Equal(TimeSpan.Zero, ShopifyCostBucket.WaitFor(0, 50, 0, Cap));
            Assert.Equal(TimeSpan.FromSeconds(1), ShopifyCostBucket.WaitFor(0, 50, 50, Cap));
            Assert.Equal(Cap, ShopifyCostBucket.WaitFor(0, 1e9, 50, Cap));
        }

        [Fact]
        public void Unknown_NeverWaits_ButCountsPending()
        {
            ShopifyCostBucket b = new ShopifyCostBucket();
            Assert.False(b.IsKnown);
            Assert.Equal(TimeSpan.Zero, b.Reserve(500, T0, Cap));
            Assert.Equal(500, b.PendingCost);
            b.Release(500);
            b.Release(0);
            Assert.Equal(0, b.PendingCost);
            Assert.Equal(0, b.Available(T0));
        }

        [Fact]
        public void Reserve_QueuesConcurrentCallers()
        {
            ShopifyCostBucket b = new ShopifyCostBucket();
            b.Observe("q", Status(100), false, T0);
            Assert.True(b.IsKnown);
            Assert.Equal(100, b.EstimateCost("q"));
            Assert.Equal(0, b.EstimateCost("other"));
            Assert.Equal(0, b.EstimateCost(null));
            Assert.Equal(TimeSpan.Zero, b.Reserve(100, T0, Cap));
            Assert.Equal(TimeSpan.FromSeconds(2), b.Reserve(100, T0, Cap));
            Assert.Equal(-100, b.Available(T0));
        }

        [Fact]
        public void Observe_SubtractsPending_AndThrottledDrains()
        {
            ShopifyCostBucket b = new ShopifyCostBucket();
            b.Observe("q", null, true, T0); // unknown: ignored
            Assert.False(b.IsKnown);
            b.Observe("q", Status(500), false, T0);
            b.Reserve(200, T0, Cap);
            b.Observe("q", Status(500), false, T0);
            Assert.Equal(300, b.Available(T0));
            b.Observe("q", null, true, T0);
            Assert.Equal(-200, b.Available(T0));
        }

        [Fact]
        public void Observe_ForgetsWhenFull()
        {
            ShopifyCostBucket b = new ShopifyCostBucket();
            for (int i = 0; i <= ShopifyCostBucket.MaxRememberedQueries; i++)
            {
                b.Observe("q" + i, new ShopifyQueryCost { RequestedQueryCost = 1 }, false, T0);
            }
            Assert.Equal(0, b.EstimateCost("q0"));
            Assert.Equal(1, b.EstimateCost("q" + ShopifyCostBucket.MaxRememberedQueries));
        }

        [Fact]
        public void Registry_PerShop()
        {
            ShopifyCostBucketRegistry r = new ShopifyCostBucketRegistry();
            Assert.Same(r.For("A.myshopify.com"), r.For(" a.myshopify.com "));
            Assert.NotSame(r.For("a.myshopify.com"), r.For("b.myshopify.com"));
            Assert.Equal(2, r.Count);
            Assert.Throws<ArgumentException>(() => r.For(" "));
        }
    }
}
