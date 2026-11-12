// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AnointedAutomation.Concepts;
using AnointedAutomation.Enums;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Sins
{
    public class SinCatalogTests
    {
        private static readonly Regex ReferenceFormat = new Regex(@"^[A-Za-z]+ \d+:\d+(-\d+)?$");

        [Fact]
        public void Catalog_Has667NumbersPlusTheDuplicate566()
        {
            var numbers = SinCatalog.All.Select(e => e.DocumentNumber).ToList();
            Assert.Equal(668, SinCatalog.All.Count);
            Assert.Equal(Enumerable.Range(1, 667), numbers.Distinct().OrderBy(n => n));
            Assert.Equal(2, numbers.Count(n => n == 566));
        }

        [Fact]
        public void OnlyNumber367_IsOmitted()
        {
            var omitted = SinCatalog.All.Where(e => e.IsOmitted).ToList();
            Assert.Single(omitted);
            Assert.Equal(367, omitted[0].DocumentNumber);
            Assert.Equal(SinId.Omitted367, omitted[0].Id);
            Assert.Empty(omitted[0].References);
            Assert.NotEqual(string.Empty, omitted[0].Note);
        }

        [Fact]
        public void SecondEntry566_HasDisambiguatedId668AndANote()
        {
            var twins = SinCatalog.All.Where(e => e.DocumentNumber == 566).ToList();
            Assert.Equal(566u, (uint)twins[0].Id);
            Assert.Equal(668u, (uint)twins[1].Id);
            Assert.NotEqual(string.Empty, twins[1].Note);
        }

        [Fact]
        public void Number63_IsANormalEntry()
        {
            SinEntry e = SinCatalog.Get((SinId)63u);
            Assert.False(e.IsOmitted);
            Assert.NotEmpty(e.References);
        }

        [Fact]
        public void EveryIdIsUniqueAndMatchesItsNumber_ExceptTheDuplicate()
        {
            Assert.Equal(SinCatalog.All.Count, SinCatalog.All.Select(e => e.Id).Distinct().Count());
            foreach (SinEntry e in SinCatalog.All.Where(x => x.Id != (SinId)668u))
            {
                Assert.Equal((uint)e.DocumentNumber, (uint)e.Id);
            }
        }

        [Fact]
        public void EverySinIdValue_IsCatalogued_AndUnknownIsZero()
        {
            Assert.Equal(0u, (uint)SinId.Unknown);
            foreach (SinId id in Enum.GetValues<SinId>().Where(i => i != SinId.Unknown))
            {
                Assert.True(SinCatalog.TryGet(id, out _), id + " is not catalogued");
            }

            Assert.Equal(SinCatalog.All.Count, Enum.GetValues<SinId>().Length - 1);
        }

        [Fact]
        public void EveryNonOmittedEntry_HasScriptureNameAndValidCategory()
        {
            foreach (SinEntry e in SinCatalog.All.Where(x => !x.IsOmitted))
            {
                Assert.NotEmpty(e.References);
                Assert.False(string.IsNullOrWhiteSpace(e.Name));
                Assert.NotEqual(SinCategory.Unknown, e.Category);
                Assert.True(Enum.IsDefined(e.Category), e.Name);
                if (e.CoarseSin.HasValue)
                {
                    Assert.True(Enum.IsDefined(e.CoarseSin.Value), e.Name);
                }
            }
        }

        [Fact]
        public void EveryReference_IsWellFormedAndWithinItsBook()
        {
            foreach (ScriptureReference r in SinCatalog.All.SelectMany(e => e.References))
            {
                Assert.NotEqual(BibleBook.Unknown, r.Book);
                Assert.InRange(r.Chapter, 1, ScriptureReference.ChapterCount(r.Book));
                Assert.True(r.Verse >= 1);
                Assert.True(r.EndVerse == 0 || r.EndVerse > r.Verse);
                Assert.Matches(ReferenceFormat, r.ToString());
            }
        }

        [Fact]
        public void BibleBook_Has66BooksAndUnknownZero()
        {
            Assert.Equal(67, Enum.GetValues<BibleBook>().Length);
            Assert.Equal(0u, (uint)BibleBook.Unknown);
            Assert.Equal(22, ScriptureReference.ChapterCount(BibleBook.Revelation));
        }

        [Fact]
        public void Lookups_ByCategoryAndCoarseSin_AreConsistent()
        {
            foreach (SinCategory c in Enum.GetValues<SinCategory>().Where(x => x != SinCategory.Unknown))
            {
                Assert.All(SinCatalog.InCategory(c), e => Assert.Equal(c, e.Category));
            }

            Assert.Empty(SinCatalog.InCategory(SinCategory.Unknown));
            int categorized = Enum.GetValues<SinCategory>().Sum(c => SinCatalog.InCategory(c).Count);
            Assert.Equal(SinCatalog.All.Count(e => !e.IsOmitted), categorized);
            foreach (Sin s in Enum.GetValues<Sin>())
            {
                Assert.All(SinCatalog.UnderCoarseSin(s), e => Assert.Equal(s, e.CoarseSin.Value));
            }

            Assert.Contains(SinCatalog.Get(SinId.Adultery), SinCatalog.UnderCoarseSin(Sin.SexualImmorality));
        }

        [Fact]
        public void Get_UnknownOrMissingId_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() => SinCatalog.Get(SinId.Unknown));
            Assert.Throws<KeyNotFoundException>(() => SinCatalog.Get((SinId)9999u));
            Assert.False(SinCatalog.TryGet((SinId)9999u, out _));
        }

        [Fact]
        public void ScriptureReference_RejectsBadParts()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScriptureReference(BibleBook.Unknown, 1, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScriptureReference(BibleBook.Jude, 2, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScriptureReference(BibleBook.Genesis, 1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScriptureReference(BibleBook.Genesis, 1, 5, 5));
            Assert.Equal("Exodus 21:22-25", new ScriptureReference(BibleBook.Exodus, 21, 22, 25).ToString());
        }

        [Fact]
        public void SinEntry_RejectsMissingScriptureCategoryOrId()
        {
            Assert.Throws<ArgumentException>(() => new SinEntry(SinId.Abortion, 1, "x", SinCategory.Sexual, null, ""));
            Assert.Throws<ArgumentException>(() => new SinEntry(SinId.Abortion, 1, "x", SinCategory.Unknown, null, "", new ScriptureReference(BibleBook.Genesis, 1, 1, 0)));
            Assert.Throws<ArgumentException>(() => new SinEntry(SinId.Unknown, 1, "x", SinCategory.Sexual, null, "", new ScriptureReference(BibleBook.Genesis, 1, 1, 0)));
        }

        private sealed class Recorder : ISinConsequence<List<SinId>>
        {
            public void Apply(SinEntry sin, List<SinId> target) => target.Add(sin.Id);
        }

        [Fact]
        public void ConsequenceRegistry_AppliesRegisteredConsequencesInOrder()
        {
            var registry = new SinConsequenceRegistry<List<SinId>>();
            registry.Register(SinId.Adultery, new Recorder());
            registry.Register(SinId.Adultery, new Recorder());
            var log = new List<SinId>();
            Assert.Equal(2, registry.Apply(SinId.Adultery, log));
            Assert.Equal(new[] { SinId.Adultery, SinId.Adultery }, log);
            Assert.Equal(0, registry.Apply(SinId.Abortion, log));
            Assert.Empty(registry.For(SinId.Abortion));
        }

        [Fact]
        public void ConsequenceRegistry_RejectsNullOmittedAndUnknown()
        {
            var registry = new SinConsequenceRegistry<List<SinId>>();
            Assert.Throws<ArgumentNullException>(() => registry.Register(SinId.Adultery, null));
            Assert.Throws<ArgumentException>(() => registry.Register(SinId.Omitted367, new Recorder()));
            Assert.Throws<KeyNotFoundException>(() => registry.Register(SinId.Unknown, new Recorder()));
            Assert.Throws<KeyNotFoundException>(() => registry.Apply(SinId.Unknown, new List<SinId>()));
        }
    }
}
