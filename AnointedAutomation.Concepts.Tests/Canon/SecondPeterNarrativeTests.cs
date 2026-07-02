// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // 2 Peter narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class SecondPeterNarrativeTests
    {
        [Fact]
        public void DivinePowerGrantsAllThingsForLifeAndGodliness_IsADivineAct()
        {
            // "His divine power has granted to us all things that pertain to life and godliness ... his precious and very great promises." (2 Peter 1:3-4)
            OracleHarness.DivineAct(OracleHarness.Witness("God's divine power graciously grants all things for life and godliness through His precious promises", new Generosity(), new PromiseFulfilled(), new Goodness()));
        }

        [Fact]
        public void BelieverSupplementsFaithWithVirtueKnowledgeAndSelfControl_IsRighteous()
        {
            // "supplement your faith with virtue ... self-control ... steadfastness ... godliness ... brotherly affection." (2 Peter 1:5-7)
            OracleHarness.Righteous(OracleHarness.Witness("the believer adds to his faith goodness, self-control, and steadfast endurance, growing in godliness and love", new Fidelity(), new SelfControl(), new Endurance(), new Kindness()));
        }

        [Fact]
        public void MakingYourCallingAndElectionSureByDiligence_IsRighteous()
        {
            // "be all the more diligent to confirm your calling and election ... so an entrance ... will be richly provided." (2 Peter 1:10-11)
            OracleHarness.Righteous(OracleHarness.Witness("the believer is diligent to confirm his calling and election, walking faithfully so he never stumbles", new Fidelity(), new Endurance(), new ObedienceToGod()));
        }

        [Fact]
        public void TransfigurationVoiceFromTheMajesticGloryHonoringTheSon_IsADivineAct()
        {
            // "he received honor and glory from God the Father ... 'This is my beloved Son' ... on the holy mountain." (2 Peter 1:16-18)
            OracleHarness.DivineAct(OracleHarness.Witness("the Father bears witness from the Majestic Glory, honoring His beloved Son on the holy mountain", new TeachingTruth(), new Trustworthiness(), new Goodness()));
        }

        [Fact]
        public void ProphecyCameFromMenCarriedAlongByTheHolySpirit_IsADivineAct()
        {
            // "men spoke from God as they were carried along by the Holy Spirit." (2 Peter 1:19-21)
            OracleHarness.DivineAct(OracleHarness.Witness("God carries holy men along by His Spirit to speak the sure prophetic word of truth", new TeachingTruth(), new Trustworthiness(), new Fidelity()));
        }

        [Fact]
        public void FalseTeachersSecretlyBringDestructiveHeresiesForGreed_IsSin()
        {
            // "false teachers ... will secretly bring in destructive heresies ... in their greed they will exploit you with false words." (2 Peter 2:1-3)
            OracleHarness.Sin(OracleHarness.Witness("false teachers secretly bring in destructive heresies, denying the Master and in greed exploiting souls with deceit", new Deceit(), new Greed(), new Blasphemy()));
        }

        [Fact]
        public void GodCastTheSinningAngelsIntoChainsOfGloomyDarkness_IsADivineAct()
        {
            // "God did not spare angels when they sinned, but cast them into hell ... to be kept until the judgment." (2 Peter 2:4)
            OracleHarness.DivineAct(OracleHarness.Witness("God justly judges the rebellious angels, committing them to chains of gloomy darkness until the day of judgment", new JustRecompense(), new Righteousness(), new CreationOrder()));
        }

        [Fact]
        public void GodPreservedRighteousNoahWhileBringingTheFlood_IsADivineAct()
        {
            // "he preserved Noah, a herald of righteousness, with seven others, when he brought a flood." (2 Peter 2:5)
            OracleHarness.DivineAct(OracleHarness.Witness("God protects righteous Noah and seven others, delivering the herald of righteousness when He brings the flood upon the ungodly", new Protection(), new Deliverance(), new Righteousness()));
        }

        [Fact]
        public void GodRescuedRighteousLotFromSodom_IsADivineAct()
        {
            // "if he rescued righteous Lot, greatly distressed by the sensual conduct of the wicked ... the Lord knows how to rescue the godly." (2 Peter 2:7-9)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord rescues righteous Lot, tormented by lawless deeds, and knows how to deliver the godly from trial", new Deliverance(), new Protection(), new Righteousness()));
        }

        [Fact]
        public void TheUngodlyIndulgeDefilingLustAndDespiseAuthority_IsSin()
        {
            // "those who indulge in the lust of defiling passion and despise authority. Bold and willful ..." (2 Peter 2:10)
            OracleHarness.Sin(OracleHarness.Witness("the ungodly indulge in defiling lust, despise authority, and blaspheme the glorious ones in willful insolence", new Defilement(), new Rebellion(), new Blasphemy()));
            Assert.Equal(1.0, OracleHarness.Witness("the ungodly indulge in defiling lust, despise authority, and blaspheme the glorious ones in willful insolence", new Defilement(), new Rebellion(), new Blasphemy()).Disorder);
        }

        [Fact]
        public void BalaamLovedTheWagesOfUnrighteousness_IsSin()
        {
            // "they have followed the way of Balaam ... who loved gain from wrongdoing." (2 Peter 2:15-16)
            OracleHarness.Sin(OracleHarness.Witness("the false teachers follow the way of Balaam, who loved the wages of wrongdoing and greed", new Greed(), new Wrongdoing(), new Covetousness()));
        }

        [Fact]
        public void FalseTeachersPromiseFreedomWhileSlavesToCorruption_IsSin()
        {
            // "They promise them freedom, but they themselves are slaves of corruption ... entangled again in defilements." (2 Peter 2:18-20)
            OracleHarness.Sin(OracleHarness.Witness("they entice with sensual lust and empty boasts, promising freedom while enslaved to corruption and defilement", new Lust(), new Boasting(), new Defilement()));
            Assert.Equal(1.0, OracleHarness.Witness("they entice with sensual lust and empty boasts, promising freedom while enslaved to corruption and defilement", new Lust(), new Boasting(), new Defilement()).Disorder);
        }

        [Fact]
        public void ScoffersWillComeFollowingTheirOwnSinfulDesires_IsSin()
        {
            // "scoffers will come ... following their own sinful desires ... 'Where is the promise of his coming?'" (2 Peter 3:3-4)
            OracleHarness.Sin(OracleHarness.Witness("scoffers walk after their own sinful desires, mocking the promise of His coming in insolent unbelief", new Lust(), new Insolence(), new Blasphemy()));
        }

        [Fact]
        public void TheLordIsPatientNotWishingAnyShouldPerishButReachRepentance_IsADivineAct()
        {
            // "The Lord is not slow ... but is patient toward you, not wishing that any should perish, but that all should reach repentance." (2 Peter 3:9)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord is patient toward all, not wishing any to perish but that all should reach repentance", new Patience(), new Compassion(), new PromiseFulfilled()));
        }

        [Fact]
        public void BelieversAwaitNewHeavensAndNewEarthLivingHolyAndGodly_IsRighteous()
        {
            // "what sort of people ought you to be in lives of holiness and godliness ... we are waiting for new heavens and a new earth." (2 Peter 3:11-14)
            OracleHarness.Righteous(OracleHarness.Witness("the believer lives in holiness and godliness, at peace and without spot, awaiting the new heavens and new earth", new Purity(), new Peace(), new Endurance()));
        }

        [Fact]
        public void GrowInTheGraceAndKnowledgeOfTheLordJesusChrist_IsRighteous()
        {
            // "grow in the grace and knowledge of our Lord and Savior Jesus Christ." (2 Peter 3:18)
            OracleHarness.Righteous(OracleHarness.Witness("the believer grows in the grace and knowledge of the Lord, giving Him glory in steadfast faith", new Fidelity(), new Worship(), new Endurance()));
        }
    }
}
