// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Epistemics
{
    /// <summary>
    /// How settled a foundational claim is. This is a statement about the claim's standing in the
    /// ledger, not about the world it describes: the claim itself is always falsifiable, but the
    /// weight of falsification it has survived (or lack thereof) puts it at a different place on
    /// this spectrum.
    /// </summary>
    public enum EpistemicStatus
    {
        /// <summary>
        /// Survived so much falsification that it functions as bedrock, e.g. conservation of energy.
        /// </summary>
        Law,

        /// <summary>
        /// Well supported, but still contested at the edges.
        /// </summary>
        Theory,

        /// <summary>
        /// Asserted and unproven, standing null, e.g. the Collatz conjecture.
        /// </summary>
        Conjecture
    }
}
