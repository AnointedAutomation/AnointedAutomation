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
            // A ledger built from the bedrock laws plus the conjectures: a conjecture's zero
            // survived-falsification weight means it has no strength to lend, so a claim resting
            // on it can never be examined as Consistent, even though the conjecture is present
            // and agrees with the claim.
            System.Collections.Generic.List<FoundationalClaim> foundations =
                new System.Collections.Generic.List<FoundationalClaim>(UniversalLaws.All);
            foundations.AddRange(Conjectures.All);
            EpistemicLedger ledger = new EpistemicLedger(foundations);
            TheologicalClaim claim = new TheologicalClaim(
                "Every Collatz sequence reaches one.",
                "test",
                0.9,
                new System.Collections.Generic.List<Proposition> { UniversalPropositions.CollatzTerminates },
                new System.Collections.Generic.List<Proposition>());

            Examination examination = ledger.Examine(claim);

            Assert.Equal(Verdict.Undetermined, examination.Verdict);
            Assert.Null(examination.Standing);
        }
    }
}
