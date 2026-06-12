// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 14: Enoch / 1 Enoch (Ethiopic-unique). The decreed cosmic order, its disordering by sin
    // (ch 80), and the judgment of the wicked.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class EnochOracleTests
    {
        [Fact]
        public void TheCoursesOfTheLuminariesKeepTheirLaw_IsADivineAct()
        {
            // The sun, moon, and stars keep the law of their courses (1 Enoch 72-82).
            OracleHarness.DivineAct(OracleHarness.Witness("the courses of the heavens", new CreationOrder()));
        }

        [Fact]
        public void TheWatchersCorruptTheEarth_IsSin()
        {
            // The Watchers teach violence and bloodshed and corrupt the earth (1 Enoch 6-9).
            OracleHarness.Sin(OracleHarness.Witness("the Watchers corrupt the earth", new Bloodshed(), new Rebellion()));
        }

        [Fact]
        public void InTheDaysOfSinners_TheOrderIsDisordered()
        {
            // "The moon shall alter her order ... the stars shall transgress." (1 Enoch 80) Sin warps
            // the very order of reality.
            Resolution r = OracleHarness.Witness("the days of sinners", new Rebellion(), new Idolatry());

            OracleHarness.Sin(r);
            Assert.True(r.Disorder > 0.0);
        }

        [Fact]
        public void TheJudgmentOfTheWicked_IsJust()
        {
            // The Holy One comes to judge the wicked and vindicate the righteous (1 Enoch 1; 91-105).
            OracleHarness.DivineAct(OracleHarness.Witness("the judgment of the wicked",
                new Righteousness(), new JustRecompense()));
        }
    }
}
