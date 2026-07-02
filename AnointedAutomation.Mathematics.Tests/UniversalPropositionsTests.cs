// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Mathematics.Tests
{
    /// <summary>
    /// Tests for the shared vocabulary of propositions the catalogs assert and deny.
    /// </summary>
    public class UniversalPropositionsTests
    {
        [Fact]
        public void EveryProposition_IsNotNull()
        {
            Assert.NotNull(UniversalPropositions.NonContradiction);
            Assert.NotNull(UniversalPropositions.Identity);
            Assert.NotNull(UniversalPropositions.ExcludedMiddle);
            Assert.NotNull(UniversalPropositions.EffectsHaveCauses);
            Assert.NotNull(UniversalPropositions.EnergyConserved);
            Assert.NotNull(UniversalPropositions.EntropyIncreases);
            Assert.NotNull(UniversalPropositions.MassEnergyEquivalent);
            Assert.NotNull(UniversalPropositions.SpeedOfLightConstant);
            Assert.NotNull(UniversalPropositions.CollatzTerminates);
            Assert.NotNull(UniversalPropositions.GoldbachHolds);
            Assert.NotNull(UniversalPropositions.RiemannZerosOnCriticalLine);
            Assert.NotNull(UniversalPropositions.SpecialRelativityHolds);
            Assert.NotNull(UniversalPropositions.GravityIsSpacetimeCurvature);
            Assert.NotNull(UniversalPropositions.MatterEnergyIsQuantized);
            Assert.NotNull(UniversalPropositions.MatterComposedOfAtoms);
            Assert.NotNull(UniversalPropositions.UniverseExpandedFromHotDenseState);
            Assert.NotNull(UniversalPropositions.PopulationsEvolveByNaturalSelection);
            Assert.NotNull(UniversalPropositions.DiseasesCausedByMicroorganisms);
            Assert.NotNull(UniversalPropositions.OrganismsComposedOfCells);
            Assert.NotNull(UniversalPropositions.LithosphereDividedIntoMovingPlates);
            Assert.NotNull(UniversalPropositions.GasBehaviorArisesFromMolecularMotion);
        }

        [Fact]
        public void NothingHere_IsBeyondObservation()
        {
            foreach (Proposition proposition in UniversalPropositions.All)
            {
                Assert.Equal(Testability.EmpiricallyTestable, proposition.Testability);
            }
        }

        [Fact]
        public void LawPropositions_StandTrue()
        {
            Assert.True(UniversalPropositions.NonContradiction.Standing);
            Assert.True(UniversalPropositions.Identity.Standing);
            Assert.True(UniversalPropositions.ExcludedMiddle.Standing);
            Assert.True(UniversalPropositions.EffectsHaveCauses.Standing);
            Assert.True(UniversalPropositions.EnergyConserved.Standing);
            Assert.True(UniversalPropositions.EntropyIncreases.Standing);
            Assert.True(UniversalPropositions.SpecialRelativityHolds.Standing);
            Assert.True(UniversalPropositions.GravityIsSpacetimeCurvature.Standing);
            Assert.True(UniversalPropositions.MatterEnergyIsQuantized.Standing);
            Assert.True(UniversalPropositions.MatterComposedOfAtoms.Standing);
            Assert.True(UniversalPropositions.UniverseExpandedFromHotDenseState.Standing);
            Assert.True(UniversalPropositions.PopulationsEvolveByNaturalSelection.Standing);
            Assert.True(UniversalPropositions.DiseasesCausedByMicroorganisms.Standing);
            Assert.True(UniversalPropositions.OrganismsComposedOfCells.Standing);
            Assert.True(UniversalPropositions.LithosphereDividedIntoMovingPlates.Standing);
            Assert.True(UniversalPropositions.GasBehaviorArisesFromMolecularMotion.Standing);
        }

        [Fact]
        public void ConjectureBackedPropositions_StandNull()
        {
            Assert.Null(UniversalPropositions.CollatzTerminates.Standing);
            Assert.Null(UniversalPropositions.GoldbachHolds.Standing);
            Assert.Null(UniversalPropositions.RiemannZerosOnCriticalLine.Standing);
        }

        [Fact]
        public void All_HasNoDuplicateNames()
        {
            HashSet<string> names = new HashSet<string>(System.StringComparer.Ordinal);
            foreach (Proposition proposition in UniversalPropositions.All)
            {
                Assert.True(names.Add(proposition.Name));
            }
        }

        [Fact]
        public void All_ContainsEveryDeclaredProposition()
        {
            Assert.Equal(79, UniversalPropositions.All.Count);
        }
    }
}
