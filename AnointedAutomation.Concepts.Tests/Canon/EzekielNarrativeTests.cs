// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Ezekiel narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class EzekielNarrativeTests
    {
        [Fact]
        public void GodRevealsHisGloryByTheRiverChebar_IsADivineAct()
        {
            // "I looked, and, behold ... the likeness of the glory of the LORD." (Ezekiel 1:1-28)
            // The throne-chariot vision opens the book: God revealed in ordered, holy glory.
            OracleHarness.DivineAct(OracleHarness.Witness("God reveals His enthroned glory to Ezekiel by the Chebar",
                new Worship(), new CreationOrder(), new Righteousness(),
                new RejoicingInTruth(), new Goodness()));
        }

        [Fact]
        public void GodCommissionsEzekielAsWatchmanToTheRebelliousHouse_IsADivineAct()
        {
            // "Son of man, I send thee ... a watchman unto the house of Israel." (Ezekiel 2:3-7; 3:17)
            // God appoints the prophet to speak His truth and warn the people for their good.
            OracleHarness.DivineAct(OracleHarness.Witness("God commissions Ezekiel as a watchman to warn Israel",
                new TeachingTruth(), new Protection(), new CovenantFaithfulness(),
                new HonoringTheVulnerable(), new Goodness()));
        }

        [Fact]
        public void EzekielEatsTheScrollAndObeysTheHardCommission_IsRighteous()
        {
            // "Eat this roll, and go speak unto the house of Israel ... it was in my mouth as honey." (Ezekiel 3:1-4)
            // The prophet humbly receives God's word and courageously goes where he is sent.
            OracleHarness.Righteous(OracleHarness.Witness("Ezekiel eats the scroll and obeys his hard commission",
                new ObedienceToGod(), new Humility(), new CourageousFaith(),
                new TeachingTruth(), new Fidelity()));
        }

        [Fact]
        public void JerusalemsBloodAndIdolatryProvokeJudgment_IsSin()
        {
            // "She hath changed my judgments into wickedness ... thy blood ... thine idols." (Ezekiel 5:6-9; 7:23)
            // Jerusalem's defiling idolatry and bloodshed bring the indictment of the city.
            Resolution r = OracleHarness.Grounded("Jerusalem fills the city with bloodshed and idols",
                Grounding.InIdol("idols of Jerusalem"),
                new Bloodshed(), new Idolatry(), new Defilement(), new Rebellion());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheEldersWorshipImagesInTheTempleAbominations_IsSin()
        {
            // "Behold the wicked abominations that they do here ... every form of creeping things ... and idols." (Ezekiel 8:9-16)
            // The temple-vision exposes secret idolatry done in the very house of God.
            OracleHarness.Sin(OracleHarness.Grounded("the elders worship images and idols within the temple",
                Grounding.InIdol("temple abominations"),
                new Idolatry(), new GravenImage(), new Defilement(), new Disobedience()));
        }

        [Fact]
        public void TheWickedShepherdsDevourTheFlockAndNeglectTheWeak_IsSin()
        {
            // "Ye eat the fat ... but ye feed not the flock. The diseased ... ye have not strengthened." (Ezekiel 34:2-4)
            // The false shepherds feed themselves and abandon the sick and scattered sheep.
            OracleHarness.Sin(OracleHarness.Witness("the shepherds of Israel devour the flock and neglect the weak",
                new Oppression(), new Indifference(), new SelfSeeking(),
                new Heartlessness(), new WithholdingWhatIsDue()));
        }

        [Fact]
        public void GodHimselfWillShepherdAndSeekTheLostSheep_IsADivineAct()
        {
            // "I will seek that which was lost ... bind up that which was broken ... feed them in good pasture." (Ezekiel 34:11-16)
            // The LORD takes up the shepherd's office to seek, heal, and protect His flock.
            OracleHarness.DivineAct(OracleHarness.Witness("God Himself shepherds Israel, seeking the lost and binding the broken",
                new Compassion(), new Healing(), new Protection(),
                new DefendingTheWeak(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodTakesNoPleasureInDeathButCallsTheWickedToRepent_IsADivineAct()
        {
            // "I have no pleasure in the death of him that dieth ... turn yourselves, and live." (Ezekiel 18:30-32; 33:11)
            // God's righteous mercy: He offers life freely to all who repent and turn.
            OracleHarness.DivineAct(OracleHarness.Witness("God calls the wicked to turn and live, taking no pleasure in death",
                new Pardon(), new Forgiveness(), new Compassion(),
                new Righteousness(), new Deliverance()));
        }

        [Fact]
        public void GodPromisesANewHeartAndNewSpirit_IsADivineAct()
        {
            // "A new heart also will I give you ... I will take away the stony heart ... and give you an heart of flesh." (Ezekiel 36:25-27)
            // God promises inward cleansing and renewal, fulfilling His covenant to restore His people.
            OracleHarness.DivineAct(OracleHarness.Witness("God promises a new heart and a new spirit to His people",
                new RestoringLife(), new Atonement(), new CovenantFaithfulness(),
                new PromiseFulfilled(), new Goodness()));
        }

        [Fact]
        public void GodRaisesTheValleyOfDryBonesToLife_IsADivineAct()
        {
            // "Behold, I will cause breath to enter into you, and ye shall live." (Ezekiel 37:1-14)
            // The vision of dry bones: God restores a dead nation to living, breathing life.
            OracleHarness.DivineAct(OracleHarness.Witness("God breathes life into the valley of dry bones and raises Israel",
                new RestoringLife(), new Deliverance(), new Healing(),
                new PromiseFulfilled(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodPromisesAnEverlastingCovenantOfPeaceAndDwelling_IsADivineAct()
        {
            // "I will make a covenant of peace with them ... my tabernacle also shall be with them." (Ezekiel 37:26-27)
            // God seals an everlasting covenant of peace and pledges to dwell among His people.
            OracleHarness.DivineAct(OracleHarness.Witness("God establishes an everlasting covenant of peace and dwells with His people",
                new Peace(), new CovenantFaithfulness(), new PromiseFulfilled(),
                new Fidelity(), new Goodness()));
        }

        [Fact]
        public void TheGloryOfTheLordReturnsToTheRestoredTemple_IsADivineAct()
        {
            // "The glory of the LORD came into the house ... the LORD is there." (Ezekiel 43:1-5; 48:35)
            // The closing vision: God's glory returns to fill the restored temple, ordered and holy.
            OracleHarness.DivineAct(OracleHarness.Witness("The glory of the LORD returns to fill the restored temple",
                new Worship(), new CreationOrder(), new PromiseFulfilled(),
                new Righteousness(), new RestoringLife()));
        }
    }
}
