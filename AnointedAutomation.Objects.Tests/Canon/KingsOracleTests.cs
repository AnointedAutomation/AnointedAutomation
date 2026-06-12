// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 10: 1 & 2 Kings.
    public class KingsOracleTests
    {
        [Fact]
        public void SolomonAsksForWisdom_IsRighteous()
        {
            // He asks for a discerning heart to govern justly, not for riches (1 Kings 3:9).
            OracleHarness.Righteous(OracleHarness.Witness("Solomon asks for wisdom", new Humility(), new Trust()));
        }

        [Fact]
        public void SolomonsIdolatryInOldAge_IsSin()
        {
            // His foreign wives turn his heart after other gods (1 Kings 11:4).
            OracleHarness.Sin(OracleHarness.Witness("Solomon turns to idols", new Idolatry()));
        }

        [Fact]
        public void ElijahCallsIsraelBackToTrueWorship_IsRighteous()
        {
            // "If the LORD is God, follow him." (1 Kings 18:21) on Mount Carmel.
            OracleHarness.Righteous(OracleHarness.Witness("Elijah on Carmel", new Worship(), new CourageousFaith()));
        }

        [Fact]
        public void AhabAndJezebelMurderNabothForHisVineyard_IsGraveSin()
        {
            // Coveting the vineyard, they have Naboth killed (1 Kings 21).
            Resolution r = OracleHarness.Witness("Naboth murdered for his vineyard", new Murder(), new Greed());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }
    }
}
