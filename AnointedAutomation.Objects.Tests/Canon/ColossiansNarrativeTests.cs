// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Colossians narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class ColossiansNarrativeTests
    {
        [Fact]
        public void ChristIsTheImageAndAllThingsCreatedInHim_IsADivineAct()
        {
            // "He is the image of the invisible God ... by him all things were created." (Colossians 1:15-17)
            OracleHarness.DivineAct(OracleHarness.Witness("in Christ all things are created and hold together", new CreationOrder()));
        }

        [Fact]
        public void ChristReconcilesAllThingsByTheBloodOfHisCross_IsADivineAct()
        {
            // "making peace through his blood, shed on the cross." (Colossians 1:19-22)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ reconciles all things by the blood of the cross", new Atonement(), new Peacemaking(), new Deliverance()));
        }

        [Fact]
        public void ChristCancelsTheRecordOfDebtNailingItToTheCross_IsADivineAct()
        {
            // "He forgave us all our sins, having canceled the charge of our legal indebtedness ... nailing it to the cross." (Colossians 2:13-15)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ forgives all sins and cancels the record of debt", new Forgiveness(), new Pardon()));
        }

        [Fact]
        public void PaulRejoicesInHisSufferingsForTheChurch_IsRighteous()
        {
            // "Now I rejoice in what I am suffering for you, and I fill up ... Christ's afflictions, for the sake of his body." (Colossians 1:24)
            OracleHarness.Righteous(OracleHarness.Witness("Paul rejoices in his sufferings for the church", new SelfSacrifice(), new Joy()));
        }

        [Fact]
        public void HollowAndDeceptivePhilosophyOfHumanTradition_IsSin()
        {
            // "See to it that no one takes you captive through hollow and deceptive philosophy." (Colossians 2:8)
            OracleHarness.Sin(OracleHarness.Witness("captive minds seduced by hollow and deceptive philosophy", new Deceit()));
        }

        [Fact]
        public void WorshipOfAngelsAndFalseHumility_IsSin()
        {
            // "Do not let anyone ... delights in false humility and the worship of angels." (Colossians 2:18)
            OracleHarness.Sin(OracleHarness.Witness("the worship of angels with false humility and unspiritual pride", new Idolatry(), new Pride()));
        }

        [Fact]
        public void SexualImmoralityImpurityAndLustOfTheEarthlyNature_IsSin()
        {
            // "Put to death ... sexual immorality, impurity, lust, evil desires and greed, which is idolatry." (Colossians 3:5)
            OracleHarness.Sin(OracleHarness.Witness("the earthly nature of immorality, impurity, lust, and greed", new SexualImmorality(), new Impurity(), new Lust(), new Greed()));
        }

        [Fact]
        public void AngerRageMaliceAndSlander_IsSin()
        {
            // "But now you must also rid yourselves of all such things as these: anger, rage, malice, slander, and filthy language." (Colossians 3:8)
            OracleHarness.Sin(OracleHarness.Witness("anger, rage, malice, slander, and filthy language from the lips", new FitsOfRage(), new Malice(), new Slander(), new FilthyLanguage()));
        }

        [Fact]
        public void PuttingOnCompassionKindnessHumilityAndGentleness_IsRighteous()
        {
            // "Clothe yourselves with compassion, kindness, humility, gentleness and patience." (Colossians 3:12)
            OracleHarness.Righteous(OracleHarness.Witness("the chosen clothe themselves in compassion, kindness, humility, gentleness, and patience", new Compassion(), new Kindness(), new Humility(), new Gentleness(), new Patience()));
        }

        [Fact]
        public void BearingWithAndForgivingOneAnotherAsTheLordForgave_IsRighteous()
        {
            // "Bear with each other and forgive one another ... Forgive as the Lord forgave you." (Colossians 3:13)
            OracleHarness.Righteous(OracleHarness.Witness("the church bears with and forgives one another as the Lord forgave", new Forgiveness(), new Patience()));
        }

        [Fact]
        public void LettingThePeaceOfChristRuleWithThankfulness_IsRighteous()
        {
            // "Let the peace of Christ rule in your hearts ... And be thankful." (Colossians 3:15)
            OracleHarness.Righteous(OracleHarness.Witness("the peace of Christ rules the heart in unity and thankfulness", new Peace(), new Peacemaking()));
        }

        [Fact]
        public void DoingAllInTheNameOfTheLordWithThanksgiving_IsRighteous()
        {
            // "Whatever you do, whether in word or deed, do it all in the name of the Lord Jesus, giving thanks." (Colossians 3:17)
            OracleHarness.Righteous(OracleHarness.Witness("believers do all in the name of the Lord, giving thanks to God", new Worship(), new Fidelity()));
        }

        [Fact]
        public void DevotingToPrayerWatchfulAndThankful_IsRighteous()
        {
            // "Devote yourselves to prayer, being watchful and thankful." (Colossians 4:2)
            OracleHarness.Righteous(OracleHarness.Witness("the church devotes itself to prayer, watchful and thankful", new Worship(), new Endurance()));
        }
    }
}
