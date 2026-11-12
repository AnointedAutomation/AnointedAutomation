// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;

namespace AnointedAutomation.Concepts.Anthropic
{
    /// <summary>
    /// The adapter is misconfigured: a required option is missing or invalid, or the Anthropic API rejected the
    /// configured key (401) or its permissions (403). Retrying does not help; fix the configuration.
    /// The message never contains the API key.
    /// </summary>
    public sealed class AnthropicConfigurationException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">What is wrong, without any secret value.</param>
        /// <param name="inner">The SDK error that revealed it, or null for a local validation failure.</param>
        public AnthropicConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
