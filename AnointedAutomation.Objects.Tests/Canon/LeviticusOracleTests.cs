// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 03: Leviticus.
    public class LeviticusOracleTests
    {
        [Fact]
        public void TheAtoningSacrifice_OfferedRightly_IsRighteous()
        {
            // The sin offering points to atonement; offered in faith it is right worship (Leviticus 16-17).
            OracleHarness.Righteous(OracleHarness.Witness("the sin offering", new Atonement(), new Worship()));
        }

        [Fact]
        public void ChildSacrifice_IsGraveSin()
        {
            // "Do not give any of your children to be sacrificed to Molek." (Leviticus 18:21)
            OracleHarness.Sin(OracleHarness.Witness("child sacrifice to Molek", new ChildSacrifice(), new Bloodshed()));
        }

        [Fact]
        public void TheLawProtectsTheVulnerable_IsRighteous()
        {
            // The law that forbids it shields the child (Leviticus 18-20).
            OracleHarness.Righteous(OracleHarness.Witness("the child protected", new HonoringTheVulnerable(), new Protection()));
        }

        [Fact]
        public void JusticeAndProvisionForThePoor_IsRighteous()
        {
            // Gleaning left for the poor and the foreigner; no partiality (Leviticus 19:9-15).
            OracleHarness.Righteous(OracleHarness.Witness("gleaning for the poor", new Generosity(), new JustRecompense()));
        }

        [Fact]
        public void LoveYourNeighbor_IsRighteous()
        {
            // "Love your neighbor as yourself." (Leviticus 19:18)
            OracleHarness.Righteous(OracleHarness.Witness("loving the neighbor", new Compassion()));
        }
    }
}
