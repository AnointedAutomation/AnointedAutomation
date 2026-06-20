// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Job narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JobNarrativeTests
    {
        [Fact]
        public void JobIsBlamelessAndUpright_IsRighteous()
        {
            // "That man was blameless and upright, one who feared God and turned away from evil." (Job 1:1)
            OracleHarness.Righteous(OracleHarness.Witness(
                "Job lives blameless, upright, fearing God",
                new Righteousness(), new ObedienceToGod(), new Purity()));
        }

        [Fact]
        public void JobSanctifiesAndOffersBurntOfferingsForHisChildren_IsRighteous()
        {
            // "Job ... offered burnt offerings according to the number of them all ... Thus Job did continually." (Job 1:5)
            OracleHarness.Righteous(OracleHarness.Witness(
                "Job rises early to sacrifice and intercede for his children",
                new Worship(), new Atonement()));
        }

        [Fact]
        public void TheAdversaryAccusesAndSchemesAgainstJob_IsSin()
        {
            // "Does Job fear God for nothing? ... stretch out your hand and touch all that he has." (Job 1:9-11)
            OracleHarness.Sin(OracleHarness.Witness(
                "the accuser schemes to ruin the righteous Job",
                new WickedScheming(), new Slander(), new Malice()));
        }

        [Fact]
        public void JobWorshipsAfterLosingEverything_IsRighteous()
        {
            // "The Lord gave, and the Lord has taken away; blessed be the name of the Lord." (Job 1:20-21)
            OracleHarness.Righteous(OracleHarness.Witness(
                "stripped of all, Job falls down and worships and trusts God",
                new Worship(), new Trust(), new Endurance()));
        }

        [Fact]
        public void JobDoesNotChargeGodWithWrong_IsRighteous()
        {
            // "In all this Job did not sin or charge God with wrong." (Job 1:22; 2:10)
            OracleHarness.Righteous(OracleHarness.Witness(
                "Job receives evil from God's hand without sinning",
                new Patience(), new Fidelity(), new SelfControl()));
        }

        [Fact]
        public void JobsWifeCounselsHimToCurseGod_IsSin()
        {
            // "Do you still hold fast your integrity? Curse God and die." (Job 2:9)
            OracleHarness.Sin(OracleHarness.Witness(
                "Job's wife urges him to curse God and die",
                new Blasphemy(), new Rebellion()));
        }

        [Fact]
        public void JobsFriendsCondemnTheInnocentSufferer_IsSin()
        {
            // The friends accuse the blameless man of hidden guilt with false witness. (Job 8:6; 22:5-9)
            OracleHarness.Sin(OracleHarness.Witness(
                "the friends condemn the innocent and bear false witness against Job",
                new Condemnation(), new FalseWitness(), new Slander()));
        }

        [Fact]
        public void JobRecountsHisCareForThePoorAndVulnerable_IsRighteous()
        {
            // "I delivered the poor ... the blind ... the lame ... I was a father to the needy." (Job 29:12-16)
            OracleHarness.Righteous(OracleHarness.Witness(
                "Job recalls delivering the poor, the orphan, the blind, and the needy",
                new Deliverance(), new DefendingTheWeak(), new HonoringTheVulnerable(), new Compassion()));
        }

        [Fact]
        public void JobOathOfFeedingTheHungryAndClothingTheNaked_IsRighteous()
        {
            // "I have not eaten my morsel alone ... if I have seen anyone perish for lack of clothing." (Job 31:16-20)
            OracleHarness.Righteous(OracleHarness.Witness(
                "Job swears he fed the hungry and clothed the naked",
                new FeedingTheHungry(), new ClothingTheNaked(), new Generosity()));
        }

        [Fact]
        public void GodAnswersJobOutOfTheWhirlwind_IsADivineAct()
        {
            // "Where were you when I laid the foundation of the earth?" (Job 38:4)
            OracleHarness.DivineAct(OracleHarness.Witness(
                "God answers from the whirlwind, declaring the order of creation",
                new CreationOrder(), new TeachingTruth(), new Goodness()));
        }

        [Fact]
        public void JobRepentsInDustAndAshes_IsRighteous()
        {
            // "I despise myself, and repent in dust and ashes." (Job 42:6)
            OracleHarness.Righteous(OracleHarness.Witness(
                "Job repents before God in dust and ashes",
                new Repentance(), new Humility(), new PovertyOfSpirit()));
        }

        [Fact]
        public void JobIntercedesForHisFriends_IsRighteous()
        {
            // "My servant Job shall pray for you ... and the Lord accepted Job's prayer." (Job 42:8-9)
            OracleHarness.Righteous(OracleHarness.Witness(
                "Job prays for the friends who wronged him",
                new Forgiveness(), new Atonement(), new Peacemaking()));
        }

        [Fact]
        public void GodRestoresJobsFortunesTwofold_IsADivineAct()
        {
            // "And the Lord restored the fortunes of Job ... gave Job twice as much as he had before." (Job 42:10)
            OracleHarness.DivineAct(OracleHarness.Witness(
                "God restores Job and gives him double for his trouble",
                new RestoringLife(), new JustRecompense(), new Goodness()));
        }
    }
}
