// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The mechanics, gravitation, conservation, thermodynamics, electromagnetism, optics, and
    /// fluids propositions behind the classical physical laws in <see cref="UniversalLaws"/>.
    /// </summary>
    public static partial class UniversalPropositions
    {
        // Mechanics

        /// <summary>
        /// Within an inertial frame, a body at rest or in uniform motion remains so unless acted
        /// on by a net force.
        /// </summary>
        public static readonly Proposition NewtonsFirstLaw = new Proposition(
            "NewtonsFirstLaw",
            "Within an inertial frame, a body at rest or in uniform motion remains so unless acted on by a net force.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For constant mass, net force equals the rate of change of momentum, reducing to force
        /// equals mass times acceleration.
        /// </summary>
        public static readonly Proposition NewtonsSecondLaw = new Proposition(
            "NewtonsSecondLaw",
            "For constant mass, net force equals the rate of change of momentum, reducing to force equals mass times acceleration.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For every action there is an equal and opposite reaction.
        /// </summary>
        public static readonly Proposition ActionHasEqualOppositeReaction = new Proposition(
            "ActionHasEqualOppositeReaction",
            "For every action there is an equal and opposite reaction.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Within the elastic limit, the restoring force of a spring is proportional to its
        /// displacement.
        /// </summary>
        public static readonly Proposition SpringForceProportionalToDisplacement = new Proposition(
            "SpringForceProportionalToDisplacement",
            "Within the elastic limit, the restoring force of a spring is proportional to its displacement.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For a small sphere at low Reynolds number, the drag force in a viscous fluid is
        /// proportional to its velocity.
        /// </summary>
        public static readonly Proposition ViscousDragProportionalToVelocity = new Proposition(
            "ViscousDragProportionalToVelocity",
            "For a small sphere at low Reynolds number, the drag force in a viscous fluid is proportional to its velocity.",
            Testability.EmpiricallyTestable,
            true);

        // Gravitation and astronomy

        /// <summary>
        /// Two masses attract with a force proportional to the product of the masses over the
        /// square of the distance between them, though general relativity supersedes this at high
        /// precision.
        /// </summary>
        public static readonly Proposition MassesAttractInverseSquareToDistance = new Proposition(
            "MassesAttractInverseSquareToDistance",
            "Two masses attract with a force proportional to the product of the masses over the square of the distance between them, though general relativity supersedes this at high precision.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Under the two body approximation, planets orbit the sun in ellipses with the sun at one
        /// focus.
        /// </summary>
        public static readonly Proposition PlanetaryOrbitsAreElliptical = new Proposition(
            "PlanetaryOrbitsAreElliptical",
            "Under the two body approximation, planets orbit the sun in ellipses with the sun at one focus.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Under the two body approximation, a line joining a planet and the sun sweeps out equal
        /// areas in equal times.
        /// </summary>
        public static readonly Proposition OrbitsSweepEqualAreas = new Proposition(
            "OrbitsSweepEqualAreas",
            "Under the two body approximation, a line joining a planet and the sun sweeps out equal areas in equal times.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Under the two body approximation, the square of a planet's orbital period is
        /// proportional to the cube of its orbit's semi-major axis.
        /// </summary>
        public static readonly Proposition OrbitalPeriodSquaredProportionalToAxisCubed = new Proposition(
            "OrbitalPeriodSquaredProportionalToAxisCubed",
            "Under the two body approximation, the square of a planet's orbital period is proportional to the cube of its orbit's semi-major axis.",
            Testability.EmpiricallyTestable,
            true);

        // Conservation

        /// <summary>
        /// The total momentum of an isolated system is constant.
        /// </summary>
        public static readonly Proposition MomentumConserved = new Proposition(
            "MomentumConserved",
            "The total momentum of an isolated system is constant.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The total angular momentum of an isolated system is constant.
        /// </summary>
        public static readonly Proposition AngularMomentumConserved = new Proposition(
            "AngularMomentumConserved",
            "The total angular momentum of an isolated system is constant.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Mass is neither created nor destroyed in a chemical reaction.
        /// </summary>
        public static readonly Proposition MassConserved = new Proposition(
            "MassConserved",
            "Mass is neither created nor destroyed in a chemical reaction.",
            Testability.EmpiricallyTestable,
            true);

        // Thermodynamics

        /// <summary>
        /// If two systems are each in thermal equilibrium with a third system, they are in thermal
        /// equilibrium with each other.
        /// </summary>
        public static readonly Proposition ThermalEquilibriumIsTransitive = new Proposition(
            "ThermalEquilibriumIsTransitive",
            "If two systems are each in thermal equilibrium with a third system, they are in thermal equilibrium with each other.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized perfect crystal, entropy approaches a constant minimum as temperature
        /// approaches absolute zero.
        /// </summary>
        public static readonly Proposition EntropyVanishesAtAbsoluteZero = new Proposition(
            "EntropyVanishesAtAbsoluteZero",
            "For an idealized perfect crystal, entropy approaches a constant minimum as temperature approaches absolute zero.",
            Testability.EmpiricallyTestable,
            true);

        // Electromagnetism

        /// <summary>
        /// For idealized point charges in a vacuum, the electrostatic force between two charges is
        /// proportional to the product of the charges over the square of the distance between them.
        /// </summary>
        public static readonly Proposition ElectrostaticForceInverseSquareToDistance = new Proposition(
            "ElectrostaticForceInverseSquareToDistance",
            "For idealized point charges in a vacuum, the electrostatic force between two charges is proportional to the product of the charges over the square of the distance between them.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The electric flux through a closed surface is proportional to the charge enclosed by
        /// that surface.
        /// </summary>
        public static readonly Proposition ElectricFluxProportionalToEnclosedCharge = new Proposition(
            "ElectricFluxProportionalToEnclosedCharge",
            "The electric flux through a closed surface is proportional to the charge enclosed by that surface.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The net magnetic flux through any closed surface is zero, since magnetic monopoles have
        /// not been observed.
        /// </summary>
        public static readonly Proposition MagneticFluxThroughClosedSurfaceIsZero = new Proposition(
            "MagneticFluxThroughClosedSurfaceIsZero",
            "The net magnetic flux through any closed surface is zero, since magnetic monopoles have not been observed.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// A changing magnetic flux through a circuit induces an electromotive force in that
        /// circuit.
        /// </summary>
        public static readonly Proposition ChangingMagneticFluxInducesEmf = new Proposition(
            "ChangingMagneticFluxInducesEmf",
            "A changing magnetic flux through a circuit induces an electromotive force in that circuit.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The circulating magnetic field around a closed loop relates to the electric current and
        /// the changing electric flux through the loop.
        /// </summary>
        public static readonly Proposition CirculatingMagneticFieldRelatesToCurrentAndFlux = new Proposition(
            "CirculatingMagneticFieldRelatesToCurrentAndFlux",
            "The circulating magnetic field around a closed loop relates to the electric current and the changing electric flux through the loop.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// An induced current flows in the direction that opposes the change in magnetic flux that
        /// produced it.
        /// </summary>
        public static readonly Proposition InducedCurrentOpposesFluxChange = new Proposition(
            "InducedCurrentOpposesFluxChange",
            "An induced current flows in the direction that opposes the change in magnetic flux that produced it.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized steady current, the magnetic field it generates is calculable from the
        /// geometry of the current.
        /// </summary>
        public static readonly Proposition MagneticFieldCalculableFromSteadyCurrentGeometry = new Proposition(
            "MagneticFieldCalculableFromSteadyCurrentGeometry",
            "For an idealized steady current, the magnetic field it generates is calculable from the geometry of the current.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized ohmic material, the current through a conductor is proportional to the
        /// voltage across it.
        /// </summary>
        public static readonly Proposition CurrentProportionalToVoltage = new Proposition(
            "CurrentProportionalToVoltage",
            "For an idealized ohmic material, the current through a conductor is proportional to the voltage across it.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Under the lumped circuit approximation, the sum of currents entering a node equals the
        /// sum leaving it.
        /// </summary>
        public static readonly Proposition CurrentIntoNodeEqualsCurrentOut = new Proposition(
            "CurrentIntoNodeEqualsCurrentOut",
            "Under the lumped circuit approximation, the sum of currents entering a node equals the sum leaving it.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// Under the lumped circuit approximation, the sum of voltages around a closed loop is
        /// zero.
        /// </summary>
        public static readonly Proposition VoltageAroundLoopSumsToZero = new Proposition(
            "VoltageAroundLoopSumsToZero",
            "Under the lumped circuit approximation, the sum of voltages around a closed loop is zero.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// At thermal equilibrium, the emissivity of a body equals its absorptivity at a given
        /// wavelength.
        /// </summary>
        public static readonly Proposition EmissivityEqualsAbsorptivityAtEquilibrium = new Proposition(
            "EmissivityEqualsAbsorptivityAtEquilibrium",
            "At thermal equilibrium, the emissivity of a body equals its absorptivity at a given wavelength.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For a constant resistance, the heat produced by a current is proportional to the square
        /// of the current, the resistance, and the time.
        /// </summary>
        public static readonly Proposition HeatProportionalToCurrentSquared = new Proposition(
            "HeatProportionalToCurrentSquared",
            "For a constant resistance, the heat produced by a current is proportional to the square of the current, the resistance, and the time.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an ideal gas, internal energy is independent of volume at constant temperature.
        /// </summary>
        public static readonly Proposition InternalEnergyIndependentOfVolumeForIdealGas = new Proposition(
            "InternalEnergyIndependentOfVolumeForIdealGas",
            "For an ideal gas, internal energy is independent of volume at constant temperature.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For a dilute, non-interacting paramagnetic system, magnetic susceptibility is inversely
        /// proportional to absolute temperature.
        /// </summary>
        public static readonly Proposition MagneticSusceptibilityInverseToTemperature = new Proposition(
            "MagneticSusceptibilityInverseToTemperature",
            "For a dilute, non-interacting paramagnetic system, magnetic susceptibility is inversely proportional to absolute temperature.",
            Testability.EmpiricallyTestable,
            true);

        // Optics and radiation

        /// <summary>
        /// The ratio of the sines of the angles of incidence and refraction equals the ratio of
        /// the refractive indices of the two media.
        /// </summary>
        public static readonly Proposition RefractionRatioEqualsIndexRatio = new Proposition(
            "RefractionRatioEqualsIndexRatio",
            "The ratio of the sines of the angles of incidence and refraction equals the ratio of the refractive indices of the two media.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The angle of incidence equals the angle of reflection.
        /// </summary>
        public static readonly Proposition AngleOfIncidenceEqualsAngleOfReflection = new Proposition(
            "AngleOfIncidenceEqualsAngleOfReflection",
            "The angle of incidence equals the angle of reflection.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized point source with no absorption, the intensity of radiation is
        /// inversely proportional to the square of the distance from the source.
        /// </summary>
        public static readonly Proposition RadiationIntensityInverseSquareToDistance = new Proposition(
            "RadiationIntensityInverseSquareToDistance",
            "For an idealized point source with no absorption, the intensity of radiation is inversely proportional to the square of the distance from the source.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized polarizer, the intensity of polarized light passing through varies as
        /// the cosine squared of the angle between the light's polarization and the polarizer's
        /// axis.
        /// </summary>
        public static readonly Proposition PolarizedLightIntensityFollowsCosineSquared = new Proposition(
            "PolarizedLightIntensityFollowsCosineSquared",
            "For an idealized polarizer, the intensity of polarized light passing through varies as the cosine squared of the angle between the light's polarization and the polarizer's axis.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized perfect black body, the total energy radiated is proportional to the
        /// fourth power of its absolute temperature.
        /// </summary>
        public static readonly Proposition BlackBodyRadiationProportionalToFourthPowerOfTemperature = new Proposition(
            "BlackBodyRadiationProportionalToFourthPowerOfTemperature",
            "For an idealized perfect black body, the total energy radiated is proportional to the fourth power of its absolute temperature.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized perfect black body, the peak wavelength of its radiation is inversely
        /// proportional to its absolute temperature.
        /// </summary>
        public static readonly Proposition BlackBodyPeakWavelengthInverseToTemperature = new Proposition(
            "BlackBodyPeakWavelengthInverseToTemperature",
            "For an idealized perfect black body, the peak wavelength of its radiation is inversely proportional to its absolute temperature.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For an idealized perfect black body, the spectral density of its radiation at a given
        /// temperature is described by Planck's law of black body radiation.
        /// </summary>
        public static readonly Proposition BlackBodyRadiationSpectrumDescribedByPlancksLaw = new Proposition(
            "BlackBodyRadiationSpectrumDescribedByPlancksLaw",
            "For an idealized perfect black body, the spectral density of its radiation at a given temperature is described by Planck's law of black body radiation.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For a dilute, non-scattering medium under monochromatic light, absorbance is
        /// proportional to concentration and path length.
        /// </summary>
        public static readonly Proposition LightAbsorbanceProportionalToConcentrationAndPathLength = new Proposition(
            "LightAbsorbanceProportionalToConcentrationAndPathLength",
            "For a dilute, non-scattering medium under monochromatic light, absorbance is proportional to concentration and path length.",
            Testability.EmpiricallyTestable,
            true);

        // Fluids

        /// <summary>
        /// For an idealized incompressible fluid, pressure applied to an enclosed fluid is
        /// transmitted undiminished throughout the fluid.
        /// </summary>
        public static readonly Proposition PressureTransmittedUndiminishedInEnclosedFluid = new Proposition(
            "PressureTransmittedUndiminishedInEnclosedFluid",
            "For an idealized incompressible fluid, pressure applied to an enclosed fluid is transmitted undiminished throughout the fluid.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// The buoyant force on a submerged body equals the weight of the fluid it displaces.
        /// </summary>
        public static readonly Proposition BuoyantForceEqualsWeightOfDisplacedFluid = new Proposition(
            "BuoyantForceEqualsWeightOfDisplacedFluid",
            "The buoyant force on a submerged body equals the weight of the fluid it displaces.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For idealized inviscid, incompressible flow, an increase in fluid speed occurs together
        /// with a decrease in pressure or potential energy.
        /// </summary>
        public static readonly Proposition FluidSpeedIncreaseAccompaniesPressureDecrease = new Proposition(
            "FluidSpeedIncreaseAccompaniesPressureDecrease",
            "For idealized inviscid, incompressible flow, an increase in fluid speed occurs together with a decrease in pressure or potential energy.",
            Testability.EmpiricallyTestable,
            true);

        /// <summary>
        /// For idealized inviscid, frictionless flow, the speed of fluid efflux from an orifice
        /// relates to the height of fluid above it.
        /// </summary>
        public static readonly Proposition FluidEffluxSpeedRelatesToHeight = new Proposition(
            "FluidEffluxSpeedRelatesToHeight",
            "For idealized inviscid, frictionless flow, the speed of fluid efflux from an orifice relates to the height of fluid above it.",
            Testability.EmpiricallyTestable,
            true);
    }
}
