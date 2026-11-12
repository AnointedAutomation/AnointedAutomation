// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;

namespace AnointedAutomation.Concepts.Anthropic
{
    /// <summary>
    /// The Anthropic API kept failing with a transient error (429 rate limit, 408, 5xx or overload, a network error
    /// or a timeout) after every bounded retry. The inner exception is the last SDK error. Callers may try again later.
    /// </summary>
    public sealed class AnthropicTransientException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">What failed.</param>
        /// <param name="attempts">How many calls were made.</param>
        /// <param name="inner">The last transient error.</param>
        public AnthropicTransientException(string message, int attempts, Exception inner)
            : base(message, inner)
        {
            Attempts = attempts;
        }

        /// <summary>
        /// How many calls were made before giving up.
        /// </summary>
        public int Attempts { get; }
    }
}
