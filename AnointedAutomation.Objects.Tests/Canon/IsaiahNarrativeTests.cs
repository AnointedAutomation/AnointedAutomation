// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Isaiah narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class IsaiahNarrativeTests
    {
        [Fact]
        public void JudahsRebellionAndBloodGuiltOfThePeople_IsSin()
        {
            // "Ah sinful nation ... they have forsaken the LORD ... your hands are full of blood." (Isaiah 1:2-4,15)
            // The opening indictment: a covenant people steeped in rebellion and bloodshed.
            Resolution r = OracleHarness.Witness("Judah rebels against the LORD, hands full of blood",
                new Rebellion(), new Bloodshed(), new CovenantBreaking(), new Wrongdoing());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void GodCallsJudahToCeaseEvilAndDefendTheOppressed_IsADivineAct()
        {
            // "Cease to do evil ... seek judgment, relieve the oppressed, judge the fatherless, plead for the widow." (Isaiah 1:16-17)
            // God's own summons to washed-clean justice and mercy toward the vulnerable.
            OracleHarness.DivineAct(OracleHarness.Witness("God calls Judah to defend the orphan and widow",
                new Righteousness(), new DefendingTheWeak(), new HonoringTheVulnerable(),
                new JustRecompense(), new Goodness()));
        }

        [Fact]
        public void GodOffersToMakeScarletSinsWhiteAsSnow_IsADivineAct()
        {
            // "Though your sins be as scarlet, they shall be as white as snow." (Isaiah 1:18)
            // The LORD freely offers cleansing pardon to the repentant.
            OracleHarness.DivineAct(OracleHarness.Witness("God offers to wash scarlet sins white as snow",
                new Pardon(), new Forgiveness(), new Atonement(),
                new CovenantFaithfulness(), new Compassion()));
        }

        [Fact]
        public void IsaiahSeesTheThriceHolyLordEnthroned_IsADivineAct()
        {
            // "Holy, holy, holy, is the LORD of hosts: the whole earth is full of his glory." (Isaiah 6:1-3)
            // The throne-room vision: God revealed in unapproachable holiness and ordered glory.
            OracleHarness.DivineAct(OracleHarness.Witness("Isaiah beholds the thrice-holy LORD enthroned in glory",
                new Worship(), new Righteousness(), new CreationOrder(),
                new RejoicingInTruth(), new Goodness()));
        }

        [Fact]
        public void IsaiahConfessesUncleanLipsAndIsSentByGod_IsRighteous()
        {
            // "Woe is me ... I am a man of unclean lips ... Here am I; send me." (Isaiah 6:5-8)
            // The prophet's humble repentance, cleansing, and courageous commission.
            OracleHarness.Righteous(OracleHarness.Witness("Isaiah confesses his uncleanness and answers 'Here am I; send me'",
                new Repentance(), new Humility(), new ObedienceToGod(),
                new CourageousFaith(), new TeachingTruth()));
        }

        [Fact]
        public void GodGivesTheSignOfImmanuelTheChildBorn_IsADivineAct()
        {
            // "A virgin shall conceive ... call his name Immanuel ... unto us a child is born." (Isaiah 7:14; 9:6-7)
            // God's promised gift of the Prince of Peace, fulfilling His covenant promise.
            OracleHarness.DivineAct(OracleHarness.Witness("God gives the sign of Immanuel, the child born, Prince of Peace",
                new PromiseFulfilled(), new Deliverance(), new Peace(),
                new CovenantFaithfulness(), new Goodness()));
        }

        [Fact]
        public void AssyriaBoastsInItsOwnStrengthAndPride_IsSin()
        {
            // "By the strength of my hand I have done it ... Shall the axe boast itself?" (Isaiah 10:13-15)
            // The Assyrian's arrogant self-exaltation against the God who wields him.
            OracleHarness.Sin(OracleHarness.Grounded("Assyria boasts in its own might against God",
                Grounding.InIdol("Assyria"),
                new Pride(), new Boasting(), new Insolence(), new HaughtyEyes()));
        }

        [Fact]
        public void TheRighteousBranchJudgesWithRighteousnessForThePoor_IsADivineAct()
        {
            // "With righteousness shall he judge the poor ... the wolf shall dwell with the lamb." (Isaiah 11:1-9)
            // The Branch from Jesse rules in Spirit-filled righteousness and restored creation-peace.
            OracleHarness.DivineAct(OracleHarness.Witness("The Branch of Jesse judges righteously for the poor and brings creation-peace",
                new Righteousness(), new DefendingTheWeak(), new Peace(),
                new CreationOrder(), new Goodness()));
        }

        [Fact]
        public void BabylonsPrideAimsToExaltItselfAboveGod_IsSin()
        {
            // "I will ascend into heaven ... I will be like the most High." (Isaiah 14:12-15)
            // The king of Babylon's god-defying pride and ambition cast down to the pit.
            OracleHarness.Sin(OracleHarness.Grounded("Babylon's king exalts himself to be like the Most High",
                Grounding.InIdol("Babylon"),
                new Pride(), new Boasting(), new SelfishAmbition(), new Rebellion()));
        }

        [Fact]
        public void TrueFastingIsToLooseOppressionAndFeedTheHungry_IsADivineAct()
        {
            // "Loose the bands of wickedness ... deal thy bread to the hungry ... bring the poor that are cast out to thy house." (Isaiah 58:6-7)
            // God names the fast He chooses: liberating the oppressed and clothing the naked.
            OracleHarness.DivineAct(OracleHarness.Witness("God names the true fast: loose oppression, feed the hungry, clothe the naked",
                new DefendingTheWeak(), new FeedingTheHungry(), new ClothingTheNaked(),
                new Compassion(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void TheSufferingServantIsWoundedToBearOurSins_IsADivineAct()
        {
            // "He was wounded for our transgressions ... with his stripes we are healed." (Isaiah 53:4-6,12)
            // The Servant pours out His soul as a sin-offering, bearing the iniquity of many.
            OracleHarness.DivineAct(OracleHarness.Witness("The Suffering Servant bears our sins, wounded for our healing",
                new Atonement(), new SelfSacrifice(), new Healing(),
                new Forgiveness(), new Righteousness()));
        }

        [Fact]
        public void GodComfortsHisPeopleAndCarriesTheLambsInHisArms_IsADivineAct()
        {
            // "Comfort ye my people ... he shall gather the lambs with his arm, and carry them in his bosom." (Isaiah 40:1,11)
            // The Shepherd-God tenderly comforts, gathers, and gently leads His flock.
            OracleHarness.DivineAct(OracleHarness.Witness("God comforts His people and carries the lambs in His bosom",
                new Compassion(), new Gentleness(), new Protection(),
                new CovenantFaithfulness(), new Goodness()));
        }

        [Fact]
        public void IdolatersWorshipTheWorkOfTheirOwnHands_IsSin()
        {
            // "He burneth part thereof in the fire ... and the residue thereof he maketh a god." (Isaiah 44:9-20)
            // The folly of bowing to a carved block, the work of one's own hands.
            OracleHarness.Sin(OracleHarness.Grounded("the craftsman bows down to the idol he carved",
                Grounding.InIdol("graven image"),
                new Idolatry(), new GravenImage(), new Deceit(), new Disobedience()));
        }

        [Fact]
        public void GodPromisesNewHeavensAndNewEarthOfPeace_IsADivineAct()
        {
            // "I create new heavens and a new earth ... the wolf and the lamb shall feed together." (Isaiah 65:17-25)
            // God's final promise: a re-created order of joy, peace, and unbroken life.
            OracleHarness.DivineAct(OracleHarness.Witness("God creates new heavens and a new earth of peace and joy",
                new CreationOrder(), new RestoringLife(), new Peace(),
                new Joy(), new PromiseFulfilled()));
        }
    }
}
