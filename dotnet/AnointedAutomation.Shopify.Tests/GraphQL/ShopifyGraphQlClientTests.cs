// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AnointedAutomation.Shopify.Tests.GraphQL
{
    public class ShopifyGraphQlClientTests
    {
        private static ShopifyGraphQlRequest Query() => new ShopifyGraphQlRequest(Bodies.Query, null);

        [Fact]
        public async Task Send_BuildsRequestAndReturnsEnvelope()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, Bodies.Ok);
            ShopifyGraphQlResponse r = await h.Client().SendAsync(
                new ShopifyGraphQlRequest(Bodies.Query, new Dictionary<string, object> { ["a"] = 1 }), CancellationToken.None);

            HttpRequestMessage sent = h.Handler.Requests[0].Request;
            Assert.Equal(HttpMethod.Post, sent.Method);
            Assert.Equal("https://example.myshopify.com/admin/api/2026-04/graphql.json", sent.RequestUri.ToString());
            Assert.Equal("shpat_secret_token", sent.Headers.GetValues(ShopifyGraphQlClient.AccessTokenHeader).Single());
            using JsonDocument body = JsonDocument.Parse(h.Handler.Requests[0].Body);
            Assert.Equal(Bodies.Query, body.RootElement.GetProperty("query").GetString());
            Assert.Equal(1, body.RootElement.GetProperty("variables").GetProperty("a").GetInt32());
            Assert.Equal(200, r.StatusCode);
            Assert.True(r.HasData);
            Assert.Equal(990, r.Cost.CurrentlyAvailable);
            Assert.Equal(new[] { "ShopName" }, h.Scheduler.Operations);
            Assert.True(h.Bucket.IsKnown);
            Assert.Equal(0, h.Bucket.PendingCost);
        }

        [Fact]
        public async Task Send_MissingStoreValues_Throw()
        {
            Harness h = new Harness();
            h.Store.ShopDomain = " ";
            await Assert.ThrowsAsync<ArgumentException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            h.Store.ShopDomain = "example.myshopify.com";
            h.Store.ApiVersion = null;
            await Assert.ThrowsAsync<ArgumentException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            h.Store.ApiVersion = "2026-04";
            h.Store.Token = "";
            await Assert.ThrowsAsync<InvalidOperationException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            Assert.Empty(h.Handler.Requests);
        }

        [Fact]
        public void Constructor_RequiresEverything()
        {
            Harness h = new Harness();
            HttpClient http = new HttpClient(h.Handler);
            Assert.Throws<ArgumentNullException>(() => new ShopifyGraphQlClient(null, h.Store, h.Logger));
            Assert.Throws<ArgumentNullException>(() => new ShopifyGraphQlClient(http, null, h.Logger));
            Assert.Throws<ArgumentNullException>(() => new ShopifyGraphQlClient(http, h.Store, null));
            Assert.Throws<ArgumentNullException>(() => new ShopifyGraphQlClient(http, h.Store, h.Logger, null));
            Assert.Throws<ArgumentException>(() => new ShopifyGraphQlClient(http, h.Store, h.Logger, new ShopifyGraphQlClientOptions()));
            ShopifyGraphQlClientOptions std = ShopifyGraphQlClientOptions.Standard(new ShopifyCostBucket());
            Assert.Throws<ArgumentOutOfRangeException>(() => new ShopifyGraphQlClient(http, h.Store, h.Logger, new ShopifyGraphQlClientOptions { RetryPolicy = std.RetryPolicy, CostBucket = std.CostBucket, Scheduler = std.Scheduler, Delay = std.Delay, Clock = std.Clock, MaxPreThrottleWait = TimeSpan.FromSeconds(-1) }));
            Assert.Throws<ArgumentNullException>(() => ShopifyGraphQlClientOptions.Standard(null));
            Assert.NotNull(new ShopifyGraphQlClient(http, h.Store, h.Logger));
        }

        [Fact]
        public async Task Throttled_RetriesWithBucketWait_ThenSucceeds()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, Bodies.Throttled).Json(HttpStatusCode.OK, Bodies.Ok);
            ShopifyGraphQlResponse r = await h.Client().SendAsync(Query(), CancellationToken.None);
            Assert.True(r.HasData);
            Assert.Equal(2, h.Handler.Requests.Count);
            Assert.Contains(TimeSpan.FromSeconds(2), h.Delays); // (100 - 0) / 50 per second
            Assert.Contains(h.Logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains("THROTTLED"));
        }

        [Fact]
        public async Task Throttled_MaxRounds_GivesUpAndLogsError()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, Bodies.Throttled).Json(HttpStatusCode.OK, Bodies.Throttled).Json(HttpStatusCode.OK, Bodies.Throttled);
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            Assert.Equal(ShopifyCallOutcome.Failed, ex.Outcome);
            Assert.Contains("THROTTLED after 3 rounds", ex.Reason);
            Assert.Equal(3, h.Handler.Requests.Count);
            Assert.Contains(h.Logger.Entries, e => e.Level == LogLevel.Error && e.Message.Contains("ShopName"));
        }

        [Fact]
        public async Task Throttled_DoesNotSpendHttpAttempts()
        {
            Harness h = new Harness();
            h.Policy = new ShopifyRetryPolicy(1, 2, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30));
            h.Handler.Json(HttpStatusCode.OK, Bodies.Throttled).Json(HttpStatusCode.OK, Bodies.Ok);
            Assert.True((await h.Client().SendAsync(Query(), CancellationToken.None)).HasData);
        }

        [Fact]
        public async Task Http429_HonorsRetryAfter_EvenForUnkeyedMutation()
        {
            Harness h = new Harness();
            h.Handler.Answer(_ =>
            {
                HttpResponseMessage m = new HttpResponseMessage((HttpStatusCode)429) { Content = new StringContent("") };
                m.Headers.Add("Retry-After", "7");
                return m;
            }).Json(HttpStatusCode.OK, "{\"data\":{\"tagsAdd\":{\"userErrors\":[]}}}");
            JsonElement payload = await h.Client().MutateAsync(new ShopifyGraphQlRequest(Bodies.Mutation, null), "tagsAdd", CancellationToken.None);
            Assert.Equal(JsonValueKind.Object, payload.ValueKind);
            Assert.Equal(new[] { TimeSpan.FromSeconds(7) }, h.Delays);
        }

        [Fact]
        public async Task Http429_Exhausted_Fails()
        {
            Harness h = new Harness();
            for (int i = 0; i < 3; i++)
            {
                h.Handler.Json((HttpStatusCode)429, "{}");
            }
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            Assert.Equal(429, ex.StatusCode);
            Assert.Equal(ShopifyCallOutcome.Failed, ex.Outcome);
        }

        [Fact]
        public async Task NonSuccessThrottledBody_UsesAttemptBudgetAndTerminates()
        {
            Harness h = new Harness();
            for (int i = 0; i < 3; i++)
            {
                h.Handler.Json((HttpStatusCode)429, Bodies.Throttled);
            }
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            Assert.Equal(429, ex.StatusCode);
            Assert.Equal(3, h.Handler.Requests.Count);

            Harness k = new Harness();
            for (int i = 0; i < 3; i++)
            {
                k.Handler.Json(HttpStatusCode.ServiceUnavailable, Bodies.Throttled);
            }
            Dictionary<string, object> vars = new Dictionary<string, object> { [ShopifyIdempotency.KeyVariable] = "k" };
            ShopifyGraphQlException ex2 = await Assert.ThrowsAsync<ShopifyGraphQlException>(
                () => k.Client().SendAsync(new ShopifyGraphQlRequest(Bodies.KeyedMutation, vars), CancellationToken.None));
            Assert.Equal(503, ex2.StatusCode);
            Assert.Equal(3, k.Handler.Requests.Count);
        }

        [Fact]
        public async Task LeadingFragmentMutation_ServerError_IsIndeterminate()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.BadGateway, "{}");
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(
                new ShopifyGraphQlRequest("fragment F on Shop { name }\nmutation M { a { ...F } }", null), CancellationToken.None));
            Assert.Equal(ShopifyCallOutcome.Indeterminate, ex.Outcome);
            Assert.Single(h.Handler.Requests);
        }

        [Fact]
        public async Task ServerError_Query_RetriesWithBackoff()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.BadGateway, "<html>bad gateway</html>").Json(HttpStatusCode.OK, Bodies.Ok);
            Assert.True((await h.Client().SendAsync(Query(), CancellationToken.None)).HasData);
            Assert.Equal(new[] { TimeSpan.FromSeconds(1) }, h.Delays);
        }

        [Fact]
        public async Task ServerError_Exhausted_LogsErrorWithStatus()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.InternalServerError, "{}").Json(HttpStatusCode.InternalServerError, "{}").Json(HttpStatusCode.InternalServerError, "{}");
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            Assert.Equal(500, ex.StatusCode);
            Assert.Equal(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) }, h.Delays);
            Assert.Contains(h.Logger.Entries, e => e.Level == LogLevel.Error && e.Message.Contains("HTTP 500"));
        }

        [Fact]
        public async Task ServerError_UnkeyedMutation_IsIndeterminateAndNotResent()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.ServiceUnavailable, "{}");
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(
                () => h.Client().MutateAsync(new ShopifyGraphQlRequest(Bodies.Mutation, null), "tagsAdd", CancellationToken.None));
            Assert.Equal(ShopifyCallOutcome.Indeterminate, ex.Outcome);
            Assert.Single(h.Handler.Requests);
            Assert.Contains(h.Logger.Entries, e => e.Level == LogLevel.Critical && e.Message.Contains("TagsAdd"));
        }

        [Fact]
        public async Task NetworkFailure_UnkeyedMutation_IsIndeterminate()
        {
            Harness h = new Harness();
            h.Handler.Throw(new HttpRequestException("reset"));
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(
                () => h.Client().SendAsync(new ShopifyGraphQlRequest(Bodies.Mutation, null), CancellationToken.None));
            Assert.Equal(ShopifyCallOutcome.Indeterminate, ex.Outcome);
            Assert.Equal(0, ex.StatusCode);
            Assert.IsType<HttpRequestException>(ex.InnerException);
            Assert.Single(h.Handler.Requests);
        }

        [Fact]
        public async Task NetworkFailure_KeyedMutation_IsResentWithSameKey()
        {
            Harness h = new Harness();
            h.Handler.Throw(new HttpRequestException("reset"))
                .Throw(new TaskCanceledException("timeout"))
                .Json(HttpStatusCode.OK, "{\"data\":{\"refundCreate\":{\"refund\":{\"id\":\"1\"},\"userErrors\":[]}}}");
            Dictionary<string, object> vars = new Dictionary<string, object> { ["input"] = new { orderId = "1" }, [ShopifyIdempotency.KeyVariable] = "k-1" };
            JsonElement payload = await h.Client().MutateAsync(new ShopifyGraphQlRequest(Bodies.KeyedMutation, vars), "refundCreate", CancellationToken.None);
            Assert.Equal("1", payload.GetProperty("refund").GetProperty("id").GetString());
            Assert.Equal(3, h.Handler.Requests.Count);
            Assert.All(h.Handler.Requests, r => Assert.Contains("\"k-1\"", r.Body));
        }

        [Fact]
        public async Task NetworkFailure_Query_ExhaustedFails()
        {
            Harness h = new Harness();
            h.Handler.Throw(new HttpRequestException("a")).Throw(new HttpRequestException("b")).Throw(new HttpRequestException("c"));
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            Assert.Equal(ShopifyCallOutcome.Failed, ex.Outcome);
            Assert.Contains("network failure after 3 attempts", ex.Reason);
        }

        [Fact]
        public async Task ClientError_FailsWithoutRetry()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.Unauthorized, "{\"errors\":\"[API] Invalid API key\"}");
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            Assert.Equal(401, ex.StatusCode);
            Assert.Contains("Invalid API key", ex.Reason);
            Assert.Single(h.Handler.Requests);
        }

        [Fact]
        public async Task SuccessWithNonJsonBody_Fails()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "not json");
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
            Assert.IsAssignableFrom<JsonException>(ex.InnerException);
            Assert.Contains(h.Logger.Entries, e => e.Level == LogLevel.Error && e.Exception != null);
        }

        [Fact]
        public async Task SuccessWithEmptyBody_Fails()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "");
            await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().SendAsync(Query(), CancellationToken.None));
        }

        [Fact]
        public async Task Cancellation_IsRethrownQuietly()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, Bodies.Ok);
            using CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => h.Client().SendAsync(Query(), cts.Token));
            Assert.DoesNotContain(h.Logger.Entries, e => e.Level >= LogLevel.Warning);
            Assert.Equal(0, h.Bucket.PendingCost);
        }

        [Fact]
        public async Task Cancellation_DuringRetryWait_IsRethrown()
        {
            Harness h = new Harness();
            using CancellationTokenSource cts = new CancellationTokenSource();
            h.Handler.Answer(_ => { cts.Cancel(); return new HttpResponseMessage(HttpStatusCode.BadGateway) { Content = new StringContent("{}") }; });
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => h.Client().SendAsync(Query(), cts.Token));
            Assert.DoesNotContain(h.Logger.Entries, e => e.Level >= LogLevel.Error);
        }

        [Fact]
        public async Task PreThrottle_WaitsOnLowBucket()
        {
            Harness h = new Harness();
            h.Bucket.Observe(Bodies.Query, new ShopifyQueryCost { RequestedQueryCost = 100, HasThrottleStatus = true, CurrentlyAvailable = 0, MaximumAvailable = 1000, RestoreRate = 50 }, false, h.Clock.Now);
            h.Handler.Json(HttpStatusCode.OK, Bodies.Ok);
            await h.Client().SendAsync(Query(), CancellationToken.None);
            Assert.Equal(new[] { TimeSpan.FromSeconds(2) }, h.Delays);
        }

        [Fact]
        public async Task Query_TopLevelErrorsWithoutData_Throws()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, Bodies.AccessDenied);
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(() => h.Client().QueryAsync(Query(), CancellationToken.None));
            Assert.Equal("ACCESS_DENIED: Access denied", ex.Reason);
            Assert.DoesNotContain(h.Logger.Entries, e => e.Message.Contains("shpat_secret_token"));
        }

        [Fact]
        public async Task Query_PartialData_ReturnsAndWarns()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"shop\":null},\"errors\":[{\"message\":\"denied\"}]}");
            ShopifyGraphQlResponse r = await h.Client().QueryAsync(Query(), CancellationToken.None);
            Assert.True(r.HasData);
            Assert.Contains(h.Logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains("partial"));
        }

        [Fact]
        public async Task Query_Clean_Returns()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, Bodies.Ok);
            Assert.Null((await h.Client().QueryAsync(Query(), CancellationToken.None)).ErrorSummary);
        }

        [Fact]
        public async Task Mutate_UserErrors_Throw()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"tagsAdd\":{\"userErrors\":[{\"message\":\"bad id\"},{\"message\":\"x\"}]}}}");
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(
                () => h.Client().MutateAsync(new ShopifyGraphQlRequest(Bodies.Mutation, null), "tagsAdd", CancellationToken.None));
            Assert.Equal("bad id, x", ex.Reason);
            Assert.Equal(ShopifyCallOutcome.Failed, ex.Outcome);
        }

        [Fact]
        public async Task Mutate_MissingPayload_AndArgs()
        {
            Harness h = new Harness();
            h.Handler.Json(HttpStatusCode.OK, "{\"data\":{}}");
            ShopifyGraphQlClient c = h.Client();
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(
                () => c.MutateAsync(new ShopifyGraphQlRequest(Bodies.Mutation, null), "tagsAdd", CancellationToken.None));
            Assert.Equal("no tagsAdd result returned", ex.Reason);
            await Assert.ThrowsAsync<ArgumentNullException>(() => c.MutateAsync(null, "f", CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentException>(() => c.MutateAsync(Query(), " ", CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentNullException>(() => c.SendAsync(null, CancellationToken.None));
        }

        [Fact]
        public void Snippet_IsCapped()
        {
            Assert.Equal(string.Empty, ShopifyGraphQlClient.Snippet(null));
            Assert.Equal("abc", ShopifyGraphQlClient.Snippet("abc"));
            Assert.Equal(ShopifyGraphQlClient.LogSnippetLength + 3, ShopifyGraphQlClient.Snippet(new string('a', 500)).Length);
        }
    }
}
