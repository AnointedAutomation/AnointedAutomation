// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// The language engine's full result: the <see cref="Reasoning.Answer"/> every reasoner returns,
    /// plus the whole validated <see cref="Language.Interpretation"/> (candidate sins and suggested
    /// assertions included) so later steps reuse it instead of asking the model again.
    /// </summary>
    public sealed record LanguageResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageResult"/> record.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="interpretation">The interpretation the answer was built from.</param>
        public LanguageResult(Answer answer, Interpretation interpretation)
        {
            Answer = ReasoningGuard.NotNull(answer, nameof(answer));
            Interpretation = ReasoningGuard.NotNull(interpretation, nameof(interpretation));
        }

        /// <summary>The answer, always Unverified.</summary>
        public Answer Answer { get; }

        /// <summary>The validated interpretation.</summary>
        public Interpretation Interpretation { get; }
    }
}
