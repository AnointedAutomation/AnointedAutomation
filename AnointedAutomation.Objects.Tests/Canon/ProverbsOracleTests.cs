// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 24: Proverbs.
    public class ProverbsOracleTests
    {
        [Fact]
        public void TheFearOfTheLordIsTheBeginningOfWisdom_IsRighteous()
        {
            // "The fear of the LORD is the beginning of knowledge." (Proverbs 1:7; 9:10)
            OracleHarness.Righteous(OracleHarness.Witness("the fear of the LORD", new Humility(), new ObedienceToGod()));
        }

        [Fact]
        public void TheSevenThingsTheLordHates_AreGraveSin()
        {
            // Haughty eyes, a lying tongue, and hands that shed innocent blood (Proverbs 6:16-19).
            Resolution r = OracleHarness.Witness("the things the LORD hates", new Bloodshed(), new Pride(), new Deceit());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void KindnessToThePoor_IsRighteous()
        {
            // "Whoever is kind to the poor lends to the LORD." (Proverbs 19:17)
            OracleHarness.Righteous(OracleHarness.Witness("kindness to the poor", new Generosity()));
        }
    }
}
