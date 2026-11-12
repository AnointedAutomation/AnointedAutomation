// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Three-valued (Kleene) truth. <see cref="Unknown"/> means "not established", which is
    /// distinct from <see cref="False"/>: an absent proof is never a disproof. Unknown is value 0 per
    /// the repo enum convention, so an unset value reads as "not established", never as False. There
    /// is deliberately no implicit conversion to or from bool; combine values with
    /// <see cref="TruthLogic"/>.
    /// </summary>
    public enum TruthValue
    {
        /// <summary>
        /// Not established: untested, unknowable, or not yet reasoned about. Never the same as False.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Established as true.
        /// </summary>
        True = 1,

        /// <summary>
        /// Established as false.
        /// </summary>
        False = 2
    }
}
