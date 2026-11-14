// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-13 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning.Language.FreeText
{
    /// <summary>
    /// Everything one paid reply returns for a free-text question: the answer, the verdict, the cited verses and the
    /// original-language words. Unverified until the owner reviews it.
    /// </summary>
    public sealed record FreeTextAnswer
    {
        /// <summary>
        /// Creates an answer.
        /// </summary>
        /// <param name="answer">The answer in plain words.</param>
        /// <param name="verdict">True or False for a yes/no question Scripture settles; otherwise Unknown.</param>
        /// <param name="confidence">Confidence from 0 to 1.</param>
        /// <param name="scripture">The verses cited (English quotations are NASB).</param>
        /// <param name="originalLanguageWords">The Hebrew, Aramaic or Greek words the answer reasons from.</param>
        /// <param name="promptId">The prompt id (the question's cache key with the model).</param>
        /// <param name="model">The model that answered.</param>
        public FreeTextAnswer(
            string answer,
            TruthValue verdict,
            double confidence,
            IEnumerable<ScriptureReference> scripture,
            IEnumerable<OriginalLanguageWord> originalLanguageWords,
            string promptId,
            string model)
        {
            Answer = ReasoningGuard.NotBlank(answer, nameof(answer));
            Verdict = ReasoningGuard.Defined(verdict, nameof(verdict));
            if (!double.IsFinite(confidence) || confidence < 0.0 || confidence > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(confidence), "Confidence must be from 0 to 1.");
            }

            Confidence = confidence;
            Scripture = ReasoningGuard.CopyOf(scripture, nameof(scripture));
            OriginalLanguageWords = ReasoningGuard.CopyOf(originalLanguageWords, nameof(originalLanguageWords));
            PromptId = ReasoningGuard.NotBlank(promptId, nameof(promptId));
            Model = ReasoningGuard.NotBlank(model, nameof(model));
        }

        /// <summary>
        /// Gets the answer in plain words.
        /// </summary>
        public string Answer { get; }

        /// <summary>
        /// Gets the verdict: Unknown when Scripture does not settle it or the question is not yes/no.
        /// </summary>
        public TruthValue Verdict { get; }

        /// <summary>
        /// Gets the confidence from 0 to 1.
        /// </summary>
        public double Confidence { get; }

        /// <summary>
        /// Gets the cited verses.
        /// </summary>
        public IReadOnlyList<ScriptureReference> Scripture { get; }

        /// <summary>
        /// Gets the original-language words.
        /// </summary>
        public IReadOnlyList<OriginalLanguageWord> OriginalLanguageWords { get; }

        /// <summary>
        /// Gets the prompt id.
        /// </summary>
        public string PromptId { get; }

        /// <summary>
        /// Gets the model that answered.
        /// </summary>
        public string Model { get; }
    }
}
