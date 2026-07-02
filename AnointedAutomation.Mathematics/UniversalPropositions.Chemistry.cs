// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The gas law and chemistry propositions behind the classical chemical laws in
    /// <see cref="UniversalLaws"/>.
    /// </summary>
    public static partial class UniversalPropositions
    {
        // Gas laws

        /// <summary>
        /// For an ideal gas at constant temperature, pressure is inversely proportional to volume.
        /// </summary>
        public static readonly Proposition GasPressureInverseToVolume = new Proposition(
            "GasPressureInverseToVolume",
            "For an ideal gas at constant temperature, pressure is inversely proportional to volume.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an ideal gas at constant pressure, volume is proportional to absolute temperature.
        /// </summary>
        public static readonly Proposition GasVolumeProportionalToTemperature = new Proposition(
            "GasVolumeProportionalToTemperature",
            "For an ideal gas at constant pressure, volume is proportional to absolute temperature.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an ideal gas at constant volume, pressure is proportional to absolute temperature.
        /// </summary>
        public static readonly Proposition GasPressureProportionalToTemperature = new Proposition(
            "GasPressureProportionalToTemperature",
            "For an ideal gas at constant volume, pressure is proportional to absolute temperature.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For ideal gases, equal volumes at the same temperature and pressure contain equal
        /// numbers of molecules.
        /// </summary>
        public static readonly Proposition EqualGasVolumesContainEqualMolecules = new Proposition(
            "EqualGasVolumesContainEqualMolecules",
            "For ideal gases, equal volumes at the same temperature and pressure contain equal numbers of molecules.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an ideal gas, pressure times volume equals the number of moles times the gas
        /// constant times absolute temperature.
        /// </summary>
        public static readonly Proposition IdealGasLawHolds = new Proposition(
            "IdealGasLawHolds",
            "For an ideal gas, pressure times volume equals the number of moles times the gas constant times absolute temperature.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For a mixture of ideal gases with no interaction, the total pressure equals the sum of
        /// the partial pressures of its components.
        /// </summary>
        public static readonly Proposition TotalPressureEqualsSumOfPartialPressures = new Proposition(
            "TotalPressureEqualsSumOfPartialPressures",
            "For a mixture of ideal gases with no interaction, the total pressure equals the sum of the partial pressures of its components.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an ideal gas, the rate of effusion is inversely proportional to the square root of
        /// its molar mass.
        /// </summary>
        public static readonly Proposition EffusionRateInverseToSquareRootOfMolarMass = new Proposition(
            "EffusionRateInverseToSquareRootOfMolarMass",
            "For an ideal gas, the rate of effusion is inversely proportional to the square root of its molar mass.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For a dilute solution, the amount of dissolved gas is proportional to its partial
        /// pressure above the liquid.
        /// </summary>
        public static readonly Proposition DissolvedGasProportionalToPartialPressure = new Proposition(
            "DissolvedGasProportionalToPartialPressure",
            "For a dilute solution, the amount of dissolved gas is proportional to its partial pressure above the liquid.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an ideal solution, the vapor pressure of a solution is proportional to the mole
        /// fraction of solvent.
        /// </summary>
        public static readonly Proposition SolutionVaporPressureProportionalToMoleFraction = new Proposition(
            "SolutionVaporPressureProportionalToMoleFraction",
            "For an ideal solution, the vapor pressure of a solution is proportional to the mole fraction of solvent.",
            Testability.EmpiricallyTestable,
            true);

        // Chemistry

        /// <summary>
        /// A pure compound always contains the same elements in the same fixed mass ratio.
        /// </summary>
        public static readonly Proposition CompoundElementRatiosAreFixed = new Proposition(
            "CompoundElementRatiosAreFixed",
            "A pure compound always contains the same elements in the same fixed mass ratio.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// When two elements form multiple compounds, the mass ratios of the second element that
        /// combine with a fixed mass of the first are small whole numbers.
        /// </summary>
        public static readonly Proposition MultipleCompoundMassRatiosAreSmallWholeNumbers = new Proposition(
            "MultipleCompoundMassRatiosAreSmallWholeNumbers",
            "When two elements form multiple compounds, the mass ratios of the second element that combine with a fixed mass of the first are small whole numbers.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The mass ratios in which elements combine with a fixed mass of a third element are
        /// simply related to the mass ratios in which those elements combine with each other.
        /// </summary>
        public static readonly Proposition ReciprocalElementMassRatiosAreSimplyRelated = new Proposition(
            "ReciprocalElementMassRatiosAreSimplyRelated",
            "The mass ratios in which elements combine with a fixed mass of a third element are simply related to the mass ratios in which those elements combine with each other.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The total enthalpy change of a chemical reaction is independent of the pathway taken
        /// between reactants and products.
        /// </summary>
        public static readonly Proposition ReactionEnthalpyIndependentOfPathway = new Proposition(
            "ReactionEnthalpyIndependentOfPathway",
            "The total enthalpy change of a chemical reaction is independent of the pathway taken between reactants and products.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The mass of a substance altered at an electrode during electrolysis is proportional to
        /// the charge passed through it.
        /// </summary>
        public static readonly Proposition ElectrolysisMassProportionalToCharge = new Proposition(
            "ElectrolysisMassProportionalToCharge",
            "The mass of a substance altered at an electrode during electrolysis is proportional to the charge passed through it.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For idealized elementary reactions in dilute conditions, the rate of a chemical
        /// reaction is proportional to the product of the concentrations of its reactants.
        /// </summary>
        public static readonly Proposition ReactionRateProportionalToConcentrationProduct = new Proposition(
            "ReactionRateProportionalToConcentrationProduct",
            "For idealized elementary reactions in dilute conditions, the rate of a chemical reaction is proportional to the product of the concentrations of its reactants.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized dilute solution at steady state, diffusive flux is proportional to the
        /// negative of the concentration gradient.
        /// </summary>
        public static readonly Proposition DiffusiveFluxProportionalToConcentrationGradient = new Proposition(
            "DiffusiveFluxProportionalToConcentrationGradient",
            "For an idealized dilute solution at steady state, diffusive flux is proportional to the negative of the concentration gradient.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized medium with constant diffusivity, the rate of change of concentration
        /// equals the diffusivity times the second spatial derivative of concentration.
        /// </summary>
        public static readonly Proposition ConcentrationChangeRateProportionalToSecondDerivative = new Proposition(
            "ConcentrationChangeRateProportionalToSecondDerivative",
            "For an idealized medium with constant diffusivity, the rate of change of concentration equals the diffusivity times the second spatial derivative of concentration.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized isotropic, homogeneous medium, heat flux is proportional to the
        /// negative of the temperature gradient.
        /// </summary>
        public static readonly Proposition HeatFluxProportionalToTemperatureGradient = new Proposition(
            "HeatFluxProportionalToTemperatureGradient",
            "For an idealized isotropic, homogeneous medium, heat flux is proportional to the negative of the temperature gradient.",
            Testability.EmpiricallyTestable,
            true);
    }
}
