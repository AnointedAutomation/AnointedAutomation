// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Exodus narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class ExodusNarrativeTests
    {
        [Fact]
        public void PharaohEnslavesIsrael_IsSin()
        {
            // Pharaoh oppresses Israel with hard bondage (Exodus 1:11-14).
            OracleHarness.Sin(OracleHarness.Witness("Pharaoh enslaves Israel", new Oppression(), new Wrongdoing()));
        }

        [Fact]
        public void PharaohCommandsHebrewSonsDrowned_IsSin()
        {
            // "Every son that is born ye shall cast into the river." (Exodus 1:22)
            Resolution r = OracleHarness.Witness("Pharaoh orders the Hebrew infants killed", new Murder(), new Bloodshed());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheMidwivesFearGodAndSpareTheChildren_IsRighteous()
        {
            // The midwives feared God and saved the male children alive (Exodus 1:17-21).
            OracleHarness.Righteous(OracleHarness.Witness("the midwives spare the Hebrew sons", new ObedienceToGod(), new Protection(), new CourageousFaith()));
        }

        [Fact]
        public void GodHearsAndRemembersHisCovenant_IsDivineAct()
        {
            // "God remembered his covenant with Abraham, with Isaac, and with Jacob." (Exodus 2:24)
            OracleHarness.DivineAct(OracleHarness.Witness("God remembers his covenant with the patriarchs", new CovenantFaithfulness(), new Compassion()));
        }

        [Fact]
        public void GodCallsMosesFromTheBurningBush_IsDivineAct()
        {
            // "I have surely seen the affliction of my people... and am come down to deliver them." (Exodus 3:7-8)
            OracleHarness.DivineAct(OracleHarness.Witness("God calls Moses to deliver Israel", new Deliverance(), new Compassion(), new PromiseFulfilled()));
        }

        [Fact]
        public void PharaohHardensHisHeartAndRefusesToReleaseIsrael_IsSin()
        {
            // Pharaoh will not let the people go and increases their burdens (Exodus 5:2-9).
            OracleHarness.Sin(OracleHarness.Witness("Pharaoh refuses to free Israel and oppresses harder", new Rebellion(), new Oppression(), new Pride()));
        }

        [Fact]
        public void GodSendsThePlaguesAsJustJudgment_IsDivineAct()
        {
            // "Against all the gods of Egypt I will execute judgment." (Exodus 12:12)
            OracleHarness.DivineAct(OracleHarness.Witness("God judges Egypt through the plagues", new JustRecompense(), new DefendingTheWeak()));
        }

        [Fact]
        public void IsraelKeepsThePassoverInObedience_IsRighteous()
        {
            // Israel keeps the Passover as the LORD commanded (Exodus 12:28).
            OracleHarness.Righteous(OracleHarness.Witness("Israel keeps the first Passover", new ObedienceToGod(), new Fidelity(), new Trust()));
        }

        [Fact]
        public void GodPassesOverAndDeliversIsraelFromEgypt_IsDivineAct()
        {
            // The LORD brings Israel out of Egypt by the blood of the lamb (Exodus 12:13, 51).
            OracleHarness.DivineAct(OracleHarness.Witness("God delivers Israel out of Egypt", new Deliverance(), new Protection(), new PromiseFulfilled()));
        }

        [Fact]
        public void GodPartsTheRedSeaAndSavesIsrael_IsDivineAct()
        {
            // "The LORD saved Israel that day out of the hand of the Egyptians." (Exodus 14:21-30)
            OracleHarness.DivineAct(OracleHarness.Witness("God divides the Red Sea and saves Israel", new Deliverance(), new Protection(), new Fidelity()));
        }

        [Fact]
        public void GodFeedsIsraelWithMannaInTheWilderness_IsDivineAct()
        {
            // "Behold, I will rain bread from heaven for you." (Exodus 16:4)
            OracleHarness.DivineAct(OracleHarness.Witness("God feeds Israel with manna", new FeedingTheHungry(), new Compassion(), new Fidelity()));
        }

        [Fact]
        public void GodGivesTheLawAtSinai_IsDivineAct()
        {
            // God speaks the Ten Commandments and establishes his covenant (Exodus 20:1-17; 24:7-8).
            OracleHarness.DivineAct(OracleHarness.Witness("God gives the Law and covenant at Sinai", new CovenantFaithfulness(), new TeachingTruth(), new Righteousness()));
        }

        [Fact]
        public void IsraelWorshipsTheGoldenCalf_IsSinGroundedInAnIdol()
        {
            // "These be thy gods, O Israel, which brought thee up out of the land of Egypt." (Exodus 32:4-8)
            OracleHarness.Sin(OracleHarness.Grounded("Israel worships the golden calf", Grounding.InIdol("the golden calf"), new Idolatry(), new GravenImage(), new Rebellion()));
        }

        [Fact]
        public void TheGloryOfTheLordFillsTheTabernacle_IsDivineAct()
        {
            // "The glory of the LORD filled the tabernacle." (Exodus 40:34-38)
            OracleHarness.DivineAct(OracleHarness.Witness("God's glory fills the finished tabernacle", new CovenantFaithfulness(), new PromiseFulfilled(), new Worship()));
        }
    }
}
