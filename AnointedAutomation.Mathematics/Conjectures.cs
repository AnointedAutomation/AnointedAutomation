// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics
{
    /// <summary>
    /// The conjecture catalog: foundational claims with status <see cref="EpistemicStatus.Conjecture"/>
    /// and zero survived falsification weight, because none of them has survived any falsification
    /// at all, only failed refutation attempts and unproven belief. A weight of zero means a
    /// conjecture must never be treated as authority in an <see cref="EpistemicLedger"/>: a claim
    /// resting only on one of these examines as undetermined, never as consistent.
    /// </summary>
    public static class Conjectures
    {
        /// <summary>
        /// The Collatz conjecture: every Collatz sequence, starting from any positive integer,
        /// eventually reaches one.
        /// </summary>
        public static readonly FoundationalClaim Collatz = new FoundationalClaim(
            "Collatz",
            "Every Collatz sequence, starting from any positive integer, eventually reaches one.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.CollatzTerminates },
            new List<Proposition>(),
            0.0,
            EpistemicStatus.Conjecture);

        /// <summary>
        /// Goldbach's conjecture: every even integer greater than two is the sum of two primes.
        /// </summary>
        public static readonly FoundationalClaim Goldbach = new FoundationalClaim(
            "Goldbach",
            "Every even integer greater than two is the sum of two primes.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.GoldbachHolds },
            new List<Proposition>(),
            0.0,
            EpistemicStatus.Conjecture);

        /// <summary>
        /// The Riemann hypothesis: every nontrivial zero of the Riemann zeta function has real
        /// part one half.
        /// </summary>
        public static readonly FoundationalClaim RiemannHypothesis = new FoundationalClaim(
            "RiemannHypothesis",
            "Every nontrivial zero of the Riemann zeta function has real part one half.",
            LawDomain.IntraUniverse,
            new List<Proposition> { UniversalPropositions.RiemannZerosOnCriticalLine },
            new List<Proposition>(),
            0.0,
            EpistemicStatus.Conjecture);

        /// <summary>
        /// Every conjecture declared by this catalog, in declaration order.
        /// </summary>
        public static readonly IReadOnlyList<FoundationalClaim> All = new List<FoundationalClaim>
        {
            Collatz,
            Goldbach,
            RiemannHypothesis
        };
    }
}
