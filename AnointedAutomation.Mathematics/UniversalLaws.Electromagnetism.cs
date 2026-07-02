// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The electromagnetism laws of the bedrock catalog.
    /// </summary>
    public static partial class UniversalLaws
    {
        /// <summary>
        /// Coulomb's Law: for idealized point charges in a vacuum, the electrostatic force between
        /// two charges is proportional to the product of the charges over the square of the
        /// distance between them.
        /// </summary>
        public static readonly FoundationalClaim CoulombsLaw = new FoundationalClaim(
            "CoulombsLaw",
            "For idealized point charges in a vacuum, the electrostatic force between two charges is proportional to the product of the charges over the square of the distance between them.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ElectrostaticForceInverseSquareToDistance },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Gauss's Law for electric fields: the electric flux through a closed surface is
        /// proportional to the charge enclosed by that surface.
        /// </summary>
        public static readonly FoundationalClaim GausssLawElectric = new FoundationalClaim(
            "GausssLawElectric",
            "The electric flux through a closed surface is proportional to the charge enclosed by that surface.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ElectricFluxProportionalToEnclosedCharge },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Gauss's Law for Magnetism: the net magnetic flux through any closed surface is zero,
        /// since magnetic monopoles have not been observed.
        /// </summary>
        public static readonly FoundationalClaim GausssLawMagnetism = new FoundationalClaim(
            "GausssLawMagnetism",
            "The net magnetic flux through any closed surface is zero, since magnetic monopoles have not been observed.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MagneticFluxThroughClosedSurfaceIsZero },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Faraday's Law of Induction: a changing magnetic flux through a circuit induces an
        /// electromotive force in that circuit.
        /// </summary>
        public static readonly FoundationalClaim FaradaysLawOfInduction = new FoundationalClaim(
            "FaradaysLawOfInduction",
            "A changing magnetic flux through a circuit induces an electromotive force in that circuit.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ChangingMagneticFluxInducesEmf },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Ampere's Circuital Law, with Maxwell's correction: the circulating magnetic field
        /// around a closed loop relates to the electric current and the changing electric flux
        /// through the loop.
        /// </summary>
        public static readonly FoundationalClaim AmperesCircuitalLaw = new FoundationalClaim(
            "AmperesCircuitalLaw",
            "The circulating magnetic field around a closed loop relates to the electric current and the changing electric flux through the loop.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.CirculatingMagneticFieldRelatesToCurrentAndFlux },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Lenz's Law: an induced current flows in the direction that opposes the change in
        /// magnetic flux that produced it.
        /// </summary>
        public static readonly FoundationalClaim LenzsLaw = new FoundationalClaim(
            "LenzsLaw",
            "An induced current flows in the direction that opposes the change in magnetic flux that produced it.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.InducedCurrentOpposesFluxChange },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The Biot-Savart Law: for an idealized steady current, the magnetic field it generates
        /// is calculable from the geometry of the current.
        /// </summary>
        public static readonly FoundationalClaim BiotSavartLaw = new FoundationalClaim(
            "BiotSavartLaw",
            "For an idealized steady current, the magnetic field it generates is calculable from the geometry of the current.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MagneticFieldCalculableFromSteadyCurrentGeometry },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Ohm's Law: for an idealized ohmic material, the current through a conductor is
        /// proportional to the voltage across it.
        /// </summary>
        public static readonly FoundationalClaim OhmsLaw = new FoundationalClaim(
            "OhmsLaw",
            "For an idealized ohmic material, the current through a conductor is proportional to the voltage across it.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.CurrentProportionalToVoltage },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Kirchhoff's Current Law: under the lumped circuit approximation, the sum of currents
        /// entering a node equals the sum leaving it.
        /// </summary>
        public static readonly FoundationalClaim KirchhoffsCurrentLaw = new FoundationalClaim(
            "KirchhoffsCurrentLaw",
            "Under the lumped circuit approximation, the sum of currents entering a node equals the sum leaving it.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.CurrentIntoNodeEqualsCurrentOut },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Kirchhoff's Voltage Law: under the lumped circuit approximation, the sum of voltages
        /// around a closed loop is zero.
        /// </summary>
        public static readonly FoundationalClaim KirchhoffsVoltageLaw = new FoundationalClaim(
            "KirchhoffsVoltageLaw",
            "Under the lumped circuit approximation, the sum of voltages around a closed loop is zero.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.VoltageAroundLoopSumsToZero },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Kirchhoff's Law of Thermal Radiation: at thermal equilibrium, the emissivity of a body
        /// equals its absorptivity at a given wavelength.
        /// </summary>
        public static readonly FoundationalClaim KirchhoffsLawOfThermalRadiation = new FoundationalClaim(
            "KirchhoffsLawOfThermalRadiation",
            "At thermal equilibrium, the emissivity of a body equals its absorptivity at a given wavelength.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.EmissivityEqualsAbsorptivityAtEquilibrium },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Joule's First Law: for a constant resistance, the heat produced by a current is
        /// proportional to the square of the current, the resistance, and the time.
        /// </summary>
        public static readonly FoundationalClaim JoulesFirstLaw = new FoundationalClaim(
            "JoulesFirstLaw",
            "For a constant resistance, the heat produced by a current is proportional to the square of the current, the resistance, and the time.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.HeatProportionalToCurrentSquared },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Joule's Second Law: for an ideal gas, internal energy is independent of volume at
        /// constant temperature.
        /// </summary>
        public static readonly FoundationalClaim JoulesSecondLaw = new FoundationalClaim(
            "JoulesSecondLaw",
            "For an ideal gas, internal energy is independent of volume at constant temperature.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.InternalEnergyIndependentOfVolumeForIdealGas },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Curie's Law: for a dilute, non-interacting paramagnetic system, magnetic
        /// susceptibility is inversely proportional to absolute temperature.
        /// </summary>
        public static readonly FoundationalClaim CuriesLaw = new FoundationalClaim(
            "CuriesLaw",
            "For a dilute, non-interacting paramagnetic system, magnetic susceptibility is inversely proportional to absolute temperature.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MagneticSusceptibilityInverseToTemperature },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);
    }
}
