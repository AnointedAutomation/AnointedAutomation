// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Tegsats (Reproof) narrative oracle (Ethiopic OT book 25, best-effort ETH canon):
    // a wisdom book of admonition and reproof, gathered with the Solomonic corpus. The engine
    // must return the verdict Scripture gives at each admonition: the wisdom of God and the
    // righteous deeds it commands read coherent, while the folly it reproves reads as sin.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class TegsatsNarrativeTests
    {
        [Fact]
        public void WisdomOfGodCallsAloudToTeachTheSimple_IsADivineAct()
        {
            // The book of Reproof opens as the wisdom of God crying out to instruct the simple,
            // disclosing truth to all who will hear. (cf. Proverbs 1:20-23)
            // The teaching of God's own wisdom is a divine act, composed only of virtue.
            OracleHarness.DivineAct(OracleHarness.Witness("The wisdom of God calls aloud in the streets to teach truth to the simple",
                new TeachingTruth(), new RejoicingInTruth(), new Goodness(),
                new Fidelity(), new Righteousness()));
        }

        [Fact]
        public void TheWiseSonReceivesCorrectionInHumility_IsRighteous()
        {
            // "Whoever heeds correction gains understanding." The reproof of Tegsats is welcomed,
            // not resented, by the one who would be wise. (cf. Proverbs 15:32)
            OracleHarness.Righteous(OracleHarness.Witness("The wise son hears reproof and turns from his folly in humility",
                new Repentance(), new Humility(), new HungerForRighteousness(),
                new ObedienceToGod(), new SelfControl()));
        }

        [Fact]
        public void TheScornerHatesReproofAndRefusesCorrection_IsSin()
        {
            // The book reproves the scoffer who despises instruction and loves his own folly. (cf. Proverbs 9:7-8, 15:12)
            // To hate reproof is rebellion against the wisdom of God.
            OracleHarness.Sin(OracleHarness.Grounded("The scorner hates reproof, mocks the one who corrects him, and clings to his folly",
                Grounding.InIdol("his own folly"),
                new Pride(), new Rebellion(), new Insolence(),
                new Disobedience(), new HaughtyEyes()));
        }

        [Fact]
        public void TheFearOfTheLordIsTheBeginningOfWisdom_IsRighteous()
        {
            // "The fear of the LORD is the beginning of wisdom." The reproof grounds all instruction
            // in reverent worship and obedience to God. (cf. Proverbs 1:7, 9:10)
            OracleHarness.Righteous(OracleHarness.Witness("The wise fear the LORD as the beginning of knowledge and keep His commandments",
                new Worship(), new ObedienceToGod(), new Humility(),
                new Fidelity(), new Trust()));
        }

        [Fact]
        public void TheProudHeartIsAnAbominationReprovedByWisdom_IsSin()
        {
            // The book reproves the haughty eyes and the proud heart that God hates. (cf. Proverbs 6:16-17, 16:5)
            OracleHarness.Sin(OracleHarness.Grounded("The proud man exalts himself with haughty eyes and a boastful heart",
                Grounding.InIdol("his own glory"),
                new Pride(), new HaughtyEyes(), new Boasting(),
                new Insolence(), new SelfSeeking()));
        }

        [Fact]
        public void TheGenerousManSharesHisBreadWithThePoor_IsRighteous()
        {
            // The admonition commends the one who is generous and gives to the needy. (cf. Proverbs 19:17, 22:9)
            OracleHarness.Righteous(OracleHarness.Witness("The wise man shares his bread with the poor and is generous to the needy",
                new Generosity(), new FeedingTheHungry(), new Compassion(),
                new HonoringTheVulnerable(), new Kindness()));
        }

        [Fact]
        public void TheSluggardWillNotPlowAndReapsNothing_IsSin()
        {
            // The reproof rebukes the sluggard who refuses to work and follows worthless pursuits. (cf. Proverbs 6:6-11, 24:30-34)
            OracleHarness.Sin(OracleHarness.Grounded("The sluggard refuses to plow because of the cold and lets his field grow over with thorns",
                Grounding.InIdol("his own ease"),
                new Sloth(), new Disobedience(), new Indifference(),
                new Ingratitude(), new SelfSeeking()));
        }

        [Fact]
        public void TheLyingTongueAndFalseWitnessAreReproved_IsSin()
        {
            // Among the things wisdom hates are a lying tongue and a false witness who breathes out lies. (cf. Proverbs 6:17-19, 12:22)
            OracleHarness.Sin(OracleHarness.Grounded("The deceiver bears false witness and sows discord among brothers with a lying tongue",
                Grounding.InIdol("his crooked schemes"),
                new Deceit(), new FalseWitness(), new SowingDiscord(),
                new Slander(), new WickedScheming()));
        }

        [Fact]
        public void HonestScalesAndJustWeightsArePraised_IsRighteous()
        {
            // "A false balance is an abomination ... but a just weight is His delight." The reproof commends
            // honest dealing and just recompense in the marketplace. (cf. Proverbs 11:1, 16:11)
            OracleHarness.Righteous(OracleHarness.Witness("The upright merchant uses honest scales and just weights and renders what is due",
                new Trustworthiness(), new Righteousness(), new JustRecompense(),
                new RejoicingInTruth(), new SelfControl()));
        }

        [Fact]
        public void TheDrunkardAndGluttonComeToPoverty_IsSin()
        {
            // The book reproves the drunkard and the glutton who waste themselves in carousing. (cf. Proverbs 23:20-21)
            OracleHarness.Sin(OracleHarness.Grounded("The drunkard and glutton give themselves to wine and carousing until they come to ruin",
                Grounding.InIdol("the cup and the table"),
                new Drunkenness(), new Gluttony(), new Carousing(),
                new SelfSeeking(), new Sloth()));
        }

        [Fact]
        public void TheRighteousDefendsTheWeakAndPleadsTheCauseOfTheNeedy_IsRighteous()
        {
            // "Open thy mouth for the dumb ... plead the cause of the poor and needy." The reproof
            // commands the wise to defend those who cannot defend themselves. (cf. Proverbs 31:8-9)
            OracleHarness.Righteous(OracleHarness.Witness("The wise open their mouth for the voiceless and plead the cause of the poor and needy",
                new DefendingTheWeak(), new HonoringTheVulnerable(), new Righteousness(),
                new Compassion(), new JustRecompense()));
        }

        [Fact]
        public void TheManWhoOppressesThePoorReproachesHisMaker_IsSin()
        {
            // "He that oppresseth the poor reproacheth his Maker." The reproof condemns the violent
            // who grind down the helpless for gain. (cf. Proverbs 14:31, 22:16)
            OracleHarness.Sin(OracleHarness.Grounded("The oppressor crushes the poor and withholds their due to enlarge his own gain",
                Grounding.InIdol("his greed for gain"),
                new Oppression(), new Greed(), new WithholdingWhatIsDue(),
                new Heartlessness(), new Wrongdoing()));
        }

        [Fact]
        public void WisdomCommendsTheSoftAnswerThatTurnsAwayWrath_IsRighteous()
        {
            // "A soft answer turneth away wrath." The reproof commends the peacemaker who is slow to anger. (cf. Proverbs 15:1, 16:32)
            OracleHarness.Righteous(OracleHarness.Witness("The wise give a soft answer that turns away wrath and are slow to anger",
                new Peacemaking(), new Gentleness(), new SelfControl(),
                new Patience(), new Peace()));
        }

        [Fact]
        public void TheGodOfWisdomKeepsAndProtectsThoseWhoWalkUprightly_IsADivineAct()
        {
            // "He layeth up sound wisdom for the righteous ... he keepeth the paths of judgment, and
            // preserveth the way of his saints." (cf. Proverbs 2:7-8)
            OracleHarness.DivineAct(OracleHarness.Witness("The God of wisdom is a shield to the upright and guards the path of His faithful ones",
                new Protection(), new Fidelity(), new Goodness(),
                new CovenantFaithfulness(), new Righteousness()));
        }
    }
}
