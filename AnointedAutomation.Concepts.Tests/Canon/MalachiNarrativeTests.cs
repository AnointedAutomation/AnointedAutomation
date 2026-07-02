// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Malachi narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class MalachiNarrativeTests
    {
        [Fact]
        public void GodDeclaresHisCovenantLoveForJacob_IsADivineAct()
        {
            // "I have loved you, saith the LORD ... Yet I loved Jacob." (Malachi 1:2)
            // God answers the people's doubt by affirming His electing, covenant love for them.
            OracleHarness.DivineAct(OracleHarness.Witness("God declares to Israel, I have loved you, and affirms His covenant love for Jacob",
                new CovenantFaithfulness(), new Fidelity(), new Goodness(),
                new Compassion(), new PromiseFulfilled()));
        }

        [Fact]
        public void ThePriestsDespiseGodsNameWithPollutedOfferings_IsSin()
        {
            // "Ye offer polluted bread upon mine altar ... when ye offer the blind for sacrifice, is it not evil?" (Malachi 1:7-8)
            // The priests dishonor the LORD by bringing defiled and blemished sacrifices to His altar.
            OracleHarness.Sin(OracleHarness.Grounded("The priests despise the name of the LORD by offering polluted bread and blind and lame animals on His altar",
                Grounding.InIdol("their own contempt"),
                new Defilement(), new Blasphemy(), new Disobedience(),
                new WithholdingWhatIsDue(), new Ingratitude()));
            // capital vice (Defilement) drives disorder to its limit
            Assert.Equal(1.0, OracleHarness.Grounded("The priests despise the name of the LORD by offering polluted bread and blind and lame animals on His altar",
                Grounding.InIdol("their own contempt"),
                new Defilement(), new Blasphemy(), new Disobedience(),
                new WithholdingWhatIsDue(), new Ingratitude()).Disorder);
        }

        [Fact]
        public void ThePriestsBringTheStolenLameAndSickAsSacrifice_IsSin()
        {
            // "Ye brought that which was torn, and the lame, and the sick ... should I accept this of your hand?" (Malachi 1:13)
            // They sniff at the LORD's table and present cheap, blemished, ill-gotten offerings, robbing Him of due honor.
            OracleHarness.Sin(OracleHarness.Grounded("The priests bring the torn, the lame, and the sick as offerings and weary of the LORD's table",
                Grounding.InIdol("their own greed"),
                new WithholdingWhatIsDue(), new Greed(), new Deceit(),
                new Disobedience(), new Indifference()));
        }

        [Fact]
        public void GodWillBeMagnifiedAndHisNameGreatAmongTheNations_IsADivineAct()
        {
            // "My name shall be great among the Gentiles ... a pure offering: for my name shall be great among the heathen." (Malachi 1:11)
            // God promises that in every place a pure offering will be brought and His name honored among the nations.
            OracleHarness.DivineAct(OracleHarness.Witness("God declares His name shall be great among the nations and a pure offering brought to Him in every place",
                new Worship(), new Righteousness(), new PromiseFulfilled(),
                new CovenantFaithfulness(), new Goodness()));
        }

        [Fact]
        public void TheCovenantOfLeviWasOnePeaceAndTruth_IsRighteous()
        {
            // "My covenant was with him of life and peace ... The law of truth was in his mouth ... he walked with me in peace and equity, and did turn many away from iniquity." (Malachi 2:5-6)
            // Faithful Levi feared the LORD, taught true instruction, and turned many from sin: the priesthood as it ought to be.
            OracleHarness.Righteous(OracleHarness.Witness("Levi walks with God in peace and equity, the law of truth in his mouth, turning many away from iniquity",
                new CovenantFaithfulness(), new TeachingTruth(), new Peace(),
                new Righteousness(), new Fidelity()));
        }

        [Fact]
        public void ThePriestsCorruptTheCovenantAndCausePeopleToStumble_IsSin()
        {
            // "Ye are departed out of the way; ye have caused many to stumble at the law; ye have corrupted the covenant of Levi." (Malachi 2:8)
            // The priests forsake God's way, show partiality in instruction, and trip up the people, breaking Levi's covenant.
            OracleHarness.Sin(OracleHarness.Grounded("The priests depart from the way, cause many to stumble at the law, and corrupt the covenant of Levi",
                Grounding.InIdol("their own partiality"),
                new CovenantBreaking(), new Disobedience(), new Deceit(),
                new SowingDiscord(), new Rebellion()));
        }

        [Fact]
        public void JudahDealsTreacherouslyAndProfanesTheCovenant_IsSin()
        {
            // "Judah hath dealt treacherously ... Judah hath profaned the holiness of the LORD ... and hath married the daughter of a strange god." (Malachi 2:11)
            // The people break faith with one another and with God, profaning the covenant by marrying into idolatry.
            OracleHarness.Sin(OracleHarness.Grounded("Judah deals treacherously, profanes the covenant of the LORD, and marries the daughter of a foreign god",
                Grounding.InIdol("foreign gods"),
                new Treachery(), new CovenantBreaking(), new Idolatry(),
                new Disobedience(), new Faction()));
        }

        [Fact]
        public void TheMenBreakFaithWithTheWivesOfTheirYouthByDivorce_IsSin()
        {
            // "The LORD ... hath been witness between thee and the wife of thy youth, against whom thou hast dealt treacherously ... For the LORD ... hateth putting away." (Malachi 2:14-16)
            // The men betray the wives of their covenant, and God testifies that He hates this treacherous putting away.
            OracleHarness.Sin(OracleHarness.Grounded("The men deal treacherously against the wives of their youth and put them away in divorce",
                Grounding.InIdol("their own desires"),
                new Treachery(), new CovenantBreaking(), new Deceit(),
                new Oppression(), new Disobedience()));
        }

        [Fact]
        public void ThePeopleWearyGodCallingEvildoersGood_IsSin()
        {
            // "Ye have wearied the LORD with your words ... When ye say, Every one that doeth evil is good in the sight of the LORD ... Where is the God of judgment?" (Malachi 2:17)
            // They tire God with cynical words, calling evil good and doubting that He judges at all.
            OracleHarness.Sin(OracleHarness.Grounded("The people weary the LORD with their words, calling every evildoer good and asking where is the God of judgment",
                Grounding.InIdol("their own cynicism"),
                new DelightingInEvil(), new Blasphemy(), new Deceit(),
                new Slander(), new Disobedience()));
        }

        [Fact]
        public void GodSendsHisMessengerAndTheRefinerToPurifyTheSonsOfLevi_IsADivineAct()
        {
            // "I will send my messenger ... the Lord ... shall suddenly come to his temple ... he shall purify the sons of Levi, and purge them ... that they may offer ... an offering in righteousness." (Malachi 3:1-3)
            // God promises His messenger and His own coming as a refiner's fire to cleanse and restore true worship.
            OracleHarness.DivineAct(OracleHarness.Witness("God sends His messenger and comes as a refiner's fire to purify the sons of Levi for an offering in righteousness",
                new PromiseFulfilled(), new Righteousness(), new Purity(),
                new CovenantFaithfulness(), new Deliverance()));
        }

        [Fact]
        public void GodWillJudgeSorcerersAdulterersAndOppressors_IsADivineAct()
        {
            // "I will come near to you to judgment ... against the sorcerers, and against the adulterers ... and those that oppress the hireling in his wages, the widow, and the fatherless." (Malachi 3:5)
            // The LORD draws near as a swift witness to judge the wicked and defend the defenseless they have wronged.
            OracleHarness.DivineAct(OracleHarness.Witness("God comes near in judgment against sorcerers and adulterers and those who oppress the widow, the fatherless, and the hireling",
                new JustRecompense(), new Righteousness(), new DefendingTheWeak(),
                new HonoringTheVulnerable(), new Fidelity()));
        }

        [Fact]
        public void ThePeopleRobGodInTithesAndOfferings_IsSin()
        {
            // "Will a man rob God? Yet ye have robbed me ... In tithes and offerings. Ye are cursed with a curse." (Malachi 3:8-9)
            // The whole nation withholds the tithes and offerings owed to God, robbing Him of what is His.
            OracleHarness.Sin(OracleHarness.Grounded("The people rob God by withholding the tithes and offerings they owe Him",
                Grounding.InIdol("their own greed"),
                new Theft(), new WithholdingWhatIsDue(), new Greed(),
                new Disobedience(), new Ingratitude()));
        }

        [Fact]
        public void GodPromisesToOpenHeavensWindowsToThoseWhoBringTheTithe_IsADivineAct()
        {
            // "Bring ye all the tithes ... prove me now herewith ... if I will not open you the windows of heaven, and pour you out a blessing." (Malachi 3:10)
            // To those who return what is His, God pledges to pour out blessing beyond measure and rebuke the devourer.
            OracleHarness.DivineAct(OracleHarness.Witness("God promises to open the windows of heaven and pour out blessing on those who bring the full tithe into His house",
                new Generosity(), new Goodness(), new PromiseFulfilled(),
                new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void ThoseWhoFearTheLordSpeakTogetherAndAreWrittenInTheBookOfRemembrance_IsRighteous()
        {
            // "Then they that feared the LORD spake often one to another ... and a book of remembrance was written ... for them that feared the LORD, and that thought upon his name." (Malachi 3:16)
            // A faithful remnant fears God, encourages one another, and reveres His name, and He records them as His own.
            OracleHarness.Righteous(OracleHarness.Witness("Those who fear the LORD speak often to one another and revere His name, and a book of remembrance is written for them",
                new Worship(), new Fidelity(), new CovenantFaithfulness(),
                new Trust(), new Righteousness()));
        }

        [Fact]
        public void GodSparesAndOwnsThoseWhoFearHimAsHisTreasuredPossession_IsADivineAct()
        {
            // "They shall be mine, saith the LORD ... in that day when I make up my jewels; and I will spare them, as a man spareth his own son." (Malachi 3:17)
            // God claims the God-fearers as His treasured possession and spares them as a father spares his obedient son.
            OracleHarness.DivineAct(OracleHarness.Witness("God claims those who fear Him as His treasured possession and spares them as a man spares his own son",
                new Compassion(), new CovenantFaithfulness(), new Protection(),
                new Goodness(), new PromiseFulfilled()));
        }

        [Fact]
        public void TheSunOfRighteousnessRisesWithHealingForThoseWhoFearGod_IsADivineAct()
        {
            // "Unto you that fear my name shall the Sun of righteousness arise with healing in his wings." (Malachi 4:2)
            // For those who fear His name God promises the Sun of righteousness, rising with healing and joyful deliverance.
            OracleHarness.DivineAct(OracleHarness.Witness("The Sun of righteousness rises with healing in His wings for those who fear the name of the LORD",
                new Healing(), new Righteousness(), new Deliverance(),
                new Joy(), new PromiseFulfilled()));
        }

        [Fact]
        public void GodPromisesToSendElijahToTurnTheHeartsOfFathersAndChildren_IsADivineAct()
        {
            // "I will send you Elijah the prophet ... And he shall turn the heart of the fathers to the children, and the heart of the children to their fathers." (Malachi 4:5-6)
            // God promises to send Elijah before the great day, reconciling generations and calling His people to repentance.
            OracleHarness.DivineAct(OracleHarness.Witness("God promises to send Elijah the prophet to turn the hearts of the fathers to the children and the children to the fathers",
                new Peacemaking(), new Repentance(), new PromiseFulfilled(),
                new CovenantFaithfulness(), new Deliverance()));
        }
    }
}
