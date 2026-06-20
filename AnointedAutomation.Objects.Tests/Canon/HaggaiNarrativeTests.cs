// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Haggai narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class HaggaiNarrativeTests
    {
        [Fact]
        public void ThePeopleSayTheTimeHasNotComeToBuildTheLordsHouse_IsSin()
        {
            // "This people say, The time is not come, the time that the LORD's house should be built." (Haggai 1:2)
            // The remnant excuses its neglect of God's house, putting off the work He gave them to do.
            OracleHarness.Sin(OracleHarness.Grounded("The people say the time has not yet come to rebuild the house of the LORD",
                Grounding.InIdol("their own excuses"),
                new Disobedience(), new Sloth(), new Indifference(),
                new WithholdingWhatIsDue(), new Ingratitude()));
        }

        [Fact]
        public void ThePeopleDwellInPaneledHousesWhileGodsHouseLiesWaste_IsSin()
        {
            // "Is it time for you ... to dwell in your cieled houses, and this house lie waste?" (Haggai 1:4)
            // They lavish on their own paneled homes while the LORD's house sits in ruins, serving self before God.
            OracleHarness.Sin(OracleHarness.Grounded("The people live in their own paneled houses while the house of God lies waste",
                Grounding.InIdol("their own comfort"),
                new SelfSeeking(), new Indifference(), new Sloth(),
                new WithholdingWhatIsDue(), new Greed()));
        }

        [Fact]
        public void GodWithholdsTheHarvestToCallThePeopleToConsiderTheirWays_IsADivineAct()
        {
            // "Ye have sown much, and bring in little ... Consider your ways." (Haggai 1:5-6,9-11)
            // God justly withholds rain and increase, that His people might weigh their neglect and return to Him.
            OracleHarness.DivineAct(OracleHarness.Witness("God withholds the harvest and the dew and calls the people to consider their ways",
                new JustRecompense(), new Righteousness(), new TeachingTruth(),
                new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void ZerubbabelJoshuaAndTheRemnantObeyTheVoiceOfTheLordAndFearHim_IsRighteous()
        {
            // "Then Zerubbabel ... and Joshua ... with all the remnant ... obeyed the voice of the LORD ... and the people did fear before the LORD." (Haggai 1:12)
            // The governor, the high priest, and the people heed the prophet and turn back to the work in holy fear.
            OracleHarness.Righteous(OracleHarness.Witness("Zerubbabel, Joshua, and all the remnant obey the voice of the LORD and fear before Him",
                new ObedienceToGod(), new Repentance(), new Humility(),
                new Worship(), new CourageousFaith()));
        }

        [Fact]
        public void GodDeclaresIAmWithYouToReassureHisDiscouragedPeople_IsADivineAct()
        {
            // "I am with you, saith the LORD." (Haggai 1:13)
            // Through His messenger God speaks His abiding presence over the people who have turned back to obey.
            OracleHarness.DivineAct(OracleHarness.Witness("God declares through Haggai, I am with you, to His people who have returned to the work",
                new Fidelity(), new CovenantFaithfulness(), new Protection(),
                new Goodness(), new Compassion()));
        }

        [Fact]
        public void GodStirsTheSpiritOfTheLeadersAndPeopleToWorkOnHisHouse_IsADivineAct()
        {
            // "And the LORD stirred up the spirit of Zerubbabel ... and ... Joshua ... and ... all the remnant; and they came and did work in the house of the LORD." (Haggai 1:14)
            // God Himself awakens their hearts so that they take up the labor on His house.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD stirs up the spirit of Zerubbabel, Joshua, and the remnant so they come and work on the house of God",
                new CovenantFaithfulness(), new Fidelity(), new Goodness(),
                new PromiseFulfilled(), new Righteousness()));
        }

        [Fact]
        public void GodCommandsBeStrongAndWorkForMySpiritRemainsAmongYou_IsADivineAct()
        {
            // "Be strong ... and work: for I am with you ... my spirit remaineth among you: fear ye not." (Haggai 2:4-5)
            // To the discouraged builders God pledges His presence and abiding Spirit, charging them to take courage.
            OracleHarness.DivineAct(OracleHarness.Witness("God commands the builders to be strong and work, for He is with them and His Spirit remains among them",
                new Fidelity(), new CovenantFaithfulness(), new Protection(),
                new Goodness(), new PromiseFulfilled()));
        }

        [Fact]
        public void GodPromisesTheLatterGloryOfTheHouseAndPeaceInThisPlace_IsADivineAct()
        {
            // "The glory of this latter house shall be greater than of the former ... and in this place will I give peace." (Haggai 2:9)
            // God promises a surpassing glory for His house and the gift of His own peace within it.
            OracleHarness.DivineAct(OracleHarness.Witness("God promises that the latter glory of this house shall surpass the former and that He will give peace in this place",
                new PromiseFulfilled(), new Peace(), new CovenantFaithfulness(),
                new Goodness(), new Fidelity()));
        }

        [Fact]
        public void ThePeoplesWorkAndOfferingsAreDefiledByTheirUncleanness_IsSin()
        {
            // "So is this people ... saith the LORD; and so is every work of their hands; and that which they offer there is unclean." (Haggai 2:14)
            // By the priestly ruling, uncleanness spreads by touch, so their neglectful works and offerings are defiled.
            OracleHarness.Sin(OracleHarness.Grounded("The people's works and offerings are pronounced unclean and defiled because of their uncleanness",
                Grounding.InIdol("their own uncleanness"),
                new Defilement(), new Impurity(), new Disobedience(),
                new WithholdingWhatIsDue(), new Indifference()));
            // capital vice (Defilement) drives disorder to its limit
            Assert.Equal(1.0, OracleHarness.Grounded("The people's works and offerings are pronounced unclean and defiled because of their uncleanness",
                Grounding.InIdol("their own uncleanness"),
                new Defilement(), new Impurity(), new Disobedience(),
                new WithholdingWhatIsDue(), new Indifference()).Disorder);
        }

        [Fact]
        public void GodPromisesToBlessHisPeopleFromTheDayTheFoundationIsLaid_IsADivineAct()
        {
            // "Consider now from this day ... from the day that the foundation of the LORD's temple was laid ... from this day will I bless you." (Haggai 2:18-19)
            // Where He had withheld, God now turns to bless His people from the day they resume His work.
            OracleHarness.DivineAct(OracleHarness.Witness("God promises that from the day the temple foundation is laid He will bless His people",
                new Generosity(), new Goodness(), new PromiseFulfilled(),
                new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void GodDeclaresHeWillShakeTheHeavensAndOverthrowTheKingdoms_IsADivineAct()
        {
            // "I will shake the heavens and the earth; And I will overthrow the throne of kingdoms ... every one by the sword of his brother." (Haggai 2:21-22)
            // God announces His sovereign judgment, toppling the powers of the nations in His appointed day.
            OracleHarness.DivineAct(OracleHarness.Witness("God declares He will shake the heavens and the earth and overthrow the thrones and kingdoms of the nations",
                new JustRecompense(), new Righteousness(), new Deliverance(),
                new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void GodChoosesZerubbabelAsHisSignetAndServant_IsADivineAct()
        {
            // "I will take thee, O Zerubbabel ... my servant ... and will make thee as a signet: for I have chosen thee." (Haggai 2:23)
            // God sets His chosen servant as a signet ring, a sign of His covenant favor and the promise to come.
            OracleHarness.DivineAct(OracleHarness.Witness("God chooses Zerubbabel His servant and makes him as a signet ring",
                new CovenantFaithfulness(), new PromiseFulfilled(), new Fidelity(),
                new Goodness(), new Protection()));
        }
    }
}
