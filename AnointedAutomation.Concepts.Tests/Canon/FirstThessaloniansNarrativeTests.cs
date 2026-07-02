// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // 1 Thessalonians narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class FirstThessaloniansNarrativeTests
    {
        [Fact]
        public void ThessaloniansTurnFromIdolsToServeTheLivingGod_IsRighteous()
        {
            // "You turned to God from idols to serve the living and true God." (1 Thessalonians 1:9)
            OracleHarness.Righteous(OracleHarness.Witness("the Thessalonians forsake idols and worship the living God", new Repentance(), new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void TheirWorkOfFaithAndLaborOfLove_IsRighteous()
        {
            // "Your work of faith and labor of love and steadfastness of hope." (1 Thessalonians 1:3)
            OracleHarness.Righteous(OracleHarness.Witness("the church labors in faith, love, and steadfast hope", new Fidelity(), new Kindness(), new Endurance()));
        }

        [Fact]
        public void PaulPreachesTheGospelGentlyNotToPleaseMen_IsRighteous()
        {
            // "We were gentle among you ... not trying to please men but God." (1 Thessalonians 2:4-7)
            OracleHarness.Righteous(OracleHarness.Witness("Paul declares the gospel gently, seeking to please God", new Gentleness(), new TeachingTruth(), new ObedienceToGod()));
        }

        [Fact]
        public void PaulLaborsNightAndDaySoAsNotToBurdenThem_IsRighteous()
        {
            // "We worked night and day, that we might not burden any of you." (1 Thessalonians 2:9)
            OracleHarness.Righteous(OracleHarness.Witness("Paul toils night and day to burden no one", new SelfSacrifice(), new Generosity()));
        }

        [Fact]
        public void PaulExhortsThemAsAFatherCaresForHisChildren_IsRighteous()
        {
            // "Like a father with his children ... exhorting and encouraging." (1 Thessalonians 2:11-12)
            OracleHarness.Righteous(OracleHarness.Witness("Paul nurtures the church like a father his children", new Compassion(), new Kindness(), new TeachingTruth()));
        }

        [Fact]
        public void TimothyIsSentToStrengthenAndEncourageTheChurch_IsRighteous()
        {
            // "We sent Timothy ... to establish and exhort you in your faith." (1 Thessalonians 3:2)
            OracleHarness.Righteous(OracleHarness.Witness("Timothy is sent to strengthen the believers in their faith", new Protection(), new CovenantFaithfulness(), new Compassion()));
        }

        [Fact]
        public void GodIsCalledToMakeTheirLoveIncreaseAndAbound_IsADivineAct()
        {
            // "May the Lord make you increase and abound in love for one another and for all." (1 Thessalonians 3:12)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord makes their love increase and abound for all", new Kindness(), new Compassion(), new Goodness()));
        }

        [Fact]
        public void AbstainFromSexualImmoralityAndLivingInHoliness_IsRighteous()
        {
            // "This is the will of God ... that you abstain from sexual immorality ... in holiness and honor." (1 Thessalonians 4:3-4)
            OracleHarness.Righteous(OracleHarness.Witness("the believers keep the body in holiness and honor", new Purity(), new SelfControl(), new ObedienceToGod()));
        }

        [Fact]
        public void DefraudingABrotherInTheMatterOfLust_IsSin()
        {
            // "That no one transgress and wrong his brother in this matter." (1 Thessalonians 4:6)
            OracleHarness.Sin(OracleHarness.Witness("a man wrongs his brother through unbridled lust", new Lust(), new Wrongdoing()));
        }

        [Fact]
        public void LoveOneAnotherAndLiveQuietlyMindingYourOwnAffairs_IsRighteous()
        {
            // "Love one another ... aspire to live quietly ... work with your hands." (1 Thessalonians 4:9-11)
            OracleHarness.Righteous(OracleHarness.Witness("the church loves one another and lives a quiet, working life", new Kindness(), new Peace(), new SelfControl()));
        }

        [Fact]
        public void ChristWillDescendAndRaiseTheDeadInChrist_IsADivineAct()
        {
            // "The dead in Christ will rise ... we will always be with the Lord." (1 Thessalonians 4:16-17)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord descends and raises the dead in Christ to be with him", new RestoringLife(), new Deliverance(), new PromiseFulfilled()));
        }

        [Fact]
        public void EncourageOneAnotherAndBuildOneAnotherUp_IsRighteous()
        {
            // "Encourage one another and build one another up." (1 Thessalonians 5:11)
            OracleHarness.Righteous(OracleHarness.Witness("the believers encourage and build up one another", new Kindness(), new Compassion(), new Peacemaking()));
        }

        [Fact]
        public void HelpTheWeakBePatientAndRepayNoOneEvilForEvil_IsRighteous()
        {
            // "Help the weak, be patient with them all ... do good to one another and to everyone." (1 Thessalonians 5:14-15)
            OracleHarness.Righteous(OracleHarness.Witness("the church helps the weak, repays no evil, and does good to all", new DefendingTheWeak(), new Patience(), new Goodness()));
        }

        [Fact]
        public void RejoiceAlwaysPrayWithoutCeasingGiveThanks_IsRighteous()
        {
            // "Rejoice always, pray without ceasing, give thanks in all circumstances." (1 Thessalonians 5:16-18)
            OracleHarness.Righteous(OracleHarness.Witness("the believers rejoice, pray, and give thanks always", new Joy(), new Worship(), new Fidelity()));
        }

        [Fact]
        public void TheGodOfPeaceSanctifiesThemCompletelyAndIsFaithful_IsADivineAct()
        {
            // "May the God of peace sanctify you completely ... He who calls you is faithful." (1 Thessalonians 5:23-24)
            OracleHarness.DivineAct(OracleHarness.Witness("the God of peace sanctifies them wholly and keeps his promise", new Peace(), new Fidelity(), new PromiseFulfilled()));
        }
    }
}
