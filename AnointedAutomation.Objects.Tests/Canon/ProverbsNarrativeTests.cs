// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Proverbs narrative oracle: the engine must return the verdict Scripture gives at each teaching.
    public class ProverbsNarrativeTests
    {
        [Fact]
        public void TheLordByWisdomFoundedTheEarth_IsADivineAct()
        {
            // "The LORD by wisdom founded the earth; by understanding he established the heavens." (Proverbs 3:19-20)
            // God's own ordering of creation by wisdom.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD founds the earth by wisdom",
                new CreationOrder(), new Goodness(), new Fidelity()));
        }

        [Fact]
        public void WisdomWasBesideGodAtCreationAsMasterCraftsman_IsADivineAct()
        {
            // "When he established the heavens, I was there ... then I was beside him, like a master workman." (Proverbs 8:27-30)
            // Wisdom rejoicing in the truth of God's good creative order.
            OracleHarness.DivineAct(OracleHarness.Witness("Wisdom is beside God ordering creation",
                new CreationOrder(), new RejoicingInTruth(), new Joy(), new Goodness()));
        }

        [Fact]
        public void TrustingTheLordWithAllYourHeart_IsRighteous()
        {
            // "Trust in the LORD with all your heart, and do not lean on your own understanding." (Proverbs 3:5-7)
            // The fear of the LORD that turns from evil and trusts him wholly.
            OracleHarness.Righteous(OracleHarness.Witness("Trust in the LORD and not your own understanding",
                new Trust(), new Humility(), new ObedienceToGod(), new Fidelity()));
        }

        [Fact]
        public void GenerosityToThePoorLendsToTheLord_IsRighteous()
        {
            // "Whoever is generous to the poor lends to the LORD." (Proverbs 19:17; 22:9; 14:31)
            // Honoring the Maker by feeding and caring for the needy.
            OracleHarness.Righteous(OracleHarness.Witness("The wise are generous and give bread to the poor",
                new Generosity(), new FeedingTheHungry(), new HonoringTheVulnerable(), new Compassion()));
        }

        [Fact]
        public void RescuingThoseBeingLedAwayToDeath_IsRighteous()
        {
            // "Rescue those who are being taken away to death." (Proverbs 24:11-12)
            // Defending and delivering the helpless from destruction.
            OracleHarness.Righteous(OracleHarness.Witness("Deliver those being dragged to death",
                new Deliverance(), new DefendingTheWeak(), new Protection(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void OpeningYourMouthForTheRightsOfTheNeedy_IsRighteous()
        {
            // "Open your mouth, judge righteously, defend the rights of the poor and needy." (Proverbs 31:8-9)
            // The royal charge to plead for the destitute.
            OracleHarness.Righteous(OracleHarness.Witness("Speak up and judge righteously for the needy",
                new Righteousness(), new DefendingTheWeak(), new JustRecompense(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void TheVirtuousWomanFeedsAndClothesHerHousehold_IsRighteous()
        {
            // "She opens her hand to the poor ... her household are clothed in scarlet." (Proverbs 31:20-21,30)
            // Diligent, God-fearing labor that feeds, clothes, and serves.
            OracleHarness.Righteous(OracleHarness.Witness("The excellent wife provides for her household and the poor",
                new Generosity(), new FeedingTheHungry(), new ClothingTheNaked(),
                new Endurance(), new Trustworthiness(), new Compassion()));
        }

        [Fact]
        public void TheGentleAnswerThatTurnsAwayWrath_IsRighteous()
        {
            // "A soft answer turns away wrath, but a harsh word stirs up anger." (Proverbs 15:1; 16:32)
            // The patient, self-controlled tongue that makes peace.
            OracleHarness.Righteous(OracleHarness.Witness("A soft answer turns away wrath",
                new Gentleness(), new SelfControl(), new Patience(), new Peacemaking()));
        }

        [Fact]
        public void FeedingYourEnemyWhoIsHungry_IsRighteous()
        {
            // "If your enemy is hungry, give him bread to eat; and if he is thirsty, give him water." (Proverbs 25:21-22)
            // Mercy that overcomes evil with good toward an enemy.
            OracleHarness.Righteous(OracleHarness.Witness("Give your hungry enemy bread and your thirsty enemy water",
                new FeedingTheHungry(), new GivingDrink(), new Kindness(), new Forgiveness()));
        }

        [Fact]
        public void TheSevenThingsTheLordHates_AreSin()
        {
            // "There are six things the LORD hates ... haughty eyes, a lying tongue, hands that shed innocent
            // blood, a heart that devises wicked plans ... a false witness, one who sows discord." (Proverbs 6:16-19)
            OracleHarness.Sin(OracleHarness.Witness("The things the LORD hates: pride, lies, bloodshed, scheming, discord",
                new HaughtyEyes(), new Deceit(), new Bloodshed(), new WickedScheming(),
                new FalseWitness(), new SowingDiscord()));
            // Bloodshed is a capital vice and drives disorder to its limit.
            Assert.Equal(1.0, OracleHarness.Witness("The things the LORD hates",
                new HaughtyEyes(), new Deceit(), new Bloodshed(), new WickedScheming(),
                new FalseWitness(), new SowingDiscord()).Disorder);
        }

        [Fact]
        public void PrideGoesBeforeDestruction_IsSin()
        {
            // "Pride goes before destruction, and a haughty spirit before a fall." (Proverbs 16:18; 11:2)
            // The arrogance the LORD abhors and that brings ruin.
            OracleHarness.Sin(OracleHarness.Witness("Pride and a haughty spirit before the fall",
                new Pride(), new HaughtyEyes(), new Boasting()));
        }

        [Fact]
        public void TheAdulteressLeadsToDeath_IsSin()
        {
            // "Her house sinks down to death ... none who go to her come back." (Proverbs 5:3-5; 7:21-27)
            // The seductress whose path is sexual immorality leading to Sheol.
            OracleHarness.Sin(OracleHarness.Witness("The forbidden woman lures the simple to her bed of death",
                new Adultery(), new SexualImmorality(), new Deceit(), new Lust()));
        }

        [Fact]
        public void TheSluggardWhoWillNotPlow_IsSin()
        {
            // "The sluggard does not plow in the autumn; he will seek at harvest and have nothing." (Proverbs 20:4; 6:9-11)
            // Slothful refusal of labor that ends in poverty.
            OracleHarness.Sin(OracleHarness.Witness("The sluggard refuses to plow and falls into want",
                new Sloth(), new Indifference()));
        }

        [Fact]
        public void WineIsAMockerAndDrunkardsComeToPoverty_IsSin()
        {
            // "Wine is a mocker, strong drink a brawler ... the drunkard and the glutton will come to poverty." (Proverbs 20:1; 23:20-21,29-35)
            // Drunkenness and gluttony that bring woe, sorrow, and ruin.
            OracleHarness.Sin(OracleHarness.Witness("Drunkenness and gluttony bring woe and poverty",
                new Drunkenness(), new Gluttony(), new Carousing()));
        }

        [Fact]
        public void DishonestScalesAndUnjustGain_AreSin()
        {
            // "A false balance is an abomination to the LORD ... unequal weights are an abomination." (Proverbs 11:1; 20:10,23; 1:10-19)
            // Deceitful greed and robbery that the LORD detests.
            OracleHarness.Sin(OracleHarness.Witness("False balances and ill-gotten gain that ambush blood",
                new Deceit(), new Greed(), new Theft(), new Oppression()));
        }
    }
}
