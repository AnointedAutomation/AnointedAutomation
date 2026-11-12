// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// The person reviewing an interpretation. Who may promote or reject is decided by an injected
    /// <see cref="IPromotionPolicy"/>, never by this record.
    /// </summary>
    public sealed record Reviewer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reviewer"/> record.
        /// </summary>
        /// <param name="reviewerId">The consumer's stable id for the reviewer.</param>
        public Reviewer(string reviewerId)
        {
            ReviewerId = ReasoningGuard.NotBlank(reviewerId, nameof(reviewerId));
        }

        /// <summary>
        /// Gets the reviewer id.
        /// </summary>
        public string ReviewerId { get; }
    }
}
