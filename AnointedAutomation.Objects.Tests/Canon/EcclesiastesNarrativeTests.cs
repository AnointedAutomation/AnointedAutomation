// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Ecclesiastes narrative oracle: the engine must return the verdict Scripture gives at each step.
    // Qoheleth (the Preacher) weighs life "under the sun" and lands on the fear of God as the whole of man.
    public class EcclesiastesNarrativeTests
    {
        [Fact]
        public void GodMadeEverythingBeautifulInItsTime_IsADivineAct()
        {
            // "He has made everything beautiful in its time... a time for every matter under heaven." (Ecclesiastes 3:1-11)
            OracleHarness.DivineAct(OracleHarness.Witness("God appoints a season for everything", new CreationOrder(), new Goodness()));
        }

        [Fact]
        public void GodSettingEternityInTheHumanHeart_IsADivineAct()
        {
            // "He has put eternity into man's heart." (Ecclesiastes 3:11)
            OracleHarness.DivineAct(OracleHarness.Witness("God places eternity in the heart", new CreationOrder(), new Goodness()));
        }

        [Fact]
        public void QohelethChasingPleasureAndPossessionsAsHisPortion_IsSin()
        {
            // "I made great works... whatever my eyes desired I did not keep from them... all was vanity and a striving after wind." (Ecclesiastes 2:1-11)
            OracleHarness.Sin(OracleHarness.Witness("the Preacher heaps up pleasure, houses, and treasure for himself", new SelfSeeking(), new Greed(), new Gluttony()));
        }

        [Fact]
        public void TheOppressionsDoneUnderTheSun_AreSin()
        {
            // "I saw all the oppressions that are done under the sun... and they had no comforter." (Ecclesiastes 4:1)
            OracleHarness.Sin(OracleHarness.Witness("the tears of the oppressed who have no comforter", new Oppression(), new Heartlessness()));
        }

        [Fact]
        public void TwoAreBetterThanOneBearingOneAnotherUp_IsRighteous()
        {
            // "Two are better than one... if they fall, one will lift up his fellow." (Ecclesiastes 4:9-12)
            OracleHarness.Righteous(OracleHarness.Witness("companions lift up the one who falls", new Kindness(), new Protection()));
        }

        [Fact]
        public void DrawingNearToListenInTheHouseOfGod_IsRighteous()
        {
            // "Guard your steps when you go to the house of God... let your words be few, for God is in heaven." (Ecclesiastes 5:1-2)
            OracleHarness.Righteous(OracleHarness.Witness("guarding one's steps and words before God", new Worship(), new Humility()));
        }

        [Fact]
        public void TheLoverOfMoneyNeverSatisfied_IsSin()
        {
            // "He who loves money will not be satisfied with money." (Ecclesiastes 5:10)
            OracleHarness.Sin(OracleHarness.Witness("the insatiable love of money and wealth", new Greed(), new Covetousness()));
        }

        [Fact]
        public void ReceivingFoodToilAndJoyAsTheGiftOfGod_IsRighteous()
        {
            // "Behold, what I have seen to be good and fitting is to eat and drink and find enjoyment in his toil... this is the gift of God." (Ecclesiastes 5:18-19)
            OracleHarness.Righteous(OracleHarness.Witness("eating, drinking, and rejoicing in one's toil as God's gift", new Joy(), new Goodness(), new Trust()));
        }

        [Fact]
        public void TheFoolGivenToFollyAndCarousing_IsSin()
        {
            // "The heart of fools is in the house of mirth... laughter of the fools." (Ecclesiastes 7:4-6)
            OracleHarness.Sin(OracleHarness.Witness("the fool feasting in mindless mirth", new Carousing(), new Sloth()));
        }

        [Fact]
        public void RememberingYourCreatorInTheDaysOfYouth_IsRighteous()
        {
            // "Remember also your Creator in the days of your youth." (Ecclesiastes 12:1)
            OracleHarness.Righteous(OracleHarness.Witness("remembering the Creator in youth", new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void TheWholeDutyOfManToFearGodAndKeepHisCommandments_IsRighteous()
        {
            // "Fear God and keep his commandments, for this is the whole duty of man." (Ecclesiastes 12:13)
            OracleHarness.Righteous(OracleHarness.Witness("fear God and keep his commandments", new ObedienceToGod(), new Humility(), new Fidelity()));
        }

        [Fact]
        public void GodBringingEveryDeedIntoJudgment_IsADivineAct()
        {
            // "God will bring every deed into judgment, with every secret thing, whether good or evil." (Ecclesiastes 12:14)
            OracleHarness.DivineAct(OracleHarness.Witness("God brings every deed and secret into judgment", new JustRecompense(), new Righteousness()));
        }
    }
}
