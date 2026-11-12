// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// The user or chat an interpretation belongs to. AI answers are private to their owner's chat and
    /// are never public, so every ledger write and read names one.
    /// </summary>
    public sealed record OwnerScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerScope"/> record.
        /// </summary>
        /// <param name="ownerId">The consumer's stable id for the user or chat; compared ordinally.</param>
        public OwnerScope(string ownerId)
        {
            OwnerId = ReasoningGuard.NotBlank(ownerId, nameof(ownerId));
        }

        /// <summary>
        /// Gets the owner id.
        /// </summary>
        public string OwnerId { get; }
    }
}
