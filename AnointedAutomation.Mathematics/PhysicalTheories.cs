// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The theory catalog: foundational claims with status <see cref="EpistemicStatus.Theory"/>,
    /// well supported by evidence but still contested at the edges rather than bedrock.
    /// </summary>
    public static class PhysicalTheories
    {
        /// <summary>
        /// Mass-energy equivalence: mass and energy are equivalent and convertible according to
        /// E equals m times c squared.
        /// </summary>
        public static readonly FoundationalClaim MassEnergyEquivalence = new FoundationalClaim(
            "MassEnergyEquivalence",
            "Mass and energy are equivalent and convertible according to E equals m times c squared.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MassEnergyEquivalent },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Invariant light speed: the speed of light in a vacuum is the same for every observer,
        /// regardless of motion.
        /// </summary>
        public static readonly FoundationalClaim InvariantLightSpeed = new FoundationalClaim(
            "InvariantLightSpeed",
            "The speed of light in a vacuum is the same for every observer, regardless of motion.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.SpeedOfLightConstant },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Every theory declared by this catalog, in declaration order.
        /// </summary>
        public static readonly IReadOnlyList<FoundationalClaim> All = new List<FoundationalClaim>
        {
            MassEnergyEquivalence,
            InvariantLightSpeed
        };
    }
}
