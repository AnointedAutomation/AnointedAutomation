// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Concepts.Reasoning;
using AnointedAutomation.Concepts.Reasoning.Language;
using AnointedAutomation.Concepts.Tests.Reasoning.Canon;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Language
{
    public class LanguageReasonerTests
    {
        private static LanguageReasoner Reasoner(ILanguageModel model, LanguageKit.CapturingLogger<InterpretationParser> logger)
        {
            return new LanguageReasoner(model, LanguageKit.Parser(logger), new CanonKit.FixedClock(CanonKit.Now));
        }

        [Fact]
        public async Task Ask_ValidReply_UnverifiedLanguageAnswer()
        {
            var model = new LanguageKit.FakeLanguageModel(LanguageKit.Golden("reply-valid.json"));
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();
            Query query = LanguageKit.AbortionViolatesLove();

            Answer answer = await Reasoner(model, logger).Ask(query, CancellationToken.None);

            Assert.Equal(TruthValue.True, answer.Truth);
            Assert.Equal(0.9, answer.Confidence);
            Assert.Equal(AnswerSource.Language, answer.Provenance.Source);
            Assert.Equal(LanguageKit.ModelName, answer.Provenance.Model);
            Assert.Equal(CanonKit.Now, answer.Provenance.At);
            Assert.Equal(VerificationStatus.Unverified, answer.Provenance.Status);
            Assert.Equal(PromptComposer.Compose(query).Id, answer.Provenance.RuleOrPromptId);
            Assert.Empty(answer.Why.FactsUsed);
            Assert.Equal(3, answer.Why.Scripture.Count);
            Assert.Equal(PromptComposer.Compose(query).Text, Assert.Single(model.Prompts));
            Assert.Empty(logger.Entries);
        }

        [Fact]
        public async Task Interpret_OneModelCall_ReturnsTheFullInterpretation()
        {
            var model = new LanguageKit.FakeLanguageModel(LanguageKit.Golden("reply-valid.json"));
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();

            LanguageResult result = await Reasoner(model, logger).Interpret(LanguageKit.AbortionViolatesLove(), CancellationToken.None);

            Assert.Single(model.Prompts);
            Assert.Equal(new[] { SinId.Abortion }, result.Interpretation.SinIds);
            Assert.Single(result.Interpretation.Assertions);
            Assert.Equal(new[] { ConceptRef.Of(SinId.Abortion), new ConceptRef(ConceptRef.MoralConceptKind, "Murder") }, result.Answer.Classification);
        }

        [Fact]
        public async Task Ask_CandidateSinNotInClassification_IsAddedOnce()
        {
            string reply = LanguageKit.Golden("reply-valid.json").Replace("\"sinIds\": [\"Abortion\"]", "\"sinIds\": [\"Abortion\", \"Accusing\"]");
            var model = new LanguageKit.FakeLanguageModel(reply);

            Answer answer = await Reasoner(model, new LanguageKit.CapturingLogger<InterpretationParser>()).Ask(LanguageKit.AbortionViolatesLove(), CancellationToken.None);

            Assert.Equal(
                new[] { ConceptRef.Of(SinId.Abortion), new ConceptRef(ConceptRef.MoralConceptKind, "Murder"), ConceptRef.Of(SinId.Accusing) },
                answer.Classification);
        }

        [Fact]
        public async Task Ask_RejectedReply_ThrowsAndLogs()
        {
            var model = new LanguageKit.FakeLanguageModel(LanguageKit.Golden("reply-bad-truth.json"));
            var logger = new LanguageKit.CapturingLogger<InterpretationParser>();

            await Assert.ThrowsAsync<InterpretationRejected>(() => Reasoner(model, logger).Ask(LanguageKit.AbortionViolatesLove(), CancellationToken.None));

            Assert.Contains(PromptComposer.Compose(LanguageKit.AbortionViolatesLove()).Id, Assert.Single(logger.Entries).Message);
        }

        [Fact]
        public async Task Ask_Cancelled_Throws()
        {
            var model = new LanguageKit.FakeLanguageModel(LanguageKit.Golden("reply-valid.json"));

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => Reasoner(model, new LanguageKit.CapturingLogger<InterpretationParser>()).Ask(LanguageKit.AbortionViolatesLove(), new CancellationToken(true)));
        }

        [Fact]
        public void Constructor_RequiresEveryDependency()
        {
            var parser = LanguageKit.Parser(new LanguageKit.CapturingLogger<InterpretationParser>());
            var model = new LanguageKit.FakeLanguageModel("{}");
            var clock = new CanonKit.FixedClock(CanonKit.Now);

            Assert.Throws<ArgumentNullException>(() => new LanguageReasoner(null, parser, clock));
            Assert.Throws<ArgumentNullException>(() => new LanguageReasoner(model, null, clock));
            Assert.Throws<ArgumentNullException>(() => new LanguageReasoner(model, parser, null));
        }

        [Fact]
        public async Task Ask_NullQuery_Throws()
        {
            var model = new LanguageKit.FakeLanguageModel("{}");

            await Assert.ThrowsAsync<ArgumentNullException>(() => Reasoner(model, new LanguageKit.CapturingLogger<InterpretationParser>()).Ask(null, CancellationToken.None));
            Assert.Empty(model.Prompts);
        }

        [Fact]
        public async Task Describe_LanguageAnswer_MatchesGolden()
        {
            var model = new LanguageKit.FakeLanguageModel(LanguageKit.Golden("reply-valid.json"));

            Answer answer = await Reasoner(model, new LanguageKit.CapturingLogger<InterpretationParser>()).Ask(LanguageKit.AbortionViolatesLove(), CancellationToken.None);

            Assert.Equal(LanguageKit.Golden("describe-language.txt"), AnswerDescriber.Describe(answer));
        }

        [Fact]
        public void Describe_CanonAnswer_MatchesGolden()
        {
            Query query = new Query(
                new Assertion(CanonKit.Compassion, RelationKind.Upholds, LanguageKit.Mercy),
                new List<Circumstance> { new Circumstance("A neighbor is hungry") });
            Answer answer = new Answer(
                query,
                TruthValue.True,
                1.0,
                new List<ConceptRef>(),
                new Justification(
                    new List<ProofStep> { new ProofStep(1, "Upholds(Compassion, Mercy)", "Compassion is an act of mercy.") },
                    new List<ScriptureReference> { ScriptureReference.Parse("Luke 6:36") },
                    new List<Assertion> { query.Assertion },
                    "Compassion upholds mercy."),
                new Provenance(AnswerSource.Canon, "canon", CanonKit.Now, VerificationStatus.Verified, "canon.test-rule"));

            Assert.Equal(LanguageKit.Golden("describe-canon.txt"), AnswerDescriber.Describe(answer));
        }

        [Fact]
        public void Describe_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AnswerDescriber.Describe(null));
        }
    }
}
