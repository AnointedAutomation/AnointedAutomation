// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Habakkuk narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class HabakkukNarrativeTests
    {
        [Fact]
        public void HabakkukCriesOutToGodOverViolenceAndAsksHowLong_IsRighteous()
        {
            // "O LORD, how long shall I cry, and thou wilt not hear! ... for spoiling and violence are before me." (Habakkuk 1:2-3)
            // The prophet does not rebel but brings his honest lament to God, trusting Him with the question of injustice.
            OracleHarness.Righteous(OracleHarness.Witness("Habakkuk cries out to the LORD over the violence and injustice in Judah and waits for His answer",
                new Trust(), new Worship(), new CourageousFaith(),
                new HungerForRighteousness(), new Endurance()));
        }

        [Fact]
        public void JudahsLawSlackedAndJudgmentPervertedWithViolence_IsSin()
        {
            // "the law is slacked, and judgment doth never go forth ... the wicked doth compass about the righteous." (Habakkuk 1:4)
            // In Judah the law is paralyzed and justice perverted as the wicked surround and crush the righteous.
            OracleHarness.Sin(OracleHarness.Grounded("Judah lets the law slacken and perverts judgment while the wicked surround the righteous",
                Grounding.InIdol("their own wickedness"),
                new Lawlessness(), new Oppression(), new Wrongdoing(),
                new WithholdingWhatIsDue(), new Treachery()));
        }

        [Fact]
        public void GodRaisesUpTheChaldeansToExecuteHisAppointedJudgment_IsADivineAct()
        {
            // "I raise up the Chaldeans ... they are terrible and dreadful ... I will work a work in your days." (Habakkuk 1:5-7)
            // God Himself sovereignly raises the Chaldeans as His instrument of judgment, working a wonder in His own time.
            OracleHarness.DivineAct(OracleHarness.Witness("God raises up the Chaldeans as His appointed instrument of judgment upon a guilty Judah",
                new JustRecompense(), new Righteousness(), new CovenantFaithfulness(),
                new PromiseFulfilled(), new Fidelity()));
        }

        [Fact]
        public void TheChaldeansAttributeTheirPowerToTheirOwnMightAsTheirGod_IsSin()
        {
            // "they shall scoff at the kings ... imputing this his power unto his god." (Habakkuk 1:10-11)
            // The Babylonian sweeps up nations like sand and worships his own strength, crediting his might as his god.
            OracleHarness.Sin(OracleHarness.Grounded("The Chaldean scoffs at kings and credits his conquering might to his own strength as his god",
                Grounding.InIdol("his own might"),
                new Pride(), new Boasting(), new Idolatry(),
                new Oppression(), new Blasphemy()));
        }

        [Fact]
        public void HabakkukStandsUponHisWatchToWaitForGodsAnswer_IsRighteous()
        {
            // "I will stand upon my watch ... and will watch to see what he will say unto me." (Habakkuk 2:1)
            // Rather than abandon faith, the prophet posts himself like a watchman, expectantly waiting for God to speak.
            OracleHarness.Righteous(OracleHarness.Witness("Habakkuk stands upon his watchtower to wait patiently and see what the LORD will answer him",
                new Patience(), new Endurance(), new Trust(),
                new CourageousFaith(), new Worship()));
        }

        [Fact]
        public void GodDeclaresTheJustShallLiveByHisFaith_IsADivineAct()
        {
            // "the just shall live by his faith." (Habakkuk 2:4)
            // God reveals the great gospel principle: the proud soul is not upright, but the righteous lives by faith.
            OracleHarness.DivineAct(OracleHarness.Witness("God declares the vision and proclaims that the just shall live by his faith",
                new TeachingTruth(), new Righteousness(), new Fidelity(),
                new PromiseFulfilled(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheBabylonianPlundersManyNationsAndIsNeverSatisfied_IsSin()
        {
            // "Woe to him that increaseth that which is not his ... that ladeth himself with thick clay!" (Habakkuk 2:6)
            // The conqueror enlarges his greed like the grave, heaping up plunder and pledges that are not his own.
            OracleHarness.Sin(OracleHarness.Grounded("The Babylonian heaps up the plunder of many nations in insatiable greed",
                Grounding.InIdol("plunder"),
                new Greed(), new Theft(), new Covetousness(),
                new Oppression(), new SelfSeeking()));
        }

        [Fact]
        public void TheBabylonianShedsBloodAndBuildsHisCityByIniquity_IsSin()
        {
            // "Woe to him that buildeth a town with blood, and stablisheth a city by iniquity!" (Habakkuk 2:12)
            // The empire founds its glory on murder and oppression, building its walls with the blood of the slain.
            OracleHarness.Sin(OracleHarness.Grounded("The Babylonian builds his city with blood and establishes it by iniquity",
                Grounding.InIdol("conquest"),
                new Bloodshed(), new Murder(), new Oppression(),
                new Lawlessness(), new Wrongdoing()));
            // capital vices (Bloodshed, Murder) drive disorder to its limit
            Assert.Equal(1.0, OracleHarness.Grounded("The Babylonian builds his city with blood and establishes it by iniquity",
                Grounding.InIdol("conquest"),
                new Bloodshed(), new Murder(), new Oppression(),
                new Lawlessness(), new Wrongdoing()).Disorder);
        }

        [Fact]
        public void TheBabylonianMakesHisNeighborDrunkToGazeOnHisNakedness_IsSin()
        {
            // "Woe unto him that giveth his neighbour drink ... that thou mayest look on their nakedness!" (Habakkuk 2:15)
            // He degrades the nations like a man who plies his neighbor with drink to shame and expose him.
            OracleHarness.Sin(OracleHarness.Grounded("The Babylonian makes his neighbor drunk to gaze upon his shame and nakedness",
                Grounding.InIdol("his own cruelty"),
                new Defilement(), new Oppression(), new Malice(),
                new DelightingInEvil(), new Drunkenness()));
            // capital vice (Defilement) drives disorder to its limit
            Assert.Equal(1.0, OracleHarness.Grounded("The Babylonian makes his neighbor drunk to gaze upon his shame and nakedness",
                Grounding.InIdol("his own cruelty"),
                new Defilement(), new Oppression(), new Malice(),
                new DelightingInEvil(), new Drunkenness()).Disorder);
        }

        [Fact]
        public void TheBabylonianWorshipsDumbIdolsOfWoodAndStone_IsSin()
        {
            // "Woe unto him that saith to the wood, Awake; to the dumb stone, Arise ... there is no breath at all in the midst of it." (Habakkuk 2:18-19)
            // The maker trusts the graven image he carved, a dumb idol with no breath, forsaking the living God.
            OracleHarness.Sin(OracleHarness.Grounded("The Babylonian worships the dumb graven images of wood and stone that he himself overlaid",
                Grounding.InIdol("graven images"),
                new Idolatry(), new GravenImage(), new Deceit(),
                new Blasphemy(), new Pride()));
        }

        [Fact]
        public void TheLordIsInHisHolyTempleAndAllTheEarthKeepsSilence_IsADivineAct()
        {
            // "But the LORD is in his holy temple: let all the earth keep silence before him." (Habakkuk 2:20)
            // Over against the lifeless idols, the living LORD reigns in His holy temple and the whole earth falls silent.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD is in His holy temple and all the earth keeps silence before Him",
                new Worship(), new Righteousness(), new CreationOrder(),
                new Goodness(), new Fidelity()));
        }

        [Fact]
        public void HabakkukPraysForGodToRevivceHisWorkAndRememberMercyInWrath_IsRighteous()
        {
            // "O LORD, revive thy work in the midst of the years ... in wrath remember mercy." (Habakkuk 3:2)
            // Awed by God's renown, the prophet prays for His saving work to live again and pleads for mercy amid judgment.
            OracleHarness.Righteous(OracleHarness.Witness("Habakkuk prays for God to revive His work in the midst of the years and in wrath to remember mercy",
                new Worship(), new Trust(), new Humility(),
                new CourageousFaith(), new HungerForRighteousness()));
        }

        [Fact]
        public void GodMarchesForthForTheSalvationOfHisPeopleAndHisAnointed_IsADivineAct()
        {
            // "thou wentest forth for the salvation of thy people, even for salvation with thine anointed." (Habakkuk 3:13)
            // God goes out in glory and power, splitting the earth, to save His people and deliver His anointed one.
            OracleHarness.DivineAct(OracleHarness.Witness("God marches forth in glory for the salvation of His people and for the deliverance of His anointed",
                new Deliverance(), new Protection(), new JustRecompense(),
                new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void HabakkukRejoicesInGodEvenWhenTheFieldsYieldNothing_IsRighteous()
        {
            // "Although the fig tree shall not blossom ... yet I will rejoice in the LORD, I will joy in the God of my salvation." (Habakkuk 3:17-18)
            // Though every crop and flock fail, the prophet chooses to rejoice in God his Savior, his strength and his joy.
            OracleHarness.Righteous(OracleHarness.Witness("Habakkuk rejoices in the LORD and joys in the God of his salvation though the fig tree does not blossom and the fields yield nothing",
                new Joy(), new Trust(), new Worship(),
                new CourageousFaith(), new Endurance()));
        }
    }
}
