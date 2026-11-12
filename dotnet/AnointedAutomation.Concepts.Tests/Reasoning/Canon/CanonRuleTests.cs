// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Concepts.Reasoning;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Canon
{
    public class CanonRuleTests
    {
        private static readonly CanonFacts Facts = CanonFacts.Canon;

        private static TruthValue Apply(ICanonRule rule, Query query, out ProofStep[] steps, out CanonFact[] grounds)
        {
            return rule.Apply(query, Facts, out steps, out grounds);
        }

        private static void AssertUnknown(ICanonRule rule, Query query)
        {
            Assert.Equal(TruthValue.Unknown, Apply(rule, query, out var steps, out var grounds));
            Assert.Empty(steps);
            Assert.Empty(grounds);
        }

        private static void AssertEstablished(TruthValue expected, ICanonRule rule, Query query)
        {
            Assert.Equal(expected, Apply(rule, query, out var steps, out var grounds));
            Assert.NotEmpty(steps);
            Assert.NotEmpty(grounds);
        }

        [Fact]
        public void FactRule_StatedUpholds_IsTrueAndCitesTheConceptsScripture()
        {
            var rule = new FactRule(RelationKind.Upholds);
            Assert.Equal(TruthValue.True, Apply(rule, CanonKit.Ask(CanonKit.Compassion, RelationKind.Upholds, CanonKit.Love), out _, out var grounds));
            Assert.Equal(new ScriptureReference(BibleBook.Luke, 6, 36, 0), Assert.Single(grounds).Scripture[0]);
        }

        [Fact]
        public void FactRule_UpholdingWhatGodForbids_IsFalse()
        {
            AssertEstablished(TruthValue.False, new FactRule(RelationKind.Upholds), CanonKit.Ask(CanonKit.Murder, RelationKind.Upholds, CanonKit.Love));
        }

        [Fact]
        public void FactRule_UnstatedUpholds_IsUnknown()
        {
            AssertUnknown(new FactRule(RelationKind.Upholds), CanonKit.Ask(CanonKit.Compassion, RelationKind.Upholds, CanonKit.Justice));
        }

        [Fact]
        public void FactRule_ViolatesHasNoContrary_SoAnUnstatedViolationIsUnknownNotFalse()
        {
            AssertUnknown(new FactRule(RelationKind.Violates), CanonKit.Ask(CanonKit.Compassion, RelationKind.Violates, CanonKit.Love));
            AssertEstablished(TruthValue.True, new FactRule(RelationKind.Violates), CanonKit.Ask(CanonKit.Murder, RelationKind.Violates, CanonKit.Justice));
        }

        [Fact]
        public void FactRule_CommandsAndForbidsAreContraries()
        {
            AssertEstablished(TruthValue.True, new FactRule(RelationKind.Commands), CanonKit.Ask(CanonKit.Love, RelationKind.Commands, CanonKit.Compassion));
            AssertEstablished(TruthValue.False, new FactRule(RelationKind.Commands), CanonKit.Ask(CanonKit.Love, RelationKind.Commands, CanonKit.Murder));
            AssertEstablished(TruthValue.True, new FactRule(RelationKind.Forbids), CanonKit.Ask(CanonKit.Love, RelationKind.Forbids, CanonKit.Murder));
            AssertEstablished(TruthValue.False, new FactRule(RelationKind.Forbids), CanonKit.Ask(CanonKit.Love, RelationKind.Forbids, CanonKit.Compassion));
        }

        [Fact]
        public void FactRule_StatedAndContradicted_IsUnknown()
        {
            var mixed = CanonKit.Concept("MixedTestConcept");
            var reference = new[] { new ScriptureReference(BibleBook.John, 3, 16, 0) };
            var facts = Facts.With(new[]
            {
                new CanonFact(new Assertion(mixed, RelationKind.Upholds, CanonKit.Love), reference),
                new CanonFact(new Assertion(CanonKit.Love, RelationKind.Forbids, mixed), reference),
            });
            Assert.Equal(TruthValue.Unknown, new FactRule(RelationKind.Upholds).Apply(CanonKit.Ask(mixed, RelationKind.Upholds, CanonKit.Love), facts, out _, out _));
        }

        [Fact]
        public void FactRule_RejectsTheUnsetRelation()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FactRule(RelationKind.None));
        }

        [Fact]
        public void FactRule_IdNamesTheRelation()
        {
            Assert.Equal("canon.fact.forbids", new FactRule(RelationKind.Forbids).Id);
        }

        [Fact]
        public void SinViolatesRule_SinViolatingAFacet_IsTrueAndCitesTheCatalog()
        {
            var rule = new SinViolatesRule();
            Assert.Equal(RelationKind.Violates, rule.Relation);
            Assert.Equal(TruthValue.True, Apply(rule, CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Justice), out var steps, out var grounds));
            Assert.Equal(3, steps.Length);
            var cited = new List<ScriptureReference>();
            foreach (var fact in grounds)
            {
                cited.AddRange(fact.Scripture);
            }

            Assert.Contains(new ScriptureReference(BibleBook.Exodus, 21, 22, 25), cited);
        }

        [Fact]
        public void SinViolatesRule_FacetTheSinDoesNotViolate_IsUnknownNotFalse()
        {
            AssertUnknown(new SinViolatesRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Mercy));
        }

        [Fact]
        public void SinViolatesRule_NonSinSubjectOrUnknownFacet_IsUnknown()
        {
            AssertUnknown(new SinViolatesRule(), CanonKit.Ask(CanonKit.Murder, RelationKind.Violates, CanonKit.Justice));
            AssertUnknown(new SinViolatesRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, new ConceptRef(CanonFacts.AttributeKind, "Wrath")));
        }

        [Fact]
        public void CategoryRule_TheSinsCategory_IsTrue()
        {
            AssertEstablished(TruthValue.True, new CategoryRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.InCategory, CanonFacts.Category(SinCategory.AngerAndViolence)));
        }

        [Fact]
        public void CategoryRule_AnotherCategory_IsFalseBecauseASinHasOneCategory()
        {
            AssertEstablished(TruthValue.False, new CategoryRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.InCategory, CanonFacts.Category(SinCategory.Sexual)));
        }

        [Fact]
        public void CategoryRule_NonCategoryObjectOrUncataloguedSubject_IsUnknown()
        {
            AssertUnknown(new CategoryRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.InCategory, CanonKit.Love));
            AssertUnknown(new CategoryRule(), CanonKit.Ask(CanonKit.Murder, RelationKind.InCategory, CanonFacts.Category(SinCategory.AngerAndViolence)));
        }

        [Fact]
        public void CategoryRule_StatedCategoryOfAnotherSubject_IsTrue()
        {
            var subject = CanonKit.Concept("CategorisedTestConcept");
            var category = CanonFacts.Category(SinCategory.Speech);
            var facts = Facts.With(new[] { new CanonFact(new Assertion(subject, RelationKind.InCategory, category), new[] { new ScriptureReference(BibleBook.James, 3, 6, 0) }) });
            Assert.Equal(TruthValue.True, new CategoryRule().Apply(CanonKit.Ask(subject, RelationKind.InCategory, category), facts, out _, out var grounds));
            Assert.Single(grounds);
        }

        [Fact]
        public void IsInstanceOfRule_CoarseSinAndGroundingVice_AreTrue()
        {
            AssertEstablished(TruthValue.True, new IsInstanceOfRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.IsInstanceOf, CanonFacts.CoarseSin(AnointedAutomation.Enums.Sin.Murder)));
            AssertEstablished(TruthValue.True, new IsInstanceOfRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.IsInstanceOf, CanonKit.Murder));
        }

        [Fact]
        public void IsInstanceOfRule_AnotherCoarseSin_IsFalse()
        {
            AssertEstablished(TruthValue.False, new IsInstanceOfRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.IsInstanceOf, CanonFacts.CoarseSin(AnointedAutomation.Enums.Sin.Theft)));
        }

        [Fact]
        public void IsInstanceOfRule_UnstatedNonCoarseOrSinWithoutCoarse_IsUnknown()
        {
            AssertUnknown(new IsInstanceOfRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.IsInstanceOf, CanonKit.Theft));
            var noCoarse = ConceptRef.Of(SinId.NotAbstainingFromAllAppearanceOfEvil);
            AssertUnknown(new IsInstanceOfRule(), CanonKit.Ask(noCoarse, RelationKind.IsInstanceOf, CanonFacts.CoarseSin(AnointedAutomation.Enums.Sin.Murder)));
        }

        [Fact]
        public void WitnessRule_RighteousConcept_IsTrue()
        {
            AssertEstablished(TruthValue.True, new WitnessRule(), CanonKit.Ask(CanonKit.Compassion, RelationKind.Upholds, CanonFacts.RevealedCharacter));
        }

        [Fact]
        public void WitnessRule_ViceAndCataloguedSin_AreFalse()
        {
            AssertEstablished(TruthValue.False, new WitnessRule(), CanonKit.Ask(CanonKit.Theft, RelationKind.Upholds, CanonFacts.RevealedCharacter));
            AssertEstablished(TruthValue.False, new WitnessRule(), CanonKit.Ask(CanonKit.Abortion, RelationKind.Upholds, CanonFacts.RevealedCharacter));
        }

        [Fact]
        public void WitnessRule_OtherObjectOrUnknownSubject_IsUnknown()
        {
            AssertUnknown(new WitnessRule(), CanonKit.Ask(CanonKit.Compassion, RelationKind.Upholds, CanonKit.Love));
            AssertUnknown(new WitnessRule(), CanonKit.Ask(CanonKit.Concept("NotACanonConcept"), RelationKind.Upholds, CanonFacts.RevealedCharacter));
        }

        [Fact]
        public void Rules_RejectNullArguments()
        {
            foreach (var rule in CanonRules.Default)
            {
                Assert.Throws<ArgumentNullException>(() => rule.Apply(null, Facts, out _, out _));
                Assert.Throws<ArgumentNullException>(() => rule.Apply(CanonKit.Ask(CanonKit.Abortion, rule.Relation, CanonKit.Love), null, out _, out _));
            }
        }
    }
}
