// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Philemon narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class PhilemonNarrativeTests
    {
        [Fact]
        public void PaulThanksGodForPhilemonsLoveAndFaith_IsRighteous()
        {
            // "I always thank my God ... because I hear of your love and of your faith toward the Lord Jesus and all the saints." (Philemon 4-5)
            OracleHarness.Righteous(OracleHarness.Witness("Paul gives thanks for Philemon's love and faith toward the Lord and the saints", new Fidelity(), new Kindness(), new Worship()));
        }

        [Fact]
        public void PhilemonRefreshesTheHeartsOfTheSaints_IsRighteous()
        {
            // "the hearts of the saints have been refreshed through you, brother." (Philemon 7)
            OracleHarness.Righteous(OracleHarness.Witness("Philemon refreshes the hearts of the saints by his love", new Kindness(), new Generosity(), new Compassion()));
        }

        [Fact]
        public void PaulAppealsForLovesSakeRatherThanCommanding_IsRighteous()
        {
            // "though I am bold enough in Christ to command you ... yet for love's sake I prefer to appeal to you." (Philemon 8-9)
            OracleHarness.Righteous(OracleHarness.Witness("Paul appeals out of love and humility instead of issuing a command", new Humility(), new Kindness(), new Gentleness()));
        }

        [Fact]
        public void PaulPleadsForOnesimusHisChildInPrison_IsRighteous()
        {
            // "I appeal to you for my child, Onesimus, whose father I became in my imprisonment." (Philemon 10)
            OracleHarness.Righteous(OracleHarness.Witness("the imprisoned Paul pleads on behalf of Onesimus, begotten in his chains", new Compassion(), new DefendingTheWeak(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void PaulSendsBackOnesimusThoughItCostsHisOwnHeart_IsRighteous()
        {
            // "I am sending him back to you, sending my very heart." (Philemon 12)
            OracleHarness.Righteous(OracleHarness.Witness("Paul sends Onesimus back though he is his very heart, doing what is right", new SelfSacrifice(), new Righteousness(), new Trustworthiness()));
        }

        [Fact]
        public void PaulRefusesToActWithoutPhilemonsConsent_IsRighteous()
        {
            // "I preferred to do nothing without your consent ... not by compulsion but of your own accord." (Philemon 14)
            OracleHarness.Righteous(OracleHarness.Witness("Paul does nothing without Philemon's free consent, refusing to compel goodness", new Patience(), new Goodness(), new Gentleness()));
        }

        [Fact]
        public void OnesimusIsToBeReceivedNoLongerAsSlaveButBelovedBrother_IsRighteous()
        {
            // "no longer as a bondservant but ... as a beloved brother." (Philemon 16)
            OracleHarness.Righteous(OracleHarness.Witness("Onesimus is welcomed no longer as a slave but as a beloved brother", new Kindness(), new HonoringTheVulnerable(), new Peacemaking()));
        }

        [Fact]
        public void PaulAsksPhilemonToReceiveOnesimusAsHeWouldPaul_IsRighteous()
        {
            // "if you consider me your partner, receive him as you would receive me." (Philemon 17)
            OracleHarness.Righteous(OracleHarness.Witness("Paul asks Philemon to receive Onesimus with welcome as he would welcome Paul", new Hospitality(), new Kindness(), new Forgiveness()));
        }

        [Fact]
        public void PaulOffersToRepayWhateverOnesimusOwes_IsRighteous()
        {
            // "If he has wronged you ... charge that to my account. I, Paul, will repay it." (Philemon 18-19)
            OracleHarness.Righteous(OracleHarness.Witness("Paul takes Onesimus's debt upon himself and pledges to repay it", new SelfSacrifice(), new JustRecompense(), new Atonement()));
        }

        [Fact]
        public void PaulIsConfidentOfPhilemonsObedience_IsRighteous()
        {
            // "Confident of your obedience, I write to you, knowing that you will do even more than I say." (Philemon 21)
            OracleHarness.Righteous(OracleHarness.Witness("Paul is confident Philemon will obey and do even more than asked", new ObedienceToGod(), new Trust(), new Goodness()));
        }
    }
}
