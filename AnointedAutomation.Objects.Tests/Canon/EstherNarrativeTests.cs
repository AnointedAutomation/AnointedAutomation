// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Esther narrative oracle (Eastern-Orthodox / Greek Esther, including the canonical Additions):
    // the engine must return the verdict Scripture gives at each step.
    public class EstherNarrativeTests
    {
        [Fact]
        public void MordecaiExposesTheAssassinationPlotAgainstTheKing_IsRighteous()
        {
            // Mordecai learns of Bigthan and Teresh's plot and discloses it through Esther, saving the king (Esther 2:21-23).
            OracleHarness.Righteous(OracleHarness.Witness("Mordecai exposes the plot and protects the king's life",
                new Protection(), new Trustworthiness()));
        }

        [Fact]
        public void MordecaiRefusesToBowToHaman_IsRighteous()
        {
            // Mordecai will not bow or do reverence to Haman, keeping faith though it costs him (Esther 3:2-4).
            OracleHarness.Righteous(OracleHarness.Witness("Mordecai refuses to bow to Haman",
                new CourageousFaith(), new Fidelity()));
        }

        [Fact]
        public void HamansGenocidalEdictToDestroyTheJews_IsSin()
        {
            // Haman, enraged, plots to destroy, slay, and annihilate all the Jews in one day (Esther 3:6-13).
            Resolution r = OracleHarness.Witness("Haman's edict to destroy all the Jews",
                new Murder(), new Bloodshed(), new WickedScheming(), new Hatred());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void HamansPrideAndContemptForMordecai_IsSin()
        {
            // Full of his own honor, Haman despises Mordecai and seeks his ruin out of wounded pride (Esther 3:5; 5:9-13).
            OracleHarness.Sin(OracleHarness.Witness("Haman's pride and contempt for Mordecai",
                new Pride(), new Boasting(), new Hatred()));
        }

        [Fact]
        public void HamanBuildsTheGallowsToHangMordecai_IsSin()
        {
            // At his wife's counsel Haman builds a fifty-cubit gallows to hang the innocent Mordecai (Esther 5:14).
            Resolution r = OracleHarness.Witness("Haman builds the gallows to hang Mordecai",
                new Murder(), new WickedScheming(), new Malice());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheJewsFastAndMournUnderTheDecree_IsRighteous()
        {
            // Mordecai and all the Jews fast, weep, and mourn, turning to God in their distress (Esther 4:1-3).
            OracleHarness.Righteous(OracleHarness.Witness("the Jews fast and mourn before God",
                new Mourning(), new Repentance(), new Worship()));
        }

        [Fact]
        public void EstherResolvesToGoToTheKingThoughSheMayPerish_IsRighteous()
        {
            // "If I perish, I perish" — Esther risks her life to intercede for her people (Esther 4:15-16).
            OracleHarness.Righteous(OracleHarness.Witness("Esther risks her life to plead for her people",
                new CourageousFaith(), new SelfSacrifice(), new DefendingTheWeak()));
        }

        [Fact]
        public void EstherPraysAndPleadsGroundedInGod_IsADivineAct()
        {
            // In the Greek Addition, Esther prays to the Lord and her plea is answered as His deliverance (Esther [Greek] C/D).
            OracleHarness.DivineAct(OracleHarness.Grounded("Esther prays to the LORD and is delivered",
                Grounding.InGod(), new Worship(), new Deliverance(), new CovenantFaithfulness()));
        }

        [Fact]
        public void MordecaiIsHonoredForSavingTheKing_IsRighteous()
        {
            // The unrewarded good deed is remembered, and Mordecai is paraded in royal honor (Esther 6:1-11).
            OracleHarness.Righteous(OracleHarness.Witness("Mordecai is justly honored for his faithfulness",
                new JustRecompense(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void HamanIsHangedOnHisOwnGallows_IsRighteous()
        {
            // Haman is hanged on the very gallows he raised for Mordecai — evil recoils on its author (Esther 7:9-10).
            OracleHarness.Righteous(OracleHarness.Witness("Haman is hanged on the gallows he built",
                new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void TheJewsAreDeliveredAndDefendThemselves_IsADivineAct()
        {
            // By a counter-decree the Jews are saved, given rest from their enemies, and preserved (Esther 8:11-17; 9:1-5).
            OracleHarness.DivineAct(OracleHarness.Grounded("God delivers the Jews and grants them rest",
                Grounding.InGod(), new Deliverance(), new Protection(), new CovenantFaithfulness()));
        }

        [Fact]
        public void EstherAndMordecaiEstablishPurimWithGiftsToThePoor_IsRighteous()
        {
            // Days of feasting, joy, and sending portions and gifts to the poor are appointed forever (Esther 9:20-22).
            OracleHarness.Righteous(OracleHarness.Witness("Purim kept with joy and gifts to the poor",
                new Joy(), new Generosity(), new FeedingTheHungry()));
        }
    }
}
