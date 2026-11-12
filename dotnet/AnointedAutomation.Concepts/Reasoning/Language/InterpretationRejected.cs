// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;

namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// Thrown when a language model reply is not a valid <see cref="Interpretation"/>. The reply is
    /// rejected whole; it is never patched or partly accepted.
    /// </summary>
    public sealed class InterpretationRejected : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterpretationRejected"/> class.
        /// </summary>
        /// <param name="reason">Why the reply was rejected.</param>
        /// <param name="reply">The raw reply, kept in full for the log.</param>
        /// <param name="inner">The underlying parse or validation error, or null when there is none.</param>
        public InterpretationRejected(string reason, string reply, Exception inner)
            : base("Language model reply rejected: " + reason, inner)
        {
            Reason = reason;
            Reply = reply;
        }

        /// <summary>
        /// Why the reply was rejected.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// The raw reply text, in full.
        /// </summary>
        public string Reply { get; }
    }
}
