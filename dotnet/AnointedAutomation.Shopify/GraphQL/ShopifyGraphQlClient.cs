// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Transport-neutral Shopify Admin GraphQL client over a caller-supplied HttpClient. Builds the graphql.json POST with
// X-Shopify-Access-Token, paces on the shop's cost bucket, retries 429 / THROTTLED / 5xx / network failures under
// ShopifyRetryPolicy (never resending an unkeyed mutation after an unknown outcome) and returns the raw envelope.
// Every failure is logged with the operation name, status and a short reason; never the token or the full body.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Shopify.GraphQL
{
    /// <summary>Shopify Admin GraphQL client for one store.</summary>
    public sealed class ShopifyGraphQlClient
    {
        /// <summary>Header carrying the Admin API access token.</summary>
        public const string AccessTokenHeader = "X-Shopify-Access-Token";

        /// <summary>Longest body snippet put in a log line.</summary>
        public const int LogSnippetLength = 200;

        private readonly HttpClient _http;
        private readonly IShopifyStoreContext _store;
        private readonly ILogger _logger;
        private readonly ShopifyGraphQlClientOptions _options;

        /// <summary>Create a client with <see cref="ShopifyGraphQlClientOptions.Standard"/> and a private cost bucket.</summary>
        public ShopifyGraphQlClient(HttpClient http, IShopifyStoreContext store, ILogger logger)
            : this(http, store, logger, ShopifyGraphQlClientOptions.Standard(new ShopifyCostBucket()))
        {
        }

        /// <summary>Create a client. Every argument and option is required.</summary>
        public ShopifyGraphQlClient(HttpClient http, IShopifyStoreContext store, ILogger logger, ShopifyGraphQlClientOptions options)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (options.RetryPolicy == null || options.CostBucket == null || options.Scheduler == null
                || options.Delay == null || options.Clock == null)
            {
                throw new ArgumentException("Every ShopifyGraphQlClientOptions property is required.", nameof(options));
            }
            if (options.MaxPreThrottleWait < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "MaxPreThrottleWait must not be negative.");
            }
        }

        /// <summary>
        /// Send with pacing and retries and return the envelope of the final 2xx answer (which may still carry
        /// top-level errors other than THROTTLED). Throws <see cref="ShopifyGraphQlException"/> on a non-retryable
        /// status, exhausted retries, a non-JSON body, or <see cref="ShopifyCallOutcome.Indeterminate"/> for an unkeyed
        /// mutation that met a 5xx or network failure. Cancellation of <paramref name="cancellationToken"/> is rethrown.
        /// </summary>
        public async Task<ShopifyGraphQlResponse> SendAsync(ShopifyGraphQlRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            string url = ShopifyEndpoints.Graphql(_store.ShopDomain, _store.ApiVersion);
            int attempt = 0;
            int throttledRounds = 0;
            int lastStatus = 0;
            while (true)
            {
                attempt++;
                double reserved = _options.CostBucket.EstimateCost(request.Query);
                TimeSpan preWait = _options.CostBucket.Reserve(reserved, _options.Clock.GetUtcNow(), _options.MaxPreThrottleWait);
                Attempt result;
                try
                {
                    if (preWait > TimeSpan.Zero)
                    {
                        await _options.Delay(preWait, cancellationToken).ConfigureAwait(false);
                    }
                    result = await _options.Scheduler.ScheduleAsync(request.OperationName,
                        token => SendOnceAsync(url, request, token), cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    _options.CostBucket.Release(reserved);
                }

                ShopifyRetryDecision decision;
                if (result.NetworkError != null)
                {
                    decision = _options.RetryPolicy.OnNetworkFailure(request, attempt);
                    lastStatus = 0;
                }
                else
                {
                    lastStatus = result.Status;
                    ShopifyQueryCost cost = result.Response == null ? null : result.Response.Cost;
                    bool bodyThrottled = result.Response != null && ShopifyGraphQlErrors.IsThrottled(result.Response.Root);
                    _options.CostBucket.Observe(request.Query, cost, bodyThrottled, _options.Clock.GetUtcNow());
                    // Only a 2xx THROTTLED answer uses the throttled-round budget; any other status (a 429 or 5xx whose
                    // body also says THROTTLED) counts against the attempt budget, so the loop always terminates.
                    bool throttled = bodyThrottled && result.Status >= 200 && result.Status < 300;
                    if (result.Status >= 200 && result.Status < 300 && result.Response == null)
                    {
                        Exception notJson = result.ParseError;
                        _logger.LogError(notJson, "Shopify GraphQL {Operation} returned HTTP {Status} with a non-JSON body: {Snippet}",
                            request.OperationName, result.Status, Snippet(result.Body));
                        throw new ShopifyGraphQlException(request.OperationName, result.Status, ShopifyCallOutcome.Failed,
                            "non-JSON body", notJson);
                    }
                    decision = _options.RetryPolicy.OnResponse(request, result.Status, throttled, cost, result.RetryAfter,
                        attempt, throttledRounds, _options.Clock.GetUtcNow());
                    if (throttled)
                    {
                        throttledRounds++;
                        attempt--; // THROTTLED rounds are counted on their own budget.
                    }
                }

                switch (decision.Action)
                {
                    case ShopifyRetryAction.Accept:
                        return result.Response;
                    case ShopifyRetryAction.Retry:
                        _logger.LogWarning(result.NetworkError,
                            "Shopify GraphQL {Operation} retrying after {Reason} (HTTP {Status}) in {DelayMs} ms",
                            request.OperationName, decision.Reason, lastStatus, (long)decision.Delay.TotalMilliseconds);
                        if (decision.Delay > TimeSpan.Zero)
                        {
                            await _options.Delay(decision.Delay, cancellationToken).ConfigureAwait(false);
                        }
                        continue;
                    case ShopifyRetryAction.StopIndeterminate:
                        _logger.LogCritical(result.NetworkError,
                            "Shopify GraphQL {Operation} outcome UNKNOWN: {Reason} (HTTP {Status}); not resent, reconcile before acting again. Body: {Snippet}",
                            request.OperationName, decision.Reason, lastStatus, Snippet(result.Body));
                        throw new ShopifyGraphQlException(request.OperationName, lastStatus, ShopifyCallOutcome.Indeterminate,
                            decision.Reason, result.NetworkError);
                    default:
                        string reason = result.Response != null && result.Response.ErrorSummary != null
                            ? decision.Reason + ": " + result.Response.ErrorSummary
                            : decision.Reason;
                        _logger.LogError(result.NetworkError, "Shopify GraphQL {Operation} gave up: {Reason} (HTTP {Status}). Body: {Snippet}",
                            request.OperationName, reason, lastStatus, Snippet(result.Body));
                        throw new ShopifyGraphQlException(request.OperationName, lastStatus, ShopifyCallOutcome.Failed,
                            reason, result.NetworkError);
                }
            }
        }

        /// <summary>
        /// <see cref="SendAsync"/>, then throw <see cref="ShopifyGraphQlException"/> when the answer carries top-level
        /// errors and no data. A partial answer (errors plus data) is logged at Warning and returned.
        /// </summary>
        public async Task<ShopifyGraphQlResponse> QueryAsync(ShopifyGraphQlRequest request, CancellationToken cancellationToken)
        {
            ShopifyGraphQlResponse response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
            string errors = response.ErrorSummary;
            if (errors == null)
            {
                return response;
            }
            if (response.HasData)
            {
                _logger.LogWarning("Shopify GraphQL {Operation} returned partial data with errors: {Errors}", request.OperationName, errors);
                return response;
            }
            _logger.LogError("Shopify GraphQL {Operation} failed (HTTP {Status}): {Errors}", request.OperationName, response.StatusCode, errors);
            throw new ShopifyGraphQlException(request.OperationName, response.StatusCode, ShopifyCallOutcome.Failed, errors, null);
        }

        /// <summary>
        /// Run a mutation and return data.&lt;<paramref name="mutationField"/>&gt;. Throws
        /// <see cref="ShopifyGraphQlException"/> (Failed) on top-level errors, userErrors, or a missing payload.
        /// </summary>
        public async Task<JsonElement> MutateAsync(ShopifyGraphQlRequest request, string mutationField, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(mutationField))
            {
                throw new ArgumentException("A mutation field is required.", nameof(mutationField));
            }
            ShopifyGraphQlResponse response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
            string error = ShopifyGraphQlErrors.FirstMutationError(response.Root, mutationField);
            if (error != null)
            {
                _logger.LogError("Shopify GraphQL mutation {Operation} ({Field}) did not apply (HTTP {Status}): {Error}",
                    request.OperationName, mutationField, response.StatusCode, error);
                throw new ShopifyGraphQlException(request.OperationName, response.StatusCode, ShopifyCallOutcome.Failed, error, null);
            }
            ShopifyGraphQlErrors.TryGetPayload(response.Root, mutationField, out JsonElement payload);
            return payload;
        }

        private async Task<Attempt> SendOnceAsync(string url, ShopifyGraphQlRequest request, CancellationToken cancellationToken)
        {
            string token = await _store.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("IShopifyStoreContext returned no access token for " + _store.ShopDomain + ".");
            }
            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                message.Headers.Add(AccessTokenHeader, token);
                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                message.Content = new StringContent(request.ToJson(), Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                try
                {
                    response = await _http.SendAsync(message, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "Shopify GraphQL {Operation} network failure", request.OperationName);
                    return Attempt.Network(ex);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning(ex, "Shopify GraphQL {Operation} timed out", request.OperationName);
                    return Attempt.Network(ex);
                }
                using (response)
                {
                    string body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    int status = (int)response.StatusCode;
                    ShopifyGraphQlResponse parsed = null;
                    JsonException parseError = null;
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        try
                        {
                            parsed = new ShopifyGraphQlResponse(status, body);
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogWarning(ex, "Shopify GraphQL {Operation} HTTP {Status} body is not JSON: {Snippet}",
                                request.OperationName, status, Snippet(body));
                            parseError = ex;
                        }
                    }
                    return new Attempt(status, body, parsed, response.Headers.RetryAfter, null, parseError);
                }
            }
        }

        internal static string Snippet(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return string.Empty;
            }
            return body.Length <= LogSnippetLength ? body : body.Substring(0, LogSnippetLength) + "...";
        }

        private sealed class Attempt
        {
            public Attempt(int status, string body, ShopifyGraphQlResponse response, RetryConditionHeaderValue retryAfter,
                Exception networkError, Exception parseError)
            {
                Status = status;
                Body = body;
                Response = response;
                RetryAfter = retryAfter;
                NetworkError = networkError;
                ParseError = parseError;
            }

            public static Attempt Network(Exception ex) => new Attempt(0, null, null, null, ex, null);

            public int Status { get; }
            public string Body { get; }
            public ShopifyGraphQlResponse Response { get; }
            public RetryConditionHeaderValue RetryAfter { get; }
            public Exception NetworkError { get; }
            public Exception ParseError { get; }
        }
    }
}
