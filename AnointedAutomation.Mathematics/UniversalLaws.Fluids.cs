// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The fluid mechanics laws of the bedrock catalog.
    /// </summary>
    public static partial class UniversalLaws
    {
        /// <summary>
        /// Pascal's Law: for an idealized incompressible fluid, pressure applied to an enclosed
        /// fluid is transmitted undiminished throughout the fluid.
        /// </summary>
        public static readonly FoundationalClaim PascalsLaw = new FoundationalClaim(
            "PascalsLaw",
            "For an idealized incompressible fluid, pressure applied to an enclosed fluid is transmitted undiminished throughout the fluid.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.PressureTransmittedUndiminishedInEnclosedFluid },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Archimedes' Principle: the buoyant force on a submerged body equals the weight of the
        /// fluid it displaces.
        /// </summary>
        public static readonly FoundationalClaim ArchimedesPrinciple = new FoundationalClaim(
            "ArchimedesPrinciple",
            "The buoyant force on a submerged body equals the weight of the fluid it displaces.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.BuoyantForceEqualsWeightOfDisplacedFluid },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Bernoulli's Principle: for idealized inviscid, incompressible flow, an increase in
        /// fluid speed occurs together with a decrease in pressure or potential energy.
        /// </summary>
        public static readonly FoundationalClaim BernoullisPrinciple = new FoundationalClaim(
            "BernoullisPrinciple",
            "For idealized inviscid, incompressible flow, an increase in fluid speed occurs together with a decrease in pressure or potential energy.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.FluidSpeedIncreaseAccompaniesPressureDecrease },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Torricelli's Law: for idealized inviscid, frictionless flow, the speed of fluid efflux
        /// from an orifice relates to the height of fluid above it.
        /// </summary>
        public static readonly FoundationalClaim TorricellisLaw = new FoundationalClaim(
            "TorricellisLaw",
            "For idealized inviscid, frictionless flow, the speed of fluid efflux from an orifice relates to the height of fluid above it.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.FluidEffluxSpeedRelatesToHeight },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);
    }
}
