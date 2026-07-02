// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Tests.Epistemics
{
    /// <summary>
    /// The worked example from the design conversation, locked in as executable specification:
    /// conservation of energy, the eternal-matter claim, the cosmological argument, and atheism.
    /// The engine must stay neutral: it never declares "therefore God exists" and never lets an
    /// intra-universe law settle an origin claim, but it shows that "eternal uncaused energy" and
    /// "no creator" are theories with the same epistemic status as theism.
    /// </summary>
    public class WorkedExampleTests
    {
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

        [Fact]
        public void TheConversation_PlaysOutOnTheLedger()
        {
            FoundationalClaim conservation = new FoundationalClaim(
                "ConservationOfEnergy",
                "Within the universe, energy is neither created nor destroyed.",
                LawDomain.IntraUniverse,
                new List<Proposition> { EnergyConserved },
                new List<Proposition>(),
                0.99);
            // An intra-universe extrapolation that, if illegitimately applied, would falsify the
            // cosmological argument. The engine must refuse: this is the neutrality property under
            // maximum pressure, and the refusal is recorded as a domain skip.
            FoundationalClaim conservationExtrapolation = new FoundationalClaim(
                "ConservationExtrapolation",
                "Extrapolation: conservation implies the total system needs no origin.",
                LawDomain.IntraUniverse,
                new List<Proposition>(),
                new List<Proposition> { CreatedUniverse },
                0.4);
            EpistemicLedger ledger = new EpistemicLedger(
                new List<FoundationalClaim> { conservation, conservationExtrapolation });

            TheologicalClaim eternalMatter = new TheologicalClaim(
                "Energy and the universe are eternal and uncaused.",
                "materialist cosmology",
                0.7,
                new List<Proposition>(),
                new List<Proposition> { CreatedUniverse });
            TheologicalClaim cosmological = new TheologicalClaim(
                "The universe began, so it has a cause outside itself.",
                "cosmological argument",
                0.7,
                new List<Proposition> { CreatedUniverse },
                new List<Proposition>());
            TheologicalClaim atheism = new TheologicalClaim(
                "No creator exists.",
                "atheism",
                0.7,
                new List<Proposition>(),
                new List<Proposition> { Creator });

            Examination eternalMatterExam = ledger.Admit(eternalMatter);
            Examination cosmologicalExam = ledger.Admit(cosmological);
            Examination atheismExam = ledger.Admit(atheism);

            // Origin claims are unfalsifiable from inside the universe, all of them alike.
            Assert.Equal(Verdict.Unfalsifiable, eternalMatterExam.Verdict);
            Assert.Equal(Verdict.Unfalsifiable, cosmologicalExam.Verdict);
            Assert.Equal(Verdict.Unfalsifiable, atheismExam.Verdict);
            Assert.Null(eternalMatterExam.Standing);
            Assert.Null(cosmologicalExam.Standing);
            Assert.Null(atheismExam.Standing);

            // Eternal matter and the cosmological argument collide on CreatedUniverse: one
            // tension, both claims still on the ledger.
            Tension tension = Assert.Single(ledger.Tensions);
            Assert.Equal("CreatedUniverse", tension.Proposition.Name);
            Assert.Equal(2, ledger.ClaimsAbout(CreatedUniverse).Count);

            // The intra-universe extrapolation touched CreatedUniverse but was refused authority
            // over an origin claim; the skip was recorded, and the cosmological argument was NOT
            // ruled Contradicts by it.
            Assert.Contains(cosmologicalExam.Derivation,
                (DerivationStep step) => step.Outcome.Contains("domain skip"));
            Assert.Contains(eternalMatterExam.Derivation,
                (DerivationStep step) => step.Outcome.Contains("domain skip"));
        }
    }
}
