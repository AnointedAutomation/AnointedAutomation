// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Baruch narrative oracle: the engine must return the verdict Scripture gives at each step.
    // Canon: EO (Eastern-Orthodox-accepted). Chapter 6 is the Letter of Jeremiah, counted within Baruch.
    public class BaruchNarrativeTests
    {
        [Fact]
        public void ExilesHearTheBookAndWeepFastAndPray_IsRighteous()
        {
            // When they heard Baruch's words, "they wept, and fasted, and prayed before the Lord."
            // (Baruch 1:1-5)
            // A humbled, mourning people turning to the Lord in repentance.
            OracleHarness.Righteous(OracleHarness.Witness("the exiles hear Baruch's book and weep, fast, and pray before the Lord",
                new Mourning(), new Humility(), new Repentance(), new Worship(), new Trust()));
        }

        [Fact]
        public void ExilesSendMoneyToJerusalemForSacrificesAndIntercession_IsRighteous()
        {
            // They gather money and send it to Jerusalem to buy burnt offerings and to pray for
            // the life of Nebuchadnezzar and for themselves. (Baruch 1:6-14)
            // Generous, worshipful provision and intercession for the welfare of others.
            OracleHarness.Righteous(OracleHarness.Witness("the exiles send money to Jerusalem for sacrifices and pray for the people",
                new Generosity(), new Worship(), new ObedienceToGod(), new Peacemaking()));
        }

        [Fact]
        public void IsraelConfessesCovenantBreakingAndDisobedience_IsSin()
        {
            // "To the Lord our God belongeth righteousness, but unto us confusion of faces ... we have
            // sinned, we have done wickedly ... and have not been obedient unto the Lord our God."
            // (Baruch 1:15-2:10)
            // Israel names the very sin that brought the exile: rebellion against the covenant.
            OracleHarness.Sin(OracleHarness.Witness("Israel confesses its covenant-breaking and disobedience to the Lord",
                new CovenantBreaking(), new Disobedience(), new Rebellion(), new Idolatry()));
        }

        [Fact]
        public void IsraelPraysForMercyAppealingToGodsCovenant_IsRighteous()
        {
            // "Hear our prayers, O Lord ... according to all thy righteousness ... remember not the
            // iniquities of our forefathers." (Baruch 2:11-3:8)
            // A penitent appeal resting wholly on God's covenant faithfulness and mercy.
            OracleHarness.Righteous(OracleHarness.Grounded("Israel prays for mercy, appealing to God's covenant",
                Grounding.InGod(),
                new Repentance(), new Humility(), new Trust(), new CovenantFaithfulness(), new Worship()));
        }

        [Fact]
        public void IsraelForsakesTheFountainOfWisdom_IsSin()
        {
            // "Thou hast forsaken the fountain of wisdom. For if thou hadst walked in the way of God,
            // thou shouldest have dwelled in peace for ever." (Baruch 3:9-13)
            // The rebuke: Israel abandoned the wisdom and commandments of God.
            OracleHarness.Sin(OracleHarness.Witness("Israel forsakes the fountain of wisdom and walks not in God's way",
                new Disobedience(), new Rebellion(), new CovenantBreaking()));
        }

        [Fact]
        public void GodAloneFindsOutWisdomAndGivesHerToJacob_IsADivineAct()
        {
            // "He hath found out all the way of knowledge, and hath given it unto Jacob his servant ...
            // afterward did he shew himself upon earth." (Baruch 3:36-4:1)
            // God Himself reveals wisdom, the book of the commandments, to His people.
            OracleHarness.DivineAct(OracleHarness.Grounded("God finds out all wisdom and gives her to Jacob His servant",
                Grounding.InGod(),
                new TeachingTruth(), new CreationOrder(), new CovenantFaithfulness(), new Goodness(), new Fidelity()));
        }

        [Fact]
        public void BaruchExhortsIsraelToHoldFastWisdomAndLive_IsRighteous()
        {
            // "Turn thee, O Jacob, and take hold of it: walk in the presence of the light thereof,
            // that thou mayest be illuminated." (Baruch 4:2-4)
            // A call to obedient return to God's commandments, the way of life.
            OracleHarness.Righteous(OracleHarness.Witness("Baruch exhorts Israel to take hold of wisdom and walk in God's light",
                new TeachingTruth(), new ObedienceToGod(), new Repentance(), new RejoicingInTruth()));
        }

        [Fact]
        public void ZionMournsButCallsHerChildrenToHopeInGod_IsRighteous()
        {
            // Jerusalem laments her exiled children yet comforts them: "Be of good cheer, O my children,
            // and cry unto God." (Baruch 4:9-29)
            // Maternal mourning joined to a steadfast call to repentance and trust.
            OracleHarness.Righteous(OracleHarness.Witness("Zion mourns her exiled children and calls them to cry unto God",
                new Mourning(), new Compassion(), new Repentance(), new Trust(), new TeachingTruth()));
        }

        [Fact]
        public void GodPromisesToBringTheExilesHomeInGloryAndJoy_IsADivineAct()
        {
            // "Arise, O Jerusalem ... God will bring them unto thee with glory ... for God will lead
            // Israel with joy in the light of his glory with the mercy and righteousness that cometh
            // from him." (Baruch 4:30-5:9)
            // God's saving promise: He gathers His scattered people in mercy and righteousness.
            OracleHarness.DivineAct(OracleHarness.Grounded("God promises to gather the exiles home in glory, mercy, and joy",
                Grounding.InGod(),
                new Deliverance(), new PromiseFulfilled(), new CovenantFaithfulness(),
                new Compassion(), new Righteousness(), new Joy()));
        }

        [Fact]
        public void ThePeopleAreWarnedThatBabylonianIdolsAreNoGods_IsRighteous()
        {
            // The Letter of Jeremiah warns the exiles: "be not ye afraid of them ... how should a man
            // think or say that they are gods?" (Baruch 6:4-23)
            // True teaching that exposes idols as powerless, guarding the people from false worship.
            OracleHarness.Righteous(OracleHarness.Witness("the exiles are warned that the idols of Babylon are no gods",
                new TeachingTruth(), new RejoicingInTruth(), new Protection(), new CourageousFaith()));
        }

        [Fact]
        public void BabylonWorshipsLifelessIdolsAndPracticesDefilement_IsSin()
        {
            // The idols are decked with gold and silver, cannot save, and "the women ... sit in the ways"
            // and burn bran for incense to them while the priests rob their temples. (Baruch 6:8-33)
            // Idolatry, deceit, theft, and sexual defilement bound up in Babylon's false worship.
            OracleHarness.Sin(OracleHarness.Witness("Babylon worships lifeless idols with deceit, theft, and immorality",
                new Idolatry(), new Deceit(), new Theft(), new SexualImmorality(), new Defilement()));
        }
    }
}
