// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 1 & 2 Kings narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class KingsNarrativeTests
    {
        [Fact]
        public void SolomonAsksForAnUnderstandingHeart_IsRighteous()
        {
            // "Give your servant a discerning heart to govern your people." (1 Kings 3:9)
            OracleHarness.Righteous(OracleHarness.Witness("Solomon asks God not for riches or long life but for a discerning heart to judge God's people", new Humility(), new HungerForRighteousness(), new Trust()));
        }

        [Fact]
        public void GodGrantsSolomonWisdom_IsADivineAct()
        {
            // "God gave Solomon wisdom and very great insight." (1 Kings 3:12; 4:29)
            OracleHarness.DivineAct(OracleHarness.Witness("God grants Solomon wisdom, discernment, and a breadth of understanding beyond all who came before", new Generosity(), new Goodness(), new PromiseFulfilled()));
        }

        [Fact]
        public void SolomonBuildsTheTempleForTheLord_IsRighteous()
        {
            // "So Solomon built the temple and completed it." (1 Kings 6:14, 38)
            OracleHarness.Righteous(OracleHarness.Witness("Solomon builds the temple as a house for the Name of the LORD according to all God commanded", new Worship(), new ObedienceToGod(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheGloryOfTheLordFillsTheTemple_IsADivineAct()
        {
            // "The glory of the LORD filled his temple." (1 Kings 8:10-11)
            OracleHarness.DivineAct(OracleHarness.Witness("the cloud of the glory of the LORD fills the temple at its dedication", new CovenantFaithfulness(), new PromiseFulfilled(), new Goodness()));
        }

        [Fact]
        public void SolomonTurnsToForeignGodsInOldAge_IsSin()
        {
            // "His wives turned his heart after other gods... He followed Ashtoreth and Molek." (1 Kings 11:4-8)
            OracleHarness.Sin(OracleHarness.Grounded("Solomon in his old age turns his heart to the gods of his foreign wives and builds high places for them", Grounding.InIdol("Molek"), new Idolatry(), new CovenantBreaking(), new Disobedience()));
        }

        [Fact]
        public void JeroboamSetsUpGoldenCalves_IsSin()
        {
            // "Here are your gods, Israel, who brought you up out of Egypt." (1 Kings 12:28-30)
            OracleHarness.Sin(OracleHarness.Grounded("Jeroboam makes two golden calves at Bethel and Dan and leads Israel into idolatry", Grounding.InIdol("GoldenCalf"), new Idolatry(), new GravenImage(), new Rebellion()));
        }

        [Fact]
        public void GodFeedsElijahByRavensAtTheBrook_IsADivineAct()
        {
            // "I have directed the ravens to supply you with food there." (1 Kings 17:4-6)
            OracleHarness.DivineAct(OracleHarness.Witness("God sustains Elijah at the Kerith Ravine, sending ravens with bread and meat and giving water from the brook", new Protection(), new FeedingTheHungry(), new GivingDrink(), new CovenantFaithfulness()));
        }

        [Fact]
        public void ElijahRaisesTheWidowsSon_IsADivineAct()
        {
            // "The LORD heard Elijah's cry, and the boy's life returned to him." (1 Kings 17:21-22)
            OracleHarness.DivineAct(OracleHarness.Witness("God answers Elijah and restores the life of the widow of Zarephath's son", new RestoringLife(), new Compassion(), new HonoringTheVulnerable(), new Healing()));
        }

        [Fact]
        public void TheLordAnswersByFireOnMountCarmel_IsADivineAct()
        {
            // "The fire of the LORD fell and burned up the sacrifice... 'The LORD, he is God!'" (1 Kings 18:38-39)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD answers Elijah by fire on Mount Carmel, consuming the sacrifice so Israel knows He alone is God", new Deliverance(), new TeachingTruth(), new CovenantFaithfulness()));
        }

        [Fact]
        public void AhabAndJezebelSeizeNabothsVineyardByMurder_IsSin()
        {
            // Jezebel has Naboth falsely accused and stoned so Ahab can seize his vineyard. (1 Kings 21:1-16)
            Resolution r = OracleHarness.Witness("Jezebel arranges false witnesses to condemn and stone Naboth so Ahab can seize his ancestral vineyard", new Murder(), new Bloodshed(), new FalseWitness(), new Covetousness(), new Theft());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void ElijahIsTakenUpToHeavenInAWhirlwind_IsADivineAct()
        {
            // "Elijah went up to heaven in a whirlwind." (2 Kings 2:11)
            OracleHarness.DivineAct(OracleHarness.Witness("God takes Elijah up to heaven in a whirlwind with chariots of fire", new Deliverance(), new Fidelity(), new Goodness()));
        }

        [Fact]
        public void ElishaHealsNaamanOfLeprosy_IsADivineAct()
        {
            // "His flesh was restored and became clean like that of a young boy." (2 Kings 5:14)
            OracleHarness.DivineAct(OracleHarness.Witness("God cleanses Naaman the Aramean of his leprosy when he washes in the Jordan at Elisha's word", new Healing(), new Compassion(), new Goodness()));
        }

        [Fact]
        public void GehaziLiesAndTakesNaamansGiftInGreed_IsSin()
        {
            // Gehazi runs after Naaman, lies to get silver and garments, and is struck with leprosy. (2 Kings 5:20-27)
            OracleHarness.Sin(OracleHarness.Witness("Gehazi greedily chases Naaman, lies to obtain silver and clothing, and deceives Elisha", new Greed(), new Deceit(), new Covetousness()));
        }

        [Fact]
        public void JosiahRepairsTheTempleAndRenewsTheCovenant_IsRighteous()
        {
            // Josiah hears the Book of the Law, tears his robes, and leads Judah back to the covenant. (2 Kings 22-23)
            OracleHarness.Righteous(OracleHarness.Witness("Josiah finds the Book of the Law, repents in humility, purges the idols, and renews the covenant before the LORD", new Repentance(), new Humility(), new ObedienceToGod(), new CovenantFaithfulness(), new Worship()));
        }

        [Fact]
        public void ManassehSacrificesHisSonAndFillsJerusalemWithBlood_IsSin()
        {
            // "He sacrificed his own son in the fire... and shed so much innocent blood." (2 Kings 21:6, 16)
            Resolution r = OracleHarness.Witness("Manasseh sacrifices his own son in the fire, practices sorcery, and fills Jerusalem with innocent blood", new ChildSacrifice(), new Bloodshed(), new Idolatry(), new Sorcery());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }
    }
}
