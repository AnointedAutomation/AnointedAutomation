// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Concepts.Reasoning;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning
{
    public class ReasoningContractsTests
    {
        private static readonly System.DateTimeOffset Now = new System.DateTimeOffset(2026, 11, 12, 0, 0, 0, System.TimeSpan.Zero);

        private static Assertion AbortionViolatesJustice()
        {
            return new Assertion(ConceptRef.Of(SinId.Abortion), RelationKind.Violates, new ConceptRef("DivineAttribute", "Justice"));
        }

        private static Query AnyQuery()
        {
            return new Query(AbortionViolatesJustice(), new List<Circumstance> { new Circumstance("Duress") });
        }

        private static Justification AnyJustification()
        {
            return new Justification(
                new List<ProofStep> { new ProofStep(1, "A -> B", "A implies B.") },
                new List<ScriptureReference> { new ScriptureReference(BibleBook.FirstCorinthians, 6, 18, 0) },
                new List<Assertion> { AbortionViolatesJustice() },
                "Because the canon says so.");
        }

        private static Provenance AnyProvenance()
        {
            return new Provenance(AnswerSource.Canon, "canon-engine@1", Now, VerificationStatus.Verified, "rule:sin-violates");
        }

        [Fact]
        public void ConceptRef_OfSinIdAndOfCataloguedSin_NameTheSameConcept()
        {
            ConceptRef byId = ConceptRef.Of(SinId.Abortion);
            ConceptRef byConcept = ConceptRef.Of(SinCatalog.Concept(SinId.Abortion));

            Assert.Equal(byId, byConcept);
            Assert.Equal(ConceptRef.SinKind, byId.Kind);
            Assert.Equal("Abortion", byId.Id);
        }

        [Fact]
        public void ConceptRef_OfMoralConcept_UsesItsName()
        {
            Compassion compassion = new Compassion();
            ConceptRef reference = ConceptRef.Of(compassion);

            Assert.Equal(ConceptRef.MoralConceptKind, reference.Kind);
            Assert.Equal(compassion.Name, reference.Id);
        }

        [Fact]
        public void ConceptRef_RejectsMissingParts()
        {
            Assert.Throws<System.ArgumentException>(() => new ConceptRef(" ", "id"));
            Assert.Throws<System.ArgumentException>(() => new ConceptRef("kind", null));
            Assert.Throws<System.ArgumentNullException>(() => ConceptRef.Of((MoralConcept)null));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => ConceptRef.Of((SinId)999999));
        }

        [Fact]
        public void Assertion_RequiresSubjectRelationAndObject()
        {
            ConceptRef sin = ConceptRef.Of(SinId.Abortion);

            Assert.Throws<System.ArgumentNullException>(() => new Assertion(null, RelationKind.Violates, sin));
            Assert.Throws<System.ArgumentNullException>(() => new Assertion(sin, RelationKind.Violates, null));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Assertion(sin, RelationKind.None, sin));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new Assertion(sin, (RelationKind)42, sin));
            Assert.Equal(AbortionViolatesJustice(), AbortionViolatesJustice());
        }

        [Fact]
        public void Query_RequiresAssertionAndContext_AndCopiesContext()
        {
            List<Circumstance> context = new List<Circumstance> { new Circumstance("Duress") };
            Query query = new Query(AbortionViolatesJustice(), context);
            context.Add(new Circumstance("Ignorance"));

            Assert.Single(query.Context);
            Assert.Throws<System.ArgumentNullException>(() => new Query(null, new List<Circumstance>()));
            Assert.Throws<System.ArgumentNullException>(() => new Query(AbortionViolatesJustice(), null));
            Assert.Throws<System.ArgumentException>(() => new Query(AbortionViolatesJustice(), new List<Circumstance> { null }));
        }

        [Fact]
        public void Justification_RequiresListsAndExplanation()
        {
            List<ProofStep> steps = new List<ProofStep>();
            List<ScriptureReference> scripture = new List<ScriptureReference>();
            List<Assertion> facts = new List<Assertion>();

            Assert.Equal("Why.", new Justification(steps, scripture, facts, "Why.").Explanation);
            Assert.Throws<System.ArgumentNullException>(() => new Justification(null, scripture, facts, "Why."));
            Assert.Throws<System.ArgumentNullException>(() => new Justification(steps, null, facts, "Why."));
            Assert.Throws<System.ArgumentNullException>(() => new Justification(steps, scripture, null, "Why."));
            Assert.Throws<System.ArgumentException>(() => new Justification(steps, scripture, facts, " "));
        }

        [Fact]
        public void Provenance_RequiresEveryField()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new Provenance(AnswerSource.Unknown, "m", Now, VerificationStatus.Verified, "r"));
            Assert.Throws<System.ArgumentException>(
                () => new Provenance(AnswerSource.Language, " ", Now, VerificationStatus.Unverified, "r"));
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new Provenance(AnswerSource.Language, "m", default(System.DateTimeOffset), VerificationStatus.Unverified, "r"));
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new Provenance(AnswerSource.Language, "m", Now, VerificationStatus.Unknown, "r"));
            Assert.Throws<System.ArgumentException>(
                () => new Provenance(AnswerSource.Language, "m", Now, VerificationStatus.Rejected, null));
        }

        [Fact]
        public void Answer_RequiresEveryField_AndKeepsUnknownAsUnknown()
        {
            Answer answer = new Answer(AnyQuery(), TruthValue.Unknown, 0.4, new List<ConceptRef>(), AnyJustification(), AnyProvenance());

            Assert.Equal(TruthValue.Unknown, answer.Truth);
            Assert.Throws<System.ArgumentNullException>(
                () => new Answer(null, TruthValue.True, 0.4, new List<ConceptRef>(), AnyJustification(), AnyProvenance()));
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new Answer(AnyQuery(), (TruthValue)9, 0.4, new List<ConceptRef>(), AnyJustification(), AnyProvenance()));
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new Answer(AnyQuery(), TruthValue.True, 1.5, new List<ConceptRef>(), AnyJustification(), AnyProvenance()));
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new Answer(AnyQuery(), TruthValue.True, double.NaN, new List<ConceptRef>(), AnyJustification(), AnyProvenance()));
            Assert.Throws<System.ArgumentNullException>(
                () => new Answer(AnyQuery(), TruthValue.True, 0.4, null, AnyJustification(), AnyProvenance()));
            Assert.Throws<System.ArgumentNullException>(
                () => new Answer(AnyQuery(), TruthValue.True, 0.4, new List<ConceptRef>(), null, AnyProvenance()));
            Assert.Throws<System.ArgumentNullException>(
                () => new Answer(AnyQuery(), TruthValue.True, 0.4, new List<ConceptRef>(), AnyJustification(), null));
        }

        [Fact]
        public void UnsetEnumMembers_AreZero()
        {
            Assert.Equal(0, (int)TruthValue.Unknown);
            Assert.Equal(0, (int)RelationKind.None);
            Assert.Equal(0, (int)AnswerSource.Unknown);
            Assert.Equal(0, (int)VerificationStatus.Unknown);
        }
    }
}
