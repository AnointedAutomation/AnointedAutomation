// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Numbers narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class NumbersNarrativeTests
    {
        [Fact]
        public void GodOrdersTheCampAndTheCensus_IsADivineAct()
        {
            // "The LORD spoke to Moses... 'Take a census of the whole Israelite community.'" (Numbers 1:1-2)
            OracleHarness.DivineAct(OracleHarness.Witness("God numbers and orders Israel's camp for the journey", new CreationOrder(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheAaronicBlessing_IsADivineAct()
        {
            // "The LORD bless you and keep you... and give you peace." (Numbers 6:24-26)
            OracleHarness.DivineAct(OracleHarness.Witness("God gives the priestly blessing of peace over Israel", new Peace(), new Goodness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void ThePeopleComplainAtTaberah_IsSin()
        {
            // "Now the people complained about their hardships in the hearing of the LORD." (Numbers 11:1)
            OracleHarness.Sin(OracleHarness.Witness("Israel grumbles bitterly against the LORD at Taberah", new Ingratitude(), new Rebellion()));
        }

        [Fact]
        public void TheRabbleCraveMeatAndDespiseManna_IsSin()
        {
            // "If only we had meat to eat!... we never see anything but this manna!" (Numbers 11:4-6)
            OracleHarness.Sin(OracleHarness.Witness("the rabble crave meat and despise the LORD's manna", new Gluttony(), new Ingratitude(), new Covetousness()));
        }

        [Fact]
        public void MiriamAndAaronSpeakAgainstMoses_IsSin()
        {
            // "Miriam and Aaron began to talk against Moses... 'Has the LORD spoken only through Moses?'" (Numbers 12:1-2)
            OracleHarness.Sin(OracleHarness.Witness("Miriam and Aaron slander Moses out of jealousy", new Slander(), new Jealousy(), new Pride()));
        }

        [Fact]
        public void MosesIntercedesForMiriamsHealing_IsRighteous()
        {
            // "So Moses cried out to the LORD, 'Please, God, heal her!'" (Numbers 12:13)
            OracleHarness.Righteous(OracleHarness.Witness("Moses pleads for the healing of leprous Miriam", new Forgiveness(), new Compassion(), new Peacemaking()));
        }

        [Fact]
        public void TheTenSpiesGiveABadReportAndInciteRebellion_IsSin()
        {
            // "They spread among the Israelites a bad report... and the whole assembly talked of stoning them." (Numbers 13:32; 14:2-4)
            OracleHarness.Sin(OracleHarness.Witness("the ten faithless spies spread a bad report and the people rebel", new FalseWitness(), new Rebellion(), new Disobedience()));
        }

        [Fact]
        public void CalebAndJoshuaTrustGodAgainstTheGiants_IsRighteous()
        {
            // "We should go up and take possession of the land, for we can certainly do it." (Numbers 13:30; 14:6-9)
            OracleHarness.Righteous(OracleHarness.Witness("Caleb and Joshua trust God and urge Israel forward", new CourageousFaith(), new Trust(), new ObedienceToGod()));
        }

        [Fact]
        public void MosesIntercedesAndGodPardonsIsrael_IsADivineAct()
        {
            // "'I have forgiven them, as you asked.'" (Numbers 14:19-20)
            OracleHarness.DivineAct(OracleHarness.Witness("God pardons rebellious Israel at Moses' intercession", new Pardon(), new Forgiveness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void KorahsRebellionAgainstMosesAndAaron_IsSin()
        {
            // "They... rose up against Moses... 'You have gone too far!'" (Numbers 16:1-3)
            OracleHarness.Sin(OracleHarness.Witness("Korah, Dathan and Abiram rebel against God's appointed leaders", new Rebellion(), new Pride(), new Discord()));
        }

        [Fact]
        public void AaronStandsBetweenTheLivingAndTheDeadToStopThePlague_IsRighteous()
        {
            // "He stood between the living and the dead, and the plague stopped." (Numbers 16:47-48)
            OracleHarness.Righteous(OracleHarness.Witness("Aaron makes atonement with incense and halts the plague", new Atonement(), new SelfSacrifice(), new Protection()));
        }

        [Fact]
        public void MosesStrikesTheRockInUnbelief_IsSin()
        {
            // "Because you did not trust in me enough to honor me as holy... you will not bring this community into the land." (Numbers 20:10-12)
            OracleHarness.Sin(OracleHarness.Witness("Moses strikes the rock in anger and disobedience at Meribah", new Disobedience(), new FitsOfRage()));
        }

        [Fact]
        public void GodGivesTheBronzeSerpentForHealing_IsADivineAct()
        {
            // "Anyone who is bitten can look at it and live." (Numbers 21:8-9)
            OracleHarness.DivineAct(OracleHarness.Witness("God gives the bronze serpent so the snakebitten may be healed and live", new Healing(), new RestoringLife(), new Deliverance()));
        }

        [Fact]
        public void BalaamBlessesIsraelInsteadOfCursing_IsRighteous()
        {
            // "How can I curse those whom God has not cursed?... I must do only what the LORD says." (Numbers 23:8, 26)
            OracleHarness.Righteous(OracleHarness.Witness("Balaam speaks only God's word and blesses Israel", new ObedienceToGod(), new TeachingTruth(), new Fidelity()));
        }

        [Fact]
        public void IsraelJoinsItselfToBaalOfPeorThroughIdolatry_IsSin()
        {
            // "The people... began to indulge in sexual immorality... and bowed down before [the] gods." (Numbers 25:1-3)
            OracleHarness.Sin(OracleHarness.Grounded("Israel yokes itself to Baal of Peor in immorality and idol worship", Grounding.InIdol("Baal of Peor"), new Idolatry(), new SexualImmorality(), new CovenantBreaking()));
        }

        [Fact]
        public void TheDaughtersOfZelophehadReceiveAJustInheritance_IsADivineAct()
        {
            // "What Zelophehad's daughters are saying is right. You must certainly give them property as an inheritance." (Numbers 27:5-7)
            OracleHarness.DivineAct(OracleHarness.Witness("God grants Zelophehad's daughters their just inheritance", new JustRecompense(), new Righteousness(), new DefendingTheWeak()));
        }
    }
}
