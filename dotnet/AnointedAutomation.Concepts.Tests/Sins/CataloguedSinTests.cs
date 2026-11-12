// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Linq;
using AnointedAutomation.Concepts;
using AnointedAutomation.Enums;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Sins
{
    public class CataloguedSinTests
    {
        [Fact]
        public void EveryCataloguedSin_WitnessedAlone_ReadsAsSin()
        {
            // Every catalogued sin offends God's character and disorders reality (1 Enoch 80).
            foreach (SinEntry e in SinCatalog.All.Where(x => !x.IsOmitted))
            {
                CataloguedSin sin = SinCatalog.Concept(e.Id);
                Assert.True(sin.Gravity > Gravity.None, e.Name);
                OracleHarness.Sin(OracleHarness.Witness(e.Name, sin));
            }
        }

        [Fact]
        public void Concept_AnswersToTheEntrysScripture()
        {
            CataloguedSin sin = SinCatalog.Concept(SinId.Abortion);
            Assert.Equal("Exodus 21:22-25", sin.Scripture);
            Assert.Equal("Abortion", sin.Name);
            Assert.Same(sin, SinCatalog.Concept(SinId.Abortion));
        }

        [Fact]
        public void Concept_TakesGravityFromItsCoarseVice()
        {
            Assert.Equal(new Murder().Gravity, SinCatalog.Concept(SinId.KillingAPerson).Gravity);
            Assert.Equal(typeof(Murder), SinCatalog.Concept(SinId.KillingAPerson).GroundingVice.GetType());
        }

        [Fact]
        public void Concept_WithoutCoarseSin_UsesItsCategoryVice()
        {
            SinEntry e = SinCatalog.All.First(x => !x.IsOmitted && !x.CoarseSin.HasValue);
            Assert.Equal(CataloguedSin.ViceFor(e.Category.Value).GetType(), SinCatalog.Concept(e.Id).GroundingVice.GetType());
        }

        [Fact]
        public void Concept_CarriesContextSensitivity_FromDeceit()
        {
            // Rahab and the midwives: deceit to protect innocent life is not condemned (Joshua 2; Exodus 1:19-21).
            SinEntry lie = SinCatalog.All.First(x => !x.IsOmitted && x.CoarseSin == Sin.Deceit);
            Act act = new Act("hide the innocent", SinCatalog.Concept(lie.Id)).InContext(new Circumstance("protectingInnocentLife"));
            Assert.False(SinCatalog.Concept(lie.Id).Violates(new Faithfulness(), act));
        }

        [Fact]
        public void EveryCoarseSinAndCategory_HasAVice()
        {
            foreach (Sin s in Enum.GetValues<Sin>())
            {
                Assert.True(CataloguedSin.ViceFor(s).Gravity > Gravity.None, s.ToString());
            }

            foreach (SinCategory c in Enum.GetValues<SinCategory>())
            {
                Assert.True(CataloguedSin.ViceFor(c).Gravity > Gravity.None, c.ToString());
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => CataloguedSin.ViceFor((SinCategory)0u));
        }

        [Fact]
        public void Concept_RejectsOmittedNullAndZero()
        {
            Assert.Throws<KeyNotFoundException>(() => SinCatalog.Concept(SinId.Omitted367));
            Assert.Throws<KeyNotFoundException>(() => SinCatalog.Concept(default));
            Assert.Throws<ArgumentException>(() => new CataloguedSin(SinCatalog.Get(SinId.Omitted367)));
            Assert.Throws<ArgumentNullException>(() => new CataloguedSin(null));
        }
    }
}
