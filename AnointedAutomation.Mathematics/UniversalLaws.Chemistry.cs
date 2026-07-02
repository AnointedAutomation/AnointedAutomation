// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The gas law and chemistry laws of the bedrock catalog.
    /// </summary>
    public static partial class UniversalLaws
    {
        // Gas laws

        /// <summary>
        /// Boyle's Law: for an ideal gas at constant temperature, pressure is inversely
        /// proportional to volume.
        /// </summary>
        public static readonly FoundationalClaim BoylesLaw = new FoundationalClaim(
            "BoylesLaw",
            "For an ideal gas at constant temperature, pressure is inversely proportional to volume.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.GasPressureInverseToVolume },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Charles's Law: for an ideal gas at constant pressure, volume is proportional to
        /// absolute temperature.
        /// </summary>
        public static readonly FoundationalClaim CharlessLaw = new FoundationalClaim(
            "CharlessLaw",
            "For an ideal gas at constant pressure, volume is proportional to absolute temperature.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.GasVolumeProportionalToTemperature },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Gay-Lussac's Law: for an ideal gas at constant volume, pressure is proportional to
        /// absolute temperature.
        /// </summary>
        public static readonly FoundationalClaim GayLussacsLaw = new FoundationalClaim(
            "GayLussacsLaw",
            "For an ideal gas at constant volume, pressure is proportional to absolute temperature.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.GasPressureProportionalToTemperature },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Avogadro's Law: for ideal gases, equal volumes at the same temperature and pressure
        /// contain equal numbers of molecules.
        /// </summary>
        public static readonly FoundationalClaim AvogadrosLaw = new FoundationalClaim(
            "AvogadrosLaw",
            "For ideal gases, equal volumes at the same temperature and pressure contain equal numbers of molecules.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.EqualGasVolumesContainEqualMolecules },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// The Ideal Gas Law: for an ideal gas, pressure times volume equals the number of moles
        /// times the gas constant times absolute temperature.
        /// </summary>
        public static readonly FoundationalClaim IdealGasLaw = new FoundationalClaim(
            "IdealGasLaw",
            "For an ideal gas, pressure times volume equals the number of moles times the gas constant times absolute temperature.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.IdealGasLawHolds },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Dalton's Law of Partial Pressures: for a mixture of ideal gases with no interaction,
        /// the total pressure equals the sum of the partial pressures of its components.
        /// </summary>
        public static readonly FoundationalClaim DaltonsLawOfPartialPressures = new FoundationalClaim(
            "DaltonsLawOfPartialPressures",
            "For a mixture of ideal gases with no interaction, the total pressure equals the sum of the partial pressures of its components.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.TotalPressureEqualsSumOfPartialPressures },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Graham's Law of Effusion: for an ideal gas, the rate of effusion is inversely
        /// proportional to the square root of its molar mass.
        /// </summary>
        public static readonly FoundationalClaim GrahamsLawOfEffusion = new FoundationalClaim(
            "GrahamsLawOfEffusion",
            "For an ideal gas, the rate of effusion is inversely proportional to the square root of its molar mass.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.EffusionRateInverseToSquareRootOfMolarMass },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Henry's Law: for a dilute solution, the amount of dissolved gas is proportional to its
        /// partial pressure above the liquid.
        /// </summary>
        public static readonly FoundationalClaim HenrysLaw = new FoundationalClaim(
            "HenrysLaw",
            "For a dilute solution, the amount of dissolved gas is proportional to its partial pressure above the liquid.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.DissolvedGasProportionalToPartialPressure },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Raoult's Law: for an ideal solution, the vapor pressure of a solution is proportional
        /// to the mole fraction of solvent.
        /// </summary>
        public static readonly FoundationalClaim RaoultsLaw = new FoundationalClaim(
            "RaoultsLaw",
            "For an ideal solution, the vapor pressure of a solution is proportional to the mole fraction of solvent.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.SolutionVaporPressureProportionalToMoleFraction },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        // Chemistry

        /// <summary>
        /// The Law of Definite Proportions: a pure compound always contains the same elements in
        /// the same fixed mass ratio.
        /// </summary>
        public static readonly FoundationalClaim LawOfDefiniteProportions = new FoundationalClaim(
            "LawOfDefiniteProportions",
            "A pure compound always contains the same elements in the same fixed mass ratio.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.CompoundElementRatiosAreFixed },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The Law of Multiple Proportions: when two elements form multiple compounds, the mass
        /// ratios of the second element that combine with a fixed mass of the first are small
        /// whole numbers.
        /// </summary>
        public static readonly FoundationalClaim LawOfMultipleProportions = new FoundationalClaim(
            "LawOfMultipleProportions",
            "When two elements form multiple compounds, the mass ratios of the second element that combine with a fixed mass of the first are small whole numbers.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MultipleCompoundMassRatiosAreSmallWholeNumbers },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The Law of Reciprocal Proportions: the mass ratios in which elements combine with a
        /// fixed mass of a third element are simply related to the mass ratios in which those
        /// elements combine with each other.
        /// </summary>
        public static readonly FoundationalClaim LawOfReciprocalProportions = new FoundationalClaim(
            "LawOfReciprocalProportions",
            "The mass ratios in which elements combine with a fixed mass of a third element are simply related to the mass ratios in which those elements combine with each other.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ReciprocalElementMassRatiosAreSimplyRelated },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Hess's Law: the total enthalpy change of a chemical reaction is independent of the
        /// pathway taken between reactants and products.
        /// </summary>
        public static readonly FoundationalClaim HesssLaw = new FoundationalClaim(
            "HesssLaw",
            "The total enthalpy change of a chemical reaction is independent of the pathway taken between reactants and products.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ReactionEnthalpyIndependentOfPathway },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Faraday's Laws of Electrolysis: the mass of a substance altered at an electrode during
        /// electrolysis is proportional to the charge passed through it.
        /// </summary>
        public static readonly FoundationalClaim FaradaysLawsOfElectrolysis = new FoundationalClaim(
            "FaradaysLawsOfElectrolysis",
            "The mass of a substance altered at an electrode during electrolysis is proportional to the charge passed through it.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ElectrolysisMassProportionalToCharge },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The Law of Mass Action: for idealized elementary reactions in dilute conditions, the
        /// rate of a chemical reaction is proportional to the product of the concentrations of its
        /// reactants.
        /// </summary>
        public static readonly FoundationalClaim LawOfMassAction = new FoundationalClaim(
            "LawOfMassAction",
            "For idealized elementary reactions in dilute conditions, the rate of a chemical reaction is proportional to the product of the concentrations of its reactants.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ReactionRateProportionalToConcentrationProduct },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Fick's First Law of Diffusion: for an idealized dilute solution at steady state,
        /// diffusive flux is proportional to the negative of the concentration gradient.
        /// </summary>
        public static readonly FoundationalClaim FicksFirstLawOfDiffusion = new FoundationalClaim(
            "FicksFirstLawOfDiffusion",
            "For an idealized dilute solution at steady state, diffusive flux is proportional to the negative of the concentration gradient.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.DiffusiveFluxProportionalToConcentrationGradient },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Fick's Second Law of Diffusion: for an idealized medium with constant diffusivity, the
        /// rate of change of concentration equals the diffusivity times the second spatial
        /// derivative of concentration.
        /// </summary>
        public static readonly FoundationalClaim FicksSecondLawOfDiffusion = new FoundationalClaim(
            "FicksSecondLawOfDiffusion",
            "For an idealized medium with constant diffusivity, the rate of change of concentration equals the diffusivity times the second spatial derivative of concentration.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ConcentrationChangeRateProportionalToSecondDerivative },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Fourier's Law of Heat Conduction: for an idealized isotropic, homogeneous medium, heat
        /// flux is proportional to the negative of the temperature gradient.
        /// </summary>
        public static readonly FoundationalClaim FouriersLawOfHeatConduction = new FoundationalClaim(
            "FouriersLawOfHeatConduction",
            "For an idealized isotropic, homogeneous medium, heat flux is proportional to the negative of the temperature gradient.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.HeatFluxProportionalToTemperatureGradient },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);
    }
}
