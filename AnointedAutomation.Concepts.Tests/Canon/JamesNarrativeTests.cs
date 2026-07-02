// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // James narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JamesNarrativeTests
    {
        [Fact]
        public void TestingOfFaithProducesSteadfastEndurance_IsRighteous()
        {
            // "the testing of your faith produces steadfastness ... that you may be perfect and complete." (James 1:3-4)
            OracleHarness.Righteous(OracleHarness.Witness("the believer counts trials all joy because tested faith produces steadfast endurance", new Endurance(), new Fidelity(), new Joy()));
        }

        [Fact]
        public void GodGivesWisdomGenerouslyToAllWhoAsk_IsADivineAct()
        {
            // "let him ask God, who gives generously to all without reproach." (James 1:5)
            OracleHarness.DivineAct(OracleHarness.Witness("God gives wisdom generously and graciously to all who ask Him", new Generosity(), new Kindness(), new Goodness()));
        }

        [Fact]
        public void EveryGoodAndPerfectGiftComesDownFromTheFatherOfLights_IsADivineAct()
        {
            // "Every good gift and every perfect gift is from above, coming down from the Father of lights." (James 1:17)
            OracleHarness.DivineAct(OracleHarness.Witness("the unchanging Father of lights sends down every good and perfect gift", new Generosity(), new Goodness(), new Fidelity()));
        }

        [Fact]
        public void GodBroughtUsForthByTheWordOfTruth_IsADivineAct()
        {
            // "Of his own will he brought us forth by the word of truth." (James 1:18)
            OracleHarness.DivineAct(OracleHarness.Witness("of His own will God brought us forth by the word of truth as firstfruits", new RestoringLife(), new TeachingTruth(), new Goodness()));
        }

        [Fact]
        public void TheBelieverMustBeQuickToHearSlowToAngerAndPure_IsRighteous()
        {
            // "let every person be quick to hear, slow to speak, slow to anger ... receive the implanted word." (James 1:19-21)
            OracleHarness.Righteous(OracleHarness.Witness("the believer is quick to hear, slow to anger, putting away filth in meekness", new SelfControl(), new Meekness(), new Purity()));
        }

        [Fact]
        public void PureReligionVisitsOrphansAndWidowsAndKeepsUnstained_IsRighteous()
        {
            // "Religion that is pure ... is this: to visit orphans and widows in their affliction, and to keep oneself unstained." (James 1:27)
            OracleHarness.Righteous(OracleHarness.Witness("pure religion visits orphans and widows in affliction and keeps oneself unstained", new HonoringTheVulnerable(), new Compassion(), new Purity()));
        }

        [Fact]
        public void ShowingPartialityAndDishonoringThePoor_IsSin()
        {
            // "you have shown partiality ... you have dishonored the poor man." (James 2:1-6)
            OracleHarness.Sin(OracleHarness.Witness("they fawn on the rich in fine clothes and dishonor and oppress the poor", new Oppression(), new Greed(), new Pride()));
        }

        [Fact]
        public void LovingYourNeighborAsYourselfFulfillsTheRoyalLaw_IsRighteous()
        {
            // "If you really fulfill the royal law ... 'You shall love your neighbor as yourself,' you are doing well." (James 2:8)
            OracleHarness.Righteous(OracleHarness.Witness("the believer fulfills the royal law, loving his neighbor as himself", new Kindness(), new Compassion(), new Goodness()));
        }

        [Fact]
        public void WithholdingFoodAndClothingFromTheNeedyBrother_IsSin()
        {
            // "If a brother or sister is poorly clothed and lacking in daily food, and one of you says ... but does not give them the things needed." (James 2:15-16)
            OracleHarness.Sin(OracleHarness.Witness("they dismiss a brother lacking food and clothing without giving what the body needs", new Indifference(), new Heartlessness(), new WithholdingWhatIsDue()));
        }

        [Fact]
        public void AbrahamOfferedIsaacAndFaithWasCompletedByWorks_IsRighteous()
        {
            // "Was not Abraham our father justified by works when he offered up his son Isaac on the altar?" (James 2:21-23)
            OracleHarness.Righteous(OracleHarness.Witness("Abraham obeys God and offers up Isaac, his faith completed by works of obedience", new ObedienceToGod(), new CourageousFaith(), new Fidelity()));
        }

        [Fact]
        public void RahabReceivedTheMessengersAndSentThemSafely_IsRighteous()
        {
            // "And in the same way was not also Rahab the prostitute justified by works when she received the messengers and sent them out by another way?" (James 2:25)
            OracleHarness.Righteous(OracleHarness.Witness("Rahab welcomes the messengers and sends them safely out another way", new Hospitality(), new Protection(), new CourageousFaith()));
        }

        [Fact]
        public void TheUntamedTongueBlessesGodAndCursesMen_IsSin()
        {
            // "With it we bless our Lord and Father, and with it we curse people ... this ought not to be so." (James 3:8-10)
            OracleHarness.Sin(OracleHarness.Witness("the restless tongue, full of deadly poison, both curses men and sows strife", new Slander(), new Discord(), new Malice()));
        }

        [Fact]
        public void WisdomFromAboveIsPurePeaceableAndGentle_IsRighteous()
        {
            // "the wisdom from above is first pure, then peaceable, gentle, open to reason, full of mercy and good fruits." (James 3:17)
            OracleHarness.Righteous(OracleHarness.Witness("the wisdom from above is pure, peaceable, and gentle, full of mercy and good fruits", new Purity(), new Peace(), new Gentleness(), new Compassion()));
        }

        [Fact]
        public void EarthlyWisdomIsJealousyAndSelfishAmbitionBreedingDisorder_IsSin()
        {
            // "where jealousy and selfish ambition exist, there will be disorder and every vile practice." (James 3:14-16)
            OracleHarness.Sin(OracleHarness.Witness("bitter jealousy and selfish ambition boast against the truth, breeding disorder", new Jealousy(), new SelfishAmbition(), new Boasting()));
        }

        [Fact]
        public void QuarrelsAndCovetousMurderingDesiresWageWarWithin_IsSin()
        {
            // "What causes quarrels and fights among you? ... You desire and do not have, so you murder." (James 4:1-2)
            OracleHarness.Sin(OracleHarness.Witness("warring passions covet and quarrel and murder to seize what they lust for", new Murder(), new Covetousness(), new Discord()));
            Assert.Equal(1.0, OracleHarness.Witness("warring passions covet and quarrel and murder to seize what they lust for", new Murder(), new Covetousness(), new Discord()).Disorder);
        }

        [Fact]
        public void FriendshipWithTheWorldIsEnmityWithGod_IsSin()
        {
            // "friendship with the world is enmity with God ... You adulterous people!" (James 4:4)
            OracleHarness.Sin(OracleHarness.Witness("the adulterous heart befriends the world in pride and makes itself an enemy of God", new Adultery(), new Pride(), new Rebellion()));
        }

        [Fact]
        public void HumbleYourselvesBeforeTheLordAndHeWillExaltYou_IsRighteous()
        {
            // "Humble yourselves before the Lord, and he will exalt you." (James 4:7-10)
            OracleHarness.Righteous(OracleHarness.Witness("the believer submits to God, draws near, and humbles himself before the Lord", new Humility(), new ObedienceToGod(), new Repentance()));
        }

        [Fact]
        public void TheRichDefraudTheirLaborersOfWithheldWages_IsSin()
        {
            // "the wages of the laborers ... which you kept back by fraud, are crying out." (James 5:1-4)
            OracleHarness.Sin(OracleHarness.Witness("the rich hoard treasure and keep back by fraud the wages of their laborers", new WithholdingWhatIsDue(), new Oppression(), new Greed()));
        }

        [Fact]
        public void ThePrayerOfFaithWillSaveTheSickAndRaiseThemUp_IsADivineAct()
        {
            // "the prayer of faith will save the one who is sick, and the Lord will raise him up." (James 5:14-15)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord answers the prayer of faith, healing the sick and forgiving sins", new Healing(), new Forgiveness(), new RestoringLife()));
        }

        [Fact]
        public void TurningASinnerFromHisWanderingSavesHisSoul_IsRighteous()
        {
            // "whoever brings back a sinner from his wandering will save his soul from death." (James 5:19-20)
            OracleHarness.Righteous(OracleHarness.Witness("the brother turns a wanderer back from error, saving his soul and covering sins", new Deliverance(), new Compassion(), new Forgiveness()));
        }
    }
}
