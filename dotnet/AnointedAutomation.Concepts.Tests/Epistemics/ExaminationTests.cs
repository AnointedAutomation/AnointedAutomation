// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Concepts.Reasoning;

namespace AnointedAutomation.Concepts.Tests.Epistemics
{
    public class ExaminationTests
    {
        private static TheologicalClaim AnyClaim()
        {
            Proposition creator = new Proposition(
                "Creator", "A creator exists.", Testability.BeyondObservation);
            return new TheologicalClaim(
                "A creator exists.", "theism", 0.8,
                new List<Proposition> { creator }, new List<Proposition>());
        }

        [Fact]
        public void Examination_MapsConsistentToTrueStanding()
        {
            Examination examination = new Examination(
                AnyClaim(), Verdict.Consistent, 0.8, new List<DerivationStep>());

            Assert.Equal(TruthValue.True, examination.Standing);
        }

        [Fact]
        public void Examination_MapsContradictsToFalseStanding()
        {
            Examination examination = new Examination(
                AnyClaim(), Verdict.Contradicts, 0.8, new List<DerivationStep>());

            Assert.Equal(TruthValue.False, examination.Standing);
        }

        [Fact]
        public void Examination_MapsBothUnknownFlavorsToUnknownStanding()
        {
            // Unfalsifiable and Undetermined are the two flavors of Unknown: can never test from
            // inside, versus could test but have not sufficiently.
            Examination unfalsifiable = new Examination(
                AnyClaim(), Verdict.Unfalsifiable, 0.8, new List<DerivationStep>());
            Examination undetermined = new Examination(
                AnyClaim(), Verdict.Undetermined, 0.8, new List<DerivationStep>());

            Assert.Equal(TruthValue.Unknown, unfalsifiable.Standing);
            Assert.Equal(TruthValue.Unknown, undetermined.Standing);
        }

        [Fact]
        public void Examination_RejectsAVerdictWithNoTruthMapping()
        {
            Examination unmapped = new Examination(
                AnyClaim(), (Verdict)99, 0.8, new List<DerivationStep>());

            Assert.Throws<System.InvalidOperationException>(() => unmapped.Standing);
        }

        [Fact]
        public void Examination_CarriesDerivation()
        {
            DerivationStep step = new DerivationStep(
                "Causality", "EffectsHaveCauses", "claim denies what the foundational claim asserts");
            Examination examination = new Examination(
                AnyClaim(), Verdict.Contradicts, 0.8, new List<DerivationStep> { step });

            Assert.Single(examination.Derivation);
            Assert.Equal("Causality", examination.Derivation[0].Authority);
            Assert.Equal("EffectsHaveCauses", examination.Derivation[0].PropositionName);
        }

        [Fact]
        public void Examination_ThrowsOnNullArguments()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Examination(
                null, Verdict.Consistent, 0.8, new List<DerivationStep>()));
            Assert.Throws<System.ArgumentNullException>(() => new Examination(
                AnyClaim(), Verdict.Consistent, 0.8, null));
        }
    }
}
