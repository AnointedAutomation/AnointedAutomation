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
    /// <remarks>
    /// A theory here is not a guess. It is a claim well supported by a large body of evidence,
    /// but unlike a law it is, by definition, still open at the edges and subject to refinement
    /// as new evidence arrives rather than treated as settled bedrock.
    /// </remarks>
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
        /// Special relativity: the laws of physics are the same in all inertial frames and the
        /// speed of light is invariant, so space and time measurements depend on relative motion.
        /// </summary>
        public static readonly FoundationalClaim SpecialRelativity = new FoundationalClaim(
            "SpecialRelativity",
            "The laws of physics are the same in all inertial frames and the speed of light is invariant, so space and time measurements depend on relative motion.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.SpecialRelativityHolds },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// General relativity: gravity is the curvature of spacetime by mass and energy.
        /// </summary>
        public static readonly FoundationalClaim GeneralRelativity = new FoundationalClaim(
            "GeneralRelativity",
            "Gravity is the curvature of spacetime by mass and energy.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.GravityIsSpacetimeCurvature },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Quantum mechanics: matter and energy at the smallest scales behave as quantized states
        /// with probabilistic outcomes.
        /// </summary>
        public static readonly FoundationalClaim QuantumMechanics = new FoundationalClaim(
            "QuantumMechanics",
            "Matter and energy at the smallest scales behave as quantized states with probabilistic outcomes.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MatterEnergyIsQuantized },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Atomic theory: all matter is composed of atoms.
        /// </summary>
        public static readonly FoundationalClaim AtomicTheory = new FoundationalClaim(
            "AtomicTheory",
            "All matter is composed of atoms.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MatterComposedOfAtoms },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Big Bang cosmology: the observable universe expanded from an extremely hot, dense
        /// early state roughly 13.8 billion years ago.
        /// </summary>
        public static readonly FoundationalClaim BigBangCosmology = new FoundationalClaim(
            "BigBangCosmology",
            "The observable universe expanded from an extremely hot, dense early state roughly 13.8 billion years ago.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.UniverseExpandedFromHotDenseState },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Evolution by natural selection: populations of organisms change across generations
        /// through heritable variation and differential reproductive success.
        /// </summary>
        public static readonly FoundationalClaim EvolutionByNaturalSelection = new FoundationalClaim(
            "EvolutionByNaturalSelection",
            "Populations of organisms change across generations through heritable variation and differential reproductive success.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.PopulationsEvolveByNaturalSelection },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Germ theory of disease: many diseases are caused by microorganisms.
        /// </summary>
        public static readonly FoundationalClaim GermTheoryOfDisease = new FoundationalClaim(
            "GermTheoryOfDisease",
            "Many diseases are caused by microorganisms.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.DiseasesCausedByMicroorganisms },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Cell theory: all living organisms are composed of cells, and all cells arise from
        /// pre-existing cells.
        /// </summary>
        public static readonly FoundationalClaim CellTheory = new FoundationalClaim(
            "CellTheory",
            "All living organisms are composed of cells, and all cells arise from pre-existing cells.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.OrganismsComposedOfCells },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Plate tectonics: Earth's lithosphere is divided into plates whose motion produces
        /// continental drift, earthquakes, and mountain building.
        /// </summary>
        public static readonly FoundationalClaim PlateTectonics = new FoundationalClaim(
            "PlateTectonics",
            "Earth's lithosphere is divided into plates whose motion produces continental drift, earthquakes, and mountain building.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.LithosphereDividedIntoMovingPlates },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Kinetic theory of gases: gas pressure and temperature arise from the motion and
        /// collisions of large numbers of molecules.
        /// </summary>
        public static readonly FoundationalClaim KineticTheoryOfGases = new FoundationalClaim(
            "KineticTheoryOfGases",
            "Gas pressure and temperature arise from the motion and collisions of large numbers of molecules.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.GasBehaviorArisesFromMolecularMotion },
            new List<Proposition>(),
            0.95,
            EpistemicStatus.Theory);

        /// <summary>
        /// Every theory declared by this catalog, in declaration order.
        /// </summary>
        public static readonly IReadOnlyList<FoundationalClaim> All = new List<FoundationalClaim>
        {
            MassEnergyEquivalence,
            InvariantLightSpeed,
            SpecialRelativity,
            GeneralRelativity,
            QuantumMechanics,
            AtomicTheory,
            BigBangCosmology,
            EvolutionByNaturalSelection,
            GermTheoryOfDisease,
            CellTheory,
            PlateTectonics,
            KineticTheoryOfGases
        };
    }
}
