// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Shopify bulk operations over ShopifyGraphQlClient:
//   query:    bulkOperationRunQuery -> poll node(id:) -> stream the result JSONL
//   mutation: stagedUploadsCreate -> upload the JSONL once -> bulkOperationRunMutation(stagedUploadPath = "key")
//             -> poll node(id:) -> stream the result JSONL
// The starts are unkeyed mutations, so the client never resends them after an unknown outcome (that surfaces as a
// ShopifyGraphQlException with Outcome Indeterminate). Only a DEFINITE refusal because another bulk operation is
// running is retried, after a backoff wait. Polling backs off; a poll failure re-polls until MaxWait.
// Results are streamed line by line, never loaded whole.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Shopify.GraphQL;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Shopify.Bulk
{
    /// <summary>Timing collaborators of <see cref="ShopifyBulkOperationRunner"/>. Every property is required.</summary>
    public sealed class ShopifyBulkOperationRunnerOptions
    {
        /// <summary>First wait between polls.</summary>
        public TimeSpan PollInterval { get; init; }

        /// <summary>Longest wait between polls (the interval doubles up to this).</summary>
        public TimeSpan MaxPollInterval { get; init; }

        /// <summary>How long one operation (or a busy slot) is waited on before giving up.</summary>
        public TimeSpan MaxWait { get; init; }

        /// <summary>How the runner waits (Task.Delay in production, a recorder in tests).</summary>
        public Func<TimeSpan, CancellationToken, Task> Delay { get; init; }

        /// <summary>Clock for the deadlines.</summary>
        public TimeProvider Clock { get; init; }

        /// <summary>2 s first poll, 30 s cap, 25 h budget (Shopify stops a bulk operation after about 24 h).</summary>
        public static ShopifyBulkOperationRunnerOptions Standard() => new ShopifyBulkOperationRunnerOptions
        {
            PollInterval = TimeSpan.FromSeconds(2),
            MaxPollInterval = TimeSpan.FromSeconds(30),
            MaxWait = TimeSpan.FromHours(25),
            Delay = (wait, token) => Task.Delay(wait, token),
            Clock = TimeProvider.System,
        };
    }

    /// <summary>Runs and reads Shopify bulk operations.</summary>
    public sealed class ShopifyBulkOperationRunner
    {
        /// <summary>Starts a bulk query.</summary>
        public const string RunQueryMutation =
            "mutation BulkOperationRunQuery($query: String!) { bulkOperationRunQuery(query: $query) { bulkOperation { id status } userErrors { field message code } } }";

        /// <summary>Starts a bulk mutation over an uploaded JSONL.</summary>
        public const string RunMutationMutation =
            "mutation BulkOperationRunMutation($mutation: String!, $path: String!) { bulkOperationRunMutation(mutation: $mutation, stagedUploadPath: $path) { bulkOperation { id status } userErrors { field message code } } }";

        /// <summary>Reads one operation by id.</summary>
        public const string NodeQuery =
            "query BulkOperation($id: ID!) { node(id: $id) { ... on BulkOperation { id type status errorCode objectCount url partialDataUrl } } }";

        /// <summary>Reads the shop's current operation of a type ($type: QUERY or MUTATION).</summary>
        public const string CurrentQuery =
            "query CurrentBulkOperation($type: BulkOperationType!) { currentBulkOperation(type: $type) { id type status errorCode objectCount url partialDataUrl } }";

        /// <summary>Asks Shopify to cancel an operation.</summary>
        public const string CancelMutation =
            "mutation BulkOperationCancel($id: ID!) { bulkOperationCancel(id: $id) { bulkOperation { id status } userErrors { field message } } }";

        private readonly ShopifyGraphQlClient _client;
        private readonly HttpClient _http;
        private readonly ILogger _logger;
        private readonly ShopifyBulkOperationRunnerOptions _options;

        /// <summary>Create a runner with <see cref="ShopifyBulkOperationRunnerOptions.Standard"/>.</summary>
        public ShopifyBulkOperationRunner(ShopifyGraphQlClient client, HttpClient http, ILogger logger)
            : this(client, http, logger, ShopifyBulkOperationRunnerOptions.Standard())
        {
        }

        /// <summary>Create a runner. <paramref name="http"/> uploads the JSONL and downloads results (no Shopify token
        /// is sent on it: staged and result URLs are pre-signed).</summary>
        public ShopifyBulkOperationRunner(ShopifyGraphQlClient client, HttpClient http, ILogger logger, ShopifyBulkOperationRunnerOptions options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (options.Delay == null || options.Clock == null)
            {
                throw new ArgumentException("Delay and Clock are required.", nameof(options));
            }
            if (options.PollInterval <= TimeSpan.Zero || options.MaxPollInterval < options.PollInterval || options.MaxWait <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "PollInterval must be positive, MaxPollInterval >= PollInterval, MaxWait positive.");
            }
        }

        /// <summary>Start a bulk query and wait for it to end. Returns the terminal state (check
        /// <see cref="ShopifyBulkOperation.IsCompleted"/>; FAILED / CANCELED / EXPIRED carry ErrorCode).</summary>
        public async Task<ShopifyBulkOperation> RunQueryAsync(string bulkQuery, CancellationToken cancellationToken)
        {
            ShopifyBulkOperation started = await StartQueryAsync(bulkQuery, cancellationToken).ConfigureAwait(false);
            return await WaitForCompletionAsync(started.Id, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Upload <paramref name="jsonl"/> (one variables object per line) once, start
        /// <paramref name="mutation"/> over it, and wait for it to end. Returns the terminal state.</summary>
        public async Task<ShopifyBulkOperation> RunMutationAsync(string mutation, Stream jsonl, string filename, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(mutation))
            {
                throw new ArgumentException("A mutation is required.", nameof(mutation));
            }
            string path = await UploadJsonlAsync(jsonl, filename, cancellationToken).ConfigureAwait(false);
            ShopifyBulkOperation started = await StartMutationAsync(mutation, path, cancellationToken).ConfigureAwait(false);
            return await WaitForCompletionAsync(started.Id, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>stagedUploadsCreate + one multipart POST of the file. Returns the stagedUploadPath ("key").
        /// The caller owns <paramref name="jsonl"/>: it is read from its current position and left open.</summary>
        public async Task<string> UploadJsonlAsync(Stream jsonl, string filename, CancellationToken cancellationToken)
        {
            if (jsonl == null)
            {
                throw new ArgumentNullException(nameof(jsonl));
            }
            JsonElement payload = await _client.MutateAsync(ShopifyStagedUploads.ForBulkMutationVariables(filename),
                ShopifyStagedUploads.Field, cancellationToken).ConfigureAwait(false);
            ShopifyStagedTarget target = ShopifyStagedUploads.ReadTarget(payload);
            string key = target.Key;
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                foreach (KeyValuePair<string, string> p in target.Parameters)
                {
                    form.Add(new StringContent(p.Value), p.Key);
                }
                StreamContent file = new StreamContent(new LeaveOpenStream(jsonl));
                file.Headers.ContentType = new MediaTypeHeaderValue(ShopifyStagedUploads.JsonlMimeType);
                form.Add(file, "file", filename);
                using (HttpResponseMessage response = await _http.PostAsync(target.Url, form, cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        HttpRequestException ex = new HttpRequestException("Staged upload of " + filename + " failed with HTTP "
                            + (int)response.StatusCode + ".", null, response.StatusCode);
                        _logger.LogError(ex, "Shopify staged upload of {Filename} failed (HTTP {Status}): {Snippet}",
                            filename, (int)response.StatusCode, ShopifyGraphQlClient.Snippet(body));
                        throw ex;
                    }
                }
            }
            return key;
        }

        /// <summary>Start a bulk query (retries the start only on a definite "already in progress" refusal).</summary>
        public Task<ShopifyBulkOperation> StartQueryAsync(string bulkQuery, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bulkQuery))
            {
                throw new ArgumentException("A bulk query is required.", nameof(bulkQuery));
            }
            return StartAsync(new ShopifyGraphQlRequest(RunQueryMutation, new Dictionary<string, object> { ["query"] = bulkQuery }),
                "bulkOperationRunQuery", "QUERY", cancellationToken);
        }

        /// <summary>Start a bulk mutation over an uploaded file (retries the start only on a definite refusal).</summary>
        public Task<ShopifyBulkOperation> StartMutationAsync(string mutation, string stagedUploadPath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(mutation))
            {
                throw new ArgumentException("A mutation is required.", nameof(mutation));
            }
            if (string.IsNullOrWhiteSpace(stagedUploadPath))
            {
                throw new ArgumentException("A staged upload path is required.", nameof(stagedUploadPath));
            }
            return StartAsync(new ShopifyGraphQlRequest(RunMutationMutation, new Dictionary<string, object>
            {
                ["mutation"] = mutation,
                ["path"] = stagedUploadPath,
            }), "bulkOperationRunMutation", "MUTATION", cancellationToken);
        }

        /// <summary>Read one operation by id; null when Shopify does not know it.</summary>
        public async Task<ShopifyBulkOperation> GetAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("An operation id is required.", nameof(id));
            }
            ShopifyGraphQlResponse r = await _client.QueryAsync(new ShopifyGraphQlRequest(NodeQuery,
                new Dictionary<string, object> { ["id"] = id }), cancellationToken).ConfigureAwait(false);
            return ReadField(r, "node");
        }

        /// <summary>The shop's current operation of <paramref name="type"/> ("QUERY" or "MUTATION"); null when none.</summary>
        public async Task<ShopifyBulkOperation> GetCurrentAsync(string type, CancellationToken cancellationToken)
        {
            if (!string.Equals(type, "QUERY", StringComparison.Ordinal) && !string.Equals(type, "MUTATION", StringComparison.Ordinal))
            {
                throw new ArgumentException("Type must be QUERY or MUTATION.", nameof(type));
            }
            ShopifyGraphQlResponse r = await _client.QueryAsync(new ShopifyGraphQlRequest(CurrentQuery,
                new Dictionary<string, object> { ["type"] = type }), cancellationToken).ConfigureAwait(false);
            return ReadField(r, "currentBulkOperation");
        }

        /// <summary>Ask Shopify to cancel an operation (it ends CANCELED; poll to see it).</summary>
        public Task CancelAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("An operation id is required.", nameof(id));
            }
            return _client.MutateAsync(new ShopifyGraphQlRequest(CancelMutation, new Dictionary<string, object> { ["id"] = id }),
                "bulkOperationCancel", cancellationToken);
        }

        /// <summary>Poll node(id:) with backoff until the operation is terminal. A failed poll is logged and retried;
        /// past <see cref="ShopifyBulkOperationRunnerOptions.MaxWait"/> a <see cref="TimeoutException"/> is thrown
        /// (the operation may still finish: read it again later by id).</summary>
        public async Task<ShopifyBulkOperation> WaitForCompletionAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("An operation id is required.", nameof(id));
            }
            DateTimeOffset deadline = _options.Clock.GetUtcNow() + _options.MaxWait;
            TimeSpan wait = _options.PollInterval;
            Exception lastProblem = null;
            while (true)
            {
                await _options.Delay(wait, cancellationToken).ConfigureAwait(false);
                wait = Next(wait);
                try
                {
                    ShopifyBulkOperation state = await GetAsync(id, cancellationToken).ConfigureAwait(false);
                    if (state == null)
                    {
                        throw new InvalidOperationException("Bulk operation " + id + " was not found.");
                    }
                    if (state.IsTerminal)
                    {
                        if (!state.IsCompleted)
                        {
                            _logger.LogWarning("Shopify bulk operation {OperationId} ended {Status} (errorCode {ErrorCode}, {Objects} objects)",
                                state.Id, state.Status, state.ErrorCode, state.ObjectCount);
                        }
                        return state;
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (ShopifyGraphQlException ex) when (IsPermanent(ex))
                {
                    _logger.LogError(ex, "Shopify bulk operation {OperationId} poll failed permanently (HTTP {Status}): {Reason}",
                        id, ex.StatusCode, ex.Reason);
                    throw;
                }
                catch (Exception ex) when (ex is ShopifyGraphQlException || ex is InvalidOperationException)
                {
                    lastProblem = ex;
                    _logger.LogWarning(ex, "Shopify bulk operation {OperationId} poll failed; re-polling", id);
                }
                if (_options.Clock.GetUtcNow() > deadline)
                {
                    TimeoutException timeout = new TimeoutException("Bulk operation " + id + " did not end within " + _options.MaxWait + ".", lastProblem);
                    _logger.LogError(timeout, "Shopify bulk operation {OperationId} gave up waiting after {MaxWait}", id, _options.MaxWait);
                    throw timeout;
                }
            }
        }

        /// <summary>Stream the result JSONL of a terminal operation (<see cref="ShopifyBulkOperation.ResultUrl"/>), one
        /// parsed line at a time. Yields nothing when there is no result file. A malformed line throws
        /// <see cref="JsonException"/> (logged with its line number).</summary>
        public async IAsyncEnumerable<JsonElement> ReadResultsAsync(ShopifyBulkOperation operation,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }
            string url = operation.ResultUrl;
            if (string.IsNullOrEmpty(url))
            {
                yield break;
            }
            using (HttpResponseMessage response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    HttpRequestException ex = new HttpRequestException("Bulk result download failed with HTTP " + (int)response.StatusCode + ".",
                        null, response.StatusCode);
                    _logger.LogError(ex, "Shopify bulk operation {OperationId} result download failed (HTTP {Status})",
                        operation.Id, (int)response.StatusCode);
                    throw ex;
                }
                using (Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                using (StreamReader reader = new StreamReader(stream))
                {
                    int lineNumber = 0;
                    string line;
                    while ((line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) != null)
                    {
                        lineNumber++;
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        yield return ParseLine(operation.Id, lineNumber, line);
                    }
                }
            }
        }

        /// <summary>True when a start refusal means another bulk operation is running (safe to retry the start).</summary>
        public static bool IsAlreadyRunning(JsonElement payload)
        {
            if (payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty("userErrors", out JsonElement errors)
                || errors.ValueKind != JsonValueKind.Array)
            {
                return false;
            }
            foreach (JsonElement e in errors.EnumerateArray())
            {
                string code = ShopifyStagedUploads.Str(e, "code");
                string message = ShopifyStagedUploads.Str(e, "message");
                if (string.Equals(code, "OPERATION_IN_PROGRESS", StringComparison.Ordinal)
                    || (message != null && message.Contains("already in progress", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<ShopifyBulkOperation> StartAsync(ShopifyGraphQlRequest request, string field, string type, CancellationToken ct)
        {
            DateTimeOffset deadline = _options.Clock.GetUtcNow() + _options.MaxWait;
            TimeSpan wait = _options.PollInterval;
            while (true)
            {
                // SendAsync: an unknown outcome throws Indeterminate and is never resent.
                ShopifyGraphQlResponse response = await _client.SendAsync(request, ct).ConfigureAwait(false);
                ShopifyGraphQlErrors.TryGetPayload(response.Root, field, out JsonElement payload);
                string error = ShopifyGraphQlErrors.FirstMutationError(response.Root, field);
                if (error == null)
                {
                    ShopifyBulkOperation op = payload.TryGetProperty("bulkOperation", out JsonElement o) ? ShopifyBulkOperation.Read(o) : null;
                    if (op == null)
                    {
                        _logger.LogError("Shopify {Field} returned no bulk operation (HTTP {Status})", field, response.StatusCode);
                        throw new ShopifyGraphQlException(request.OperationName, response.StatusCode, ShopifyCallOutcome.Failed,
                            field + " returned no bulk operation", null);
                    }
                    _logger.LogInformation("Shopify bulk {Type} {OperationId} started", type, op.Id);
                    return op;
                }
                if (!IsAlreadyRunning(payload))
                {
                    _logger.LogError("Shopify {Field} refused (HTTP {Status}): {Error}", field, response.StatusCode, error);
                    throw new ShopifyGraphQlException(request.OperationName, response.StatusCode, ShopifyCallOutcome.Failed, error, null);
                }
                if (_options.Clock.GetUtcNow() > deadline)
                {
                    TimeoutException timeout = new TimeoutException("The shop's bulk " + type + " slot stayed busy for " + _options.MaxWait + ": " + error);
                    _logger.LogError(timeout, "Shopify {Field} gave up waiting for a free slot", field);
                    throw timeout;
                }
                _logger.LogWarning("Shopify {Field}: another bulk {Type} is running, retrying the start ({Error})", field, type, error);
                await _options.Delay(wait, ct).ConfigureAwait(false);
                wait = Next(wait);
            }
        }

        /// <summary>True for a poll failure that re-polling cannot fix: HTTP 401, 403 or 404, or an ACCESS_DENIED /
        /// UNAUTHORIZED error code (Shopify answers those with HTTP 200).</summary>
        public static bool IsPermanent(ShopifyGraphQlException ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }
            if (ex.StatusCode == 401 || ex.StatusCode == 403 || ex.StatusCode == 404)
            {
                return true;
            }
            string reason = ex.Reason;
            return reason != null && (reason.Contains("ACCESS_DENIED", StringComparison.Ordinal)
                || reason.Contains("UNAUTHORIZED", StringComparison.Ordinal));
        }

        private JsonElement ParseLine(string operationId, int lineNumber, string line)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(line))
                {
                    return doc.RootElement.Clone();
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Shopify bulk operation {OperationId} result line {Line} is not JSON", operationId, lineNumber);
                throw;
            }
        }

        private TimeSpan Next(TimeSpan wait)
        {
            TimeSpan doubled = TimeSpan.FromTicks(wait.Ticks * 2);
            return doubled > _options.MaxPollInterval ? _options.MaxPollInterval : doubled;
        }

        /// <summary>Passes reads through but ignores Dispose, so the multipart content never closes the caller's stream.</summary>
        private sealed class LeaveOpenStream : Stream
        {
            private readonly Stream _inner;

            public LeaveOpenStream(Stream inner)
            {
                _inner = inner;
            }

            public override bool CanRead => _inner.CanRead;
            public override bool CanSeek => _inner.CanSeek;
            public override bool CanWrite => false;
            public override long Length => _inner.Length;
            public override long Position { get => _inner.Position; set => _inner.Position = value; }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
                _inner.ReadAsync(buffer, offset, count, cancellationToken);
            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
                _inner.ReadAsync(buffer, cancellationToken);
            public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        private static ShopifyBulkOperation ReadField(ShopifyGraphQlResponse response, string field)
        {
            if (!response.TryGetData(out JsonElement data) || !data.TryGetProperty(field, out JsonElement op))
            {
                throw new InvalidOperationException("The answer has no data." + field + ".");
            }
            return ShopifyBulkOperation.Read(op);
        }
    }
}
