// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics.Tests
{
    /// <summary>
    /// Tests for the catalog of unproven conjectures.
    /// </summary>
    public class ConjecturesTests
    {
        [Fact]
        public void EveryConjecture_IsNotNull()
        {
            Assert.NotNull(Conjectures.Collatz);
            Assert.NotNull(Conjectures.Goldbach);
            Assert.NotNull(Conjectures.RiemannHypothesis);
        }

        [Fact]
        public void EveryConjecture_HasStatusConjectureAndZeroWeight()
        {
            foreach (FoundationalClaim conjecture in Conjectures.All)
            {
                Assert.Equal(EpistemicStatus.Conjecture, conjecture.Status);
                Assert.Equal(0.0, conjecture.SurvivedFalsificationWeight);
            }
        }

        [Fact]
        public void All_ContainsExactlyThreeConjectures()
        {
            Assert.Equal(3, Conjectures.All.Count);
        }

        [Fact]
        public void ConjectureBackedClaim_ExaminesAsUndeterminedNotConsistent()
        {
            // A ledger built only from the bedrock laws: a conjecture's zero survived-falsification
            // weight means it is never admitted as authority, so a claim resting on it can never
            // be examined as Consistent.
            EpistemicLedger ledger = new EpistemicLedger(UniversalLaws.All);
            TheologicalClaim claim = new TheologicalClaim(
                "Every Collatz sequence reaches one.",
                "number theory",
                0.5,
                new System.Collections.Generic.List<Proposition> { UniversalPropositions.CollatzTerminates },
                new System.Collections.Generic.List<Proposition>());

            Examination examination = ledger.Examine(claim);

            Assert.Equal(Verdict.Undetermined, examination.Verdict);
            Assert.Null(examination.Standing);
        }
    }
}
