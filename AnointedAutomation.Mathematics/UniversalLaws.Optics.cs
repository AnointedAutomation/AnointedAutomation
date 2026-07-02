// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The optics and radiation laws of the bedrock catalog.
    /// </summary>
    public static partial class UniversalLaws
    {
        /// <summary>
        /// Snell's Law: the ratio of the sines of the angles of incidence and refraction equals
        /// the ratio of the refractive indices of the two media.
        /// </summary>
        public static readonly FoundationalClaim SnellsLaw = new FoundationalClaim(
            "SnellsLaw",
            "The ratio of the sines of the angles of incidence and refraction equals the ratio of the refractive indices of the two media.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.RefractionRatioEqualsIndexRatio },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The Law of Reflection: the angle of incidence equals the angle of reflection.
        /// </summary>
        public static readonly FoundationalClaim LawOfReflection = new FoundationalClaim(
            "LawOfReflection",
            "The angle of incidence equals the angle of reflection.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.AngleOfIncidenceEqualsAngleOfReflection },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The Inverse-Square Law of radiation intensity: for an idealized point source with no
        /// absorption, the intensity of radiation is inversely proportional to the square of the
        /// distance from the source.
        /// </summary>
        public static readonly FoundationalClaim InverseSquareLawOfRadiation = new FoundationalClaim(
            "InverseSquareLawOfRadiation",
            "For an idealized point source with no absorption, the intensity of radiation is inversely proportional to the square of the distance from the source.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.RadiationIntensityInverseSquareToDistance },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Malus's Law: for an idealized polarizer, the intensity of polarized light passing
        /// through varies as the cosine squared of the angle between the light's polarization and
        /// the polarizer's axis.
        /// </summary>
        public static readonly FoundationalClaim MalussLaw = new FoundationalClaim(
            "MalussLaw",
            "For an idealized polarizer, the intensity of polarized light passing through varies as the cosine squared of the angle between the light's polarization and the polarizer's axis.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.PolarizedLightIntensityFollowsCosineSquared },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// The Stefan-Boltzmann Law: for an idealized perfect black body, the total energy
        /// radiated is proportional to the fourth power of its absolute temperature.
        /// </summary>
        public static readonly FoundationalClaim StefanBoltzmannLaw = new FoundationalClaim(
            "StefanBoltzmannLaw",
            "For an idealized perfect black body, the total energy radiated is proportional to the fourth power of its absolute temperature.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.BlackBodyRadiationProportionalToFourthPowerOfTemperature },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Wien's Displacement Law: for an idealized perfect black body, the peak wavelength of
        /// its radiation is inversely proportional to its absolute temperature.
        /// </summary>
        public static readonly FoundationalClaim WiensDisplacementLaw = new FoundationalClaim(
            "WiensDisplacementLaw",
            "For an idealized perfect black body, the peak wavelength of its radiation is inversely proportional to its absolute temperature.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.BlackBodyPeakWavelengthInverseToTemperature },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Planck's Law of Black-Body Radiation: for an idealized perfect black body, the spectral
        /// density of its radiation at a given temperature is described by this law.
        /// </summary>
        public static readonly FoundationalClaim PlancksLawOfBlackBodyRadiation = new FoundationalClaim(
            "PlancksLawOfBlackBodyRadiation",
            "For an idealized perfect black body, the spectral density of its radiation at a given temperature is described by Planck's law of black body radiation.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.BlackBodyRadiationSpectrumDescribedByPlancksLaw },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// The Beer-Lambert Law: for a dilute, non-scattering medium under monochromatic light,
        /// absorbance is proportional to concentration and path length.
        /// </summary>
        public static readonly FoundationalClaim BeerLambertLaw = new FoundationalClaim(
            "BeerLambertLaw",
            "For a dilute, non-scattering medium under monochromatic light, absorbance is proportional to concentration and path length.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.LightAbsorbanceProportionalToConcentrationAndPathLength },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);
    }
}
