// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The shared vocabulary of propositions the mathematics and physics catalogs assert and deny.
    /// Everything here can be tested from inside the universe: logic and intra-universe physics are
    /// empirically testable, never beyond observation. Standing is true for the propositions behind
    /// bedrock laws and well supported theories, and honestly null for the propositions behind
    /// unproven conjectures.
    /// </summary>
    public static partial class UniversalPropositions
    {
        // Logic

        /// <summary>
        /// A statement and its negation cannot both be true at the same time and in the same sense.
        /// </summary>
        public static readonly Proposition NonContradiction = new Proposition(
            "NonContradiction",
            "A statement and its negation cannot both be true at the same time and in the same sense.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// A thing is identical to itself.
        /// </summary>
        public static readonly Proposition Identity = new Proposition(
            "Identity",
            "A thing is identical to itself.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For any proposition, either it or its negation holds.
        /// </summary>
        public static readonly Proposition ExcludedMiddle = new Proposition(
            "ExcludedMiddle",
            "For any proposition, either it or its negation holds.",
            Testability.EmpiricallyTestable,
            true);

        // Mechanics (causality)

        /// <summary>
        /// Every effect has a cause.
        /// </summary>
        public static readonly Proposition EffectsHaveCauses = new Proposition(
            "EffectsHaveCauses",
            "Every effect has a cause.",
            Testability.EmpiricallyTestable,
            true);

        // Conservation

        /// <summary>
        /// Within a closed system, energy is neither created nor destroyed.
        /// </summary>
        public static readonly Proposition EnergyConserved = new Proposition(
            "EnergyConserved",
            "Within a closed system, energy is neither created nor destroyed.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The entropy of an isolated system never decreases over time.
        /// </summary>
        public static readonly Proposition EntropyIncreases = new Proposition(
            "EntropyIncreases",
            "The entropy of an isolated system never decreases over time.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Mass and energy are equivalent and convertible according to E equals m times c squared.
        /// </summary>
        public static readonly Proposition MassEnergyEquivalent = new Proposition(
            "MassEnergyEquivalent",
            "Mass and energy are equivalent and convertible according to E equals m times c squared.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The speed of light in a vacuum is the same for every observer, regardless of motion.
        /// </summary>
        public static readonly Proposition SpeedOfLightConstant = new Proposition(
            "SpeedOfLightConstant",
            "The speed of light in a vacuum is the same for every observer, regardless of motion.",
            Testability.EmpiricallyTestable,
            true);

        // Major theories

        /// <summary>
        /// The laws of physics are the same in all inertial frames and the speed of light is
        /// invariant, so space and time measurements depend on relative motion.
        /// </summary>
        public static readonly Proposition SpecialRelativityHolds = new Proposition(
            "SpecialRelativityHolds",
            "The laws of physics are the same in all inertial frames and the speed of light is invariant, so space and time measurements depend on relative motion.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Gravity is the curvature of spacetime by mass and energy.
        /// </summary>
        public static readonly Proposition GravityIsSpacetimeCurvature = new Proposition(
            "GravityIsSpacetimeCurvature",
            "Gravity is the curvature of spacetime by mass and energy.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Matter and energy at the smallest scales behave as quantized states with probabilistic
        /// outcomes.
        /// </summary>
        public static readonly Proposition MatterEnergyIsQuantized = new Proposition(
            "MatterEnergyIsQuantized",
            "Matter and energy at the smallest scales behave as quantized states with probabilistic outcomes.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// All matter is composed of atoms.
        /// </summary>
        public static readonly Proposition MatterComposedOfAtoms = new Proposition(
            "MatterComposedOfAtoms",
            "All matter is composed of atoms.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The observable universe expanded from an extremely hot, dense early state roughly
        /// 13.8 billion years ago.
        /// </summary>
        public static readonly Proposition UniverseExpandedFromHotDenseState = new Proposition(
            "UniverseExpandedFromHotDenseState",
            "The observable universe expanded from an extremely hot, dense early state roughly 13.8 billion years ago.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Populations of organisms change across generations through heritable variation and
        /// differential reproductive success.
        /// </summary>
        public static readonly Proposition PopulationsEvolveByNaturalSelection = new Proposition(
            "PopulationsEvolveByNaturalSelection",
            "Populations of organisms change across generations through heritable variation and differential reproductive success.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Many diseases are caused by microorganisms.
        /// </summary>
        public static readonly Proposition DiseasesCausedByMicroorganisms = new Proposition(
            "DiseasesCausedByMicroorganisms",
            "Many diseases are caused by microorganisms.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// All living organisms are composed of cells, and all cells arise from pre-existing
        /// cells.
        /// </summary>
        public static readonly Proposition OrganismsComposedOfCells = new Proposition(
            "OrganismsComposedOfCells",
            "All living organisms are composed of cells, and all cells arise from pre-existing cells.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Earth's lithosphere is divided into plates whose motion produces continental drift,
        /// earthquakes, and mountain building.
        /// </summary>
        public static readonly Proposition LithosphereDividedIntoMovingPlates = new Proposition(
            "LithosphereDividedIntoMovingPlates",
            "Earth's lithosphere is divided into plates whose motion produces continental drift, earthquakes, and mountain building.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Gas pressure and temperature arise from the motion and collisions of large numbers of
        /// molecules.
        /// </summary>
        public static readonly Proposition GasBehaviorArisesFromMolecularMotion = new Proposition(
            "GasBehaviorArisesFromMolecularMotion",
            "Gas pressure and temperature arise from the motion and collisions of large numbers of molecules.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Every Collatz sequence, starting from any positive integer, eventually reaches one.
        /// </summary>
        public static readonly Proposition CollatzTerminates = new Proposition(
            "CollatzTerminates",
            "Every Collatz sequence, starting from any positive integer, eventually reaches one.",
            Testability.EmpiricallyTestable);

        /// <summary>
        /// Every even integer greater than two is the sum of two primes.
        /// </summary>
        public static readonly Proposition GoldbachHolds = new Proposition(
            "GoldbachHolds",
            "Every even integer greater than two is the sum of two primes.",
            Testability.EmpiricallyTestable);

        /// <summary>
        /// Every nontrivial zero of the Riemann zeta function has real part one half.
        /// </summary>
        public static readonly Proposition RiemannZerosOnCriticalLine = new Proposition(
            "RiemannZerosOnCriticalLine",
            "Every nontrivial zero of the Riemann zeta function has real part one half.",
            Testability.EmpiricallyTestable);

        /// <summary>
        /// Every proposition declared by this catalog, in declaration order. Built in an explicit
        /// static constructor, rather than a field initializer, so it runs after every field
        /// initializer across all partial declarations of this class, regardless of file order.
        /// </summary>
        public static readonly System.Collections.Generic.IReadOnlyList<Proposition> All;

        static UniversalPropositions()
        {
            All = new System.Collections.Generic.List<Proposition>
            {
                // Logic
                NonContradiction,
                Identity,
                ExcludedMiddle,

                // Mechanics
                EffectsHaveCauses,
                NewtonsFirstLaw,
                NewtonsSecondLaw,
                ActionHasEqualOppositeReaction,
                SpringForceProportionalToDisplacement,
                ViscousDragProportionalToVelocity,

                // Gravitation and astronomy
                MassesAttractInverseSquareToDistance,
                PlanetaryOrbitsAreElliptical,
                OrbitsSweepEqualAreas,
                OrbitalPeriodSquaredProportionalToAxisCubed,

                // Conservation
                EnergyConserved,
                MomentumConserved,
                AngularMomentumConserved,
                MassConserved,

                // Thermodynamics
                EntropyIncreases,
                ThermalEquilibriumIsTransitive,
                EntropyVanishesAtAbsoluteZero,

                // Electromagnetism
                ElectrostaticForceInverseSquareToDistance,
                ElectricFluxProportionalToEnclosedCharge,
                MagneticFluxThroughClosedSurfaceIsZero,
                ChangingMagneticFluxInducesEmf,
                CirculatingMagneticFieldRelatesToCurrentAndFlux,
                InducedCurrentOpposesFluxChange,
                MagneticFieldCalculableFromSteadyCurrentGeometry,
                CurrentProportionalToVoltage,
                CurrentIntoNodeEqualsCurrentOut,
                VoltageAroundLoopSumsToZero,
                EmissivityEqualsAbsorptivityAtEquilibrium,
                HeatProportionalToCurrentSquared,
                InternalEnergyIndependentOfVolumeForIdealGas,
                MagneticSusceptibilityInverseToTemperature,

                // Optics and radiation
                RefractionRatioEqualsIndexRatio,
                AngleOfIncidenceEqualsAngleOfReflection,
                RadiationIntensityInverseSquareToDistance,
                PolarizedLightIntensityFollowsCosineSquared,
                BlackBodyRadiationProportionalToFourthPowerOfTemperature,
                BlackBodyPeakWavelengthInverseToTemperature,
                BlackBodyRadiationSpectrumDescribedByPlancksLaw,
                LightAbsorbanceProportionalToConcentrationAndPathLength,

                // Fluids
                PressureTransmittedUndiminishedInEnclosedFluid,
                BuoyantForceEqualsWeightOfDisplacedFluid,
                FluidSpeedIncreaseAccompaniesPressureDecrease,
                FluidEffluxSpeedRelatesToHeight,

                // Gas laws
                GasPressureInverseToVolume,
                GasVolumeProportionalToTemperature,
                GasPressureProportionalToTemperature,
                EqualGasVolumesContainEqualMolecules,
                IdealGasLawHolds,
                TotalPressureEqualsSumOfPartialPressures,
                EffusionRateInverseToSquareRootOfMolarMass,
                DissolvedGasProportionalToPartialPressure,
                SolutionVaporPressureProportionalToMoleFraction,

                // Chemistry
                CompoundElementRatiosAreFixed,
                MultipleCompoundMassRatiosAreSmallWholeNumbers,
                ReciprocalElementMassRatiosAreSimplyRelated,
                ReactionEnthalpyIndependentOfPathway,
                ElectrolysisMassProportionalToCharge,
                ReactionRateProportionalToConcentrationProduct,
                DiffusiveFluxProportionalToConcentrationGradient,
                ConcentrationChangeRateProportionalToSecondDerivative,
                HeatFluxProportionalToTemperatureGradient,

                // Relativity (theory-backed)
                MassEnergyEquivalent,
                SpeedOfLightConstant,

                // Major theories
                SpecialRelativityHolds,
                GravityIsSpacetimeCurvature,
                MatterEnergyIsQuantized,
                MatterComposedOfAtoms,
                UniverseExpandedFromHotDenseState,
                PopulationsEvolveByNaturalSelection,
                DiseasesCausedByMicroorganisms,
                OrganismsComposedOfCells,
                LithosphereDividedIntoMovingPlates,
                GasBehaviorArisesFromMolecularMotion,

                // Mathematics conjectures
                CollatzTerminates,
                GoldbachHolds,
                RiemannZerosOnCriticalLine
            };
        }
    }
}
