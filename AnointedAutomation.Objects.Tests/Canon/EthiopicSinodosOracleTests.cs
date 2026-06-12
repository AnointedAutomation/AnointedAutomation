// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 74-81: the Ethiopic broader New Testament (Sinodos and related): Sirate Tsion, Tizaz,
    // Gitsew, Abtilis, the two Books of the Covenant (Dominos), Clement, and Didascalia. These are
    // books of church order and apostolic teaching; their calling is holiness, love, justice, and
    // mercy to the penitent.
    public class EthiopicSinodosOracleTests
    {
        [Fact]
        public void SirateTsion_RightOrderAndHolyWorship_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("the order of Zion", new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void Tizaz_TheCommandToLoveGodAndNeighbor_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("love God and neighbor", new Compassion(), new ObedienceToGod()));
        }

        [Fact]
        public void Gitsew_RestoringTheErringInMercy_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("restoring the erring", new Forgiveness(), new Repentance()));
        }

        [Fact]
        public void Abtilis_CharityToTheNeedy_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("charity to the needy", new Generosity(), new Compassion()));
        }

        [Fact]
        public void FirstDominos_TheLordsTeachingAndMercy_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("the Book of the Covenant", new Worship(), new Compassion()));
        }

        [Fact]
        public void SecondDominos_FaithfulnessAndHopeOfHisComing_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("faithfulness and hope", new CovenantFaithfulness(), new Trust()));
        }

        [Fact]
        public void Clement_OrderRepentanceAndCharity_IsRighteous()
        {
            OracleHarness.Righteous(OracleHarness.Witness("apostolic order via Clement", new Repentance(), new Generosity()));
        }

        [Fact]
        public void Didascalia_MercyToThePenitentAndCareForOrphans_IsRighteous()
        {
            // The Didascalia urges receiving the penitent with mercy and caring for the orphan.
            OracleHarness.Righteous(OracleHarness.Witness("mercy to the penitent, care for orphans",
                new Compassion(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void Didascalia_RefusingMercyToThePenitent_IsSin()
        {
            // To crush the contrite rather than restore them offends the mercy of God.
            OracleHarness.Sin(OracleHarness.Witness("refusing mercy to the penitent", new Condemnation()));
        }
    }
}
