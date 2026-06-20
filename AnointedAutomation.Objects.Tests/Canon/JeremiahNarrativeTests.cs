// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Jeremiah narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JeremiahNarrativeTests
    {
        [Fact]
        public void GodCallsAndConsecratesJeremiahBeforeBirth_IsADivineAct()
        {
            // Before Jeremiah was formed in the womb the LORD knew, consecrated, and appointed him a prophet (Jeremiah 1:4-10).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD consecrates Jeremiah and appoints him a prophet to the nations",
                new CreationOrder(), new Protection(), new CovenantFaithfulness()));
        }

        [Fact]
        public void JudahForsakesGodForBrokenCisterns_IsSin()
        {
            // My people have forsaken Me, the fountain of living waters, for broken cisterns that hold no water (Jeremiah 2:13).
            OracleHarness.Sin(OracleHarness.Witness("Judah forsakes the LORD for worthless idols",
                new Idolatry(), new CovenantBreaking()));
        }

        [Fact]
        public void GodPleadsWithBackslidingIsraelToReturn_IsADivineAct()
        {
            // Return, faithless Israel, declares the LORD; I will not look on you in anger, for I am merciful (Jeremiah 3:12-14).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD pleads with faithless Israel to return and be forgiven",
                new Compassion(), new Forgiveness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void JudahsLeadersOppressTheFatherlessAndNeedy_IsSin()
        {
            // Among My people are wicked men; they grow rich and do not defend the orphan's cause or the rights of the needy (Jeremiah 5:26-28).
            OracleHarness.Sin(OracleHarness.Witness("the wealthy of Judah oppress the fatherless and the needy",
                new Oppression(), new Greed(), new WithholdingWhatIsDue()));
        }

        [Fact]
        public void JerusalemBurnsTheirSonsToBaalInTheValley_IsSin()
        {
            // They built the high places of Baal to burn their sons in the fire, which I never commanded (Jeremiah 7:31; 19:5).
            Resolution r = OracleHarness.Grounded("Judah sacrifices its children to Baal in the Valley of Hinnom",
                Grounding.InIdol("Baal"), new ChildSacrifice(), new Idolatry(), new Bloodshed());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheProphetsAndPriestsHealTheWoundDeceitfully_IsSin()
        {
            // From prophet to priest everyone deals falsely, crying Peace, peace, when there is no peace (Jeremiah 6:13-14; 8:10-11).
            OracleHarness.Sin(OracleHarness.Witness("priests and prophets deal falsely and cry peace where there is none",
                new Deceit(), new Greed(), new FalseWitness()));
        }

        [Fact]
        public void GodPromisesAnEverlastingCovenantWrittenOnTheHeart_IsADivineAct()
        {
            // Behold, the days come when I will make a new covenant, write My law on their hearts, and forgive their iniquity (Jeremiah 31:31-34).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD promises a new covenant and forgives their iniquity",
                new PromiseFulfilled(), new CovenantFaithfulness(), new Forgiveness(), new Healing()));
        }

        [Fact]
        public void GodPromisesToBringJudahHomeAfterSeventyYears_IsADivineAct()
        {
            // When seventy years are completed I will visit you and bring you back to this place, plans for welfare and a future (Jeremiah 29:10-14).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD promises to gather His people home after seventy years",
                new PromiseFulfilled(), new Deliverance(), new CovenantFaithfulness()));
        }

        [Fact]
        public void JeremiahFaithfullyPreachesDespitePersecution_IsRighteous()
        {
            // Though beaten, put in stocks, and threatened with death, Jeremiah keeps speaking the LORD's word (Jeremiah 20:1-9; 26:12-15).
            OracleHarness.Righteous(OracleHarness.Witness("Jeremiah keeps proclaiming the LORD's word under persecution",
                new CourageousFaith(), new TeachingTruth(), new Endurance(), new Fidelity()));
        }

        [Fact]
        public void KingJehoiakimCutsAndBurnsTheScrollOfTheWord_IsSin()
        {
            // As Jehudi read, the king cut the scroll and threw it into the fire, refusing the word of the LORD (Jeremiah 36:23-25).
            OracleHarness.Sin(OracleHarness.Witness("King Jehoiakim cuts and burns the scroll of the LORD's word",
                new Rebellion(), new Blasphemy(), new Disobedience()));
        }

        [Fact]
        public void JeremiahBuysTheFieldAtAnathothInHope_IsRighteous()
        {
            // While the city is besieged, Jeremiah buys the field at Anathoth as a sign that houses and fields shall again be bought (Jeremiah 32:6-15).
            OracleHarness.Righteous(OracleHarness.Witness("Jeremiah redeems the field at Anathoth as a sign of restoration",
                new CourageousFaith(), new Trust(), new ObedienceToGod()));
        }

        [Fact]
        public void OfficialsCastJeremiahIntoTheMiryCistern_IsSin()
        {
            // The officials, hating his words, lower Jeremiah into a cistern of mud to die of hunger (Jeremiah 38:4-6).
            OracleHarness.Sin(OracleHarness.Witness("the officials cast Jeremiah into the miry cistern to die",
                new Hatred(), new Oppression(), new Malice()));
        }

        [Fact]
        public void EbedMelechRescuesJeremiahFromTheCistern_IsRighteous()
        {
            // Ebed-melech the Cushite pleads for Jeremiah and lifts him from the cistern with rags and ropes (Jeremiah 38:7-13).
            OracleHarness.Righteous(OracleHarness.Witness("Ebed-melech mercifully rescues Jeremiah from the cistern",
                new Compassion(), new Deliverance(), new DefendingTheWeak(), new Kindness()));
        }
    }
}
