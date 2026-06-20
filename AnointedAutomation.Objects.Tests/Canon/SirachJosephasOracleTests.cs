// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 45 & 46: Sirach (Joshua son of Sirach) and Josephas son of Bengorion (Ethiopic Yosippon).
    public class SirachJosephasOracleTests
    {
        [Fact]
        public void Sirach_MercyAndAlmsgiving_IsRighteous()
        {
            // "Water extinguishes a blazing fire: so almsgiving atones for sin." (Sirach 3:30; 4:1-10)
            OracleHarness.Righteous(OracleHarness.Witness("almsgiving and mercy", new Generosity(), new Compassion()));
        }

        [Fact]
        public void Sirach_TheFearOfTheLord_IsRighteous()
        {
            // "The fear of the Lord is the beginning of wisdom." (Sirach 1:14)
            OracleHarness.Righteous(OracleHarness.Witness("the fear of the Lord", new Humility(), new ObedienceToGod()));
        }

        [Fact]
        public void Josephas_FaithfulnessAcrossHistory_IsRighteous()
        {
            // The Ethiopic history recounts the people kept by God when they were faithful.
            OracleHarness.Righteous(OracleHarness.Witness("faithfulness across history",
                new CovenantFaithfulness(), new ObedienceToGod()));
        }

        [Fact]
        public void Josephas_ApostasyBringsRuin_IsSin()
        {
            // And ruined when they turned to idols and treachery.
            OracleHarness.Sin(OracleHarness.Witness("apostasy brings ruin", new Idolatry(), new Rebellion()));
        }
    }
}
