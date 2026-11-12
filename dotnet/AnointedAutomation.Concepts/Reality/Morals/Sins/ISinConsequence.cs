// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// Extension point for what committing a sin does to a target (a character, an account, a
    /// score). The catalog defines no consequences; a consumer such as a game implements this and
    /// registers it in a <see cref="SinConsequenceRegistry{TTarget}"/>.
    /// </summary>
    /// <typeparam name="TTarget">What the consequence acts on.</typeparam>
    public interface ISinConsequence<in TTarget>
    {
        /// <summary>
        /// Applies the consequence.
        /// </summary>
        /// <param name="sin">The sin committed.</param>
        /// <param name="target">What it acts on.</param>
        void Apply(SinEntry sin, TTarget target);
    }
}
