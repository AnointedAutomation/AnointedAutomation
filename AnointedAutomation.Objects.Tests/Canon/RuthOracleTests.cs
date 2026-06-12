// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 08: Ruth.
    public class RuthOracleTests
    {
        [Fact]
        public void RuthsLoyalLoveToNaomi_IsRighteous()
        {
            // "Where you go I will go ... your God will be my God." (Ruth 1:16) Loyal love (hesed).
            OracleHarness.Righteous(OracleHarness.Witness("Ruth cleaves to Naomi",
                new Compassion(), new CovenantFaithfulness()));
        }

        [Fact]
        public void BoazRedeemsAndProvidesForTheWidow_IsRighteous()
        {
            // The kinsman-redeemer's kindness to the foreign widow (Ruth 2-4).
            OracleHarness.Righteous(OracleHarness.Witness("Boaz redeems Ruth",
                new Generosity(), new HonoringTheVulnerable()));
        }
    }
}
