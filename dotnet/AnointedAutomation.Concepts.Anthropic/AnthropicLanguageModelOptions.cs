// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Anthropic
{
    /// <summary>
    /// Settings for <see cref="AnthropicLanguageModel"/>, bound from configuration. Every value is required:
    /// there are no defaults and no fallback model. Keep the API key in a secret store or environment
    /// configuration, never in source control.
    /// </summary>
    public sealed class AnthropicLanguageModelOptions
    {
        /// <summary>
        /// The upper bound for <see cref="MaxAttempts"/>, so a typo cannot turn into an unbounded retry loop.
        /// </summary>
        public const int MaxAttemptsLimit = 10;

        /// <summary>
        /// The Anthropic API key. Required. Never logged.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The Claude model id, for example "claude-opus-5-5". Required.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The most tokens one reply may use. Required, above zero.
        /// </summary>
        public int MaxTokens { get; set; }

        /// <summary>
        /// The time allowed for one HTTP call, in seconds. Required, above zero.
        /// </summary>
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// How many calls to make in total when the API fails with a transient error. Required, 1 to <see cref="MaxAttemptsLimit"/>.
        /// </summary>
        public int MaxAttempts { get; set; }

        /// <summary>
        /// The first backoff delay in milliseconds; it doubles on each retry. Required, above zero.
        /// </summary>
        public int RetryBaseDelayMilliseconds { get; set; }

        /// <summary>
        /// Lists every missing or invalid setting. Empty when the options are usable. Never includes the key's value.
        /// </summary>
        /// <returns>The problems found.</returns>
        public IReadOnlyList<string> Problems()
        {
            List<string> problems = new List<string>();
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                problems.Add("ApiKey is required");
            }

            if (string.IsNullOrWhiteSpace(Model))
            {
                problems.Add("Model is required");
            }

            if (MaxTokens <= 0)
            {
                problems.Add("MaxTokens must be above zero");
            }

            if (TimeoutSeconds <= 0)
            {
                problems.Add("TimeoutSeconds must be above zero");
            }

            if (MaxAttempts < 1 || MaxAttempts > MaxAttemptsLimit)
            {
                problems.Add("MaxAttempts must be from 1 to " + MaxAttemptsLimit);
            }

            if (RetryBaseDelayMilliseconds <= 0)
            {
                problems.Add("RetryBaseDelayMilliseconds must be above zero");
            }

            return problems;
        }

        /// <summary>
        /// Throws <see cref="AnthropicConfigurationException"/> naming every problem when the options are not usable.
        /// </summary>
        public void ThrowIfInvalid()
        {
            IReadOnlyList<string> problems = Problems();
            if (problems.Count > 0)
            {
                throw new AnthropicConfigurationException("Anthropic language model configuration is invalid: " + string.Join("; ", problems) + ".", null);
            }
        }
    }
}
