// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // 1 Timothy narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class FirstTimothyNarrativeTests
    {
        [Fact]
        public void PaulChargesTimothyToSilenceFalseTeachers_IsRighteous()
        {
            // "Command certain people not to teach false doctrines any longer." (1 Timothy 1:3-4)
            OracleHarness.Righteous(OracleHarness.Witness("Timothy is charged to guard sound teaching", new TeachingTruth(), new Protection()));
        }

        [Fact]
        public void TheFalseTeachersPromoteDisputesAndMyths_IsSin()
        {
            // "Myths and endless genealogies ... promote controversial speculations." (1 Timothy 1:4-6)
            OracleHarness.Sin(OracleHarness.Witness("the false teachers stir up empty controversy", new Deceit(), new Discord()));
        }

        [Fact]
        public void ChristCameIntoTheWorldToSaveSinners_IsADivineAct()
        {
            // "Christ Jesus came into the world to save sinners, of whom I am the worst." (1 Timothy 1:15)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ comes to save sinners", new Deliverance(), new SelfSacrifice()));
        }

        [Fact]
        public void ChristShowsMercyToPaulTheFormerBlasphemer_IsADivineAct()
        {
            // "I was shown mercy ... the grace of our Lord was poured out on me abundantly." (1 Timothy 1:13-16)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ pours out mercy on the chief of sinners", new Pardon(), new Compassion()));
        }

        [Fact]
        public void PrayingForAllPeopleAndForKings_IsRighteous()
        {
            // "I urge ... petitions, prayers ... be made for all people, for kings and all those in authority." (1 Timothy 2:1-2)
            OracleHarness.Righteous(OracleHarness.Witness("the church intercedes for all people and rulers", new Worship(), new Peacemaking()));
        }

        [Fact]
        public void GodWantsAllPeopleToBeSavedThroughTheOneMediator_IsADivineAct()
        {
            // "God our Savior, who wants all people to be saved ... one mediator ... Christ Jesus." (1 Timothy 2:3-6)
            OracleHarness.DivineAct(OracleHarness.Witness("God wills the salvation of all through the one mediator", new Deliverance(), new SelfSacrifice(), new Atonement()));
        }

        [Fact]
        public void TheOverseerMustBeAboveReproachAndSelfControlled_IsRighteous()
        {
            // "Now the overseer is to be above reproach ... temperate, self-controlled ... hospitable." (1 Timothy 3:2-3)
            OracleHarness.Righteous(OracleHarness.Witness("the overseer lives a blameless life", new SelfControl(), new Hospitality(), new Gentleness()));
        }

        [Fact]
        public void DeaconsMustBeWorthyOfRespectAndSincere_IsRighteous()
        {
            // "Deacons ... are to be worthy of respect, sincere ... keep hold of the deep truths." (1 Timothy 3:8-9)
            OracleHarness.Righteous(OracleHarness.Witness("the deacons serve with sincere and tested faith", new Trustworthiness(), new Fidelity()));
        }

        [Fact]
        public void GreatIsTheMysteryOfGodlinessChristRevealed_IsADivineAct()
        {
            // "He appeared in the flesh ... was taken up in glory." (1 Timothy 3:16)
            OracleHarness.DivineAct(OracleHarness.Witness("God is manifested in the flesh and glorified", new SelfSacrifice(), new Righteousness()));
        }

        [Fact]
        public void DeceivingSpiritsForbidMarriageAndFoods_IsSin()
        {
            // "Some will abandon the faith and follow ... things taught by demons ... liars." (1 Timothy 4:1-3)
            OracleHarness.Sin(OracleHarness.Grounded("apostates follow deceiving spirits and hypocritical lies", Grounding.InIdol("deceiving spirits"), new Deceit(), new Rebellion()));
        }

        [Fact]
        public void TimothySetsAnExampleInSpeechAndPurity_IsRighteous()
        {
            // "Set an example ... in speech, in conduct, in love, in faith and in purity." (1 Timothy 4:12)
            OracleHarness.Righteous(OracleHarness.Witness("Timothy is an example in love, faith, and purity", new Purity(), new Fidelity(), new Kindness()));
        }

        [Fact]
        public void CaringForWidowsWhoAreTrulyInNeed_IsRighteous()
        {
            // "Give proper recognition to those widows who are really in need." (1 Timothy 5:3-5)
            OracleHarness.Righteous(OracleHarness.Witness("the church honors and provides for true widows", new HonoringTheVulnerable(), new Compassion()));
        }

        [Fact]
        public void RefusingToProvideForOnesOwnHousehold_IsSin()
        {
            // "Anyone who does not provide for ... their own household has denied the faith." (1 Timothy 5:8)
            OracleHarness.Sin(OracleHarness.Witness("a believer abandons his own family in need", new WithholdingWhatIsDue(), new Heartlessness()));
        }

        [Fact]
        public void TheLoveOfMoneyDrivesPeopleFromTheFaith_IsSin()
        {
            // "The love of money is a root of all kinds of evil ... pierced themselves with many griefs." (1 Timothy 6:9-10)
            OracleHarness.Sin(OracleHarness.Witness("the love of money leads people into ruin", new Greed(), new Covetousness()));
        }

        [Fact]
        public void TheRichAreToBeGenerousAndDoGood_IsRighteous()
        {
            // "Command them ... to be rich in good deeds, and to be generous and willing to share." (1 Timothy 6:17-18)
            OracleHarness.Righteous(OracleHarness.Witness("the rich are generous, rich in good deeds, willing to share", new Generosity(), new Goodness(), new Humility()));
        }

        [Fact]
        public void TimothyFightsTheGoodFightAndPursuesRighteousness_IsRighteous()
        {
            // "Pursue righteousness, godliness, faith, love, endurance and gentleness." (1 Timothy 6:11-12)
            OracleHarness.Righteous(OracleHarness.Witness("Timothy pursues righteousness and fights the good fight of faith", new Righteousness(), new CourageousFaith(), new Endurance(), new Gentleness()));
        }
    }
}
