// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Pure retry decisions for Shopify Admin GraphQL: 429 (Retry-After), 5xx and network failures (backoff), and HTTP 200
// THROTTLED answers (wait what throttleStatus says the query needs). A 429 or a THROTTLED answer means Shopify did not
// run the document, so any request may be resent. A 5xx or a network failure leaves the outcome unknown: only a query
// or a mutation carrying an @idempotent key is resent; any other mutation is surfaced as Indeterminate, never retried.

using System;
using System.Net.Http.Headers;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>What to do after one attempt.</summary>
    public enum ShopifyRetryAction
    {
        /// <summary>Not set.</summary>
        Unknown = 0,

        /// <summary>Done: return the answer.</summary>
        Accept = 1,

        /// <summary>Wait <see cref="ShopifyRetryDecision.Delay"/> and send again.</summary>
        Retry = 2,

        /// <summary>Stop with <see cref="ShopifyCallOutcome.Failed"/>.</summary>
        GiveUp = 3,

        /// <summary>Stop with <see cref="ShopifyCallOutcome.Indeterminate"/> (unkeyed mutation, unknown outcome).</summary>
        StopIndeterminate = 4,
    }

    /// <summary>One retry decision.</summary>
    public readonly record struct ShopifyRetryDecision(ShopifyRetryAction Action, TimeSpan Delay, string Reason);

    /// <summary>Retry limits and backoff. Immutable; every value is validated.</summary>
    public sealed class ShopifyRetryPolicy
    {
        /// <summary>Create a policy.</summary>
        public ShopifyRetryPolicy(int maxAttempts, int maxThrottledRounds, TimeSpan baseBackoff, TimeSpan maxDelay)
        {
            if (maxAttempts < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, "at least one attempt");
            }
            if (maxThrottledRounds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxThrottledRounds), maxThrottledRounds, "not negative");
            }
            if (baseBackoff <= TimeSpan.Zero || maxDelay < baseBackoff)
            {
                throw new ArgumentOutOfRangeException(nameof(baseBackoff), "baseBackoff must be positive and not above maxDelay");
            }
            MaxAttempts = maxAttempts;
            MaxThrottledRounds = maxThrottledRounds;
            BaseBackoff = baseBackoff;
            MaxDelay = maxDelay;
        }

        /// <summary>4 attempts for 429/5xx/network, 6 THROTTLED rounds, 1 s base backoff, 30 s cap.</summary>
        public static ShopifyRetryPolicy Default { get; } =
            new ShopifyRetryPolicy(4, 6, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30));

        /// <summary>Total attempts allowed for 429, 5xx and network failures (the first send counts).</summary>
        public int MaxAttempts { get; }

        /// <summary>Extra rounds allowed for HTTP 200 THROTTLED answers (counted separately).</summary>
        public int MaxThrottledRounds { get; }

        /// <summary>Backoff for attempt 1; doubles each attempt.</summary>
        public TimeSpan BaseBackoff { get; }

        /// <summary>Cap on any single wait.</summary>
        public TimeSpan MaxDelay { get; }

        /// <summary>Exponential backoff for the given attempt (1-based), capped.</summary>
        public TimeSpan Backoff(int attempt)
        {
            int shift = Math.Clamp(attempt - 1, 0, 20);
            double ms = BaseBackoff.TotalMilliseconds * (1L << shift);
            return TimeSpan.FromMilliseconds(Math.Min(ms, MaxDelay.TotalMilliseconds));
        }

        /// <summary>Retry-After (seconds or a date) when present, else the backoff; capped.</summary>
        public TimeSpan RetryAfter(RetryConditionHeaderValue header, int attempt, DateTimeOffset now)
        {
            TimeSpan wait = Backoff(attempt);
            if (header != null && header.Delta.HasValue)
            {
                wait = header.Delta.Value;
            }
            else if (header != null && header.Date.HasValue)
            {
                wait = header.Date.Value - now;
            }
            if (wait < TimeSpan.Zero)
            {
                wait = TimeSpan.Zero;
            }
            return wait > MaxDelay ? MaxDelay : wait;
        }

        /// <summary>Wait until the bucket can pay the throttled query: (requested - available) / restoreRate, never
        /// below the backoff, capped. Uses the backoff alone when the answer carries no throttleStatus.</summary>
        public TimeSpan ThrottledWait(ShopifyQueryCost cost, int round)
        {
            TimeSpan floor = Backoff(round);
            if (cost == null || !cost.HasThrottleStatus || cost.RequestedQueryCost <= cost.CurrentlyAvailable)
            {
                return floor;
            }
            TimeSpan need = TimeSpan.FromMilliseconds(Math.Ceiling((cost.RequestedQueryCost - cost.CurrentlyAvailable) / cost.RestoreRate * 1000.0));
            TimeSpan wait = need > floor ? need : floor;
            return wait > MaxDelay ? MaxDelay : wait;
        }

        /// <summary>Decide after an HTTP answer. <paramref name="throttled"/> = a 2xx THROTTLED body.</summary>
        public ShopifyRetryDecision OnResponse(ShopifyGraphQlRequest request, int status, bool throttled, ShopifyQueryCost cost,
            RetryConditionHeaderValue retryAfter, int attempt, int throttledRounds, DateTimeOffset now)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (status == 429)
            {
                return attempt < MaxAttempts
                    ? new ShopifyRetryDecision(ShopifyRetryAction.Retry, RetryAfter(retryAfter, attempt, now), "HTTP 429")
                    : new ShopifyRetryDecision(ShopifyRetryAction.GiveUp, TimeSpan.Zero, "HTTP 429 after " + attempt + " attempts");
            }
            if (status >= 500 && status < 600)
            {
                return OnUnknownOutcome(request, attempt, "HTTP " + status);
            }
            if (status >= 200 && status < 300)
            {
                if (!throttled)
                {
                    return new ShopifyRetryDecision(ShopifyRetryAction.Accept, TimeSpan.Zero, null);
                }
                return throttledRounds < MaxThrottledRounds
                    ? new ShopifyRetryDecision(ShopifyRetryAction.Retry, ThrottledWait(cost, throttledRounds + 1), "THROTTLED")
                    : new ShopifyRetryDecision(ShopifyRetryAction.GiveUp, TimeSpan.Zero, "THROTTLED after " + (throttledRounds + 1) + " rounds");
            }
            return new ShopifyRetryDecision(ShopifyRetryAction.GiveUp, TimeSpan.Zero, "HTTP " + status);
        }

        /// <summary>Decide after a network failure or timeout (no answer).</summary>
        public ShopifyRetryDecision OnNetworkFailure(ShopifyGraphQlRequest request, int attempt)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            return OnUnknownOutcome(request, attempt, "network failure");
        }

        private ShopifyRetryDecision OnUnknownOutcome(ShopifyGraphQlRequest request, int attempt, string what)
        {
            if (!request.SafeToResendAfterUnknownOutcome)
            {
                return new ShopifyRetryDecision(ShopifyRetryAction.StopIndeterminate, TimeSpan.Zero,
                    what + " on a mutation without an @idempotent key; outcome unknown, not resent");
            }
            return attempt < MaxAttempts
                ? new ShopifyRetryDecision(ShopifyRetryAction.Retry, Backoff(attempt), what)
                : new ShopifyRetryDecision(ShopifyRetryAction.GiveUp, TimeSpan.Zero, what + " after " + attempt + " attempts");
        }
    }
}
