// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 11 & 12: 1 and 2 Chronicles.
    public class ChroniclesOracleTests
    {
        [Fact]
        public void DavidOrdersWholeheartedWorship_IsRighteous()
        {
            // David sets the worship of the LORD in order and gives wholeheartedly (1 Chronicles 16; 29).
            OracleHarness.Righteous(OracleHarness.Witness("David orders the worship", new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void DavidsPridefulCensus_IsSin()
        {
            // Numbering the fighting men in pride, trusting strength over God (1 Chronicles 21).
            OracleHarness.Sin(OracleHarness.Witness("David's prideful census", new Pride()));
        }

        [Fact]
        public void IfMyPeopleHumbleThemselves_IsRighteous()
        {
            // "If my people ... will humble themselves and pray ... I will forgive." (2 Chronicles 7:14)
            OracleHarness.Righteous(OracleHarness.Witness("the people humble themselves", new Repentance(), new Humility()));
        }

        [Fact]
        public void ApostasyUnderWickedKings_IsSin()
        {
            // Manasseh and others lead Judah into idolatry and bloodshed (2 Chronicles 33).
            OracleHarness.Sin(OracleHarness.Witness("Judah's apostasy", new Idolatry(), new Rebellion()));
        }
    }
}
