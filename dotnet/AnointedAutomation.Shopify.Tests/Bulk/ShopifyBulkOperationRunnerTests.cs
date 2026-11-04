// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.Bulk;
using AnointedAutomation.Shopify.GraphQL;
using AnointedAutomation.Shopify.Tests.GraphQL;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AnointedAutomation.Shopify.Tests.Bulk
{
    public class ShopifyBulkOperationRunnerTests
    {
        private const string Started = "{\"data\":{\"bulkOperationRunQuery\":{\"bulkOperation\":{\"id\":\"gid://shopify/BulkOperation/1\",\"status\":\"CREATED\"},\"userErrors\":[]}}}";
        private const string StartedMutation = "{\"data\":{\"bulkOperationRunMutation\":{\"bulkOperation\":{\"id\":\"gid://shopify/BulkOperation/2\",\"status\":\"CREATED\"},\"userErrors\":[]}}}";
        private const string Busy = "{\"data\":{\"bulkOperationRunQuery\":{\"bulkOperation\":null,\"userErrors\":[{\"field\":null,\"message\":\"A bulk query operation for this app and shop is already in progress\",\"code\":\"OPERATION_IN_PROGRESS\"}]}}}";
        private const string Refused = "{\"data\":{\"bulkOperationRunQuery\":{\"bulkOperation\":null,\"userErrors\":[{\"field\":[\"query\"],\"message\":\"Invalid bulk query\",\"code\":\"INVALID\"}]}}}";

        private static string Node(string status, string errorCode = null, string url = null, string partial = null) =>
            "{\"data\":{\"node\":{\"id\":\"gid://shopify/BulkOperation/1\",\"type\":\"QUERY\",\"status\":\"" + status + "\",\"errorCode\":"
            + Q(errorCode) + ",\"objectCount\":\"42\",\"url\":" + Q(url) + ",\"partialDataUrl\":" + Q(partial) + "}}}";

        private static string Q(string s) => s == null ? "null" : "\"" + s + "\"";

        private sealed class Rig
        {
            public Harness H { get; } = new Harness();
            public FakeHandler Files { get; } = new FakeHandler();
            public List<TimeSpan> Waits { get; } = new List<TimeSpan>();
            public TimeSpan MaxWait { get; set; } = TimeSpan.FromHours(1);

            public ShopifyBulkOperationRunner Runner() => new ShopifyBulkOperationRunner(H.Client(), new HttpClient(Files), H.Logger,
                new ShopifyBulkOperationRunnerOptions
                {
                    PollInterval = TimeSpan.FromSeconds(1),
                    MaxPollInterval = TimeSpan.FromSeconds(4),
                    MaxWait = MaxWait,
                    Delay = (d, t) => { Waits.Add(d); H.Clock.Now += d; return Task.CompletedTask; },
                    Clock = H.Clock,
                });
        }

        [Fact]
        public async Task RunQuery_Completed_PollsWithBackoffAndStreamsResults()
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.OK, Started).Json(HttpStatusCode.OK, Node("RUNNING")).Json(HttpStatusCode.OK, Node("RUNNING"))
                .Json(HttpStatusCode.OK, Node("RUNNING")).Json(HttpStatusCode.OK, Node("COMPLETED", url: "https://files.example/r.jsonl"));
            r.Files.Json(HttpStatusCode.OK, "{\"id\":\"gid://shopify/Product/1\"}\n\n{\"id\":\"gid://shopify/Product/2\",\"__parentId\":\"x\"}\n");
            ShopifyBulkOperationRunner runner = r.Runner();

            ShopifyBulkOperation op = await runner.RunQueryAsync("{ products { edges { node { id } } } }", CancellationToken.None);

            Assert.True(op.IsCompleted);
            Assert.Equal(42, op.ObjectCount);
            Assert.Equal(new[] { 1d, 2d, 4d, 4d }, r.Waits.Select(w => w.TotalSeconds));
            using JsonDocument start = JsonDocument.Parse(r.H.Handler.Requests[0].Body);
            Assert.Equal(ShopifyBulkOperationRunner.RunQueryMutation, start.RootElement.GetProperty("query").GetString());
            List<string> ids = new List<string>();
            await foreach (JsonElement line in runner.ReadResultsAsync(op, CancellationToken.None))
            {
                ids.Add(line.GetProperty("id").GetString());
            }
            Assert.Equal(new[] { "gid://shopify/Product/1", "gid://shopify/Product/2" }, ids);
            Assert.Equal("https://files.example/r.jsonl", r.Files.Requests[0].Request.RequestUri.ToString());
            Assert.False(r.Files.Requests[0].Request.Headers.Contains(ShopifyGraphQlClient.AccessTokenHeader));
        }

        [Theory]
        [InlineData("FAILED", "INTERNAL_SERVER_ERROR")]
        [InlineData("CANCELED", null)]
        [InlineData("EXPIRED", null)]
        public async Task RunQuery_NonCompletedTerminal_ReturnsStateWithErrorCodeAndPartialUrl(string status, string code)
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.OK, Started)
                .Json(HttpStatusCode.OK, Node(status, code, "https://files.example/full", "https://files.example/partial"));

            ShopifyBulkOperation op = await r.Runner().RunQueryAsync("{ products { edges { node { id } } } }", CancellationToken.None);

            Assert.Equal(status, op.Status);
            Assert.Equal(code, op.ErrorCode);
            Assert.False(op.IsCompleted);
            Assert.True(op.IsTerminal);
            Assert.Equal("https://files.example/partial", op.ResultUrl);
            Assert.Contains(r.H.Logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains(status, StringComparison.Ordinal));
        }

        [Fact]
        public async Task Start_AlreadyInProgress_WaitsAndRetriesStart()
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.OK, Busy).Json(HttpStatusCode.OK, Started);
            ShopifyBulkOperation op = await r.Runner().StartQueryAsync("{ shop { id } }", CancellationToken.None);
            Assert.Equal("gid://shopify/BulkOperation/1", op.Id);
            Assert.Equal(2, r.H.Handler.Requests.Count);
            Assert.Single(r.Waits);
        }

        [Fact]
        public async Task Start_BusyPastMaxWait_Throws()
        {
            Rig r = new Rig { MaxWait = TimeSpan.FromSeconds(2) };
            for (int i = 0; i < 5; i++)
            {
                r.H.Handler.Json(HttpStatusCode.OK, Busy);
            }
            await Assert.ThrowsAsync<TimeoutException>(() => r.Runner().StartQueryAsync("{ shop { id } }", CancellationToken.None));
            Assert.Contains(r.H.Logger.Entries, e => e.Level == LogLevel.Error);
        }

        [Fact]
        public async Task Start_OtherRefusal_ThrowsFailedWithoutRetry()
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.OK, Refused);
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(
                () => r.Runner().StartQueryAsync("{ bad }", CancellationToken.None));
            Assert.Equal(ShopifyCallOutcome.Failed, ex.Outcome);
            Assert.Single(r.H.Handler.Requests);
        }

        [Fact]
        public async Task Start_ServerError_IsIndeterminateAndNeverResent()
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.InternalServerError, "{}");
            ShopifyGraphQlException ex = await Assert.ThrowsAsync<ShopifyGraphQlException>(
                () => r.Runner().StartQueryAsync("{ shop { id } }", CancellationToken.None));
            Assert.Equal(ShopifyCallOutcome.Indeterminate, ex.Outcome);
            Assert.Single(r.H.Handler.Requests);
        }

        [Fact]
        public async Task Wait_PollFailure_IsLoggedAndRepolled()
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.OK, Bodies.AccessDenied).Json(HttpStatusCode.OK, Node("COMPLETED", url: "https://f/x"));
            ShopifyBulkOperation op = await r.Runner().WaitForCompletionAsync("gid://shopify/BulkOperation/1", CancellationToken.None);
            Assert.True(op.IsCompleted);
            Assert.Contains(r.H.Logger.Entries, e => e.Level == LogLevel.Warning && e.Exception is ShopifyGraphQlException);
        }

        [Fact]
        public async Task Wait_NeverEnds_TimesOut()
        {
            Rig r = new Rig { MaxWait = TimeSpan.FromSeconds(5) };
            for (int i = 0; i < 10; i++)
            {
                r.H.Handler.Json(HttpStatusCode.OK, Node("RUNNING"));
            }
            await Assert.ThrowsAsync<TimeoutException>(() => r.Runner().WaitForCompletionAsync("gid://shopify/BulkOperation/1", CancellationToken.None));
            Assert.Contains(r.H.Logger.Entries, e => e.Level == LogLevel.Error && e.Exception is TimeoutException);
        }

        [Fact]
        public async Task Wait_UnknownOperation_RepollsThenTimesOutWithLastProblem()
        {
            Rig r = new Rig { MaxWait = TimeSpan.FromSeconds(1) };
            r.H.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"node\":null}}").Json(HttpStatusCode.OK, "{\"data\":{\"node\":null}}");
            TimeoutException ex = await Assert.ThrowsAsync<TimeoutException>(
                () => r.Runner().WaitForCompletionAsync("gid://shopify/BulkOperation/9", CancellationToken.None));
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public async Task RunMutation_UploadsOnceThenStartsWithKey()
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"stagedUploadsCreate\":{\"stagedTargets\":[{\"url\":\"https://upload.example/\",\"resourceUrl\":null,"
                + "\"parameters\":[{\"name\":\"key\",\"value\":\"tmp/1/bulk.jsonl\"},{\"name\":\"policy\",\"value\":\"p\"}]}],\"userErrors\":[]}}}")
                .Json(HttpStatusCode.OK, StartedMutation)
                .Json(HttpStatusCode.OK, Node("COMPLETED", url: "https://f/r"));
            r.Files.Answer(_ => new HttpResponseMessage(HttpStatusCode.Created));
            using MemoryStream jsonl = new MemoryStream(Encoding.UTF8.GetBytes("{\"input\":{\"id\":\"gid://shopify/Product/1\"}}\n"));

            ShopifyBulkOperation op = await r.Runner().RunMutationAsync("mutation call($input: ProductInput!) { productUpdate(input: $input) { product { id } } }",
                jsonl, "bulk.jsonl", CancellationToken.None);

            Assert.True(op.IsCompleted);
            Assert.Single(r.Files.Requests);
            string form = r.Files.Requests[0].Body;
            Assert.Contains("tmp/1/bulk.jsonl", form, StringComparison.Ordinal);
            Assert.Contains("name=policy", form, StringComparison.Ordinal);
            Assert.Contains("gid://shopify/Product/1", form, StringComparison.Ordinal);
            using JsonDocument staged = JsonDocument.Parse(r.H.Handler.Requests[0].Body);
            JsonElement input = staged.RootElement.GetProperty("variables").GetProperty("input")[0];
            Assert.Equal("BULK_MUTATION_VARIABLES", input.GetProperty("resource").GetString());
            Assert.Equal("text/jsonl", input.GetProperty("mimeType").GetString());
            using JsonDocument start = JsonDocument.Parse(r.H.Handler.Requests[1].Body);
            Assert.Equal("tmp/1/bulk.jsonl", start.RootElement.GetProperty("variables").GetProperty("path").GetString());
        }

        [Fact]
        public async Task Upload_Rejected_ThrowsAndLogs()
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"stagedUploadsCreate\":{\"stagedTargets\":[{\"url\":\"https://upload.example/\","
                + "\"parameters\":[{\"name\":\"key\",\"value\":\"k\"}]}],\"userErrors\":[]}}}");
            r.Files.Answer(_ => new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("denied") });
            await Assert.ThrowsAsync<HttpRequestException>(() => r.Runner().UploadJsonlAsync(new MemoryStream(new byte[] { 1 }), "a.jsonl", CancellationToken.None));
            Assert.Contains(r.H.Logger.Entries, e => e.Level == LogLevel.Error && e.Exception is HttpRequestException);
        }

        [Fact]
        public async Task ReadResults_NoFile_YieldsNothing_AndMalformedLineThrows()
        {
            Rig r = new Rig();
            ShopifyBulkOperationRunner runner = r.Runner();
            int count = 0;
            await foreach (JsonElement _ in runner.ReadResultsAsync(new ShopifyBulkOperation("id", "QUERY", "COMPLETED", null, 0, null, null), CancellationToken.None))
            {
                count++;
            }
            Assert.Equal(0, count);

            r.Files.Json(HttpStatusCode.OK, "{\"ok\":1}\nnot json\n");
            await Assert.ThrowsAnyAsync<JsonException>(async () =>
            {
                await foreach (JsonElement _ in runner.ReadResultsAsync(new ShopifyBulkOperation("id", "QUERY", "COMPLETED", null, 2, "https://f/r", null), CancellationToken.None))
                {
                }
            });
            Assert.Contains(r.H.Logger.Entries, e => e.Level == LogLevel.Error && e.Message.Contains("line 2", StringComparison.Ordinal));
        }

        [Fact]
        public async Task ReadResults_DownloadFails_Throws()
        {
            Rig r = new Rig();
            r.Files.Answer(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await foreach (JsonElement _ in r.Runner().ReadResultsAsync(new ShopifyBulkOperation("id", "QUERY", "COMPLETED", null, 1, "https://f/r", null), CancellationToken.None))
                {
                }
            });
        }

        [Fact]
        public async Task CurrentAndCancel_UseTheirQueries()
        {
            Rig r = new Rig();
            r.H.Handler.Json(HttpStatusCode.OK, "{\"data\":{\"currentBulkOperation\":null}}")
                .Json(HttpStatusCode.OK, "{\"data\":{\"bulkOperationCancel\":{\"bulkOperation\":{\"id\":\"x\",\"status\":\"CANCELING\"},\"userErrors\":[]}}}");
            ShopifyBulkOperationRunner runner = r.Runner();
            Assert.Null(await runner.GetCurrentAsync("MUTATION", CancellationToken.None));
            await runner.CancelAsync("x", CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentException>(() => runner.GetCurrentAsync("OTHER", CancellationToken.None));
            using JsonDocument cur = JsonDocument.Parse(r.H.Handler.Requests[0].Body);
            Assert.Equal("MUTATION", cur.RootElement.GetProperty("variables").GetProperty("type").GetString());
        }

        [Fact]
        public void Options_Validated()
        {
            Harness h = new Harness();
            Assert.Throws<ArgumentOutOfRangeException>(() => new ShopifyBulkOperationRunner(h.Client(), new HttpClient(), h.Logger,
                new ShopifyBulkOperationRunnerOptions { PollInterval = TimeSpan.Zero, MaxPollInterval = TimeSpan.FromSeconds(1), MaxWait = TimeSpan.FromSeconds(1), Delay = (d, t) => Task.CompletedTask, Clock = h.Clock }));
            Assert.Throws<ArgumentException>(() => new ShopifyBulkOperationRunner(h.Client(), new HttpClient(), h.Logger, new ShopifyBulkOperationRunnerOptions()));
            Assert.NotNull(new ShopifyBulkOperationRunner(h.Client(), new HttpClient(), h.Logger));
        }

        [Fact]
        public void StagedUploads_BuildAndRead()
        {
            ShopifyGraphQlRequest req = ShopifyStagedUploads.Build("IMAGE", "a.png", "image/png", "PUT", 1234);
            Assert.True(req.IsMutation);
            Assert.False(req.IsIdempotent);
            using JsonDocument body = JsonDocument.Parse(req.ToJson());
            Assert.Equal("1234", body.RootElement.GetProperty("variables").GetProperty("input")[0].GetProperty("fileSize").GetString());
            Assert.Throws<ArgumentException>(() => ShopifyStagedUploads.Build("IMAGE", " ", "image/png", "PUT", null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ShopifyStagedUploads.Build("IMAGE", "a", "b", "PUT", -1));

            using JsonDocument none = JsonDocument.Parse("{\"stagedTargets\":[]}");
            Assert.Throws<InvalidOperationException>(() => ShopifyStagedUploads.ReadTarget(none.RootElement));
            using JsonDocument noKey = JsonDocument.Parse("{\"stagedTargets\":[{\"url\":\"https://u\",\"parameters\":[]}]}");
            ShopifyStagedTarget t = ShopifyStagedUploads.ReadTarget(noKey.RootElement);
            Assert.Throws<InvalidOperationException>(() => t.Key);
        }

        [Fact]
        public void BulkOperation_ReadAndIsAlreadyRunning()
        {
            using JsonDocument nul = JsonDocument.Parse("null");
            Assert.Null(ShopifyBulkOperation.Read(nul.RootElement));
            using JsonDocument bad = JsonDocument.Parse("{\"id\":\"x\"}");
            Assert.Throws<InvalidOperationException>(() => ShopifyBulkOperation.Read(bad.RootElement));
            using JsonDocument msgOnly = JsonDocument.Parse("{\"userErrors\":[{\"message\":\"Already in progress\"}]}");
            Assert.True(ShopifyBulkOperationRunner.IsAlreadyRunning(msgOnly.RootElement));
            using JsonDocument other = JsonDocument.Parse("{\"userErrors\":[{\"message\":\"nope\"}]}");
            Assert.False(ShopifyBulkOperationRunner.IsAlreadyRunning(other.RootElement));
        }
    }
}
