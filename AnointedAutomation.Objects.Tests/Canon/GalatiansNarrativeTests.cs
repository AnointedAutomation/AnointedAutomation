// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Galatians narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class GalatiansNarrativeTests
    {
        [Fact]
        public void ChristGaveHimselfToRescueFromThePresentEvilAge_IsADivineAct()
        {
            // "Christ ... gave himself for our sins to rescue us from the present evil age." (Galatians 1:3-4)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ gives himself to deliver us", new SelfSacrifice(), new Deliverance()));
        }

        [Fact]
        public void PreachingADifferentGospel_IsSin()
        {
            // "Some ... are trying to pervert the gospel of Christ. ... let them be under God's curse!" (Galatians 1:7-9)
            OracleHarness.Sin(OracleHarness.Witness("the agitators pervert the gospel of Christ", new Deceit(), new SowingDiscord()));
        }

        [Fact]
        public void PaulPreachesTheFaithHeOnceTriedToDestroy_IsRighteous()
        {
            // "The man who formerly persecuted us is now preaching the faith he once tried to destroy." (Galatians 1:23-24)
            OracleHarness.Righteous(OracleHarness.Witness("the former persecutor now proclaims the truth", new Repentance(), new TeachingTruth()));
        }

        [Fact]
        public void RememberingThePoorInTheGospelAgreement_IsRighteous()
        {
            // "All they asked was that we should continue to remember the poor." (Galatians 2:10)
            OracleHarness.Righteous(OracleHarness.Witness("the apostles charge Paul to remember the poor", new Generosity(), new Compassion()));
        }

        [Fact]
        public void PeterWithdrawsFromTheGentilesInFearOfTheCircumcisionGroup_IsSin()
        {
            // "He began to draw back ... fearing those of the circumcision group ... led astray by their hypocrisy." (Galatians 2:12-13)
            OracleHarness.Sin(OracleHarness.Witness("Peter withdraws from the Gentile table in hypocrisy", new Deceit(), new Faction()));
        }

        [Fact]
        public void PaulOpposesPeterToHisFaceForTheTruthOfTheGospel_IsRighteous()
        {
            // "I opposed him to his face, because he stood condemned ... not acting in line with the truth of the gospel." (Galatians 2:11, 14)
            OracleHarness.Righteous(OracleHarness.Witness("Paul confronts Peter for the truth of the gospel", new CourageousFaith(), new TeachingTruth()));
        }

        [Fact]
        public void ChristLovedMeAndGaveHimselfForMe_IsADivineAct()
        {
            // "The Son of God, who loved me and gave himself for me." (Galatians 2:20)
            OracleHarness.DivineAct(OracleHarness.Witness("the Son of God loves and gives himself for me", new SelfSacrifice(), new Atonement()));
        }

        [Fact]
        public void AbrahamBelievedGodAndItWasCreditedAsRighteousness_IsRighteous()
        {
            // "Abraham believed God, and it was credited to him as righteousness." (Galatians 3:6)
            OracleHarness.Righteous(OracleHarness.Witness("Abraham trusts God and is counted righteous", new Trust(), new CourageousFaith()));
        }

        [Fact]
        public void ChristRedeemsUsFromTheCurseOfTheLaw_IsADivineAct()
        {
            // "Christ redeemed us from the curse of the law by becoming a curse for us." (Galatians 3:13-14)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ redeems us by bearing the curse", new SelfSacrifice(), new Deliverance(), new PromiseFulfilled()));
        }

        [Fact]
        public void GodSendsHisSonToRedeemAndAdoptUsAsHeirs_IsADivineAct()
        {
            // "God sent his Son ... to redeem those under the law, that we might receive adoption to sonship." (Galatians 4:4-7)
            OracleHarness.DivineAct(OracleHarness.Witness("God redeems us and adopts us as sons and heirs", new Deliverance(), new PromiseFulfilled(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TurningBackToTheWeakElementalForces_IsSin()
        {
            // "How is it that you are turning back to those weak and miserable forces? ... slaves all over again." (Galatians 4:9)
            OracleHarness.Sin(OracleHarness.Grounded("the Galatians turn back to enslaving elemental forces", Grounding.InIdol("the weak and miserable elemental forces"), new Idolatry()));
        }

        [Fact]
        public void TheWholeLawIsFulfilledInLovingYourNeighbor_IsRighteous()
        {
            // "The entire law is fulfilled in keeping this one command: 'Love your neighbor as yourself.'" (Galatians 5:14)
            OracleHarness.Righteous(OracleHarness.Witness("the law is fulfilled in loving the neighbor", new Kindness(), new SelfSacrifice(), new CovenantFaithfulness()));
        }

        [Fact]
        public void BitingAndDevouringOneAnother_IsSin()
        {
            // "If you bite and devour each other ... you will be destroyed by each other." (Galatians 5:15)
            OracleHarness.Sin(OracleHarness.Witness("the Galatians bite and devour one another", new Discord(), new Hatred()));
        }

        [Fact]
        public void TheWorksOfTheFlesh_IsSin()
        {
            // "The acts of the flesh ... sexual immorality, impurity, debauchery ... hatred, discord, jealousy, fits of rage." (Galatians 5:19-21)
            OracleHarness.Sin(OracleHarness.Witness("the acts of the flesh are made plain", new SexualImmorality(), new Impurity(), new Debauchery(), new Idolatry(), new Sorcery(), new Hatred(), new Discord(), new Jealousy(), new FitsOfRage(), new SelfishAmbition(), new Faction(), new Envy(), new Drunkenness(), new Carousing()));
        }

        [Fact]
        public void TheFruitOfTheSpirit_IsRighteous()
        {
            // "The fruit of the Spirit is love, joy, peace, forbearance, kindness, goodness, faithfulness, gentleness and self-control." (Galatians 5:22-23)
            OracleHarness.Righteous(OracleHarness.Witness("the fruit of the Spirit is borne in the believer", new Kindness(), new Joy(), new Peace(), new Patience(), new Goodness(), new Fidelity(), new Gentleness(), new SelfControl()));
        }

        [Fact]
        public void CarryingEachOthersBurdensFulfillsTheLawOfChrist_IsRighteous()
        {
            // "Carry each other's burdens, and in this way you will fulfill the law of Christ." (Galatians 6:2)
            OracleHarness.Righteous(OracleHarness.Witness("the believers carry one another's burdens", new Compassion(), new SelfSacrifice(), new Gentleness()));
        }

        [Fact]
        public void RestoringTheCaughtBrotherGently_IsRighteous()
        {
            // "If someone is caught in a sin ... restore that person gently." (Galatians 6:1)
            OracleHarness.Righteous(OracleHarness.Witness("the spiritual restore the fallen brother gently", new Gentleness(), new Forgiveness(), new Humility()));
        }

        [Fact]
        public void DoingGoodToAllEspeciallyTheHousehold_IsRighteous()
        {
            // "As we have opportunity, let us do good to all people, especially ... the family of believers." (Galatians 6:9-10)
            OracleHarness.Righteous(OracleHarness.Witness("the church does good to all without growing weary", new Goodness(), new Generosity(), new Endurance()));
        }
    }
}
