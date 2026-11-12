// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// Decides who may review interpretations. Only the owner of the canon may promote an AI answer
    /// toward a permanent fact; the consumer supplies that rule. Core ships no implementation, so there
    /// is no permissive default.
    /// </summary>
    public interface IPromotionPolicy
    {
        /// <summary>
        /// Returns whether <paramref name="reviewer"/> may promote <paramref name="entry"/>.
        /// </summary>
        /// <param name="reviewer">The reviewer asking.</param>
        /// <param name="entry">The entry to promote.</param>
        /// <returns>True only when the reviewer is allowed.</returns>
        bool MayPromote(Reviewer reviewer, InterpretationEntry entry);

        /// <summary>
        /// Returns whether <paramref name="reviewer"/> may reject <paramref name="entry"/>.
        /// </summary>
        /// <param name="reviewer">The reviewer asking.</param>
        /// <param name="entry">The entry to reject.</param>
        /// <returns>True only when the reviewer is allowed.</returns>
        bool MayReject(Reviewer reviewer, InterpretationEntry entry);
    }
}
