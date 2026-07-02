// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // 2 Timothy narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class SecondTimothyNarrativeTests
    {
        [Fact]
        public void GodSavesAndCallsUsByGraceNotWorks_IsADivineAct()
        {
            // "He has saved us and called us to a holy life ... because of his own purpose and grace." (2 Timothy 1:9)
            OracleHarness.DivineAct(OracleHarness.Witness("God saves and calls his people by grace", new Deliverance(), new Fidelity()));
        }

        [Fact]
        public void ChristJesusDestroysDeathAndBringsLifeToLight_IsADivineAct()
        {
            // "Christ Jesus ... has destroyed death and has brought life and immortality to light through the gospel." (2 Timothy 1:10)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ abolishes death and brings life to light", new RestoringLife(), new Deliverance()));
        }

        [Fact]
        public void TimothyFannsIntoFlameTheGiftWithoutTimidity_IsRighteous()
        {
            // "Fan into flame the gift of God ... a spirit of power, of love and of self-discipline." (2 Timothy 1:6-7)
            OracleHarness.Righteous(OracleHarness.Witness("Timothy stirs up his gift in courage and self-control", new CourageousFaith(), new SelfControl()));
        }

        [Fact]
        public void PaulIsUnashamedAndGuardsTheDepositEntrustedToHim_IsRighteous()
        {
            // "I am not ashamed ... he is able to guard what I have entrusted to him." (2 Timothy 1:12)
            OracleHarness.Righteous(OracleHarness.Witness("Paul keeps faith and guards the gospel deposit", new Fidelity(), new Endurance()));
        }

        [Fact]
        public void OnesiphorusRefreshesPaulAndSeeksHimOutInChains_IsRighteous()
        {
            // "He often refreshed me and was not ashamed of my chains ... he searched hard for me." (2 Timothy 1:16-17)
            OracleHarness.Righteous(OracleHarness.Witness("Onesiphorus seeks out and refreshes the imprisoned Paul", new Compassion(), new VisitingThePrisoner()));
        }

        [Fact]
        public void EntrustingTheTeachingToFaithfulMenWhoWillTeachOthers_IsRighteous()
        {
            // "Entrust to reliable people who will also be qualified to teach others." (2 Timothy 2:2)
            OracleHarness.Righteous(OracleHarness.Witness("Paul commits the sound teaching to trustworthy men", new TeachingTruth(), new Trustworthiness()));
        }

        [Fact]
        public void TheSoldierEnduresHardshipAndCompetesByTheRules_IsRighteous()
        {
            // "Endure hardship ... competes as an athlete ... according to the rules." (2 Timothy 2:3-5)
            OracleHarness.Righteous(OracleHarness.Witness("Timothy endures as a soldier of Christ Jesus", new Endurance(), new SelfControl()));
        }

        [Fact]
        public void ChristRemainsFaithfulEvenWhenWeAreFaithless_IsADivineAct()
        {
            // "If we are faithless, he remains faithful, for he cannot disown himself." (2 Timothy 2:13)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ remains faithful and cannot disown himself", new Fidelity(), new CovenantFaithfulness()));
        }

        [Fact]
        public void QuarrelingOverWordsAndGodlessChatterThatRuinsHearers_IsSin()
        {
            // "Quarreling about words ... only ruins those who listen ... godless chatter." (2 Timothy 2:14, 16)
            OracleHarness.Sin(OracleHarness.Witness("the false teachers quarrel over words and spread godless chatter", new Discord(), new Deceit()));
        }

        [Fact]
        public void HymenaeusAndPhiletusSpreadTheLieThatTheResurrectionIsPast_IsSin()
        {
            // "They say that the resurrection has already taken place, and they destroy the faith of some." (2 Timothy 2:17-18)
            OracleHarness.Sin(OracleHarness.Witness("Hymenaeus and Philetus teach a destroying falsehood", new Deceit(), new FalseWitness()));
        }

        [Fact]
        public void TheLordsServantIsKindNotQuarrelsomeGentlyInstructingOpponents_IsRighteous()
        {
            // "Must not be quarrelsome but must be kind to everyone ... gently instruct." (2 Timothy 2:24-25)
            OracleHarness.Righteous(OracleHarness.Witness("the Lord's servant teaches gently and kindly", new Kindness(), new Gentleness(), new TeachingTruth()));
        }

        [Fact]
        public void TheGodlessOfTheLastDaysAreLoversOfSelfAndMoney_IsSin()
        {
            // "Lovers of themselves, lovers of money, boastful, proud ... unforgiving ... without self-control." (2 Timothy 3:2-3)
            OracleHarness.Sin(OracleHarness.Witness("the people of the last days are lovers of self and money", new SelfSeeking(), new Greed(), new Boasting()));
        }

        [Fact]
        public void TheLastDaysCrowdIsHeartlessUnforgivingAndUngrateful_IsSin()
        {
            // "Ungrateful, unholy, without love, unforgiving, slanderous." (2 Timothy 3:2-3)
            OracleHarness.Sin(OracleHarness.Witness("the godless are heartless, ungrateful, and unforgiving", new Heartlessness(), new Ingratitude(), new Unforgiveness()));
        }

        [Fact]
        public void AllScriptureIsGodBreathedForTrainingInRighteousness_IsADivineAct()
        {
            // "All Scripture is God-breathed ... for training in righteousness." (2 Timothy 3:16)
            OracleHarness.DivineAct(OracleHarness.Witness("God breathes out the Scriptures to train in righteousness", new TeachingTruth(), new Righteousness()));
        }

        [Fact]
        public void PaulChargesTimothyToPreachTheWordWithGreatPatience_IsRighteous()
        {
            // "Preach the word ... with great patience and careful instruction." (2 Timothy 4:2)
            OracleHarness.Righteous(OracleHarness.Witness("Timothy preaches the word with patience and teaching", new TeachingTruth(), new Patience()));
        }

        [Fact]
        public void PaulHasFoughtTheGoodFightAndKeptTheFaith_IsRighteous()
        {
            // "I have fought the good fight, I have finished the race, I have kept the faith." (2 Timothy 4:7)
            OracleHarness.Righteous(OracleHarness.Witness("Paul finishes the race and keeps the faith", new Endurance(), new Fidelity()));
        }

        [Fact]
        public void DemasDesertsPaulInLoveWithThisPresentWorld_IsSin()
        {
            // "Demas, because he loved this world, has deserted me." (2 Timothy 4:10)
            OracleHarness.Sin(OracleHarness.Witness("Demas loves this present world and deserts Paul", new Treachery(), new SelfSeeking()));
        }

        [Fact]
        public void TheLordStandsAtPaulsSideAndRescuesHimFromTheLionsMouth_IsADivineAct()
        {
            // "The Lord stood at my side ... I was delivered from the lion's mouth." (2 Timothy 4:17-18)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord stands by Paul and rescues him", new Protection(), new Deliverance()));
        }
    }
}
