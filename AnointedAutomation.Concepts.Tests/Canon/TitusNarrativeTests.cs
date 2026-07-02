// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Titus narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class TitusNarrativeTests
    {
        [Fact]
        public void GodWhoDoesNotLiePromisedEternalLifeBeforeTheAges_IsADivineAct()
        {
            // "in the hope of eternal life, which God, who does not lie, promised before the ages began." (Titus 1:2)
            OracleHarness.DivineAct(OracleHarness.Witness("God who never lies keeps His promise of eternal life", new PromiseFulfilled(), new Trustworthiness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TitusIsLeftInCreteToAppointElders_IsRighteous()
        {
            // "appoint elders in every town as I directed you." (Titus 1:5)
            OracleHarness.Righteous(OracleHarness.Witness("Titus sets in order what was lacking and appoints faithful elders", new CreationOrder(), new Fidelity()));
        }

        [Fact]
        public void TheOverseerMustBeHospitableAndSelfControlled_IsRighteous()
        {
            // "hospitable, a lover of good, self-controlled, upright, holy and disciplined." (Titus 1:8)
            OracleHarness.Righteous(OracleHarness.Witness("the overseer holds firm to the trustworthy word, hospitable and self-controlled", new Hospitality(), new SelfControl(), new Goodness()));
        }

        [Fact]
        public void EldersMustHoldFirmAndTeachSoundDoctrine_IsRighteous()
        {
            // "He must hold firmly to the trustworthy message ... encourage others by sound doctrine." (Titus 1:9)
            OracleHarness.Righteous(OracleHarness.Witness("the elder holds firm to the trustworthy word and teaches sound doctrine", new TeachingTruth(), new Trustworthiness()));
        }

        [Fact]
        public void TheCretanDeceiversUpsetWholeHouseholdsForGain_IsSin()
        {
            // "They must be silenced, because they are disrupting whole households by teaching ... for the sake of dishonest gain." (Titus 1:11)
            OracleHarness.Sin(OracleHarness.Witness("the rebellious deceivers ruin households for dishonest gain", new Deceit(), new Greed(), new Discord()));
        }

        [Fact]
        public void TheCretansAreLiarsEvilBrutesAndGluttons_IsSin()
        {
            // "Cretans are always liars, evil brutes, lazy gluttons." (Titus 1:12)
            OracleHarness.Sin(OracleHarness.Witness("the Cretans are liars, brutes, and lazy gluttons", new Deceit(), new Gluttony(), new Sloth()));
        }

        [Fact]
        public void TheDefiledRejectGodWhileClaimingToKnowHim_IsSin()
        {
            // "To the corrupt and unbelieving nothing is pure ... They claim to know God, but by their actions they deny him." (Titus 1:15-16)
            OracleHarness.Sin(OracleHarness.Witness("they profess to know God but by their works deny Him, their minds defiled", new Defilement(), new Disobedience()));
        }

        [Fact]
        public void OlderWomenTeachWhatIsGoodAndLoveTheirFamilies_IsRighteous()
        {
            // "they can urge the younger women to love their husbands and children, to be self-controlled and pure." (Titus 2:3-5)
            OracleHarness.Righteous(OracleHarness.Witness("older women teach what is good so the young are self-controlled and pure", new TeachingTruth(), new SelfControl(), new Purity()));
        }

        [Fact]
        public void TitusModelsIntegrityAndSoundSpeech_IsRighteous()
        {
            // "In your teaching show integrity, seriousness and soundness of speech that cannot be condemned." (Titus 2:7-8)
            OracleHarness.Righteous(OracleHarness.Witness("Titus shows integrity and sound speech as a model of good works", new Goodness(), new TeachingTruth(), new Trustworthiness()));
        }

        [Fact]
        public void TheGraceOfGodAppearedBringingSalvation_IsADivineAct()
        {
            // "the grace of God has appeared that offers salvation to all people." (Titus 2:11)
            OracleHarness.DivineAct(OracleHarness.Witness("the saving grace of God appears to all people", new Deliverance(), new Kindness()));
        }

        [Fact]
        public void ChristGaveHimselfToRedeemAndPurifyAPeople_IsADivineAct()
        {
            // "Jesus Christ, who gave himself for us to redeem us ... and to purify for himself a people." (Titus 2:13-14)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ gives Himself to redeem and purify a people for His own", new SelfSacrifice(), new Atonement(), new Purity()));
        }

        [Fact]
        public void GodSavedUsByHisMercyThroughRebirthAndRenewal_IsADivineAct()
        {
            // "he saved us ... because of his mercy ... through the washing of rebirth and renewal by the Holy Spirit." (Titus 3:4-5)
            OracleHarness.DivineAct(OracleHarness.Witness("God saves us by His mercy through the washing of rebirth and renewal", new Compassion(), new Deliverance(), new RestoringLife()));
        }

        [Fact]
        public void BelieversDevoteThemselvesToGoodWorks_IsRighteous()
        {
            // "those who have trusted in God may be careful to devote themselves to doing what is good." (Titus 3:8)
            OracleHarness.Righteous(OracleHarness.Witness("believers devote themselves to good works that profit everyone", new Goodness(), new Generosity()));
        }

        [Fact]
        public void TheDivisivePersonIsWarpedAndSelfCondemned_IsSin()
        {
            // "Warn a divisive person once, and then warn them a second time ... they are warped and sinful." (Titus 3:10-11)
            OracleHarness.Sin(OracleHarness.Witness("the divisive person stirs foolish controversies and is warped, self-condemned", new Faction(), new Discord(), new Disobedience()));
        }
    }
}
