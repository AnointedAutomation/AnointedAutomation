// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Zephaniah narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class ZephaniahNarrativeTests
    {
        [Fact]
        public void GodDeclaresTheGreatDayOfJudgmentOverTheWholeEarth_IsADivineAct()
        {
            // I will utterly sweep away everything from the face of the earth, declares the LORD (Zephaniah 1:2-3).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD announces His just judgment to sweep away the corruption of the whole land",
                new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void JudahWorshipsBaalAndBowsToTheHostOfHeaven_IsSin()
        {
            // I will cut off the remnant of Baal, and those who bow down on the roofs to the host of the heavens (Zephaniah 1:4-5).
            OracleHarness.Sin(OracleHarness.Grounded("Judah burns incense to Baal and bows to the starry host upon their rooftops",
                Grounding.InIdol("Baal"), new Idolatry(), new Worship()));
        }

        [Fact]
        public void ThePeopleSwearByMilcomAndTurnBackFromTheLord_IsSin()
        {
            // Those who bow down and swear to the LORD and yet swear by Milcom, who have turned back from following the LORD (Zephaniah 1:5-6).
            OracleHarness.Sin(OracleHarness.Grounded("the people swear by Milcom and turn back from following the LORD",
                Grounding.InIdol("Milcom"), new Idolatry(), new Treachery(), new CovenantBreaking()));
        }

        [Fact]
        public void TheOfficialsClotheThemselvesInForeignAttireAndPlunder_IsSin()
        {
            // I will punish the officials and the king's sons and all who fill their master's house with violence and fraud (Zephaniah 1:8-9).
            Resolution r = OracleHarness.Witness("the officials and the king's sons fill their master's house with violence and fraud",
                new Bloodshed(), new Oppression(), new Deceit(), new Theft());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheComplacentSayTheLordWillDoNothing_IsSin()
        {
            // I will punish the men who are complacent, who say in their hearts, the LORD will not do good, nor will He do ill (Zephaniah 1:12).
            OracleHarness.Sin(OracleHarness.Witness("the complacent men say in their hearts that the LORD will do neither good nor ill",
                new Sloth(), new Pride(), new Disobedience()));
        }

        [Fact]
        public void TheDayOfTheLordIsAtHandAsWrathAndDistress_IsADivineAct()
        {
            // The great day of the LORD is near; a day of wrath, a day of distress and anguish (Zephaniah 1:14-18).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD brings near the great Day of the LORD, a day of wrath and just reckoning against sin",
                new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void ZephaniahCallsTheNationToSeekTheLordInHumility_IsRighteous()
        {
            // Seek the LORD, all you humble of the land; seek righteousness, seek humility (Zephaniah 2:1-3).
            OracleHarness.Righteous(OracleHarness.Witness("Zephaniah calls the humble of the land to gather, seek the LORD, and pursue righteousness and humility",
                new Repentance(), new Humility(), new Righteousness(), new Worship()));
        }

        [Fact]
        public void GodJudgesPhilistiaMoabAndAmmonForTheirTaunts_IsADivineAct()
        {
            // I have heard the taunts of Moab and the revilings of the Ammonites, how they have taunted My people (Zephaniah 2:4-11).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD justly judges Philistia, Moab, and Ammon and defends the remnant of His people whom they reviled",
                new JustRecompense(), new DefendingTheWeak(), new Righteousness()));
        }

        [Fact]
        public void NinevehBoastsThatThereIsNoneBesidesHer_IsSin()
        {
            // This is the exultant city that said in her heart, I am, and there is no one else (Zephaniah 2:15).
            OracleHarness.Sin(OracleHarness.Witness("Nineveh, the exultant city, boasts in her heart that there is none besides her",
                new Pride(), new Boasting(), new SelfSeeking()));
        }

        [Fact]
        public void JerusalemsOfficialsAreRoaringLionsAndJudgesAreEveningWolves_IsSin()
        {
            // Her officials within her are roaring lions; her judges are evening wolves that leave nothing till the morning (Zephaniah 3:1-3).
            Resolution r = OracleHarness.Witness("the officials of Jerusalem are roaring lions and her judges are evening wolves who devour the people",
                new Oppression(), new Bloodshed(), new Greed(), new Lawlessness());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void HerProphetsAreTreacherousAndPriestsProfaneWhatIsHoly_IsSin()
        {
            // Her prophets are fickle, treacherous men; her priests profane what is holy and do violence to the law (Zephaniah 3:4).
            OracleHarness.Sin(OracleHarness.Witness("the prophets of Jerusalem are treacherous and her priests profane the sanctuary and do violence to the law",
                new Treachery(), new Defilement(), new Lawlessness()));
        }

        [Fact]
        public void TheLordWithinHerIsRighteousAndDoesNoInjustice_IsADivineAct()
        {
            // The LORD within her is righteous; He does no injustice; every morning He shows forth His justice (Zephaniah 3:5).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD in the midst of the city is righteous and does no injustice, bringing His justice to light every morning",
                new Righteousness(), new JustRecompense(), new Fidelity()));
        }

        [Fact]
        public void GodPromisesToPurifyThePeoplesSpeechToCallOnHisName_IsADivineAct()
        {
            // For at that time I will change the speech of the peoples to a pure speech, that all of them may call upon the LORD (Zephaniah 3:9).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD purifies the lips of the peoples that all may call upon His name and serve Him with one accord",
                new Purity(), new RestoringLife(), new PromiseFulfilled()));
        }

        [Fact]
        public void GodLeavesAHumbleAndLowlyRemnantWhoTrustInHisName_IsADivineAct()
        {
            // I will leave in your midst a people humble and lowly; they shall seek refuge in the name of the LORD (Zephaniah 3:12-13).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD leaves a humble and lowly remnant who take refuge in His name and speak no lies",
                new Protection(), new Deliverance(), new CovenantFaithfulness(), new Humility()));
        }

        [Fact]
        public void GodRejoicesOverHisPeopleWithGladnessAndQuietsThemWithLove_IsADivineAct()
        {
            // The LORD your God is in your midst, a mighty one who will save; He will rejoice over you with gladness and quiet you by His love (Zephaniah 3:17).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD rejoices over His people with gladness, quiets them by His love, and exults over them with loud singing",
                new Joy(), new Compassion(), new Deliverance(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodGathersTheOutcastAndRestoresTheFortunesOfHisPeople_IsADivineAct()
        {
            // I will save the lame and gather the outcast; I will restore your fortunes before your eyes, says the LORD (Zephaniah 3:19-20).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD saves the lame, gathers the outcast, and restores the fortunes of His people, gathering them home",
                new HonoringTheVulnerable(), new RestoringLife(), new Deliverance(), new PromiseFulfilled()));
        }
    }
}
