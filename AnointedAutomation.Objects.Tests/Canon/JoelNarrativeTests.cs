// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Joel narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JoelNarrativeTests
    {
        [Fact]
        public void JoelCallsTheElttersToTellTheComingGenerations_IsRighteous()
        {
            // Tell your children of it, and let your children tell their children, and their children another generation (Joel 1:2-3).
            OracleHarness.Righteous(OracleHarness.Witness("Joel teaches the elders to recount the LORD's dealings to every coming generation",
                new TeachingTruth(), new CovenantFaithfulness()));
        }

        [Fact]
        public void JoelSummonsThePeopleToFastAndLament_IsRighteous()
        {
            // Sanctify a fast, call a solemn assembly; gather the elders and cry out to the LORD (Joel 1:13-14).
            OracleHarness.Righteous(OracleHarness.Witness("Joel summons the priests and the people to a solemn fast and lament before the LORD",
                new Mourning(), new Worship(), new Repentance()));
        }

        [Fact]
        public void TheDayOfTheLordAdvancesAsTerribleJudgment_IsADivineAct()
        {
            // The day of the LORD is great and very terrible; a day of darkness and gloom, of clouds and thick darkness (Joel 2:1-2,11).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD musters His army and brings the great and terrible Day of the LORD in just judgment",
                new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void GodCallsForWholeheartedReturnWithRendedHearts_IsADivineAct()
        {
            // Return to Me with all your heart, with fasting, weeping, and mourning; rend your hearts and not your garments (Joel 2:12-13).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD calls His people to return to Him with all their heart, for He is gracious and merciful",
                new Compassion(), new CovenantFaithfulness(), new Forgiveness()));
        }

        [Fact]
        public void GodIsGraciousMercifulAndRelentsFromDisaster_IsADivineAct()
        {
            // For He is gracious and merciful, slow to anger, abounding in steadfast love, and relents over disaster (Joel 2:13).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD shows Himself gracious, slow to anger, abounding in steadfast love, relenting from the threatened disaster",
                new Compassion(), new Patience(), new Pardon(), new CovenantFaithfulness()));
        }

        [Fact]
        public void ThePeopleRepentAndSanctifyAFast_IsRighteous()
        {
            // Blow the trumpet in Zion, consecrate a fast, gather the people, even nursing infants; let the priests weep (Joel 2:15-17).
            OracleHarness.Righteous(OracleHarness.Witness("Israel consecrates a fast, gathers as one assembly, and weeps before the LORD in repentance",
                new Repentance(), new Mourning(), new Humility(), new Worship()));
        }

        [Fact]
        public void GodPitiesHisLandAndRestoresHerHarvest_IsADivineAct()
        {
            // Then the LORD became jealous for His land and had pity on His people; behold, I am sending you grain, wine, and oil (Joel 2:18-19).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD has pity on His people and sends grain, wine, and oil to satisfy them",
                new Compassion(), new Generosity(), new FeedingTheHungry(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodRestoresTheYearsTheLocustHasEaten_IsADivineAct()
        {
            // I will restore to you the years that the swarming locust has eaten; you shall eat in plenty and be satisfied (Joel 2:25-26).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD restores the years the locust devoured and satisfies His people with plenty",
                new RestoringLife(), new Generosity(), new PromiseFulfilled(), new Healing()));
        }

        [Fact]
        public void GodPoursOutHisSpiritOnAllFlesh_IsADivineAct()
        {
            // I will pour out My Spirit on all flesh; your sons and daughters shall prophesy (Joel 2:28-29).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD pours out His Spirit on all flesh, that sons and daughters and servants might prophesy",
                new Generosity(), new PromiseFulfilled(), new CovenantFaithfulness()));
        }

        [Fact]
        public void WhoeverCallsOnTheNameOfTheLordShallBeSaved_IsADivineAct()
        {
            // And everyone who calls on the name of the LORD shall be saved (Joel 2:32).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD promises deliverance to everyone who calls upon His name",
                new Deliverance(), new PromiseFulfilled(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheNationsSoldGodsPeopleAndShedInnocentBlood_IsSin()
        {
            // They have scattered My people, sold a girl for wine, and shed innocent blood in their land (Joel 3:2-3,19).
            Resolution r = OracleHarness.Witness("Tyre, Sidon, and the nations sell the children of Judah and shed innocent blood in their land",
                new Bloodshed(), new Oppression(), new Greed());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void GodJudgesTheNationsInTheValleyOfDecision_IsADivineAct()
        {
            // I will gather all the nations into the Valley of Jehoshaphat and enter into judgment with them there (Joel 3:12-14).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD gathers the nations into the valley of decision and judges them justly for their deeds",
                new JustRecompense(), new Righteousness(), new DefendingTheWeak()));
        }

        [Fact]
        public void GodIsARefugeAndStrongholdForHisPeople_IsADivineAct()
        {
            // The LORD is a refuge to His people and a stronghold to the people of Israel (Joel 3:16).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD becomes a refuge and stronghold for His people in the day of judgment",
                new Protection(), new Deliverance(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodPardonsAndDwellsInZionForever_IsADivineAct()
        {
            // I will avenge their blood, which I have not yet avenged, for the LORD dwells in Zion (Joel 3:20-21).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD pardons His people, restores Judah forever, and dwells with them in Zion",
                new Pardon(), new RestoringLife(), new CovenantFaithfulness(), new PromiseFulfilled()));
        }
    }
}
