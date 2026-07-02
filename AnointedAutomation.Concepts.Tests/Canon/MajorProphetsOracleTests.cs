// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 29-32: Isaiah, Jeremiah, Ezekiel, Daniel.
    public class MajorProphetsOracleTests
    {
        [Fact]
        public void TheSufferingServantBearsOurSins_IsADivineAct()
        {
            // "He was pierced for our transgressions ... the LORD has laid on him the iniquity of us
            // all." (Isaiah 53:5-6) Atonement and self-giving love.
            OracleHarness.DivineAct(OracleHarness.Witness("the suffering Servant", new Atonement(), new SelfSacrifice()));
        }

        [Fact]
        public void IsaiahsCallToSeekJustice_IsRighteous()
        {
            // "Seek justice. Defend the oppressed. Take up the cause of the fatherless." (Isaiah 1:17)
            OracleHarness.Righteous(OracleHarness.Witness("seek justice, defend the oppressed",
                new JustRecompense(), new DefendingTheWeak()));
        }

        [Fact]
        public void JeremiahsNewCovenant_IsADivineAct()
        {
            // "I will make a new covenant ... I will forgive their wickedness." (Jeremiah 31:31-34)
            OracleHarness.DivineAct(OracleHarness.Witness("the new covenant", new CovenantFaithfulness(), new Pardon()));
        }

        [Fact]
        public void ChildSacrificeInTheValley_NeverEnteredGodsMind_IsGraveSin()
        {
            // "Something I did not command, nor did it enter my mind." (Jeremiah 32:35)
            Resolution r = OracleHarness.Witness("child sacrifice in the valley", new ChildSacrifice(), new Bloodshed());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void EzekielTakesNoPleasureInTheDeathOfTheWicked_IsADivineAct()
        {
            // "I take no pleasure in the death of anyone ... Repent and live!" (Ezekiel 18:32)
            OracleHarness.DivineAct(OracleHarness.Witness("God calls the wicked to live", new Compassion(), new JustRecompense()));
        }

        [Fact]
        public void TheValleyOfDryBonesRestoredToLife_IsADivineAct()
        {
            // "I will put my Spirit in you and you will live." (Ezekiel 37:14)
            OracleHarness.DivineAct(OracleHarness.Witness("the dry bones live", new RestoringLife(), new CreationOrder()));
        }

        [Fact]
        public void DanielRefusesTheKingsIdol_IsRighteous()
        {
            // The three will not bow to the image, and Daniel keeps praying (Daniel 3; 6).
            OracleHarness.Righteous(OracleHarness.Witness("Daniel will not bow to the idol",
                new CourageousFaith(), new ObedienceToGod()));
        }
    }
}
