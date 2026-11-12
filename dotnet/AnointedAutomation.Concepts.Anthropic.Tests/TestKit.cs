// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Reasoning;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Concepts.Anthropic.Tests
{
    internal static class TestKit
    {
        public const string ApiKey = "sk-ant-test-key-never-logged";
        public const string ModelId = "claude-opus-5-5";

        public const string ValidReply =
            "{\"classification\":[{\"kind\":\"Sin\",\"id\":\"Abortion\"}],\"truth\":\"true\",\"confidence\":0.9,"
            + "\"explanation\":\"Taking an unborn life sheds innocent blood, which God's love forbids.\","
            + "\"scripture\":[\"Jeremiah 1:5\"],\"sinIds\":[\"Abortion\"],\"assertions\":[]}";

        public static AnthropicLanguageModelOptions Options()
        {
            return new AnthropicLanguageModelOptions
            {
                ApiKey = ApiKey,
                Model = ModelId,
                MaxTokens = 4096,
                TimeoutSeconds = 30,
                MaxAttempts = 3,
                RetryBaseDelayMilliseconds = 1,
            };
        }

        public static Query AbortionViolatesLove()
        {
            return new Query(
                new Assertion(ConceptRef.Of(SinId.Abortion), RelationKind.Violates, new ConceptRef(CanonFacts.AttributeKind, "Love")),
                new List<Circumstance>());
        }

        /// <summary>
        /// A recorded Messages API success body whose single text block carries <paramref name="text"/>.
        /// </summary>
        public static HttpResponseMessage Message(string text)
        {
            string body = JsonSerializer.Serialize(new
            {
                id = "msg_recorded_01",
                type = "message",
                role = "assistant",
                model = ModelId,
                content = new object[] { new { type = "text", text, citations = (object)null } },
                stop_reason = "end_turn",
                stop_sequence = (string)null,
                stop_details = (object)null,
                container = (object)null,
                usage = new { input_tokens = 812, output_tokens = 164, cache_creation_input_tokens = 0, cache_read_input_tokens = 0, server_tool_use = (object)null, service_tier = "standard" },
            });
            return Json(HttpStatusCode.OK, body);
        }

        /// <summary>
        /// A recorded Messages API error body.
        /// </summary>
        public static HttpResponseMessage Error(int status, string type, string message)
        {
            string body = JsonSerializer.Serialize(new { type = "error", error = new { type, message }, request_id = "req_recorded_01" });
            return Json((HttpStatusCode)status, body);
        }

        private static HttpResponseMessage Json(HttpStatusCode status, string body)
        {
            HttpResponseMessage response = new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
            response.Headers.Add("request-id", "req_recorded_01");
            return response;
        }
    }

    /// <summary>
    /// Replays recorded responses in order and records every request. Never touches the network; an unexpected
    /// extra call fails the test.
    /// </summary>
    internal sealed class RecordedHandler : HttpMessageHandler
    {
        private readonly Queue<Func<CancellationToken, Task<HttpResponseMessage>>> responses = new Queue<Func<CancellationToken, Task<HttpResponseMessage>>>();

        public RecordedHandler(params Func<HttpResponseMessage>[] responses)
        {
            foreach (Func<HttpResponseMessage> response in responses)
            {
                this.responses.Enqueue(_ => Task.FromResult(response()));
            }
        }

        /// <summary>
        /// A handler whose steps may throw or wait (for network failures and timeouts).
        /// </summary>
        public static RecordedHandler Steps(params Func<CancellationToken, Task<HttpResponseMessage>>[] steps)
        {
            RecordedHandler handler = new RecordedHandler();
            foreach (Func<CancellationToken, Task<HttpResponseMessage>> step in steps)
            {
                handler.responses.Enqueue(step);
            }

            return handler;
        }

        public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

        public List<string> Bodies { get; } = new List<string>();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            Bodies.Add(await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            if (responses.Count == 0)
            {
                throw new InvalidOperationException("No recorded response left: the adapter made an unexpected call.");
            }

            return await responses.Dequeue()(cancellationToken).ConfigureAwait(false);
        }
    }

    internal sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message, Exception Exception)> Entries { get; } = new List<(LogLevel, string, Exception)>();

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception), exception));
        }
    }
}
