// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using AnointedAutomation.Concepts.Reasoning;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Canon
{
    public class CanonFactsTests
    {
        [Fact]
        public void Canon_IsBuiltOnce()
        {
            Assert.Same(CanonFacts.Canon, CanonFacts.Canon);
        }

        [Fact]
        public void Canon_IndexesEveryWitnessableSinWithItsCatalogScripture()
        {
            foreach (var entry in SinCatalog.All)
            {
                if (entry.IsOmitted)
                {
                    continue;
                }

                var subject = ConceptRef.Of(entry.Id);
                Assert.True(CanonFacts.Canon.TryGetConcept(subject, out _), entry.Name);
                Assert.Single(CanonFacts.Canon.About(subject, RelationKind.InCategory));
                var grounded = new HashSet<ScriptureReference>();
                foreach (var fact in CanonFacts.Canon.About(subject, RelationKind.GroundedIn))
                {
                    grounded.UnionWith(fact.Scripture);
                }

                Assert.True(grounded.SetEquals(entry.References), entry.Name);
            }
        }

        [Fact]
        public void Canon_IndexesEveryFacetOfGod()
        {
            foreach (var facet in new[] { CanonKit.Love, CanonKit.Justice, CanonKit.Mercy, CanonKit.Faithfulness })
            {
                Assert.True(CanonFacts.Canon.TryGetAttribute(facet, out _), facet.ToString());
            }
        }

        [Fact]
        public void Canon_EveryFactCitesScripture()
        {
            Assert.NotEmpty(CanonFacts.Canon.All);
            foreach (var fact in CanonFacts.Canon.All)
            {
                Assert.NotEmpty(fact.Scripture);
            }
        }

        [Fact]
        public void About_UnknownSubject_IsEmpty()
        {
            Assert.Empty(CanonFacts.Canon.About(new ConceptRef("Nowhere", "Nothing"), RelationKind.Upholds));
        }

        [Fact]
        public void With_ReturnsANewIndexAndLeavesTheCanonUnchanged()
        {
            var subject = CanonKit.Concept("AddedTestConcept");
            var fact = new CanonFact(new Assertion(subject, RelationKind.Upholds, CanonKit.Love), new[] { new ScriptureReference(BibleBook.John, 13, 34, 0) });
            var extended = CanonFacts.Canon.With(new[] { fact });

            Assert.True(extended.TryGetFact(fact.Assertion, out _));
            Assert.False(CanonFacts.Canon.TryGetFact(fact.Assertion, out _));
            Assert.Equal(CanonFacts.Canon.All.Count + 1, extended.All.Count);
        }

        [Fact]
        public void With_SameAssertionTwice_MergesScripture()
        {
            var assertion = new Assertion(CanonKit.Concept("MergedTestConcept"), RelationKind.Upholds, CanonKit.Love);
            var first = new ScriptureReference(BibleBook.John, 13, 34, 0);
            var second = new ScriptureReference(BibleBook.FirstJohn, 4, 8, 0);
            var extended = CanonFacts.Canon.With(new[]
            {
                new CanonFact(assertion, new[] { first }),
                new CanonFact(assertion, new[] { second, first }),
            });

            Assert.True(extended.TryGetFact(assertion, out var merged));
            Assert.Equal(new[] { first, second }, merged.Scripture);
        }

        [Fact]
        public void With_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => CanonFacts.Canon.With(null));
            Assert.Throws<ArgumentNullException>(() => CanonFacts.Canon.With(new CanonFact[] { null }));
        }

        [Fact]
        public void CanonFact_NeedsScripture()
        {
            var assertion = new Assertion(CanonKit.Compassion, RelationKind.Upholds, CanonKit.Love);
            Assert.Throws<ArgumentException>(() => new CanonFact(assertion, new ScriptureReference[0]));
            Assert.Throws<ArgumentNullException>(() => new CanonFact(null, new[] { new ScriptureReference(BibleBook.Luke, 6, 36, 0) }));
            Assert.Throws<ArgumentNullException>(() => new CanonFact(assertion, null));
        }

        [Fact]
        public void References_ForCategoryAndCoarseSin_UseTheirKinds()
        {
            Assert.Equal(new ConceptRef(CanonFacts.CategoryKind, "Speech"), CanonFacts.Category(SinCategory.Speech));
            Assert.Equal(new ConceptRef(CanonFacts.CoarseSinKind, "Murder"), CanonFacts.CoarseSin(AnointedAutomation.Enums.Sin.Murder));
            Assert.Throws<ArgumentOutOfRangeException>(() => CanonFacts.Category((SinCategory)999));
            Assert.Throws<ArgumentNullException>(() => CanonFacts.Attribute(null));
        }

        [Theory]
        [InlineData("1 Corinthians 13:4", BibleBook.FirstCorinthians, 13, 4, 0)]
        [InlineData("Exodus 21:22-25", BibleBook.Exodus, 21, 22, 25)]
        [InlineData("Psalm 25:21", BibleBook.Psalms, 25, 21, 0)]
        [InlineData("Song of Solomon 2:4", BibleBook.SongOfSolomon, 2, 4, 0)]
        public void ScriptureReference_Parse_ReadsDisplayReferences(string text, BibleBook book, int chapter, int verse, int endVerse)
        {
            Assert.Equal(new ScriptureReference(book, chapter, verse, endVerse), ScriptureReference.Parse(text));
        }

        [Theory]
        [InlineData("")]
        [InlineData("Luke")]
        [InlineData("Luke 6")]
        [InlineData("Hezekiah 1:1")]
        [InlineData("Luke x:36")]
        [InlineData("Luke 6:-1")]
        public void ScriptureReference_Parse_RejectsMalformedText(string text)
        {
            Assert.Throws<FormatException>(() => ScriptureReference.Parse(text));
        }
    }
}
