// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Tests.Epistemics
{
    public class EpistemicLedgerAdmitTests
    {
        private static readonly Proposition CreatedUniverse = new Proposition(
            "CreatedUniverse",
            "The universe was brought into being by something outside itself.",
            Testability.BeyondObservation);

        private static EpistemicLedger EmptyLedger()
        {
            Proposition effectsHaveCauses = new Proposition(
                "EffectsHaveCauses",
                "Within the universe, effects have causes.",
                Testability.EmpiricallyTestable,
                true);
            FoundationalClaim causality = new FoundationalClaim(
                "Causality",
                "Within the universe, effects have causes.",
                LawDomain.IntraUniverse,
                new List<Proposition> { effectsHaveCauses },
                new List<Proposition>(),
                0.99);
            return new EpistemicLedger(new List<FoundationalClaim> { causality });
        }

        private static TheologicalClaim Theist()
        {
            return new TheologicalClaim(
                "The universe was created.", "Genesis 1:1", 0.9,
                new List<Proposition> { CreatedUniverse }, new List<Proposition>());
        }

        private static TheologicalClaim Materialist()
        {
            return new TheologicalClaim(
                "The universe is eternal and uncaused.", "materialist cosmology", 0.6,
                new List<Proposition>(), new List<Proposition> { CreatedUniverse });
        }

        [Fact]
        public void Admit_TwoContradictingClaims_RecordsOneTension_BothClaimsStand()
        {
            EpistemicLedger ledger = EmptyLedger();
            TheologicalClaim theist = Theist();
            TheologicalClaim materialist = Materialist();

            ledger.Admit(theist);
            ledger.Admit(materialist);

            Tension tension = Assert.Single(ledger.Tensions);
            Assert.Equal("CreatedUniverse", tension.Proposition.Name);
            // Neither claim was deleted; both remain queryable.
            System.Collections.Generic.IReadOnlyList<TheologicalClaim> about =
                ledger.ClaimsAbout(CreatedUniverse);
            Assert.Equal(2, about.Count);
            Assert.Contains(theist, about);
            Assert.Contains(materialist, about);
        }

        [Fact]
        public void Admit_TensionDoesNotChangeTheVerdict()
        {
            // Peer collisions create tensions; they do not falsify either claim. Both origin
            // claims stay Unfalsifiable.
            EpistemicLedger ledger = EmptyLedger();
            ledger.Admit(Theist());

            Examination examination = ledger.Admit(Materialist());

            Assert.Equal(Verdict.Unfalsifiable, examination.Verdict);
            Assert.Null(examination.Standing);
        }

        [Fact]
        public void ClaimsFrom_FiltersBySourceOrdinally()
        {
            EpistemicLedger ledger = EmptyLedger();
            TheologicalClaim theist = Theist();
            ledger.Admit(theist);
            ledger.Admit(Materialist());

            System.Collections.Generic.IReadOnlyList<TheologicalClaim> fromGenesis =
                ledger.ClaimsFrom("Genesis 1:1");

            TheologicalClaim only = Assert.Single(fromGenesis);
            Assert.Same(theist, only);
        }

        [Fact]
        public void Admit_ThrowsOnNullClaim_AndQueriesThrowOnNull()
        {
            EpistemicLedger ledger = EmptyLedger();

            Assert.Throws<System.ArgumentNullException>(() => ledger.Admit(null));
            Assert.Throws<System.ArgumentNullException>(() => ledger.ClaimsAbout(null));
            Assert.Throws<System.ArgumentException>(() => ledger.ClaimsFrom(null));
        }
    }
}
