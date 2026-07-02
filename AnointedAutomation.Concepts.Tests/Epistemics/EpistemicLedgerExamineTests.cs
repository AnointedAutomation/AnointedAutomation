// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using System.Linq;
using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Tests.Epistemics
{
    public class EpistemicLedgerExamineTests
    {
        private static readonly Proposition EffectsHaveCauses = new Proposition(
            "EffectsHaveCauses",
            "Within the universe, effects have causes.",
            Testability.EmpiricallyTestable,
            true);

        private static readonly Proposition EnergyConserved = new Proposition(
            "EnergyConserved",
            "Within the universe, energy is neither created nor destroyed.",
            Testability.EmpiricallyTestable,
            true);

        private static readonly Proposition CreatedUniverse = new Proposition(
            "CreatedUniverse",
            "The universe was brought into being by something outside itself.",
            Testability.BeyondObservation);

        private static readonly Proposition Creator = new Proposition(
            "Creator",
            "A creator of the universe exists.",
            Testability.BeyondObservation);

        private static EpistemicLedger NewLedger()
        {
            FoundationalClaim causality = new FoundationalClaim(
                "Causality",
                "Within the universe, effects have causes.",
                LawDomain.IntraUniverse,
                new List<Proposition> { EffectsHaveCauses },
                new List<Proposition>(),
                0.99);
            FoundationalClaim conservation = new FoundationalClaim(
                "ConservationOfEnergy",
                "Within the universe, energy is neither created nor destroyed.",
                LawDomain.IntraUniverse,
                new List<Proposition> { EnergyConserved },
                new List<Proposition>(),
                0.99);
            return new EpistemicLedger(new List<FoundationalClaim> { causality, conservation });
        }

        [Fact]
        public void Examine_ClaimDenyingCausalityInsideTheUniverse_Contradicts()
        {
            TheologicalClaim uncausedMiracles = new TheologicalClaim(
                "Events inside the universe routinely happen with no cause at all.",
                "test tradition",
                0.7,
                new List<Proposition>(),
                new List<Proposition> { EffectsHaveCauses });

            Examination examination = NewLedger().Examine(uncausedMiracles);

            Assert.Equal(Verdict.Contradicts, examination.Verdict);
            Assert.False(examination.Standing);
            // The foundational claim is named in the derivation.
            Assert.Contains(examination.Derivation,
                (DerivationStep step) => step.Authority.Equals("Causality"));
            // Weakest premise: min(0.7, 0.99).
            Assert.Equal(0.7, examination.Confidence);
        }

        [Fact]
        public void Examine_OriginClaims_AreUnfalsifiableSymmetrically()
        {
            // Neutrality: creator, no creator, and eternal uncaused matter all get the same
            // verdict, because none can be tested from inside the universe.
            EpistemicLedger ledger = NewLedger();
            TheologicalClaim theism = new TheologicalClaim(
                "A creator exists.", "theism", 0.9,
                new List<Proposition> { Creator }, new List<Proposition>());
            TheologicalClaim atheism = new TheologicalClaim(
                "No creator exists.", "atheism", 0.9,
                new List<Proposition>(), new List<Proposition> { Creator });
            TheologicalClaim eternalMatter = new TheologicalClaim(
                "The universe is eternal and uncaused.", "materialist cosmology", 0.9,
                new List<Proposition>(), new List<Proposition> { CreatedUniverse });

            Assert.Equal(Verdict.Unfalsifiable, ledger.Examine(theism).Verdict);
            Assert.Equal(Verdict.Unfalsifiable, ledger.Examine(atheism).Verdict);
            Assert.Equal(Verdict.Unfalsifiable, ledger.Examine(eternalMatter).Verdict);
            Assert.Null(ledger.Examine(theism).Standing);
            Assert.Null(ledger.Examine(atheism).Standing);
        }

        [Fact]
        public void Examine_IntraUniverseLaw_NeverSettlesAnOriginClaim_AndTheSkipIsRecorded()
        {
            // Conservation of energy is proven inside the universe; it cannot rule on the origin
            // of the universe. The skip is recorded so neutrality is auditable.
            TheologicalClaim eternalMatter = new TheologicalClaim(
                "The universe is eternal and uncaused.", "materialist cosmology", 0.9,
                new List<Proposition>(), new List<Proposition> { CreatedUniverse });

            Examination examination = NewLedger().Examine(eternalMatter);

            Assert.Equal(Verdict.Unfalsifiable, examination.Verdict);
            Assert.Contains(examination.Derivation,
                (DerivationStep step) => step.Outcome.Contains("domain skip"));
        }

        [Fact]
        public void Examine_ClaimAssertingWhatAFoundationAsserts_IsConsistent()
        {
            TheologicalClaim orderlyWorld = new TheologicalClaim(
                "The world runs on cause and effect.", "test tradition", 0.8,
                new List<Proposition> { EffectsHaveCauses }, new List<Proposition>());

            Examination examination = NewLedger().Examine(orderlyWorld);

            Assert.Equal(Verdict.Consistent, examination.Verdict);
            Assert.True(examination.Standing);
            Assert.Equal(0.8, examination.Confidence);
        }

        [Fact]
        public void Examine_TestableClaimNoFoundationBearsOn_IsUndetermined()
        {
            Proposition prayerHeals = new Proposition(
                "IntercessoryPrayerAffectsRecovery",
                "Intercessory prayer measurably affects medical recovery.",
                Testability.EmpiricallyTestable);
            TheologicalClaim claim = new TheologicalClaim(
                "Prayer heals the sick.", "test tradition", 0.6,
                new List<Proposition> { prayerHeals }, new List<Proposition>());

            Examination examination = NewLedger().Examine(claim);

            Assert.Equal(Verdict.Undetermined, examination.Verdict);
            Assert.Null(examination.Standing);
        }

        [Fact]
        public void Examine_ScientificMethodItself_IsUnfalsifiable()
        {
            // Faith at the root, formally acknowledged: the method cannot prove the method.
            Proposition methodYieldsTruth = new Proposition(
                "ScientificMethodYieldsTruth",
                "The scientific method yields truth about reality.",
                Testability.BeyondObservation);
            TheologicalClaim claim = new TheologicalClaim(
                "The scientific method yields truth.", "scientism", 0.9,
                new List<Proposition> { methodYieldsTruth }, new List<Proposition>());

            Examination examination = NewLedger().Examine(claim);

            Assert.Equal(Verdict.Unfalsifiable, examination.Verdict);
            Assert.Null(examination.Standing);
        }

        [Fact]
        public void Ledger_ThrowsOnDuplicateFoundationalNames()
        {
            FoundationalClaim a = new FoundationalClaim(
                "Causality", "s", LawDomain.IntraUniverse,
                new List<Proposition> { EffectsHaveCauses }, new List<Proposition>(), 0.9);
            FoundationalClaim b = new FoundationalClaim(
                "Causality", "s2", LawDomain.IntraUniverse,
                new List<Proposition> { EffectsHaveCauses }, new List<Proposition>(), 0.8);

            Assert.Throws<System.ArgumentException>(
                () => new EpistemicLedger(new List<FoundationalClaim> { a, b }));
        }

        [Fact]
        public void Examine_ThrowsOnNullClaim()
        {
            Assert.Throws<System.ArgumentNullException>(() => NewLedger().Examine(null));
        }
    }
}
