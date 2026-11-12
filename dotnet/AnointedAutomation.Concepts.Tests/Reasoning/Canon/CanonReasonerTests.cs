// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Concepts.Reasoning;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Canon
{
    public class CanonReasonerTests
    {
        [Fact]
        public async Task Ask_EstablishedAnswer_HasFullConfidenceScriptureAndCanonProvenance()
        {
            var answer = await CanonKit.Reasoner().Ask(CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Justice), CancellationToken.None);

            Assert.Equal(TruthValue.True, answer.Truth);
            Assert.Equal(1.0, answer.Confidence);
            Assert.NotEmpty(answer.Why.Steps);
            Assert.Contains(new ScriptureReference(BibleBook.Exodus, 21, 22, 25), answer.Why.Scripture);
            Assert.NotEmpty(answer.Why.FactsUsed);
            Assert.Equal(AnswerSource.Canon, answer.Provenance.Source);
            Assert.Equal(CanonReasoner.ModelName, answer.Provenance.Model);
            Assert.Equal(CanonKit.Now, answer.Provenance.At);
            Assert.Equal(VerificationStatus.Verified, answer.Provenance.Status);
            Assert.Equal("canon.sin-violates", answer.Provenance.RuleOrPromptId);
        }

        [Fact]
        public async Task Ask_GivesTheSameAnswerAsDecide()
        {
            var query = CanonKit.Ask(CanonKit.Compassion, RelationKind.Upholds, CanonFacts.RevealedCharacter);
            var asked = await CanonKit.Reasoner().Ask(query, CancellationToken.None);
            Assert.Equal(CanonKit.Signature(CanonKit.Reasoner().Decide(query)), CanonKit.Signature(asked));
        }

        [Fact]
        public void Decide_ClassifiesASinByCategoryCoarseSinAndGroundingVice()
        {
            var answer = CanonKit.Reasoner().Decide(CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Mercy));

            Assert.Equal(
                new[] { CanonFacts.Category(SinCategory.AngerAndViolence), CanonFacts.CoarseSin(AnointedAutomation.Enums.Sin.Murder), CanonKit.Murder },
                answer.Classification);
        }

        [Fact]
        public void Decide_Unsettled_IsUnknownWithZeroConfidenceAndNoProof()
        {
            var answer = CanonKit.Reasoner().Decide(CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Mercy));

            Assert.Equal(TruthValue.Unknown, answer.Truth);
            Assert.Equal(0.0, answer.Confidence);
            Assert.Empty(answer.Why.Steps);
            Assert.Empty(answer.Why.Scripture);
            Assert.Empty(answer.Why.FactsUsed);
            Assert.Equal(CanonReasoner.NoRuleId, answer.Provenance.RuleOrPromptId);
            Assert.Equal(VerificationStatus.Unverified, answer.Provenance.Status);
            Assert.Equal("I don't know. No scripture fact or rule in the catalog settles this question.", answer.Why.Explanation);
        }

        [Fact]
        public void Decide_SubjectOutsideTheCanon_IsUnknown()
        {
            var answer = CanonKit.Reasoner().Decide(CanonKit.Ask(new ConceptRef("Nowhere", "Nothing"), RelationKind.Violates, CanonKit.Love));
            Assert.Equal(TruthValue.Unknown, answer.Truth);
            Assert.Equal(VerificationStatus.Unverified, answer.Provenance.Status);
            Assert.Empty(answer.Classification);
        }

        [Fact]
        public void Decide_RelationWithNoRule_IsUnknown()
        {
            Assert.Equal(TruthValue.Unknown, CanonKit.Reasoner().Decide(CanonKit.Ask(CanonKit.Abortion, RelationKind.HasGravity, CanonKit.Love)).Truth);
        }

        [Fact]
        public async Task Ask_Cancelled_Throws()
        {
            using var source = new CancellationTokenSource();
            source.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => CanonKit.Reasoner().Ask(CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Love), source.Token));
        }

        [Fact]
        public void Constructor_RejectsNullsAndDuplicateRuleIds()
        {
            var clock = new CanonKit.FixedClock(CanonKit.Now);
            Assert.Throws<ArgumentNullException>(() => new CanonReasoner(null));
            Assert.Throws<ArgumentNullException>(() => new CanonReasoner(null, CanonRules.Default, clock));
            Assert.Throws<ArgumentNullException>(() => new CanonReasoner(CanonFacts.Canon, null, clock));
            Assert.Throws<ArgumentException>(() => new CanonReasoner(CanonFacts.Canon, new ICanonRule[] { new WitnessRule(), new WitnessRule() }, clock));
            Assert.Throws<ArgumentException>(() => new CanonReasoner(CanonFacts.Canon, new ICanonRule[] { null }, clock));
        }

        [Fact]
        public void Decide_NullQuery_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => CanonKit.Reasoner().Decide(null));
        }

        [Fact]
        public void Decide_RulesThatDisagree_IsACanonContradiction()
        {
            var reasoner = new CanonReasoner(
                CanonFacts.Canon,
                new ICanonRule[] { new StubRule("stub.true", TruthValue.True, true), new StubRule("stub.false", TruthValue.False, true) },
                new CanonKit.FixedClock(CanonKit.Now));
            Assert.Throws<InvalidOperationException>(() => reasoner.Decide(CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Love)));
        }

        [Fact]
        public void Decide_RulesThatAgree_TakeTheFirst()
        {
            var reasoner = new CanonReasoner(
                CanonFacts.Canon,
                new ICanonRule[] { new StubRule("stub.first", TruthValue.True, true), new StubRule("stub.second", TruthValue.True, true) },
                new CanonKit.FixedClock(CanonKit.Now));
            Assert.Equal("stub.first", reasoner.Decide(CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Love)).Provenance.RuleOrPromptId);
        }

        [Fact]
        public void Decide_EstablishedWithoutCanonFacts_Throws()
        {
            var reasoner = new CanonReasoner(
                CanonFacts.Canon,
                new ICanonRule[] { new StubRule("stub.ungrounded", TruthValue.False, false) },
                new CanonKit.FixedClock(CanonKit.Now));
            Assert.Throws<InvalidOperationException>(() => reasoner.Decide(CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Love)));
        }

        [Fact]
        public void Decide_UndefinedTruthValue_Throws()
        {
            var reasoner = new CanonReasoner(
                CanonFacts.Canon,
                new ICanonRule[] { new StubRule("stub.undefined", (TruthValue)9, true) },
                new CanonKit.FixedClock(CanonKit.Now));
            Assert.Throws<InvalidOperationException>(() => reasoner.Decide(CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Love)));
        }

        private sealed class StubRule : ICanonRule
        {
            private readonly TruthValue result;
            private readonly bool grounded;

            public StubRule(string id, TruthValue result, bool grounded)
            {
                Id = id;
                this.result = result;
                this.grounded = grounded;
            }

            public string Id { get; }

            public RelationKind Relation => RelationKind.Violates;

            public TruthValue Apply(Query query, CanonFacts facts, out ProofStep[] steps, out CanonFact[] grounds)
            {
                steps = new[] { new ProofStep(1, "stub", "A stub step.") };
                grounds = grounded ? new List<CanonFact>(facts.About(query.Assertion.Subject, RelationKind.GroundedIn)).ToArray() : new CanonFact[0];
                return result;
            }
        }
    }
}
