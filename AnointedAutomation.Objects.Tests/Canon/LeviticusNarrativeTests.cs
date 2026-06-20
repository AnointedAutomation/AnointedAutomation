// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Leviticus narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class LeviticusNarrativeTests
    {
        [Fact]
        public void TheBurntOffering_ForAtonement_IsRighteous()
        {
            // The sacrifice is offered "to make atonement for him." (Leviticus 1:3-4)
            OracleHarness.Righteous(OracleHarness.Witness("the burnt offering for atonement",
                new Atonement(), new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void TheSinOfferingForUnintentionalSin_IsRighteous()
        {
            // The priest "shall make atonement for them, and they will be forgiven." (Leviticus 4:20)
            OracleHarness.Righteous(OracleHarness.Witness("the sin offering brings forgiveness",
                new Atonement(), new Repentance(), new Pardon()));
        }

        [Fact]
        public void AaronAndSonsObeyTheOrdinationCommand_IsRighteous()
        {
            // "Aaron and his sons did all the things the LORD commanded through Moses." (Leviticus 8:36)
            OracleHarness.Righteous(OracleHarness.Witness("Aaron and his sons obey the ordination",
                new ObedienceToGod(), new Worship(), new Fidelity()));
        }

        [Fact]
        public void TheLordsGloryConsumesTheOffering_IsADivineAct()
        {
            // "Fire came out from the presence of the LORD and consumed the burnt offering." (Leviticus 9:23-24)
            OracleHarness.DivineAct(OracleHarness.Witness("the glory of the LORD accepts the offering",
                new CovenantFaithfulness(), new Righteousness(), new PromiseFulfilled()));
        }

        [Fact]
        public void NadabAndAbihuOfferStrangeFire_IsSin()
        {
            // They offered "unauthorized fire ... contrary to his command." (Leviticus 10:1-2)
            OracleHarness.Sin(OracleHarness.Witness("Nadab and Abihu offer unauthorized fire",
                new Disobedience(), new Rebellion()));
        }

        [Fact]
        public void TheDayOfAtonementScapegoat_IsRighteous()
        {
            // The goat "carry on itself all their sins ... atonement is made for you." (Leviticus 16:21-30)
            OracleHarness.Righteous(OracleHarness.Witness("the Day of Atonement, the scapegoat bears Israel's sin",
                new Atonement(), new Pardon(), new Deliverance(), new SelfSacrifice()));
        }

        [Fact]
        public void OfferingChildrenToMolech_IsSin()
        {
            // "Do not give any of your children to be sacrificed to Molek." (Leviticus 18:21; 20:2)
            Resolution r = OracleHarness.Witness("giving children to Molek",
                new ChildSacrifice(), new Idolatry(), new Bloodshed());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void LeavingGleaningsForThePoor_IsRighteous()
        {
            // "Leave them for the poor and the foreigner." (Leviticus 19:9-10)
            OracleHarness.Righteous(OracleHarness.Witness("leaving the gleanings for the poor and the foreigner",
                new Generosity(), new FeedingTheHungry(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void LoveYourNeighborAsYourself_IsRighteous()
        {
            // "Love your neighbor as yourself. I am the LORD." (Leviticus 19:18)
            OracleHarness.Righteous(OracleHarness.Witness("love your neighbor as yourself",
                new Kindness(), new Compassion(), new Forgiveness()));
        }

        [Fact]
        public void HoldingAGrudgeAndSeekingRevenge_IsSin()
        {
            // "Do not seek revenge or bear a grudge." (Leviticus 19:18a)
            OracleHarness.Sin(OracleHarness.Witness("seeking revenge and bearing a grudge",
                new Grudge(), new Unforgiveness()));
        }

        [Fact]
        public void DishonestWeightsAndMeasures_IsSin()
        {
            // "Do not use dishonest standards ... use honest scales and honest weights." (Leviticus 19:35-36)
            OracleHarness.Sin(OracleHarness.Witness("using dishonest scales and weights",
                new Deceit(), new Wrongdoing(), new WithholdingWhatIsDue()));
        }

        [Fact]
        public void RisingBeforeTheElderly_HonorsTheVulnerable_IsRighteous()
        {
            // "Stand up in the presence of the aged, show respect for the elderly." (Leviticus 19:32)
            OracleHarness.Righteous(OracleHarness.Witness("standing in honor of the aged",
                new HonoringTheVulnerable(), new Kindness()));
        }

        [Fact]
        public void BlasphemingTheNameOfTheLord_IsSin()
        {
            // The son of an Israelite woman "blasphemed the Name with a curse." (Leviticus 24:11-16)
            OracleHarness.Sin(OracleHarness.Witness("blaspheming the Name of the LORD",
                new Blasphemy(), new MisusingGodsName()));
        }

        [Fact]
        public void TheYearOfJubileeReleasesTheCaptive_IsRighteous()
        {
            // "Proclaim liberty throughout the land ... each of you is to return to your property." (Leviticus 25:10)
            OracleHarness.Righteous(OracleHarness.Witness("the Jubilee proclaims liberty and restores the poor",
                new Deliverance(), new JustRecompense(), new Generosity(), new HonoringTheVulnerable()));
        }
    }
}
