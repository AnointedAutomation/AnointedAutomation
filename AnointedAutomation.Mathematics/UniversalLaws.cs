// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The bedrock catalog: foundational claims with status <see cref="EpistemicStatus.Law"/> that
    /// have survived so much falsification they function as the measuring stick for an
    /// <see cref="EpistemicLedger"/>. Includes the laws of logic, which apply without restriction,
    /// and the intra-universe physical laws of causality, conservation of energy, and entropy.
    /// </summary>
    public static partial class UniversalLaws
    {
        /// <summary>
        /// The law of non-contradiction: a statement and its negation cannot both be true.
        /// </summary>
        public static readonly FoundationalClaim NonContradiction = new FoundationalClaim(
            "NonContradiction",
            "A statement and its negation cannot both be true at the same time and in the same sense.",
            LawDomain.Unrestricted,
            new List<Proposition> { UniversalPropositions.NonContradiction },
            new List<Proposition>(),
            1.0,
            EpistemicStatus.Law);

        /// <summary>
        /// The law of identity: a thing is identical to itself.
        /// </summary>
        public static readonly FoundationalClaim Identity = new FoundationalClaim(
            "Identity",
            "A thing is identical to itself.",
            LawDomain.Unrestricted,
            new List<Proposition> { UniversalPropositions.Identity },
            new List<Proposition>(),
            1.0,
            EpistemicStatus.Law);

        /// <summary>
        /// The law of excluded middle: for any proposition, either it or its negation holds.
        /// </summary>
        public static readonly FoundationalClaim ExcludedMiddle = new FoundationalClaim(
            "ExcludedMiddle",
            "For any proposition, either it or its negation holds.",
            LawDomain.Unrestricted,
            new List<Proposition> { UniversalPropositions.ExcludedMiddle },
            new List<Proposition>(),
            0.98,
            EpistemicStatus.Law);

        /// <summary>
        /// The law of causality: every effect has a cause, within the universe.
        /// </summary>
        public static readonly FoundationalClaim Causality = new FoundationalClaim(
            "Causality",
            "Every effect has a cause.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.EffectsHaveCauses },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The law of conservation of energy: within a closed system, energy is neither created
        /// nor destroyed.
        /// </summary>
        public static readonly FoundationalClaim ConservationOfEnergy = new FoundationalClaim(
            "ConservationOfEnergy",
            "Within a closed system, energy is neither created nor destroyed.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.EnergyConserved },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The second law of thermodynamics: the entropy of an isolated system never decreases.
        /// </summary>
        public static readonly FoundationalClaim EntropyIncrease = new FoundationalClaim(
            "EntropyIncrease",
            "The entropy of an isolated system never decreases over time.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.EntropyIncreases },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Every law declared by this catalog, in declaration order. Built in an explicit static
        /// constructor, rather than a field initializer, so it runs after every field initializer
        /// across all partial declarations of this class, regardless of file order.
        /// </summary>
        public static readonly IReadOnlyList<FoundationalClaim> All;

        static UniversalLaws()
        {
            All = new List<FoundationalClaim>
        {
            // Logic
            NonContradiction,
            Identity,
            ExcludedMiddle,

            // Mechanics
            Causality,
            NewtonsFirstLaw,
            NewtonsSecondLaw,
            NewtonsThirdLaw,
            HookesLaw,
            StokesLaw,

            // Gravitation and astronomy
            NewtonsLawOfUniversalGravitation,
            KeplersFirstLaw,
            KeplersSecondLaw,
            KeplersThirdLaw,

            // Conservation
            ConservationOfEnergy,
            ConservationOfMomentum,
            ConservationOfAngularMomentum,
            ConservationOfMass,

            // Thermodynamics
            EntropyIncrease,
            ZerothLawOfThermodynamics,
            ThirdLawOfThermodynamics,

            // Electromagnetism
            CoulombsLaw,
            GausssLawElectric,
            GausssLawMagnetism,
            FaradaysLawOfInduction,
            AmperesCircuitalLaw,
            LenzsLaw,
            BiotSavartLaw,
            OhmsLaw,
            KirchhoffsCurrentLaw,
            KirchhoffsVoltageLaw,
            KirchhoffsLawOfThermalRadiation,
            JoulesFirstLaw,
            JoulesSecondLaw,
            CuriesLaw,

            // Optics and radiation
            SnellsLaw,
            LawOfReflection,
            InverseSquareLawOfRadiation,
            MalussLaw,
            StefanBoltzmannLaw,
            WiensDisplacementLaw,
            PlancksLawOfBlackBodyRadiation,
            BeerLambertLaw,

            // Fluids
            PascalsLaw,
            ArchimedesPrinciple,
            BernoullisPrinciple,
            TorricellisLaw,

            // Gas laws
            BoylesLaw,
            CharlessLaw,
            GayLussacsLaw,
            AvogadrosLaw,
            IdealGasLaw,
            DaltonsLawOfPartialPressures,
            GrahamsLawOfEffusion,
            HenrysLaw,
            RaoultsLaw,

            // Chemistry
            LawOfDefiniteProportions,
            LawOfMultipleProportions,
            LawOfReciprocalProportions,
            HesssLaw,
            FaradaysLawsOfElectrolysis,
            LawOfMassAction,
            FicksFirstLawOfDiffusion,
            FicksSecondLawOfDiffusion,
            FouriersLawOfHeatConduction
        };
        }
    }
}
