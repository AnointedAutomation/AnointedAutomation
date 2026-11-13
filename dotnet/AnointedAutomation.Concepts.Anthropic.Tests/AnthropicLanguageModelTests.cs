// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Anthropic.Exceptions;
using AnointedAutomation.Concepts.Reasoning;
using AnointedAutomation.Concepts.Reasoning.Language;
using AnointedAutomation.Concepts.Reasoning.Language.FreeText;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AnointedAutomation.Concepts.Anthropic.Tests
{
    public class AnthropicLanguageModelTests
    {
        private static (AnthropicLanguageModel Model, RecordedHandler Handler, CapturingLogger<AnthropicLanguageModel> Logger) Build(params Func<HttpResponseMessage>[] responses)
        {
            return Build(new RecordedHandler(responses), TestKit.Options());
        }

        private static (AnthropicLanguageModel Model, RecordedHandler Handler, CapturingLogger<AnthropicLanguageModel> Logger) Build(RecordedHandler handler, AnthropicLanguageModelOptions options)
        {
            CapturingLogger<AnthropicLanguageModel> logger = new CapturingLogger<AnthropicLanguageModel>();
            AnthropicLanguageModel model = new AnthropicLanguageModel(handler, options, logger);
            return (model, handler, logger);
        }

        private static LanguageReasoner Reasoner(ILanguageModel model)
        {
            return new LanguageReasoner(model, new InterpretationParser(NullLogger<InterpretationParser>.Instance), TimeProvider.System);
        }

        private static void AssertKeyNeverLogged(CapturingLogger<AnthropicLanguageModel> logger)
        {
            foreach ((LogLevel _, string message, Exception exception) in logger.Entries)
            {
                Assert.DoesNotContain(TestKit.ApiKey, message, StringComparison.Ordinal);
                if (exception != null)
                {
                    Assert.DoesNotContain(TestKit.ApiKey, exception.ToString(), StringComparison.Ordinal);
                }
            }
        }

        [Fact]
        public async Task Success_ReturnsTheReplyText_FromOneStructuredOutputCall()
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> logger) = Build(() => TestKit.Message(TestKit.ValidReply));

            string reply = await model.Complete("the prompt", CancellationToken.None);

            Assert.Equal(TestKit.ValidReply, reply);
            Assert.Single(handler.Requests);
            Assert.Equal("Bearer", handler.Requests[0].Headers.Authorization.Scheme);
            Assert.Equal(TestKit.ApiKey, handler.Requests[0].Headers.Authorization.Parameter);
            using (JsonDocument body = JsonDocument.Parse(handler.Bodies[0]))
            {
                JsonElement root = body.RootElement;
                Assert.Equal(TestKit.ModelId, root.GetProperty("model").GetString());
                Assert.Equal(4096, root.GetProperty("max_tokens").GetInt32());
                Assert.Equal("the prompt", root.GetProperty("messages")[0].GetProperty("content").GetString());
                JsonElement format = root.GetProperty("output_config").GetProperty("format");
                Assert.Equal("json_schema", format.GetProperty("type").GetString());
                Assert.Equal("object", format.GetProperty("schema").GetProperty("type").GetString());
            }

            Assert.DoesNotContain("\"minimum\"", handler.Bodies[0], StringComparison.Ordinal);
            Assert.DoesNotContain("\"minLength\"", handler.Bodies[0], StringComparison.Ordinal);
            Assert.Equal("anthropic/" + TestKit.ModelId, model.Name);
            AssertKeyNeverLogged(logger);
        }

        [Fact]
        public async Task Success_FlowsThroughTheCoreParser_IntoAnAnswer()
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> _) = Build(() => TestKit.Message(TestKit.ValidReply));

            Answer answer = await Reasoner(model).Ask(TestKit.AbortionViolatesLove(), CancellationToken.None);

            Assert.Equal(TruthValue.True, answer.Truth);
            Assert.Equal(AnswerSource.Language, answer.Provenance.Source);
            Assert.Single(handler.Requests);
        }

        [Fact]
        public async Task MalformedReply_IsRejectedByTheCoreParser()
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> _) = Build(() => TestKit.Message("Sure! Here is my answer: it is wrong."));

            await Assert.ThrowsAsync<InterpretationRejected>(() => Reasoner(model).Ask(TestKit.AbortionViolatesLove(), CancellationToken.None));
            Assert.Single(handler.Requests);
        }

        [Fact]
        public async Task RateLimited_RetriesWithBackoff_ThenSucceeds()
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> logger) = Build(
                () => TestKit.Error(429, "rate_limit_error", "slow down"),
                () => TestKit.Message(TestKit.ValidReply));

            string reply = await model.Complete("the prompt", CancellationToken.None);

            Assert.Equal(TestKit.ValidReply, reply);
            Assert.Equal(2, handler.Requests.Count);
            Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Exception != null && e.Message.Contains("429", StringComparison.Ordinal));
            AssertKeyNeverLogged(logger);
        }

        [Theory]
        [InlineData(429, "rate_limit_error")]
        [InlineData(500, "api_error")]
        [InlineData(503, "api_error")]
        [InlineData(408, "timeout_error")]
        [InlineData(529, "overloaded_error")]
        public async Task TransientErrors_AreBounded_ThenRaisedAsTransient(int status, string type)
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> logger) = Build(
                () => TestKit.Error(status, type, "try later"),
                () => TestKit.Error(status, type, "try later"),
                () => TestKit.Error(status, type, "try later"));

            AnthropicTransientException failed = await Assert.ThrowsAsync<AnthropicTransientException>(() => model.Complete("the prompt", CancellationToken.None));

            Assert.Equal(3, failed.Attempts);
            Assert.NotNull(failed.InnerException);
            Assert.Equal(3, handler.Requests.Count);
            Assert.Equal(2, logger.Entries.Count(e => e.Level == LogLevel.Warning));
            Assert.Contains(logger.Entries, e => e.Level == LogLevel.Error && e.Exception == failed);
            AssertKeyNeverLogged(logger);
        }

        [Fact]
        public async Task NetworkErrors_AreRetried_ThenRaisedAsTransient()
        {
            RecordedHandler handler = RecordedHandler.Steps(
                _ => throw new HttpRequestException("connection reset"),
                _ => throw new HttpRequestException("connection reset"),
                _ => throw new HttpRequestException("connection reset"));
            (AnthropicLanguageModel model, RecordedHandler _, CapturingLogger<AnthropicLanguageModel> logger) = Build(handler, TestKit.Options());

            AnthropicTransientException failed = await Assert.ThrowsAsync<AnthropicTransientException>(() => model.Complete("the prompt", CancellationToken.None));

            Assert.Equal(3, failed.Attempts);
            Assert.Equal(3, handler.Requests.Count);
            Assert.Equal(2, logger.Entries.Count(e => e.Level == LogLevel.Warning));
        }

        [Fact]
        public async Task ClientTimeout_IsRetried_ThenSucceeds()
        {
            AnthropicLanguageModelOptions options = TestKit.Options();
            options.TimeoutSeconds = 1;
            RecordedHandler handler = RecordedHandler.Steps(
                async ct =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), ct).ConfigureAwait(false);
                    throw new InvalidOperationException("The SDK timeout should have cancelled this call.");
                },
                _ => Task.FromResult(TestKit.Message(TestKit.ValidReply)));
            (AnthropicLanguageModel model, RecordedHandler _, CapturingLogger<AnthropicLanguageModel> logger) = Build(handler, options);

            string reply = await model.Complete("the prompt", CancellationToken.None);

            Assert.Equal(TestKit.ValidReply, reply);
            Assert.Equal(2, handler.Requests.Count);
            Assert.Single(logger.Entries, e => e.Level == LogLevel.Warning);
        }

        [Fact]
        public async Task RateLimited_HonorsRetryAfter_OverTheLocalBackoff()
        {
            AnthropicLanguageModelOptions options = TestKit.Options();
            options.RetryBaseDelayMilliseconds = 25000;
            RecordedHandler handler = new RecordedHandler(
                () =>
                {
                    HttpResponseMessage limited = TestKit.Error(429, "rate_limit_error", "slow down");
                    limited.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.Zero);
                    return limited;
                },
                () => TestKit.Message(TestKit.ValidReply));
            (AnthropicLanguageModel model, RecordedHandler _, CapturingLogger<AnthropicLanguageModel> logger) = Build(handler, options);

            string reply = await model.Complete("the prompt", CancellationToken.None);

            Assert.Equal(TestKit.ValidReply, reply);
            Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains("retrying in 0 ms", StringComparison.Ordinal));
        }

        [Fact]
        public void RetryAfter_ReadsDeltaAndDate_AndNeverGoesNegative()
        {
            DateTimeOffset now = new DateTimeOffset(2026, 11, 12, 12, 0, 0, TimeSpan.Zero);

            Assert.Null(RetryAfterRecorder.Read(null, now));
            Assert.Equal(TimeSpan.FromSeconds(7), RetryAfterRecorder.Read(new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(7)), now));
            Assert.Equal(TimeSpan.FromSeconds(5), RetryAfterRecorder.Read(new System.Net.Http.Headers.RetryConditionHeaderValue(now.AddSeconds(5)), now));
            Assert.Equal(TimeSpan.Zero, RetryAfterRecorder.Read(new System.Net.Http.Headers.RetryConditionHeaderValue(now.AddSeconds(-5)), now));
        }

        [Theory]
        [InlineData(401, "authentication_error")]
        [InlineData(403, "permission_error")]
        public async Task AuthErrors_RaiseConfiguration_WithoutRetrying(int status, string type)
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> logger) = Build(() => TestKit.Error(status, type, "invalid x-api-key"));

            AnthropicConfigurationException failed = await Assert.ThrowsAsync<AnthropicConfigurationException>(() => model.Complete("the prompt", CancellationToken.None));

            Assert.NotNull(failed.InnerException);
            Assert.Single(handler.Requests);
            Assert.Contains(logger.Entries, e => e.Level == LogLevel.Critical && e.Exception == failed);
            Assert.DoesNotContain(TestKit.ApiKey, failed.Message, StringComparison.Ordinal);
            AssertKeyNeverLogged(logger);
        }

        [Fact]
        public async Task BadRequest_IsLoggedAndRethrown_WithoutRetrying()
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> logger) = Build(() => TestKit.Error(400, "invalid_request_error", "bad schema"));

            AnthropicApiException failed = await Assert.ThrowsAnyAsync<AnthropicApiException>(() => model.Complete("the prompt", CancellationToken.None));

            Assert.Single(handler.Requests);
            Assert.Contains(logger.Entries, e => e.Level == LogLevel.Error && e.Exception == failed);
        }

        [Fact]
        public async Task Cancellation_IsNotRetried()
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> logger) = Build(() => TestKit.Message(TestKit.ValidReply));
            using (CancellationTokenSource cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => model.Complete("the prompt", cancelled.Token));
            }

            Assert.True(handler.Requests.Count <= 1);
            Assert.DoesNotContain(logger.Entries, e => e.Level == LogLevel.Warning || e.Level == LogLevel.Error);
        }

        [Theory]
        [InlineData("ApiKey")]
        [InlineData("Model")]
        [InlineData("MaxTokens")]
        [InlineData("TimeoutSeconds")]
        [InlineData("MaxAttempts")]
        [InlineData("RetryBaseDelayMilliseconds")]
        public void MissingConfig_ThrowsConfiguration_AndNamesTheSetting(string setting)
        {
            AnthropicLanguageModelOptions options = TestKit.Options();
            switch (setting)
            {
                case "ApiKey": options.ApiKey = null; break;
                case "Model": options.Model = " "; break;
                case "MaxTokens": options.MaxTokens = 0; break;
                case "TimeoutSeconds": options.TimeoutSeconds = 0; break;
                case "MaxAttempts": options.MaxAttempts = 0; break;
                default: options.RetryBaseDelayMilliseconds = 0; break;
            }

            CapturingLogger<AnthropicLanguageModel> logger = new CapturingLogger<AnthropicLanguageModel>();
            AnthropicConfigurationException failed = Assert.Throws<AnthropicConfigurationException>(() => new AnthropicLanguageModel(new RecordedHandler(), options, logger));

            Assert.Contains(setting, failed.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(TestKit.ApiKey, failed.Message, StringComparison.Ordinal);
            Assert.Contains(logger.Entries, e => e.Level == LogLevel.Critical && e.Exception == failed);
        }

        [Fact]
        public void MaxAttempts_AboveTheLimit_IsRejected()
        {
            AnthropicLanguageModelOptions options = TestKit.Options();
            options.MaxAttempts = AnthropicLanguageModelOptions.MaxAttemptsLimit + 1;

            Assert.Throws<AnthropicConfigurationException>(() => options.ThrowIfInvalid());
        }

        [Fact]
        public async Task EmptyPrompt_IsRejected_BeforeAnyCall()
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> _) = Build();

            await Assert.ThrowsAsync<ArgumentException>(() => model.Complete(" ", CancellationToken.None));
            Assert.Empty(handler.Requests);
        }

        [Fact]
        public void ReplySchema_DropsOnlyUnsupportedKeywords()
        {
            Dictionary<string, JsonElement> schema = ReplySchemaFormat.Create();
            string json = JsonSerializer.Serialize(schema);

            foreach (string keyword in ReplySchemaFormat.UnsupportedKeywords)
            {
                Assert.DoesNotContain("\"" + keyword + "\"", json, StringComparison.Ordinal);
            }

            Assert.Equal("object", schema["type"].GetString());
            Assert.False(schema["additionalProperties"].GetBoolean());
            Assert.Equal(7, schema["required"].GetArrayLength());
            Assert.True(schema.ContainsKey("$defs"));
        }
        [Fact]
        public async Task CallerSchema_IsSentInsteadOfTheCoreSchema()
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> logger) = Build(() => TestKit.Message("{}"), () => TestKit.Message("{}"));

            await model.Complete("free text", FreeTextPromptComposer.ReplySchema, CancellationToken.None);
            await model.Complete("free text again", FreeTextPromptComposer.ReplySchema, CancellationToken.None);

            Assert.Equal(2, handler.Requests.Count);
            foreach (string sent in handler.Bodies)
            {
                using (JsonDocument body = JsonDocument.Parse(sent))
                {
                    JsonElement schema = body.RootElement.GetProperty("output_config").GetProperty("format").GetProperty("schema");
                    Assert.Equal(5, schema.GetProperty("required").GetArrayLength());
                    Assert.True(schema.GetProperty("properties").TryGetProperty("originalLanguageWords", out JsonElement _));
                }

                Assert.DoesNotContain("\"minLength\"", sent, StringComparison.Ordinal);
            }

            AssertKeyNeverLogged(logger);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        public async Task CallerSchema_Blank_IsRejected_BeforeAnyCall(string replySchema)
        {
            (AnthropicLanguageModel model, RecordedHandler handler, CapturingLogger<AnthropicLanguageModel> _) = Build(() => TestKit.Message("{}"));

            await Assert.ThrowsAsync<ArgumentException>(() => model.Complete("prompt", replySchema, CancellationToken.None));
            Assert.Empty(handler.Requests);
        }
    }
}
