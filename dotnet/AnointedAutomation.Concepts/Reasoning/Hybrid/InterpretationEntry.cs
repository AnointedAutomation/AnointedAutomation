// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Reasoning.Language;

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// One language-engine answer kept in the <see cref="InterpretationLedger"/>, private to its owner.
    /// </summary>
    public sealed record InterpretationEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterpretationEntry"/> record.
        /// </summary>
        /// <param name="id">The entry id.</param>
        /// <param name="owner">The user or chat the entry belongs to.</param>
        /// <param name="queryKey">The query hash (the composed prompt id) used as the cache key with the model.</param>
        /// <param name="query">The query asked.</param>
        /// <param name="answer">The language-derived answer.</param>
        /// <param name="interpretation">The full validated interpretation behind the answer.</param>
        /// <param name="provenance">Where the answer came from: always the language engine, unverified when recorded.</param>
        /// <param name="status">The review state.</param>
        /// <param name="reviewNote">The last reviewer's note; empty until reviewed, never null.</param>
        public InterpretationEntry(
            string id,
            OwnerScope owner,
            string queryKey,
            Query query,
            Answer answer,
            Interpretation interpretation,
            Provenance provenance,
            InterpretationStatus status,
            string reviewNote)
        {
            Id = ReasoningGuard.NotBlank(id, nameof(id));
            Owner = ReasoningGuard.NotNull(owner, nameof(owner));
            QueryKey = ReasoningGuard.NotBlank(queryKey, nameof(queryKey));
            Query = ReasoningGuard.NotNull(query, nameof(query));
            Answer = ReasoningGuard.NotNull(answer, nameof(answer));
            Interpretation = ReasoningGuard.NotNull(interpretation, nameof(interpretation));
            Provenance = ReasoningGuard.NotNull(provenance, nameof(provenance));
            if (provenance.Source != AnswerSource.Language)
            {
                throw new System.ArgumentException("Only language-derived answers belong in the ledger.", nameof(provenance));
            }

            Status = ReasoningGuard.DefinedAndSet(status, nameof(status));
            ReviewNote = ReasoningGuard.NotNull(reviewNote, nameof(reviewNote));
        }

        /// <summary>Gets the entry id.</summary>
        public string Id { get; }

        /// <summary>Gets the owner the entry is private to.</summary>
        public OwnerScope Owner { get; }

        /// <summary>Gets the query hash.</summary>
        public string QueryKey { get; }

        /// <summary>Gets the query asked.</summary>
        public Query Query { get; }

        /// <summary>Gets the language-derived answer.</summary>
        public Answer Answer { get; }

        /// <summary>Gets the full interpretation.</summary>
        public Interpretation Interpretation { get; }

        /// <summary>Gets the provenance recorded with the entry.</summary>
        public Provenance Provenance { get; }

        /// <summary>Gets the review state.</summary>
        public InterpretationStatus Status { get; }

        /// <summary>Gets the last reviewer's note.</summary>
        public string ReviewNote { get; }

        /// <summary>
        /// Returns a copy with a new status and note.
        /// </summary>
        /// <param name="status">The new status.</param>
        /// <param name="note">The reviewer's note.</param>
        /// <returns>The reviewed entry.</returns>
        public InterpretationEntry Reviewed(InterpretationStatus status, string note)
        {
            return new InterpretationEntry(Id, Owner, QueryKey, Query, Answer, Interpretation, Provenance, status, ReasoningGuard.NotBlank(note, nameof(note)));
        }
    }
}
