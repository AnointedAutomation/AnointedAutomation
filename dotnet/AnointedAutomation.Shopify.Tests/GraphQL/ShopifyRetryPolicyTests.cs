// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using AnointedAutomation.Shopify.GraphQL;
using Xunit;

namespace AnointedAutomation.Shopify.Tests.GraphQL
{
    public class ShopifyRetryPolicyTests
    {
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private static readonly ShopifyRetryPolicy P = new ShopifyRetryPolicy(3, 2, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
        private static readonly ShopifyGraphQlRequest Q = new ShopifyGraphQlRequest(Bodies.Query, null);
        private static readonly ShopifyGraphQlRequest M = new ShopifyGraphQlRequest(Bodies.Mutation, null);
        private static readonly ShopifyGraphQlRequest K = new ShopifyGraphQlRequest(Bodies.KeyedMutation,
            new Dictionary<string, object> { [ShopifyIdempotency.KeyVariable] = "k" });

        [Fact]
        public void Constructor_Validates()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ShopifyRetryPolicy(0, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ShopifyRetryPolicy(1, -1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ShopifyRetryPolicy(1, 1, TimeSpan.Zero, TimeSpan.FromSeconds(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ShopifyRetryPolicy(1, 1, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1)));
            Assert.Equal(4, ShopifyRetryPolicy.Default.MaxAttempts);
        }

        [Fact]
        public void Backoff_DoublesAndCaps()
        {
            Assert.Equal(TimeSpan.FromSeconds(1), P.Backoff(1));
            Assert.Equal(TimeSpan.FromSeconds(4), P.Backoff(3));
            Assert.Equal(TimeSpan.FromSeconds(10), P.Backoff(99));
            Assert.Equal(TimeSpan.FromSeconds(1), P.Backoff(0));
        }

        [Fact]
        public void RetryAfter_DeltaDateMissingAndCap()
        {
            Assert.Equal(TimeSpan.FromSeconds(3), P.RetryAfter(new RetryConditionHeaderValue(TimeSpan.FromSeconds(3)), 1, Now));
            Assert.Equal(TimeSpan.FromSeconds(5), P.RetryAfter(new RetryConditionHeaderValue(Now.AddSeconds(5)), 1, Now));
            Assert.Equal(TimeSpan.Zero, P.RetryAfter(new RetryConditionHeaderValue(Now.AddSeconds(-5)), 1, Now));
            Assert.Equal(TimeSpan.FromSeconds(2), P.RetryAfter(null, 2, Now));
            Assert.Equal(TimeSpan.FromSeconds(10), P.RetryAfter(new RetryConditionHeaderValue(TimeSpan.FromHours(1)), 1, Now));
        }

        [Fact]
        public void ThrottledWait_UsesBucketMath()
        {
            Assert.Equal(TimeSpan.FromSeconds(1), P.ThrottledWait(null, 1));
            ShopifyQueryCost c = new ShopifyQueryCost { RequestedQueryCost = 200, HasThrottleStatus = true, CurrentlyAvailable = 50, RestoreRate = 50 };
            Assert.Equal(TimeSpan.FromSeconds(3), P.ThrottledWait(c, 1));
            Assert.Equal(TimeSpan.FromSeconds(10), P.ThrottledWait(new ShopifyQueryCost { RequestedQueryCost = 5000, HasThrottleStatus = true, RestoreRate = 50 }, 1));
            Assert.Equal(TimeSpan.FromSeconds(2), P.ThrottledWait(new ShopifyQueryCost { RequestedQueryCost = 1, HasThrottleStatus = true, CurrentlyAvailable = 10, RestoreRate = 50 }, 2));
        }

        [Fact]
        public void OnResponse_Paths()
        {
            Assert.Equal(ShopifyRetryAction.Accept, P.OnResponse(Q, 200, false, null, null, 1, 0, Now).Action);
            Assert.Equal(ShopifyRetryAction.Retry, P.OnResponse(M, 200, true, null, null, 1, 0, Now).Action);
            Assert.Equal(ShopifyRetryAction.GiveUp, P.OnResponse(Q, 200, true, null, null, 1, 2, Now).Action);
            Assert.Equal(ShopifyRetryAction.Retry, P.OnResponse(M, 429, false, null, null, 1, 0, Now).Action);
            Assert.Equal(ShopifyRetryAction.GiveUp, P.OnResponse(Q, 429, false, null, null, 3, 0, Now).Action);
            Assert.Equal(ShopifyRetryAction.Retry, P.OnResponse(Q, 503, false, null, null, 1, 0, Now).Action);
            Assert.Equal(ShopifyRetryAction.GiveUp, P.OnResponse(Q, 503, false, null, null, 3, 0, Now).Action);
            Assert.Equal(ShopifyRetryAction.StopIndeterminate, P.OnResponse(M, 500, false, null, null, 1, 0, Now).Action);
            Assert.Equal(ShopifyRetryAction.Retry, P.OnResponse(K, 500, false, null, null, 1, 0, Now).Action);
            Assert.Equal(ShopifyRetryAction.GiveUp, P.OnResponse(Q, 404, false, null, null, 1, 0, Now).Action);
            Assert.Throws<ArgumentNullException>(() => P.OnResponse(null, 200, false, null, null, 1, 0, Now));
        }

        [Fact]
        public void OnNetworkFailure_Paths()
        {
            Assert.Equal(ShopifyRetryAction.Retry, P.OnNetworkFailure(Q, 1).Action);
            Assert.Equal(ShopifyRetryAction.GiveUp, P.OnNetworkFailure(Q, 3).Action);
            Assert.Equal(ShopifyRetryAction.StopIndeterminate, P.OnNetworkFailure(M, 1).Action);
            Assert.Equal(ShopifyRetryAction.Retry, P.OnNetworkFailure(K, 1).Action);
            Assert.Throws<ArgumentNullException>(() => P.OnNetworkFailure(null, 1));
        }
    }
}
