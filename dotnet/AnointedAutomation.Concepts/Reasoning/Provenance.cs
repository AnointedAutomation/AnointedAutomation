// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Where an <see cref="Answer"/> came from: which engine, which model, when, its verification
    /// status, and the rule or prompt that produced it. Every field is required; there are no
    /// defaults.
    /// <para>
    /// Owner rule: only the owner can approve promoting an AI interpretation (an answer whose
    /// <see cref="Source"/> is <see cref="AnswerSource.Language"/>) into a permanent fact. Provenance
    /// records where an answer came from; it never grants that promotion.
    /// </para>
    /// </summary>
    public sealed record Provenance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Provenance"/> record.
        /// </summary>
        /// <param name="source">The engine; <see cref="AnswerSource.Unknown"/> is rejected.</param>
        /// <param name="model">The engine or model identifier and version.</param>
        /// <param name="at">When the answer was produced; the default value is rejected.</param>
        /// <param name="status">The verification status; <see cref="VerificationStatus.Unknown"/> is rejected.</param>
        /// <param name="ruleOrPromptId">The canon rule id or prompt id that produced the answer.</param>
        public Provenance(
            AnswerSource source,
            string model,
            System.DateTimeOffset at,
            VerificationStatus status,
            string ruleOrPromptId)
        {
            Source = ReasoningGuard.DefinedAndSet(source, nameof(source));
            Model = ReasoningGuard.NotBlank(model, nameof(model));
            if (at == default(System.DateTimeOffset))
            {
                throw new System.ArgumentOutOfRangeException(nameof(at), at, "A timestamp is required.");
            }

            At = at;
            Status = ReasoningGuard.DefinedAndSet(status, nameof(status));
            RuleOrPromptId = ReasoningGuard.NotBlank(ruleOrPromptId, nameof(ruleOrPromptId));
        }

        /// <summary>
        /// The engine that produced the answer.
        /// </summary>
        public AnswerSource Source { get; }

        /// <summary>
        /// The engine or model identifier and version.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// When the answer was produced.
        /// </summary>
        public System.DateTimeOffset At { get; }

        /// <summary>
        /// Whether the answer was checked against canon.
        /// </summary>
        public VerificationStatus Status { get; }

        /// <summary>
        /// The canon rule id or prompt id that produced the answer.
        /// </summary>
        public string RuleOrPromptId { get; }
    }
}
