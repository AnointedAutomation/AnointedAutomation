// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// Whether a proposition can, even in principle, be tested from inside the universe. This
    /// drives the <see cref="Verdict.Unfalsifiable"/> verdict: origin-of-universe and
    /// outside-the-universe subject matter cannot be checked by observers who cannot step outside
    /// to look.
    /// </summary>
    public enum Testability
    {
        /// <summary>
        /// Observation or experiment inside the universe can bear on it.
        /// </summary>
        EmpiricallyTestable,

        /// <summary>
        /// No observation from inside the universe can ever settle it.
        /// </summary>
        BeyondObservation
    }
}
