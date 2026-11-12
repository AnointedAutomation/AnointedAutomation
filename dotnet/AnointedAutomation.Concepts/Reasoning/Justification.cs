// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Why an <see cref="Answer"/> holds: the derivation steps, the scripture it rests on, the facts
    /// it used, and a plain-words explanation. Lists may be empty (an Unknown answer may have no
    /// derivation) but are never null; the explanation is always required.
    /// </summary>
    public sealed record Justification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Justification"/> record.
        /// </summary>
        /// <param name="steps">The ordered derivation steps.</param>
        /// <param name="scripture">The scripture the answer rests on.</param>
        /// <param name="factsUsed">The assertions used as facts.</param>
        /// <param name="explanation">The plain-words explanation.</param>
        public Justification(
            IReadOnlyList<ProofStep> steps,
            IReadOnlyList<ScriptureReference> scripture,
            IReadOnlyList<Assertion> factsUsed,
            string explanation)
        {
            Steps = ReasoningGuard.CopyOf(steps, nameof(steps));
            Scripture = ReasoningGuard.CopyOf(scripture, nameof(scripture));
            FactsUsed = ReasoningGuard.CopyOf(factsUsed, nameof(factsUsed));
            Explanation = ReasoningGuard.NotBlank(explanation, nameof(explanation));
        }

        /// <summary>
        /// The ordered derivation steps.
        /// </summary>
        public IReadOnlyList<ProofStep> Steps { get; }

        /// <summary>
        /// The scripture the answer rests on.
        /// </summary>
        public IReadOnlyList<ScriptureReference> Scripture { get; }

        /// <summary>
        /// The assertions used as facts.
        /// </summary>
        public IReadOnlyList<Assertion> FactsUsed { get; }

        /// <summary>
        /// The plain-words explanation.
        /// </summary>
        public string Explanation { get; }
    }
}
