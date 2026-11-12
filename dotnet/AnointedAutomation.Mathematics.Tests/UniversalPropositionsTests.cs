// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Concepts.Reasoning;

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
            Assert.Equal(TruthValue.True, UniversalPropositions.NonContradiction.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.Identity.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.ExcludedMiddle.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.EffectsHaveCauses.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.EnergyConserved.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.EntropyIncreases.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.SpecialRelativityHolds.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.GravityIsSpacetimeCurvature.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.MatterEnergyIsQuantized.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.MatterComposedOfAtoms.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.UniverseExpandedFromHotDenseState.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.PopulationsEvolveByNaturalSelection.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.DiseasesCausedByMicroorganisms.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.OrganismsComposedOfCells.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.LithosphereDividedIntoMovingPlates.Standing);
            Assert.Equal(TruthValue.True, UniversalPropositions.GasBehaviorArisesFromMolecularMotion.Standing);
        }

        [Fact]
        public void ConjectureBackedPropositions_StandNull()
        {
            Assert.Equal(TruthValue.Unknown, UniversalPropositions.CollatzTerminates.Standing);
            Assert.Equal(TruthValue.Unknown, UniversalPropositions.GoldbachHolds.Standing);
            Assert.Equal(TruthValue.Unknown, UniversalPropositions.RiemannZerosOnCriticalLine.Standing);
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
