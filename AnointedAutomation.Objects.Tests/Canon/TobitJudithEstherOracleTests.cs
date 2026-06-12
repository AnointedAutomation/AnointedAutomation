// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 17, 18, 19: Tobit, Judith, Esther.
    public class TobitJudithEstherOracleTests
    {
        [Fact]
        public void TobitsAlmsgivingAndBurialOfTheDead_IsRighteous()
        {
            // Tobit risks himself to feed the hungry and bury the dead (Tobit 1:16-17; 4:7-11).
            OracleHarness.Righteous(OracleHarness.Witness("Tobit's almsgiving", new Generosity(), new Compassion()));
        }

        [Fact]
        public void GodsProvidentialHealing_IsADivineAct()
        {
            // God sends Raphael to heal Tobit and deliver Sarah (Tobit 3:16-17; 11).
            OracleHarness.DivineAct(OracleHarness.Witness("God heals Tobit", new Healing(), new Compassion()));
        }

        [Fact]
        public void GodDeliversThroughJudith_DefendsTheWeak()
        {
            // God saves His people from the besieging oppressor through Judith's faith (Judith 8-13).
            OracleHarness.DivineAct(OracleHarness.Witness("the deliverance through Judith",
                new Deliverance(), new DefendingTheWeak()));
        }

        [Fact]
        public void EstherDeliversHerPeopleFromGenocide_IsRighteous()
        {
            // "If I perish, I perish." (Esther 4:16) Courage to save her people.
            OracleHarness.Righteous(OracleHarness.Witness("Esther pleads for her people",
                new CourageousFaith(), new Protection()));
        }

        [Fact]
        public void HamansGenocidalPlot_IsGraveSin()
        {
            // Haman plots to destroy all the Jews out of wounded pride (Esther 3).
            Resolution r = OracleHarness.Witness("Haman's plot to destroy a people", new Murder(), new Pride());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }
    }
}
