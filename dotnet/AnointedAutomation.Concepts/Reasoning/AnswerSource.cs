// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Which engine produced an <see cref="Answer"/>. Value 0 is <see cref="Unknown"/> per the repo
    /// enum convention so an unset source is detectable; <see cref="Provenance"/> rejects it, because
    /// every answer must say where it came from.
    /// </summary>
    public enum AnswerSource
    {
        /// <summary>
        /// Not determined. Never valid inside a <see cref="Provenance"/>.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The deterministic engine, reasoning only over codified canon (catalogs, rules, proofs).
        /// </summary>
        Canon = 1,

        /// <summary>
        /// The language engine (an AI model). Its interpretations are never permanent facts on their
        /// own: only the owner can approve promoting an AI interpretation into a permanent fact.
        /// </summary>
        Language = 2
    }
}
