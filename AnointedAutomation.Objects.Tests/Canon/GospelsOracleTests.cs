// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 47-50: Matthew, Mark, Luke, John.
    public class GospelsOracleTests
    {
        [Fact]
        public void Matthew_TheWeightierMattersOfTheLaw_IsRighteous()
        {
            // "Justice, mercy and faithfulness." (Matthew 23:23) The whole character of God at once.
            OracleHarness.Righteous(OracleHarness.Witness("the weightier matters",
                new JustRecompense(), new Compassion(), new CovenantFaithfulness()));
        }

        [Fact]
        public void Matthew_TheUnforgivingServant_IsSin()
        {
            // Forgiven much, he would not forgive (Matthew 18:23-35).
            OracleHarness.Sin(OracleHarness.Witness("the unforgiving servant", new Unforgiveness(), new Condemnation()));
        }

        [Fact]
        public void Mark_JesusHealsAndForgives_IsADivineAct()
        {
            // "Your sins are forgiven ... get up, take your mat and walk." (Mark 2:5-11)
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus heals and forgives", new Healing(), new Pardon()));
        }

        [Fact]
        public void Mark_TheSonOfManGivesHisLifeAsRansom_IsADivineAct()
        {
            // "The Son of Man came ... to give his life as a ransom for many." (Mark 10:45)
            OracleHarness.DivineAct(OracleHarness.Witness("a ransom for many", new SelfSacrifice(), new Atonement()));
        }

        [Fact]
        public void Luke_TheGoodSamaritan_IsRighteous()
        {
            // "Go and do likewise." (Luke 10:37)
            OracleHarness.Righteous(OracleHarness.Witness("the Good Samaritan", new Compassion(), new Protection()));
        }

        [Fact]
        public void Luke_TheProdigalReceivedBack_IsADivineAct()
        {
            // "This son of mine was dead and is alive again." (Luke 15:24) The Father's running mercy.
            OracleHarness.DivineAct(OracleHarness.Witness("the prodigal received back", new Pardon(), new Compassion()));
        }

        [Fact]
        public void John_TheWordMadeFleshAndJohn316_IsADivineAct()
        {
            // "The Word became flesh." (John 1:14); "For God so loved the world." (John 3:16)
            OracleHarness.DivineAct(OracleHarness.Witness("God gives His Son", new SelfSacrifice(), new Compassion()));
        }

        [Fact]
        public void John_NeitherDoICondemnYou_IsADivineAct()
        {
            // To the woman caught in adultery: "Neither do I condemn you ... go and sin no more."
            // (John 8:11) Mercy that does not crush the broken.
            OracleHarness.DivineAct(OracleHarness.Witness("neither do I condemn you", new Pardon(), new Compassion()));
        }

        [Fact]
        public void John_TheCrossSavesTheWorld_IsPerfectlyCoherent()
        {
            // The fullness of God's character in one act (John 19; Romans 3:25-26).
            Resolution r = OracleHarness.Witness("the cross",
                new SelfSacrifice(), new Atonement(), new Pardon(), new CovenantFaithfulness());

            Assert.Equal(1.0, r.Coherence);
            Assert.Equal(0.0, r.Disorder);
        }
    }
}
