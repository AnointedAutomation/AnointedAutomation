// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// What the owner approved for the canon: the assertion and its scripture, to be added to the source
    /// catalog in a reviewed PR. It does not change the running <see cref="CanonFacts"/>.
    /// </summary>
    public sealed record FactPromotion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactPromotion"/> record.
        /// </summary>
        /// <param name="entryId">The promoted ledger entry.</param>
        /// <param name="assertion">The assertion to add.</param>
        /// <param name="truth">The approved truth value; True or False.</param>
        /// <param name="scripture">The scripture to cite.</param>
        /// <param name="reviewer">Who approved it.</param>
        /// <param name="note">The reviewer's note.</param>
        /// <param name="at">When it was approved.</param>
        public FactPromotion(
            string entryId,
            Assertion assertion,
            TruthValue truth,
            IReadOnlyList<ScriptureReference> scripture,
            Reviewer reviewer,
            string note,
            System.DateTimeOffset at)
        {
            EntryId = ReasoningGuard.NotBlank(entryId, nameof(entryId));
            Assertion = ReasoningGuard.NotNull(assertion, nameof(assertion));
            Truth = ReasoningGuard.DefinedAndSet(truth, nameof(truth));
            Scripture = ReasoningGuard.CopyOf(scripture, nameof(scripture));
            Reviewer = ReasoningGuard.NotNull(reviewer, nameof(reviewer));
            Note = ReasoningGuard.NotBlank(note, nameof(note));
            if (at == default(System.DateTimeOffset))
            {
                throw new System.ArgumentOutOfRangeException(nameof(at), at, "A timestamp is required.");
            }

            At = at;
        }

        /// <summary>Gets the promoted ledger entry id.</summary>
        public string EntryId { get; }

        /// <summary>Gets the assertion to add.</summary>
        public Assertion Assertion { get; }

        /// <summary>Gets the approved truth value.</summary>
        public TruthValue Truth { get; }

        /// <summary>Gets the scripture to cite.</summary>
        public IReadOnlyList<ScriptureReference> Scripture { get; }

        /// <summary>Gets who approved it.</summary>
        public Reviewer Reviewer { get; }

        /// <summary>Gets the reviewer's note.</summary>
        public string Note { get; }

        /// <summary>Gets when it was approved.</summary>
        public System.DateTimeOffset At { get; }
    }
}
