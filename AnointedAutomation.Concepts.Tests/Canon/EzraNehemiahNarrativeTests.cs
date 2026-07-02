// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Ezra-Nehemiah narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class EzraNehemiahNarrativeTests
    {
        [Fact]
        public void GodStirsCyrusToReleaseTheExiles_IsADivineAct()
        {
            // The LORD stirs the spirit of Cyrus to fulfill His word through Jeremiah and free His people (Ezra 1:1-4).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD stirs Cyrus to release the exiles",
                new CovenantFaithfulness(), new PromiseFulfilled(), new Deliverance()));
        }

        [Fact]
        public void TheRemnantRebuildsTheAltarAndRestoresWorship_IsRighteous()
        {
            // Returned under Zerubbabel and Jeshua, they set up the altar and offer sacrifice to God (Ezra 3:1-6).
            OracleHarness.Righteous(OracleHarness.Witness("the altar rebuilt and worship restored",
                new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void ThePeopleShoutForJoyAtTheTempleFoundation_IsRighteous()
        {
            // When the foundation of the LORD's house is laid the people praise Him with a great shout (Ezra 3:10-11).
            OracleHarness.Righteous(OracleHarness.Witness("joy at the laying of the temple foundation",
                new Joy(), new Worship()));
        }

        [Fact]
        public void GodFinishesTheHouseDespiteOpposition_IsADivineAct()
        {
            // The work prospers and the temple is completed because the eye of God watches over the elders (Ezra 5-6).
            OracleHarness.DivineAct(OracleHarness.Witness("God protects and completes the rebuilding of His house",
                new Protection(), new CovenantFaithfulness()));
        }

        [Fact]
        public void EzraReadsAndTeachesTheLawToThePeople_IsRighteous()
        {
            // Ezra reads the Book of the Law and gives the sense so the people understand (Nehemiah 8:1-8).
            OracleHarness.Righteous(OracleHarness.Witness("Ezra teaches the Law to the assembly",
                new TeachingTruth(), new RejoicingInTruth()));
        }

        [Fact]
        public void ThePeopleMournAndThenRejoiceAtTheWord_IsRighteous()
        {
            // Hearing the Law the people weep in repentance, then keep the feast with great joy (Nehemiah 8:9-12).
            OracleHarness.Righteous(OracleHarness.Witness("the people repent and then rejoice at the Law",
                new Repentance(), new Joy()));
        }

        [Fact]
        public void NehemiahCourageouslyRebuildsTheWall_IsRighteous()
        {
            // Against scorn and threat Nehemiah leads the people to rebuild Jerusalem's wall, trusting God (Nehemiah 4).
            OracleHarness.Righteous(OracleHarness.Witness("Nehemiah rebuilds the wall in courageous faith",
                new CourageousFaith(), new Protection()));
        }

        [Fact]
        public void NehemiahRelievesThePoorFromUsury_IsRighteous()
        {
            // He rebukes the nobles for exacting usury and restores fields, vineyards, and money (Nehemiah 5:6-13).
            OracleHarness.Righteous(OracleHarness.Witness("Nehemiah relieves the poor and restores what was taken",
                new JustRecompense(), new Generosity()));
        }

        [Fact]
        public void TheCovenantIsRenewedInConfession_IsRighteous()
        {
            // The Levites lead a great confession and the people bind themselves anew to the LORD (Nehemiah 9-10).
            OracleHarness.Righteous(OracleHarness.Witness("the covenant renewed in confession",
                new Repentance(), new Worship(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheNoblesExactUsuryAndOppressTheirBrothers_IsSin()
        {
            // Before Nehemiah's rebuke, the nobles seize fields and enslave their own brothers (Nehemiah 5:1-5).
            OracleHarness.Sin(OracleHarness.Witness("the nobles oppress the poor with usury",
                new Oppression(), new Greed()));
        }

        [Fact]
        public void ThePeopleBreakCovenantByMarryingForeignWomen_IsSin()
        {
            // The returned people intermarry with the peoples of the land, breaking faith with God (Ezra 9:1-2; Nehemiah 13:23-27).
            OracleHarness.Sin(OracleHarness.Witness("breaking covenant by foreign intermarriage",
                new CovenantBreaking(), new Disobedience()));
        }

        [Fact]
        public void TheMerchantsProfaneTheSabbathByTrading_IsSin()
        {
            // Men tread wine presses and sell their wares on the Sabbath until Nehemiah shuts the gates (Nehemiah 13:15-22).
            OracleHarness.Sin(OracleHarness.Witness("profaning the Sabbath with trade",
                new SabbathBreaking(), new Disobedience()));
        }
    }
}
