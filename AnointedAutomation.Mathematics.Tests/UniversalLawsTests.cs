// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics.Tests
{
    /// <summary>
    /// Tests for the bedrock catalog of universal laws.
    /// </summary>
    public class UniversalLawsTests
    {
        [Fact]
        public void EveryLaw_IsNotNull()
        {
            Assert.NotNull(UniversalLaws.NonContradiction);
            Assert.NotNull(UniversalLaws.Identity);
            Assert.NotNull(UniversalLaws.ExcludedMiddle);
            Assert.NotNull(UniversalLaws.Causality);
            Assert.NotNull(UniversalLaws.ConservationOfEnergy);
            Assert.NotNull(UniversalLaws.EntropyIncrease);
        }

        [Fact]
        public void EveryLaw_HasStatusLaw()
        {
            foreach (FoundationalClaim law in UniversalLaws.All)
            {
                Assert.Equal(EpistemicStatus.Law, law.Status);
            }
        }

        [Fact]
        public void NonContradiction_IsUnrestrictedWithFullWeight()
        {
            Assert.Equal(LawDomain.Unrestricted, UniversalLaws.NonContradiction.Domain);
            Assert.Equal(1.0, UniversalLaws.NonContradiction.SurvivedFalsificationWeight);
        }

        [Fact]
        public void Identity_IsUnrestrictedWithFullWeight()
        {
            Assert.Equal(LawDomain.Unrestricted, UniversalLaws.Identity.Domain);
            Assert.Equal(1.0, UniversalLaws.Identity.SurvivedFalsificationWeight);
        }

        [Fact]
        public void ExcludedMiddle_IsUnrestrictedAtPointNineEight()
        {
            Assert.Equal(LawDomain.Unrestricted, UniversalLaws.ExcludedMiddle.Domain);
            Assert.Equal(0.98, UniversalLaws.ExcludedMiddle.SurvivedFalsificationWeight);
        }

        [Fact]
        public void Causality_IsIntraUniverseAtPointNineNine()
        {
            Assert.Equal(LawDomain.IntraUniverse, UniversalLaws.Causality.Domain);
            Assert.Equal(0.99, UniversalLaws.Causality.SurvivedFalsificationWeight);
        }

        [Fact]
        public void ConservationOfEnergy_IsIntraUniverseAtPointNineNine()
        {
            Assert.Equal(LawDomain.IntraUniverse, UniversalLaws.ConservationOfEnergy.Domain);
            Assert.Equal(0.99, UniversalLaws.ConservationOfEnergy.SurvivedFalsificationWeight);
        }

        [Fact]
        public void EntropyIncrease_IsIntraUniverseAtPointNineNine()
        {
            Assert.Equal(LawDomain.IntraUniverse, UniversalLaws.EntropyIncrease.Domain);
            Assert.Equal(0.99, UniversalLaws.EntropyIncrease.SurvivedFalsificationWeight);
        }

        [Fact]
        public void All_ContainsExactlySixtyFourLaws()
        {
            Assert.Equal(64, UniversalLaws.All.Count);
        }

        [Fact]
        public void EveryLaw_HasIntraUniverseOrUnrestrictedDomainAndWeightWithinRange()
        {
            foreach (FoundationalClaim law in UniversalLaws.All)
            {
                Assert.Equal(EpistemicStatus.Law, law.Status);
                Assert.True(
                    law.Domain.Equals(LawDomain.IntraUniverse) || law.Domain.Equals(LawDomain.Unrestricted));
                Assert.InRange(law.SurvivedFalsificationWeight, 0.9, 1.0);
            }
        }

        [Fact]
        public void Laws_CanConstructAnEpistemicLedger()
        {
            EpistemicLedger ledger = new EpistemicLedger(UniversalLaws.All);
            TheologicalClaim claim = new TheologicalClaim(
                "Energy is conserved in this closed system.",
                "physics classroom",
                0.9,
                new System.Collections.Generic.List<Proposition> { UniversalPropositions.EnergyConserved },
                new System.Collections.Generic.List<Proposition>());

            Examination examination = ledger.Examine(claim);

            Assert.Equal(Verdict.Consistent, examination.Verdict);
        }
    }
}
