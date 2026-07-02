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
    public static class UniversalPropositions
    {
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

        /// <summary>
        /// Every effect has a cause.
        /// </summary>
        public static readonly Proposition EffectsHaveCauses = new Proposition(
            "EffectsHaveCauses",
            "Every effect has a cause.",
            Testability.EmpiricallyTestable,
            true);

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
        /// Every proposition declared by this catalog, in declaration order.
        /// </summary>
        public static readonly System.Collections.Generic.IReadOnlyList<Proposition> All =
            new System.Collections.Generic.List<Proposition>
            {
                NonContradiction,
                Identity,
                ExcludedMiddle,
                EffectsHaveCauses,
                EnergyConserved,
                EntropyIncreases,
                MassEnergyEquivalent,
                SpeedOfLightConstant,
                CollatzTerminates,
                GoldbachHolds,
                RiemannZerosOnCriticalLine
            };
    }
}
