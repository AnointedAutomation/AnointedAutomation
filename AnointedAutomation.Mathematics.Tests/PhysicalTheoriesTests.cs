// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics.Tests
{
    /// <summary>
    /// Tests for the catalog of well supported but still contested physical theories.
    /// </summary>
    public class PhysicalTheoriesTests
    {
        [Fact]
        public void EveryTheory_IsNotNull()
        {
            Assert.NotNull(PhysicalTheories.MassEnergyEquivalence);
            Assert.NotNull(PhysicalTheories.InvariantLightSpeed);
        }

        [Fact]
        public void EveryTheory_HasStatusTheory()
        {
            foreach (FoundationalClaim theory in PhysicalTheories.All)
            {
                Assert.Equal(EpistemicStatus.Theory, theory.Status);
            }
        }

        [Fact]
        public void MassEnergyEquivalence_IsIntraUniverseAtPointNineFive()
        {
            Assert.Equal(LawDomain.IntraUniverse, PhysicalTheories.MassEnergyEquivalence.Domain);
            Assert.Equal(0.95, PhysicalTheories.MassEnergyEquivalence.SurvivedFalsificationWeight);
        }

        [Fact]
        public void InvariantLightSpeed_IsIntraUniverseAtPointNineFive()
        {
            Assert.Equal(LawDomain.IntraUniverse, PhysicalTheories.InvariantLightSpeed.Domain);
            Assert.Equal(0.95, PhysicalTheories.InvariantLightSpeed.SurvivedFalsificationWeight);
        }

        [Fact]
        public void All_ContainsExactlyTwoTheories()
        {
            Assert.Equal(2, PhysicalTheories.All.Count);
        }
    }
}
