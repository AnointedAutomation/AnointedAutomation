// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Song of Songs narrative oracle: the engine must return the verdict Scripture gives at each step.
    // The Song celebrates covenant marital love between the bride (the Shulammite) and her beloved,
    // read by the Church as a figure of the love between Christ and His bride. Its delights are pure,
    // and its one warning is against rousing love out of its God-appointed order.
    public class SongOfSongsNarrativeTests
    {
        [Fact]
        public void BrideLongingForTheKissesOfHerBeloved_IsRighteous()
        {
            // "Let him kiss me with the kisses of his mouth! For your love is better than wine." (Song 1:2)
            OracleHarness.Righteous(OracleHarness.Witness("the bride yearns purely for her beloved's love", new Purity(), new Joy(), new CovenantFaithfulness()));
        }

        [Fact]
        public void BelovedDrawsHisBrideAndSheRejoices_IsRighteous()
        {
            // "Draw me after you; let us run... we will exult and rejoice in you." (Song 1:4)
            OracleHarness.Righteous(OracleHarness.Witness("the beloved draws his bride and they rejoice together", new Joy(), new Kindness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheBelovedPraisesHisBrideAsAltogetherFair_IsRighteous()
        {
            // "Behold, you are beautiful, my love... there is no flaw in you." (Song 1:15; 4:7)
            OracleHarness.Righteous(OracleHarness.Witness("the beloved honors his bride as wholly fair and flawless", new Kindness(), new HonoringTheVulnerable(), new RejoicingInTruth()));
        }

        [Fact]
        public void HisBannerOverHerWasLove_IsRighteous()
        {
            // "He brought me to the banqueting house, and his banner over me was love." (Song 2:4)
            OracleHarness.Righteous(OracleHarness.Witness("the beloved shelters his bride under the banner of love", new Protection(), new Kindness(), new Gentleness()));
        }

        [Fact]
        public void TheChargeNotToStirUpLoveUntilItPleases_IsRighteous()
        {
            // "I adjure you... that you not stir up or awaken love until it pleases." (Song 2:7; 3:5; 8:4)
            OracleHarness.Righteous(OracleHarness.Witness("the daughters of Jerusalem are charged to keep love in its appointed order", new SelfControl(), new Purity(), new CreationOrder()));
        }

        [Fact]
        public void TheCatchingOfTheLittleFoxesThatSpoilTheVineyard_IsRighteous()
        {
            // "Catch the foxes for us, the little foxes that spoil the vineyards." (Song 2:15)
            OracleHarness.Righteous(OracleHarness.Witness("guarding the vineyard of love from the little foxes that would spoil it", new Protection(), new SelfControl(), new Fidelity()));
        }

        [Fact]
        public void MyBelovedIsMineAndIAmHis_IsRighteous()
        {
            // "My beloved is mine, and I am his; he grazes among the lilies." (Song 2:16; 6:3)
            OracleHarness.Righteous(OracleHarness.Witness("the bride and beloved belong wholly to one another", new CovenantFaithfulness(), new Fidelity(), new Joy()));
        }

        [Fact]
        public void TheBrideSeeksHerBelovedThroughTheCityUntilSheFindsHim_IsRighteous()
        {
            // "I will seek him whom my soul loves... I found him whom my soul loves." (Song 3:1-4)
            OracleHarness.Righteous(OracleHarness.Witness("the bride seeks her beloved through the night until she finds him", new Endurance(), new CovenantFaithfulness(), new Patience()));
        }

        [Fact]
        public void TheWatchmenBeatAndWoundTheSeekingBride_IsSin()
        {
            // "The watchmen found me... they beat me, they bruised me, they took away my veil." (Song 5:7)
            OracleHarness.Sin(OracleHarness.Witness("the city watchmen strike and wound the bride who seeks her beloved", new Oppression(), new Wrongdoing(), new Rudeness()));
        }

        [Fact]
        public void ABridePerfectAndAGardenLockedFountainSealed_IsRighteous()
        {
            // "A garden locked is my sister, my bride, a spring locked, a fountain sealed." (Song 4:12)
            OracleHarness.Righteous(OracleHarness.Witness("the bride kept as a garden locked and a fountain sealed", new Purity(), new SelfControl(), new Fidelity()));
        }

        [Fact]
        public void LoveIsStrongAsDeathAndManyWatersCannotQuenchIt_IsRighteous()
        {
            // "Set me as a seal upon your heart... for love is strong as death... Many waters cannot quench love." (Song 8:6-7)
            OracleHarness.Righteous(OracleHarness.Witness("covenant love proves strong as death and unquenchable", new CovenantFaithfulness(), new Endurance(), new SelfSacrifice()));
        }

        [Fact]
        public void TheManWhoWouldBuyLoveWithAllHisWealthIsUtterlyScorned_IsSin()
        {
            // "If a man offered for love all the wealth of his house, he would be utterly despised." (Song 8:7)
            OracleHarness.Sin(OracleHarness.Witness("a man tries to purchase covenant love with the wealth of his house", new Greed(), new Covetousness()));
        }
    }
}
