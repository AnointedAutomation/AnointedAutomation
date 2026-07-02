// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Obadiah narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class ObadiahNarrativeTests
    {
        [Fact]
        public void EdomTrustsItsHeightAndIsDeceivedByPride_IsSin()
        {
            // The pride of your heart has deceived you, you who dwell in the clefts of the rock... who say, Who will bring me down? (Obadiah 1:3).
            Resolution r = OracleHarness.Grounded("Edom trusts in the height of its rocky strongholds and boasts in its heart that none can bring it down",
                Grounding.InIdol("Edom's might"), new Pride(), new Boasting());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void GodWillBringEdomDownFromTheHeights_IsADivineAct()
        {
            // Though you soar like the eagle and set your nest among the stars, from there I will bring you down, declares the LORD (Obadiah 1:4).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD declares He will bring proud Edom down from its lofty nest in just judgment",
                new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void EdomDoesViolenceToHisBrotherJacob_IsSin()
        {
            // Because of the violence done to your brother Jacob, shame shall cover you, and you shall be cut off forever (Obadiah 1:10).
            Resolution r = OracleHarness.Witness("Edom does violence to his brother Jacob and sheds the blood of kindred",
                new Bloodshed(), new Treachery(), new Oppression());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void EdomStoodAloofWhileStrangersPlunderedJerusalem_IsSin()
        {
            // On the day you stood aloof, while strangers carried off his wealth and cast lots for Jerusalem, you were like one of them (Obadiah 1:11).
            Resolution r = OracleHarness.Witness("Edom stands aloof while strangers plunder Jerusalem and casts lots for the city, making himself one with the looters",
                new Indifference(), new Heartlessness(), new Treachery());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void EdomGloatedOverTheDayOfJudahsDisaster_IsSin()
        {
            // Do not gloat over your brother in the day of his misfortune, nor rejoice over Judah in the day of their ruin (Obadiah 1:12).
            Resolution r = OracleHarness.Witness("Edom gloats over his brother Judah and rejoices over their ruin in the day of their calamity",
                new DelightingInEvil(), new Malice(), new Boasting());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void EdomLootedTheGoodsOfTheStrickenCity_IsSin()
        {
            // Do not enter the gate of My people in the day of their calamity, nor loot his wealth in the day of his disaster (Obadiah 1:13).
            Resolution r = OracleHarness.Witness("Edom enters the gate of God's stricken people and seizes their wealth in the day of their disaster",
                new Theft(), new Greed(), new Oppression());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void EdomCutDownAndHandedOverTheFugitives_IsSin()
        {
            // Do not stand at the crossroads to cut off his fugitives, nor hand over his survivors in the day of distress (Obadiah 1:14).
            Resolution r = OracleHarness.Witness("Edom stands at the crossroads to cut down the fugitives of Judah and hands over the survivors to the enemy",
                new Bloodshed(), new Murder(), new Treachery());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheDayOfTheLordRepaysEachNationAsItHasDone_IsADivineAct()
        {
            // The day of the LORD is near upon all the nations; as you have done, it shall be done to you; your deeds shall return on your own head (Obadiah 1:15).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD brings the day of judgment near upon all the nations and returns each one's deeds justly upon its own head",
                new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void OnMountZionThereShallBeDeliveranceAndHoliness_IsADivineAct()
        {
            // But on Mount Zion there shall be those who escape, and it shall be holy, and the house of Jacob shall possess its possessions (Obadiah 1:17).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD grants deliverance and holiness on Mount Zion and restores to the house of Jacob its inheritance",
                new Deliverance(), new Protection(), new RestoringLife(), new PromiseFulfilled()));
        }

        [Fact]
        public void TheHouseOfJacobConsumesEsauAsStubble_IsADivineAct()
        {
            // The house of Jacob shall be a fire... and the house of Esau stubble; they shall burn and consume them, for the LORD has spoken (Obadiah 1:18).
            OracleHarness.DivineAct(OracleHarness.Witness("by the word of the LORD the house of Jacob becomes a flame that justly consumes proud Esau like stubble, leaving no survivor",
                new JustRecompense(), new Righteousness(), new PromiseFulfilled()));
        }

        [Fact]
        public void SaviorsGoUpToMountZionAndTheKingdomIsTheLords_IsADivineAct()
        {
            // Saviors shall go up on Mount Zion to rule Mount Esau, and the kingdom shall be the LORD's (Obadiah 1:21).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD raises up deliverers on Mount Zion and establishes His own everlasting kingdom over all",
                new Deliverance(), new Righteousness(), new CovenantFaithfulness(), new PromiseFulfilled()));
        }
    }
}
