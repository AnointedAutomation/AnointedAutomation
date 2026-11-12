// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// A question for an <see cref="IReasoner"/>: does this assertion hold, given these
    /// circumstances? The context is copied, so later changes to the caller's list do not leak in.
    /// Equality of <see cref="Context"/> is by reference, as with every list-valued record member.
    /// </summary>
    public sealed record Query
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> record.
        /// </summary>
        /// <param name="assertion">The assertion to evaluate.</param>
        /// <param name="context">The circumstances in force; may be empty, never null.</param>
        public Query(Assertion assertion, IReadOnlyList<Circumstance> context)
        {
            Assertion = ReasoningGuard.NotNull(assertion, nameof(assertion));
            Context = ReasoningGuard.CopyOf(context, nameof(context));
        }

        /// <summary>
        /// The assertion to evaluate.
        /// </summary>
        public Assertion Assertion { get; }

        /// <summary>
        /// The circumstances in force.
        /// </summary>
        public IReadOnlyList<Circumstance> Context { get; }
    }
}
