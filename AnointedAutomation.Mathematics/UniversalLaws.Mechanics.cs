// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The mechanics, gravitation, astronomy, conservation, and thermodynamics laws of the
    /// bedrock catalog.
    /// </summary>
    public static partial class UniversalLaws
    {
        // Mechanics

        /// <summary>
        /// Newton's First Law: within an inertial frame, a body at rest or in uniform motion
        /// remains so unless acted on by a net force.
        /// </summary>
        public static readonly FoundationalClaim NewtonsFirstLaw = new FoundationalClaim(
            "NewtonsFirstLaw",
            "Within an inertial frame, a body at rest or in uniform motion remains so unless acted on by a net force.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.NewtonsFirstLaw },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Newton's Second Law: for constant mass, net force equals the rate of change of
        /// momentum, reducing to force equals mass times acceleration.
        /// </summary>
        public static readonly FoundationalClaim NewtonsSecondLaw = new FoundationalClaim(
            "NewtonsSecondLaw",
            "For constant mass, net force equals the rate of change of momentum, reducing to force equals mass times acceleration.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.NewtonsSecondLaw },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Newton's Third Law: for every action there is an equal and opposite reaction.
        /// </summary>
        public static readonly FoundationalClaim NewtonsThirdLaw = new FoundationalClaim(
            "NewtonsThirdLaw",
            "For every action there is an equal and opposite reaction.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ActionHasEqualOppositeReaction },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Hooke's Law: within the elastic limit, the restoring force of a spring is proportional
        /// to its displacement.
        /// </summary>
        public static readonly FoundationalClaim HookesLaw = new FoundationalClaim(
            "HookesLaw",
            "Within the elastic limit, the restoring force of a spring is proportional to its displacement.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.SpringForceProportionalToDisplacement },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Stokes' Law: for a small sphere at low Reynolds number, the drag force in a viscous
        /// fluid is proportional to its velocity.
        /// </summary>
        public static readonly FoundationalClaim StokesLaw = new FoundationalClaim(
            "StokesLaw",
            "For a small sphere at low Reynolds number, the drag force in a viscous fluid is proportional to its velocity.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ViscousDragProportionalToVelocity },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        // Gravitation and astronomy

        /// <summary>
        /// Newton's Law of Universal Gravitation: two masses attract with a force proportional to
        /// the product of the masses over the square of the distance between them, though general
        /// relativity supersedes this at high precision.
        /// </summary>
        public static readonly FoundationalClaim NewtonsLawOfUniversalGravitation = new FoundationalClaim(
            "NewtonsLawOfUniversalGravitation",
            "Two masses attract with a force proportional to the product of the masses over the square of the distance between them, though general relativity supersedes this at high precision.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MassesAttractInverseSquareToDistance },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Kepler's First Law: under the two body approximation, planets orbit the sun in
        /// ellipses with the sun at one focus.
        /// </summary>
        public static readonly FoundationalClaim KeplersFirstLaw = new FoundationalClaim(
            "KeplersFirstLaw",
            "Under the two body approximation, planets orbit the sun in ellipses with the sun at one focus.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.PlanetaryOrbitsAreElliptical },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Kepler's Second Law: under the two body approximation, a line joining a planet and the
        /// sun sweeps out equal areas in equal times.
        /// </summary>
        public static readonly FoundationalClaim KeplersSecondLaw = new FoundationalClaim(
            "KeplersSecondLaw",
            "Under the two body approximation, a line joining a planet and the sun sweeps out equal areas in equal times.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.OrbitsSweepEqualAreas },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        /// <summary>
        /// Kepler's Third Law: under the two body approximation, the square of a planet's orbital
        /// period is proportional to the cube of its orbit's semi-major axis.
        /// </summary>
        public static readonly FoundationalClaim KeplersThirdLaw = new FoundationalClaim(
            "KeplersThirdLaw",
            "Under the two body approximation, the square of a planet's orbital period is proportional to the cube of its orbit's semi-major axis.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.OrbitalPeriodSquaredProportionalToAxisCubed },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);

        // Conservation

        /// <summary>
        /// Law of Conservation of Momentum: the total momentum of an isolated system is constant.
        /// </summary>
        public static readonly FoundationalClaim ConservationOfMomentum = new FoundationalClaim(
            "ConservationOfMomentum",
            "The total momentum of an isolated system is constant.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MomentumConserved },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Law of Conservation of Angular Momentum: the total angular momentum of an isolated
        /// system is constant.
        /// </summary>
        public static readonly FoundationalClaim ConservationOfAngularMomentum = new FoundationalClaim(
            "ConservationOfAngularMomentum",
            "The total angular momentum of an isolated system is constant.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.AngularMomentumConserved },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// Law of Conservation of Mass: mass is neither created nor destroyed in a chemical
        /// reaction.
        /// </summary>
        public static readonly FoundationalClaim ConservationOfMass = new FoundationalClaim(
            "ConservationOfMass",
            "Mass is neither created nor destroyed in a chemical reaction.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.MassConserved },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        // Thermodynamics

        /// <summary>
        /// The Zeroth Law of Thermodynamics: if two systems are each in thermal equilibrium with a
        /// third system, they are in thermal equilibrium with each other.
        /// </summary>
        public static readonly FoundationalClaim ZerothLawOfThermodynamics = new FoundationalClaim(
            "ZerothLawOfThermodynamics",
            "If two systems are each in thermal equilibrium with a third system, they are in thermal equilibrium with each other.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.ThermalEquilibriumIsTransitive },
            new List<Proposition>(),
            0.99,
            EpistemicStatus.Law);

        /// <summary>
        /// The Third Law of Thermodynamics: for an idealized perfect crystal, entropy approaches a
        /// constant minimum as temperature approaches absolute zero.
        /// </summary>
        public static readonly FoundationalClaim ThirdLawOfThermodynamics = new FoundationalClaim(
            "ThirdLawOfThermodynamics",
            "For an idealized perfect crystal, entropy approaches a constant minimum as temperature approaches absolute zero.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.EntropyVanishesAtAbsoluteZero },
            new List<Proposition>(),
            0.9,
            EpistemicStatus.Law);
    }
}
