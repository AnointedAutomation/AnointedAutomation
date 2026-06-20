// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Philippians narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class PhilippiansNarrativeTests
    {
        [Fact]
        public void PaulThanksGodForThePhilippiansPartnershipInTheGospel_IsRighteous()
        {
            // "I thank my God ... because of your partnership in the gospel." (Philippians 1:3-5)
            OracleHarness.Righteous(OracleHarness.Witness("Paul gives thanks for the Philippian partnership", new Joy(), new Fidelity()));
        }

        [Fact]
        public void GodBeganAGoodWorkAndWillCompleteIt_IsADivineAct()
        {
            // "He who began a good work in you will carry it on to completion." (Philippians 1:6)
            OracleHarness.DivineAct(OracleHarness.Witness("God carries the good work to completion", new PromiseFulfilled(), new Fidelity()));
        }

        [Fact]
        public void PaulRejoicesThatChristIsPreachedEvenFromSelfishAmbition_IsRighteous()
        {
            // "Whether from false motives or true, Christ is preached. And because of this I rejoice." (Philippians 1:18)
            OracleHarness.Righteous(OracleHarness.Witness("Paul rejoices that Christ is proclaimed", new Joy(), new RejoicingInTruth()));
        }

        [Fact]
        public void SomePreachChristOutOfEnvyAndRivalry_IsSin()
        {
            // "Some preach Christ out of envy and rivalry ... out of selfish ambition." (Philippians 1:15-17)
            OracleHarness.Sin(OracleHarness.Witness("rivals preach from envy and ambition", new Envy(), new SelfishAmbition()));
        }

        [Fact]
        public void ChristEmptiedHimselfTakingTheFormOfAServant_IsADivineAct()
        {
            // "He made himself nothing, taking the very nature of a servant." (Philippians 2:6-7)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ empties himself and takes the servant's form", new Humility(), new SelfSacrifice()));
        }

        [Fact]
        public void ChristHumbledHimselfObedientToDeathOnACross_IsADivineAct()
        {
            // "He humbled himself by becoming obedient to death, even death on a cross." (Philippians 2:8)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ obeys unto death on the cross", new ObedienceToGod(), new Humility(), new SelfSacrifice()));
        }

        [Fact]
        public void DoNothingFromSelfishAmbitionButInHumilityCountOthersBetter_IsRighteous()
        {
            // "In humility value others above yourselves." (Philippians 2:3-4)
            OracleHarness.Righteous(OracleHarness.Witness("the saints count others better than themselves", new Humility(), new Kindness()));
        }

        [Fact]
        public void TimothyGenuinelyCaresForTheWelfareOfThePhilippians_IsRighteous()
        {
            // "I have no one else like him, who will show genuine concern for your welfare." (Philippians 2:20-22)
            OracleHarness.Righteous(OracleHarness.Witness("Timothy serves with genuine care for the church", new Compassion(), new Fidelity()));
        }

        [Fact]
        public void EpaphroditusRisksHisLifeToCompleteTheirService_IsRighteous()
        {
            // "He almost died for the work of Christ, risking his life." (Philippians 2:25-30)
            OracleHarness.Righteous(OracleHarness.Witness("Epaphroditus risks his life to minister to Paul", new SelfSacrifice(), new Fidelity()));
        }

        [Fact]
        public void BewareTheDogsTheEvildoersWhoMutilateTheFlesh_IsSin()
        {
            // "Watch out for those dogs, those evildoers, those mutilators of the flesh." (Philippians 3:2)
            OracleHarness.Sin(OracleHarness.Witness("the false teachers do evil and mutilate the flesh", new Wrongdoing(), new Deceit()));
        }

        [Fact]
        public void PaulCountsAllThingsLossForTheSakeOfChrist_IsRighteous()
        {
            // "I consider everything a loss because of the surpassing worth of knowing Christ." (Philippians 3:7-8)
            OracleHarness.Righteous(OracleHarness.Witness("Paul counts all things loss to gain Christ", new SelfSacrifice(), new Worship()));
        }

        [Fact]
        public void EnemiesOfTheCrossWhoseGodIsTheirBelly_IsSin()
        {
            // "Their god is their stomach ... their mind is set on earthly things." (Philippians 3:18-19)
            OracleHarness.Sin(OracleHarness.Grounded("the enemies of the cross worship the belly", Grounding.InIdol("their belly"), new Gluttony(), new Idolatry()));
        }

        [Fact]
        public void RejoiceInTheLordAlwaysWithGentlenessAndPrayer_IsRighteous()
        {
            // "Rejoice in the Lord always ... Let your gentleness be evident to all." (Philippians 4:4-6)
            OracleHarness.Righteous(OracleHarness.Witness("the saints rejoice and show gentleness to all", new Joy(), new Gentleness(), new Peace()));
        }

        [Fact]
        public void PaulHasLearnedTheSecretOfContentmentInEveryCircumstance_IsRighteous()
        {
            // "I have learned to be content whatever the circumstances." (Philippians 4:11-13)
            OracleHarness.Righteous(OracleHarness.Witness("Paul learns contentment in want and plenty", new SelfControl(), new Endurance()));
        }

        [Fact]
        public void ThePhilippiansSendAGiftToPaulsNeed_IsRighteous()
        {
            // "It was good of you to share in my troubles ... the gifts you sent." (Philippians 4:14-18)
            OracleHarness.Righteous(OracleHarness.Witness("the Philippians send a generous gift to Paul", new Generosity(), new Compassion()));
        }

        [Fact]
        public void GodWillSupplyEveryNeedAccordingToHisRiches_IsADivineAct()
        {
            // "My God will meet all your needs according to the riches of his glory." (Philippians 4:19)
            OracleHarness.DivineAct(OracleHarness.Witness("God supplies every need of his people", new Generosity(), new PromiseFulfilled()));
        }
    }
}
