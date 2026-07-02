// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Gitsew narrative oracle: the engine must return the verdict Scripture gives at each step.
    // Canon: ETH (Ethiopic-only). Gitsew (Gessew / Gitzew) is one of the four parts of the
    // apostolic Sinodos, a book of church order (~56 canons) attributed to the Apostles. It governs
    // the study of Scripture and keeping of the commandments, the duties of husband and wife,
    // the offices and duties of the ministers, the care of widows, baptism, vows of virginity,
    // honor toward the martyrs and the faithful departed, and warnings against heresy. Best-effort.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class GitsewNarrativeTests
    {
        [Fact]
        public void TheApostlesDeliverTheChurchOrderInGodsAuthority_IsADivineAct()
        {
            // The Sinodos opens with the canons delivered through the Apostles by the authority of God,
            // the church order handed down as commandment to the faithful. (Gitsew, prologue of the
            // apostolic canons)
            // God Himself teaching and ordering His Church through the apostolic deposit.
            OracleHarness.DivineAct(OracleHarness.Grounded("God delivers the apostolic church order to His Church through the Apostles",
                Grounding.InGod(),
                new TeachingTruth(), new CreationOrder(), new CovenantFaithfulness(), new Goodness(), new Fidelity()));
        }

        [Fact]
        public void TheFaithfulAreCommandedToStudyScriptureAndKeepTheCommandments_IsRighteous()
        {
            // The canons charge the faithful to give themselves to the study of the Scriptures and to
            // the observance of the commandments of the Lord. (Gitsew, on study of Scripture)
            // Obedient, truth-loving devotion to God's word.
            OracleHarness.Righteous(OracleHarness.Witness("the faithful study the Scriptures and keep the commandments of the Lord",
                new ObedienceToGod(), new TeachingTruth(), new RejoicingInTruth(), new Fidelity()));
        }

        [Fact]
        public void HusbandAndWifeKeepTheirMutualDuties_IsRighteous()
        {
            // The canons set out the mutual duties of husband and wife, that they keep faith and honor
            // toward one another in the Lord. (Gitsew, on the duties of husband and wife)
            // Covenant faithfulness, purity, and kindness within marriage.
            OracleHarness.Righteous(OracleHarness.Witness("husband and wife keep faith and honor toward one another",
                new CovenantFaithfulness(), new Fidelity(), new Purity(), new Kindness(), new SelfControl()));
        }

        [Fact]
        public void TheBishopShepherdsAndTeachesTheFlock_IsRighteous()
        {
            // The canons define the office and duty of the bishop, that he feed and teach the flock,
            // guard the truth, and care for the people committed to him. (Gitsew, on the ministers)
            // Faithful shepherding: teaching truth, guarding, and serving the vulnerable.
            OracleHarness.Righteous(OracleHarness.Witness("the bishop shepherds, teaches, and guards the flock committed to him",
                new TeachingTruth(), new Protection(), new CaringForTheSick(), new Fidelity(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void TheDeaconsServeTheNeedyAndMinisterAtTheTable_IsRighteous()
        {
            // The canons charge the deacons to minister, serving the poor, the sick, and the needs of
            // the congregation. (Gitsew, on the office of deacon)
            // Lowly, generous service to the hungry and the sick.
            OracleHarness.Righteous(OracleHarness.Witness("the deacons serve the poor, feed the hungry, and tend the sick",
                new FeedingTheHungry(), new CaringForTheSick(), new Generosity(), new Humility(), new Kindness()));
        }

        [Fact]
        public void TheChurchCaresForTheWidows_IsRighteous()
        {
            // The canons set the duties of and toward the widows, that the church honor and provide for
            // them. (Gitsew, on the duties of widows)
            // Honoring and providing for the vulnerable, the very mark of pure religion.
            OracleHarness.Righteous(OracleHarness.Witness("the church honors and provides for the widows",
                new HonoringTheVulnerable(), new Compassion(), new Generosity(), new DefendingTheWeak(), new Kindness()));
        }

        [Fact]
        public void BaptismIsAdministeredRightlyByTheOrderedMinister_IsRighteous()
        {
            // The canons appoint the method of baptism and forbid laymen to baptize, that the sacrament
            // be kept in its right order. (Gitsew, on baptism)
            // Obedient guarding of the sacrament according to God's appointed order.
            OracleHarness.Righteous(OracleHarness.Witness("baptism is administered rightly by the ordained minister in its appointed order",
                new ObedienceToGod(), new CreationOrder(), new Protection(), new Fidelity()));
        }

        [Fact]
        public void TheVowOfVirginityIsKeptInPurity_IsRighteous()
        {
            // The canons commend the vow of virginity, kept in purity and self-mastery unto the Lord.
            // (Gitsew, on vows of virginity)
            // Pure, self-controlled consecration to God.
            OracleHarness.Righteous(OracleHarness.Witness("the consecrated keep their vow of virginity in purity unto the Lord",
                new Purity(), new SelfControl(), new Worship(), new Fidelity()));
        }

        [Fact]
        public void TheFaithfulHonorTheMartyrsAndTheDeparted_IsRighteous()
        {
            // The canons enjoin honor toward the martyrs and respect to be shown to the faithful
            // departed. (Gitsew, on the martyrs and the departed)
            // Reverent remembrance and honor of those who endured for the faith.
            OracleHarness.Righteous(OracleHarness.Witness("the faithful honor the martyrs and the faithful departed",
                new Worship(), new HonoringTheVulnerable(), new Endurance(), new CourageousFaith(), new Mourning()));
        }

        [Fact]
        public void APriestWhoBetraysHisOfficeAndDefilesTheSacred_IsSin()
        {
            // The canons warn and depose the minister who betrays his office, robbing the church and
            // defiling holy things. (Gitsew, disciplinary canons against unworthy ministers)
            // Treachery, theft, and defilement of what is holy.
            OracleHarness.Sin(OracleHarness.Witness("a minister betrays his office, robs the church, and defiles holy things",
                new Treachery(), new Theft(), new Defilement(), new Greed()));
        }

        [Fact]
        public void TheHereticPervertsTheTruthAndDividesTheChurch_IsSin()
        {
            // The canons warn the faithful against heresy, the teaching that perverts the truth and
            // tears the body of the church. (Gitsew, warning against heresy)
            // Deceit, blasphemy, and sowing of discord against the truth.
            OracleHarness.Sin(OracleHarness.Witness("the heretic perverts the truth and divides the church",
                new Deceit(), new Blasphemy(), new SowingDiscord(), new Rebellion(), new DelightingInEvil()));
        }

        [Fact]
        public void TheUnrepentantSinnerSpurnsTheChurchsDiscipline_IsSin()
        {
            // The canons set the treatment of sinners; the one who spurns correction and persists in
            // wrongdoing is cut off until he repents. (Gitsew, on the treatment of sinners)
            // Hardened disobedience and rebellion against the order of the Lord.
            OracleHarness.Sin(OracleHarness.Witness("the unrepentant sinner spurns the church's discipline and persists in wrongdoing",
                new Disobedience(), new Rebellion(), new Wrongdoing(), new Pride()));
        }
    }
}
