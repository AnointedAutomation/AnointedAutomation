// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// One page of an owner's ledger entries plus the owner's total.
    /// </summary>
    public sealed record InterpretationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterpretationPage"/> record.
        /// </summary>
        /// <param name="entries">The entries on this page.</param>
        /// <param name="total">The owner's total entry count.</param>
        public InterpretationPage(IReadOnlyList<InterpretationEntry> entries, int total)
        {
            Entries = ReasoningGuard.CopyOf(entries, nameof(entries));
            if (total < Entries.Count)
            {
                throw new System.ArgumentOutOfRangeException(nameof(total), total, "The total cannot be smaller than the page.");
            }

            Total = total;
        }

        /// <summary>Gets the entries on this page.</summary>
        public IReadOnlyList<InterpretationEntry> Entries { get; }

        /// <summary>Gets the owner's total entry count.</summary>
        public int Total { get; }
    }
}
