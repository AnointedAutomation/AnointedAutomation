// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// What examining a claim yields. Four states rather than a boolean, because "contradicted"
    /// and "untestable" are different findings, and both differ from "not yet determined".
    /// Contradiction is a verdict, never an exception.
    /// </summary>
    public enum Verdict
    {
        /// <summary>
        /// Nothing falsified it; provisional and true-leaning, like every scientific claim.
        /// </summary>
        Consistent,

        /// <summary>
        /// Collides with the current unfalsified set.
        /// </summary>
        Contradicts,

        /// <summary>
        /// Can never be tested from inside the universe. A statement about testability, not truth,
        /// and symmetric across theism and atheism.
        /// </summary>
        Unfalsifiable,

        /// <summary>
        /// Testable in principle, but the evidence is insufficient.
        /// </summary>
        Undetermined
    }
}
