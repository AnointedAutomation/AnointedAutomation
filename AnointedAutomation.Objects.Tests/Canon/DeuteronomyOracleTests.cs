// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 05: Deuteronomy.
    public class DeuteronomyOracleTests
    {
        [Fact]
        public void TheShema_LoveTheLord_IsRighteous()
        {
            // "Love the LORD your God with all your heart." (Deuteronomy 6:5)
            OracleHarness.Righteous(OracleHarness.Witness("the Shema", new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void LovingTheForeignerAndTheFatherless_IsRighteous()
        {
            // "He ... loves the foreigner ... and you are to love those who are foreigners." (Deut 10:18-19)
            OracleHarness.Righteous(OracleHarness.Witness("loving the foreigner",
                new Hospitality(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void ObedienceBringsBlessing_IsRighteous()
        {
            // "If you fully obey the LORD your God ... you will be blessed." (Deuteronomy 28:1-2)
            OracleHarness.Righteous(OracleHarness.Witness("Israel keeps the covenant", new ObedienceToGod()));
        }

        [Fact]
        public void TurningToOtherGods_IsSin()
        {
            // The covenant curse falls on apostasy (Deuteronomy 28:15; 30:17-18).
            OracleHarness.Sin(OracleHarness.Witness("turning to other gods", new Idolatry(), new Rebellion()));
        }

        [Fact]
        public void ViolationOfAPerson_IsEquatedWithMurder()
        {
            // "This case is like that of someone who attacks and murders a neighbor." (Deuteronomy 22:25-27)
            Resolution r = OracleHarness.Witness("the violation of a person", new Defilement());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }
    }
}
