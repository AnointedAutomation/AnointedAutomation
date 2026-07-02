// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Jude narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JudeNarrativeTests
    {
        [Fact]
        public void ContendingEarnestlyForTheFaithOnceDelivered_IsRighteous()
        {
            // "contend for the faith that was once for all delivered to the saints." (Jude 3)
            OracleHarness.Righteous(OracleHarness.Witness("the beloved contend earnestly for the faith once for all delivered to the saints", new CourageousFaith(), new Fidelity(), new RejoicingInTruth()));
        }

        [Fact]
        public void UngodlyIntrudersPervertingGraceAndDenyingChrist_IsSin()
        {
            // "ungodly people, who pervert the grace of our God into sensuality and deny our only Master and Lord." (Jude 4)
            OracleHarness.Sin(OracleHarness.Witness("ungodly intruders creep in to pervert grace into sensuality and deny their only Master", new Debauchery(), new Lawlessness(), new Blasphemy()));
        }

        [Fact]
        public void TheLordSavedHisPeopleOutOfTheLandOfEgypt_IsADivineAct()
        {
            // "the Lord, having saved a people out of the land of Egypt." (Jude 5)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord saves and brings His people out of the land of Egypt", new Deliverance(), new Fidelity(), new Goodness()));
        }

        [Fact]
        public void UnbelievingIsraelDestroyedForRefusingToTrustTheLord_IsSin()
        {
            // "afterward destroyed those who did not believe." (Jude 5)
            OracleHarness.Sin(OracleHarness.Witness("those saved out of Egypt afterward did not believe and rebelled against the Lord", new Disobedience(), new Rebellion(), new Ingratitude()));
        }

        [Fact]
        public void TheAngelsWhoLeftTheirProperDwelling_IsSin()
        {
            // "the angels who did not stay within their own position of authority, but left their proper dwelling." (Jude 6)
            OracleHarness.Sin(OracleHarness.Witness("the angels abandon their proper dwelling, rebelling in pride against their appointed position", new Rebellion(), new Pride(), new Disobedience()));
        }

        [Fact]
        public void SodomAndGomorrahPursuingUnnaturalDesire_IsSin()
        {
            // "Sodom and Gomorrah ... indulged in sexual immorality and pursued unnatural desire." (Jude 7)
            OracleHarness.Sin(OracleHarness.Witness("Sodom and Gomorrah indulge in sexual immorality and pursue unnatural desire, defiling themselves", new SexualImmorality(), new Impurity(), new Defilement()));
            Assert.Equal(1.0, OracleHarness.Witness("Sodom and Gomorrah indulge in sexual immorality and pursue unnatural desire, defiling themselves", new SexualImmorality(), new Impurity(), new Defilement()).Disorder);
        }

        [Fact]
        public void MichaelTheArchangelDidNotPronounceBlasphemingJudgment_IsRighteous()
        {
            // "Michael ... did not presume to pronounce a blasphemous judgment, but said, 'The Lord rebuke you.'" (Jude 9)
            OracleHarness.Righteous(OracleHarness.Witness("Michael disputes with the devil yet refuses a reviling judgment, humbly leaving the rebuke to the Lord", new Humility(), new SelfControl(), new ObedienceToGod()));
        }

        [Fact]
        public void TheWayOfCainBalaamAndKorah_IsSin()
        {
            // "they walked in the way of Cain and abandoned themselves for the sake of gain to Balaam's error and perished in Korah's rebellion." (Jude 11)
            OracleHarness.Sin(OracleHarness.Witness("they walk in the way of Cain who murdered, run after Balaam's error for gain, and perish in Korah's rebellion", new Murder(), new Greed(), new Rebellion()));
            Assert.Equal(1.0, OracleHarness.Witness("they walk in the way of Cain who murdered, run after Balaam's error for gain, and perish in Korah's rebellion", new Murder(), new Greed(), new Rebellion()).Disorder);
        }

        [Fact]
        public void ShepherdsFeedingOnlyThemselvesWithoutFear_IsSin()
        {
            // "hidden reefs ... shepherds feeding themselves; waterless clouds ... fruitless trees." (Jude 12)
            OracleHarness.Sin(OracleHarness.Witness("at the love feasts they shepherd only themselves without fear, giving nothing, caring for none", new SelfSeeking(), new Indifference(), new Heartlessness()));
        }

        [Fact]
        public void GrumblersBoastingAndFlatteringForAdvantage_IsSin()
        {
            // "grumblers, malcontents ... loud-mouthed boasters, showing favoritism to gain advantage." (Jude 16)
            OracleHarness.Sin(OracleHarness.Witness("they grumble and find fault, mouthing loud boasts and flattering people to gain advantage", new Ingratitude(), new Boasting(), new Greed()));
        }

        [Fact]
        public void BuildingUpInFaithPrayingAndKeepingInTheLoveOfGod_IsRighteous()
        {
            // "building yourselves up in your most holy faith ... keep yourselves in the love of God." (Jude 20-21)
            OracleHarness.Righteous(OracleHarness.Witness("the beloved build themselves up in the holy faith, pray in the Spirit, and keep themselves in the love of God", new Fidelity(), new Worship(), new CovenantFaithfulness()));
        }

        [Fact]
        public void HavingMercyOnDoubtersAndSnatchingThemFromTheFire_IsRighteous()
        {
            // "have mercy on those who doubt; save others by snatching them out of the fire." (Jude 22-23)
            OracleHarness.Righteous(OracleHarness.Witness("the beloved have mercy on those who doubt and snatch others out of the fire to save them", new Compassion(), new Deliverance(), new Protection()));
        }

        [Fact]
        public void GodIsAbleToKeepYouFromStumblingAndPresentYouBlameless_IsADivineAct()
        {
            // "to him who is able to keep you from stumbling and to present you blameless ... with great joy." (Jude 24-25)
            OracleHarness.DivineAct(OracleHarness.Witness("God is able to keep them from stumbling and present them blameless before His glory with great joy", new Protection(), new Deliverance(), new Joy()));
        }
    }
}
