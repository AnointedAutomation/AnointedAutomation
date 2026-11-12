// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// A validated language model reply: everything any consumer needs about one query, asked for
    /// in a single call so it never has to be asked again.
    /// </summary>
    public sealed record Interpretation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Interpretation"/> record.
        /// </summary>
        /// <param name="classification">The concepts the situation falls under.</param>
        /// <param name="truth">The three-valued verdict on the asserted relation.</param>
        /// <param name="confidence">Confidence, 0.0 to 1.0.</param>
        /// <param name="explanation">The plain-words explanation.</param>
        /// <param name="scripture">The validated scripture cited.</param>
        /// <param name="sinIds">Candidate catalogued sins.</param>
        /// <param name="assertions">Suggested assertions; suggestions only, never facts until the owner promotes them.</param>
        public Interpretation(
            IReadOnlyList<ConceptRef> classification,
            TruthValue truth,
            double confidence,
            string explanation,
            IReadOnlyList<ScriptureReference> scripture,
            IReadOnlyList<SinId> sinIds,
            IReadOnlyList<Assertion> assertions)
        {
            Classification = ReasoningGuard.CopyOf(classification, nameof(classification));
            Truth = ReasoningGuard.Defined(truth, nameof(truth));
            if (double.IsNaN(confidence) || confidence < 0.0 || confidence > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(confidence), confidence, "Confidence must be between 0.0 and 1.0.");
            }

            Confidence = confidence;
            Explanation = ReasoningGuard.NotBlank(explanation, nameof(explanation));
            Scripture = ReasoningGuard.CopyOf(scripture, nameof(scripture));
            SinIds = ReasoningGuard.CopyOf(sinIds, nameof(sinIds));
            Assertions = ReasoningGuard.CopyOf(assertions, nameof(assertions));
        }

        /// <summary>The concepts the situation falls under.</summary>
        public IReadOnlyList<ConceptRef> Classification { get; }

        /// <summary>The three-valued verdict.</summary>
        public TruthValue Truth { get; }

        /// <summary>Confidence, 0.0 to 1.0.</summary>
        public double Confidence { get; }

        /// <summary>The plain-words explanation.</summary>
        public string Explanation { get; }

        /// <summary>The validated scripture cited.</summary>
        public IReadOnlyList<ScriptureReference> Scripture { get; }

        /// <summary>Candidate catalogued sins.</summary>
        public IReadOnlyList<SinId> SinIds { get; }

        /// <summary>Suggested assertions (unverified).</summary>
        public IReadOnlyList<Assertion> Assertions { get; }
    }
}
