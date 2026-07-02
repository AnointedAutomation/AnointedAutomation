// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Ephesians narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class EphesiansNarrativeTests
    {
        [Fact]
        public void GodChoosesAndAdoptsUsBeforeTheCreationOfTheWorld_IsADivineAct()
        {
            // "He chose us in him before the creation of the world ... he predestined us for adoption to sonship." (Ephesians 1:4-5)
            OracleHarness.DivineAct(OracleHarness.Witness("God chooses and adopts us in Christ before creation", new CovenantFaithfulness(), new CreationOrder(), new Kindness()));
        }

        [Fact]
        public void RedemptionThroughHisBloodTheForgivenessOfSins_IsADivineAct()
        {
            // "In him we have redemption through his blood, the forgiveness of sins." (Ephesians 1:7)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ redeems us by his blood for the forgiveness of sins", new SelfSacrifice(), new Atonement(), new Forgiveness(), new Deliverance()));
        }

        [Fact]
        public void GodRaisesChristFromTheDeadAndSeatsHimAboveAllPower_IsADivineAct()
        {
            // "He raised Christ from the dead and seated him at his right hand ... far above all rule and authority." (Ephesians 1:20-21)
            OracleHarness.DivineAct(OracleHarness.Witness("God raises Christ from the dead and exalts him over all", new RestoringLife(), new PromiseFulfilled()));
        }

        [Fact]
        public void GodMakesUsAliveWithChristWhileWeWereDeadInSins_IsADivineAct()
        {
            // "Because of his great love ... God, who is rich in mercy, made us alive with Christ even when we were dead in transgressions." (Ephesians 2:4-5)
            OracleHarness.DivineAct(OracleHarness.Witness("God who is rich in mercy makes us alive together with Christ", new Compassion(), new RestoringLife(), new Deliverance()));
        }

        [Fact]
        public void SalvationByGraceAsTheGiftOfGod_IsADivineAct()
        {
            // "For it is by grace you have been saved, through faith ... it is the gift of God." (Ephesians 2:8-9)
            OracleHarness.DivineAct(OracleHarness.Witness("God saves us by grace as his free gift", new Generosity(), new Deliverance(), new Kindness()));
        }

        [Fact]
        public void ChristMakesPeaceAndReconcilesJewAndGentileIntoOneBody_IsADivineAct()
        {
            // "He himself is our peace, who has made the two groups one ... to create in himself one new humanity ... making peace." (Ephesians 2:14-16)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ destroys the dividing wall and reconciles the two into one", new Peacemaking(), new Atonement(), new Peace()));
        }

        [Fact]
        public void WalkingInTheGoodWorksGodPreparedForUs_IsRighteous()
        {
            // "We are God's handiwork, created in Christ Jesus to do good works, which God prepared in advance for us to do." (Ephesians 2:10)
            OracleHarness.Righteous(OracleHarness.Witness("the believers walk in the good works God prepared for them", new Goodness(), new ObedienceToGod(), new Righteousness()));
        }

        [Fact]
        public void TheFormerGentileLifeWithoutHopeAndWithoutGod_IsSin()
        {
            // "Remember that ... you were separate from Christ ... without hope and without God in the world." (Ephesians 2:11-12)
            OracleHarness.Sin(OracleHarness.Grounded("the Gentiles once lived without God, following the cravings of the flesh", Grounding.InIdol("the gods of the uncircumcised nations"), new Idolatry(), new Impurity()));
        }

        [Fact]
        public void LivingWorthyOfTheCallingInHumilityGentlenessAndPatience_IsRighteous()
        {
            // "Be completely humble and gentle; be patient, bearing with one another in love." (Ephesians 4:2-3)
            OracleHarness.Righteous(OracleHarness.Witness("the church keeps the unity of the Spirit in humility, gentleness, and patience", new Humility(), new Gentleness(), new Patience(), new Peacemaking()));
        }

        [Fact]
        public void SpeakingTheTruthInLoveToBuildUpTheBody_IsRighteous()
        {
            // "Speaking the truth in love, we will grow to become in every respect the mature body." (Ephesians 4:15)
            OracleHarness.Righteous(OracleHarness.Witness("the body grows up as it speaks the truth in love", new TeachingTruth(), new RejoicingInTruth(), new Kindness()));
        }

        [Fact]
        public void TheGentileLifeGivenOverToSensualityAndImpurity_IsSin()
        {
            // "Having lost all sensitivity, they have given themselves over to sensuality so as to indulge in every kind of impurity, with a continual lust for more." (Ephesians 4:19)
            OracleHarness.Sin(OracleHarness.Witness("the Gentiles give themselves over to sensuality, impurity, and greed", new Debauchery(), new Impurity(), new Greed(), new Lust()));
        }

        [Fact]
        public void FallingBackIntoFalsehoodTheftRageAndSlander_IsSin()
        {
            // "Get rid of all bitterness, rage and anger ... brawling and slander, along with every form of malice." (Ephesians 4:25, 28, 31)
            OracleHarness.Sin(OracleHarness.Witness("the old self lives in falsehood, theft, fits of rage, slander, and malice", new Deceit(), new Theft(), new FitsOfRage(), new Slander(), new Malice()));
        }

        [Fact]
        public void BeingKindForgivingOneAnotherAsGodForgaveYou_IsRighteous()
        {
            // "Be kind and compassionate to one another, forgiving each other, just as in Christ God forgave you." (Ephesians 4:32)
            OracleHarness.Righteous(OracleHarness.Witness("the believers are kind, compassionate, and forgiving as God forgave them", new Kindness(), new Compassion(), new Forgiveness()));
        }

        [Fact]
        public void SexualImmoralityImpurityAndGreedHaveNoPlaceAmongTheSaints_IsSin()
        {
            // "But among you there must not be even a hint of sexual immorality, or of any kind of impurity, or of greed." (Ephesians 5:3-5)
            OracleHarness.Sin(OracleHarness.Witness("the impure indulge in immorality, impurity, greed, and obscene talk", new SexualImmorality(), new Impurity(), new Greed(), new FilthyLanguage(), new Idolatry()));
        }

        [Fact]
        public void DrunkennessWithWineLeadingToDebauchery_IsSin()
        {
            // "Do not get drunk on wine, which leads to debauchery." (Ephesians 5:18)
            OracleHarness.Sin(OracleHarness.Witness("the foolish get drunk on wine, which leads to debauchery", new Drunkenness(), new Debauchery()));
        }

        [Fact]
        public void ChristLovesTheChurchAndGivesHimselfUpForHer_IsADivineAct()
        {
            // "Christ loved the church and gave himself up for her to make her holy, cleansing her ... without stain or wrinkle." (Ephesians 5:25-27)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ loves the church and gives himself up to sanctify her", new SelfSacrifice(), new Atonement(), new Purity(), new CovenantFaithfulness()));
        }

        [Fact]
        public void ChildrenObeyingParentsAndFathersNotProvoking_IsRighteous()
        {
            // "Children, obey your parents in the Lord ... Fathers, do not exasperate your children; instead, bring them up in the ... instruction of the Lord." (Ephesians 6:1-4)
            OracleHarness.Righteous(OracleHarness.Witness("children obey their parents and fathers raise them gently in the Lord", new ObedienceToGod(), new Gentleness(), new Kindness()));
        }

        [Fact]
        public void StandingFirmInTheArmorOfGodAgainstTheDevilsSchemes_IsRighteous()
        {
            // "Put on the full armor of God, so that you can take your stand against the devil's schemes." (Ephesians 6:11-17)
            OracleHarness.Righteous(OracleHarness.Witness("the believer stands firm in the armor of God, girded with truth and righteousness", new CourageousFaith(), new Righteousness(), new RejoicingInTruth(), new Endurance()));
        }
    }
}
