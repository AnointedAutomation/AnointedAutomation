// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Anthropic;
using Anthropic.Exceptions;
using Anthropic.Models.Messages;
using AnointedAutomation.Concepts.Reasoning.Language;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Concepts.Anthropic
{
    /// <summary>
    /// The Claude implementation of the core <see cref="ILanguageModel"/> port, through the official Anthropic .NET SDK.
    /// One <see cref="Complete"/> call is one Messages API request: the core prompt already asks for everything at once,
    /// and the reply is constrained to the core reply schema with structured outputs. 429, 408, 5xx, overload, network
    /// errors and timeouts are retried with exponential backoff up to <see cref="AnthropicLanguageModelOptions.MaxAttempts"/>
    /// and then raised as <see cref="AnthropicTransientException"/>; 401 and 403 raise
    /// <see cref="AnthropicConfigurationException"/> at once. Answers are not cached here: HybridReasoner caches by
    /// prompt and model. The API key is never logged.
    /// </summary>
    public sealed class AnthropicLanguageModel : ILanguageModel
    {
        internal const string SystemInstruction =
            "You answer for the AnointedAutomation.Concepts reasoning engine. Reply with exactly one JSON object that matches "
            + "the JSON schema in the user's message, answering every field in this single reply. No prose, no code fences.";

        private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(30);

        private readonly ILogger<AnthropicLanguageModel> logger;
        private readonly AnthropicClient client;
        private readonly string model;
        private readonly int maxTokens;
        private readonly int maxAttempts;
        private readonly int retryBaseDelayMilliseconds;
        private readonly Dictionary<string, JsonElement> schema;

        /// <summary>
        /// Creates the adapter. Throws <see cref="AnthropicConfigurationException"/> when a required option is missing.
        /// </summary>
        /// <param name="handler">The HTTP handler pipeline the SDK sends through (from IHttpMessageHandlerFactory in DI). Not disposed here.</param>
        /// <param name="options">The required settings.</param>
        /// <param name="logger">Receives retries and failures; never the key.</param>
        public AnthropicLanguageModel(HttpMessageHandler handler, AnthropicLanguageModelOptions options, ILogger<AnthropicLanguageModel> logger)
        {
            ArgumentNullException.ThrowIfNull(handler);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(logger);
            this.logger = logger;

            IReadOnlyList<string> problems = options.Problems();
            if (problems.Count > 0)
            {
                AnthropicConfigurationException invalid = new AnthropicConfigurationException(
                    "Anthropic language model configuration is invalid: " + string.Join("; ", problems) + ".", null);
                logger.LogCritical(invalid, "Anthropic language model is misconfigured: {Problems}", string.Join("; ", problems));
                throw invalid;
            }

            model = options.Model;
            maxTokens = options.MaxTokens;
            maxAttempts = options.MaxAttempts;
            retryBaseDelayMilliseconds = options.RetryBaseDelayMilliseconds;
            schema = ReplySchemaFormat.Create();
            client = new AnthropicClient
            {
                ApiKey = options.ApiKey,
                // The SDK enforces TimeoutSeconds per call, so the HttpClient's own 100 second limit must not cut in first.
                HttpClient = new HttpClient(new RetryAfterRecorder(handler), false) { Timeout = System.Threading.Timeout.InfiniteTimeSpan },
                MaxRetries = 0,
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds),
            };
        }

        /// <inheritdoc />
        public string Name => "anthropic/" + model;

        /// <inheritdoc />
        public async Task<string> Complete(string prompt, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("The prompt is empty.", nameof(prompt));
            }

            MessageCreateParams request = new MessageCreateParams
            {
                Model = model,
                MaxTokens = maxTokens,
                System = SystemInstruction,
                Messages = [new UserMessageParam { Content = prompt }],
                OutputConfig = new OutputConfig { Format = new JsonOutputFormat { Schema = schema } },
            };

            for (int attempt = 1; ; attempt++)
            {
                RetryAfterSlot retryAfter = new RetryAfterSlot();
                RetryAfterRecorder.Current.Value = retryAfter;
                try
                {
                    Message message = await client.Messages.Create(request, ct).ConfigureAwait(false);
                    return ReadText(message);
                }
                catch (AnthropicUnauthorizedException ex)
                {
                    throw AuthFailure(ex, 401);
                }
                catch (AnthropicForbiddenException ex)
                {
                    throw AuthFailure(ex, 403);
                }
                catch (AnthropicRateLimitException ex)
                {
                    await Backoff(ex, "429 rate limit", attempt, retryAfter.Delay, ct).ConfigureAwait(false);
                }
                catch (AnthropicRequestTimeoutException ex)
                {
                    await Backoff(ex, "408 request timeout", attempt, retryAfter.Delay, ct).ConfigureAwait(false);
                }
                catch (AnthropicOverloadedException ex)
                {
                    await Backoff(ex, "overloaded", attempt, retryAfter.Delay, ct).ConfigureAwait(false);
                }
                catch (Anthropic5xxException ex)
                {
                    await Backoff(ex, "5xx server error", attempt, retryAfter.Delay, ct).ConfigureAwait(false);
                }
                catch (AnthropicIOException ex)
                {
                    await Backoff(ex, "network error", attempt, retryAfter.Delay, ct).ConfigureAwait(false);
                }
                catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
                {
                    await Backoff(ex, "timeout", attempt, retryAfter.Delay, ct).ConfigureAwait(false);
                }
                catch (TimeoutException ex)
                {
                    await Backoff(ex, "timeout", attempt, retryAfter.Delay, ct).ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    await Backoff(ex, "network error", attempt, retryAfter.Delay, ct).ConfigureAwait(false);
                }
                catch (AnthropicApiException ex)
                {
                    logger.LogError(ex, "Anthropic request to model {Model} failed with a non-retryable error on attempt {Attempt}; request id {RequestId}.", model, attempt, ex.RequestID);
                    throw;
                }
            }
        }

        private string ReadText(Message message)
        {
            StringBuilder text = new StringBuilder();
            foreach (ContentBlock block in message.Content)
            {
                if (block.TryPickText(out TextBlock textBlock))
                {
                    text.Append(textBlock.Text);
                }
            }

            if (text.Length == 0)
            {
                logger.LogWarning("Anthropic model {Model} returned no text (message {MessageId}, stop reason {StopReason}); the parser will reject the reply.", model, message.ID, message.StopReason);
            }

            return text.ToString();
        }

        private AnthropicConfigurationException AuthFailure(Exception ex, int status)
        {
            AnthropicConfigurationException failed = new AnthropicConfigurationException(
                "The Anthropic API rejected the configured API key or its permissions (HTTP " + status + ") for model " + model + ".", ex);
            logger.LogCritical(failed, "Anthropic rejected the configured credentials with HTTP {Status} for model {Model}; fix the ApiKey setting.", status, model);
            return failed;
        }

        private async Task Backoff(Exception ex, string kind, int attempt, TimeSpan? retryAfter, CancellationToken ct)
        {
            // A caller's cancellation is shutdown, not a failure: rethrow it quietly instead of retrying.
            ct.ThrowIfCancellationRequested();
            if (attempt >= maxAttempts)
            {
                AnthropicTransientException failed = new AnthropicTransientException(
                    "The Anthropic API kept failing (" + kind + ") for model " + model + " after " + attempt + " attempts.", attempt, ex);
                logger.LogError(failed, "Anthropic request to model {Model} failed after {Attempts} attempts; last error: {Kind}.", model, attempt, kind);
                throw failed;
            }

            // The server's Retry-After hint wins when present; otherwise exponential backoff. Both are capped.
            double wanted = retryAfter.HasValue ? retryAfter.Value.TotalMilliseconds : retryBaseDelayMilliseconds * Math.Pow(2, attempt - 1);
            double milliseconds = Math.Min(wanted, MaxBackoff.TotalMilliseconds);
            TimeSpan delay = TimeSpan.FromMilliseconds(milliseconds);
            logger.LogWarning(ex, "Anthropic transient failure ({Kind}) on attempt {Attempt} of {MaxAttempts} for model {Model}; retrying in {DelayMs} ms.", kind, attempt, maxAttempts, model, (long)milliseconds);
            await Task.Delay(delay, ct).ConfigureAwait(false);
        }
    }
}
