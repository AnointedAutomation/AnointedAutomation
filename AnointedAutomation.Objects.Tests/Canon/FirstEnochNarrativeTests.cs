// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 1 Enoch (ETH) narrative oracle: the engine must return the verdict Scripture gives at each step.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class FirstEnochNarrativeTests
    {
        [Fact]
        public void TheWatchersSwearAndDescendToTakeWivesOfMen_IsSin()
        {
            // "the angels, the children of heaven, saw and lusted after them ... and they all descended on Hermon and took unto themselves wives." (1 Enoch 6:1-2; 7:1)
            OracleHarness.Sin(OracleHarness.Witness("the Watchers, sons of heaven, lust after the daughters of men and descend to take them as wives, defiling themselves", new Lust(), new SexualImmorality(), new Defilement()));
            Assert.Equal(1.0, OracleHarness.Witness("the Watchers, sons of heaven, lust after the daughters of men and descend to take them as wives, defiling themselves", new Lust(), new SexualImmorality(), new Defilement()).Disorder);
        }

        [Fact]
        public void TheGiantsDevourMankindAndSpillBlood_IsSin()
        {
            // "the giants ... devoured all the acquisitions of men ... they began to sin against birds, and beasts ... and to devour one another's flesh, and drink the blood." (1 Enoch 7:3-5)
            OracleHarness.Sin(OracleHarness.Witness("the giants born of the Watchers devour the labor of men, then murder and feed on flesh and drink the blood of the earth", new Murder(), new Bloodshed(), new Oppression()));
            Assert.Equal(1.0, OracleHarness.Witness("the giants born of the Watchers devour the labor of men, then murder and feed on flesh and drink the blood of the earth", new Murder(), new Bloodshed(), new Oppression()).Disorder);
        }

        [Fact]
        public void AzazelTeachesWeaponsSorceryAndForbiddenArts_IsSin()
        {
            // "Azazel taught men to make swords ... and made known to them the metals ... and all kinds of costly stones ... and there arose much godlessness ... Semjaza taught enchantments." (1 Enoch 8:1-3)
            OracleHarness.Sin(OracleHarness.Witness("Azazel teaches men to forge swords and ornaments and seductions, and the Watchers teach sorcery and enchantments, leading the world astray", new Sorcery(), new Deceit(), new Wrongdoing()));
        }

        [Fact]
        public void TheCryOfTheEarthAndTheBloodOfMenAscends_IsRighteous()
        {
            // "the souls of men made their suit, saying, 'Bring our cause before the Most High.'" (1 Enoch 8:4; 9:3,10)
            OracleHarness.Righteous(OracleHarness.Witness("the dying souls of men cry out and bring their cause before the Most High, mourning under the violence done to them", new Mourning(), new HungerForRighteousness(), new Endurance()));
        }

        [Fact]
        public void TheArchangelsIntercedeAndPleadForJudgmentBeforeGod_IsRighteous()
        {
            // "Michael, Uriel, Raphael, and Gabriel looked down ... and said one to another: 'The earth made without inhabitant cries the voice ... unto the gates of heaven.'" (1 Enoch 9:1-3)
            OracleHarness.Righteous(OracleHarness.Witness("the holy archangels Michael, Uriel, Raphael, and Gabriel behold the bloodshed and faithfully bring the plea of the earth before the throne of God", new Fidelity(), new ObedienceToGod(), new DefendingTheWeak()));
        }

        [Fact]
        public void GodSendsRaphaelToBindAzazelAndHealTheEarth_IsADivineAct()
        {
            // "the Most High ... said to Raphael: 'Bind Azazel hand and foot ... and the whole earth has been corrupted ... and heal the earth which the angels have corrupted.'" (1 Enoch 10:4-7)
            OracleHarness.DivineAct(OracleHarness.Witness("God sends Raphael to bind Azazel and to heal the earth that the Watchers corrupted, restoring its order", new Healing(), new Righteousness(), new CreationOrder()));
        }

        [Fact]
        public void GodSendsGabrielAgainstTheGiantsToCleanseTheEarth_IsADivineAct()
        {
            // "to Gabriel said the Lord: 'Proceed against the bastards ... and the children of fornication, and destroy the children of the Watchers from amongst men.'" (1 Enoch 10:9-10)
            OracleHarness.DivineAct(OracleHarness.Witness("God sends Gabriel to recompense the violent giants with just judgment and to cleanse the earth of their oppression", new JustRecompense(), new Righteousness(), new Protection()));
        }

        [Fact]
        public void GodPromisesToCleanseTheEarthAndPlantRighteousness_IsADivineAct()
        {
            // "Cleanse the earth from all oppression ... and all the children of men shall become righteous ... and I will plant it in righteousness." (1 Enoch 10:16-22)
            OracleHarness.DivineAct(OracleHarness.Witness("God promises to cleanse the earth of all oppression and plant the plant of righteousness and truth, blessing it with peace", new Righteousness(), new PromiseFulfilled(), new Peace()));
        }

        [Fact]
        public void EnochIntercedesAndWritesThePetitionOfTheFallenWatchers_IsRighteous()
        {
            // "they besought me to draw up a petition for them ... And Enoch went and said: 'Azazel, thou shalt have no peace.'" (1 Enoch 13:1-6)
            OracleHarness.Righteous(OracleHarness.Witness("Enoch the scribe of righteousness compassionately writes the petition of the fallen Watchers and carries their request before God", new Compassion(), new Trustworthiness(), new Righteousness()));
        }

        [Fact]
        public void GodPronouncesTheWatchersHaveNoPeaceForForsakingHeaven_IsADivineAct()
        {
            // "Wherefore have ye left the high, holy, and eternal heaven ... and defiled yourselves with the daughters of men ... ye shall have no peace nor forgiveness of sin." (1 Enoch 15:3; 16:4)
            OracleHarness.DivineAct(OracleHarness.Witness("God justly declares to the Watchers that they have no peace, having forsaken holy heaven for defilement, and renders righteous judgment", new Righteousness(), new JustRecompense(), new CreationOrder()));
        }

        [Fact]
        public void EnochWalksWithGodAndIsShownTheVisionsOfHeaven_IsRighteous()
        {
            // "And Enoch was hidden ... and his activities had to do with the Watchers, and his days were with the holy ones." (1 Enoch 12:1-2; 14:8ff)
            OracleHarness.Righteous(OracleHarness.Witness("Enoch walks faithfully with God, worshipping before the throne of glory and obediently receiving the visions of heaven", new Worship(), new Fidelity(), new ObedienceToGod()));
        }

        [Fact]
        public void TheLuminariesTransgressTheirOrderedCoursesBecauseOfSin_IsSin()
        {
            // "in the days of the sinners the years shall be shortened ... and all things on the earth shall alter ... and the moon shall alter her order." (1 Enoch 80:2-8)
            OracleHarness.Sin(OracleHarness.Witness("because of the sin of men the heavenly luminaries forsake their ordered courses and the seasons fall into confusion", new Chaos(), new Lawlessness(), new Wrongdoing()));
        }

        [Fact]
        public void TheElectOneSitsToJudgeAndCastDownTheKingsAndMighty_IsADivineAct()
        {
            // "the Lord of Spirits seated him on the throne of His glory, and the spirit of righteousness was poured out upon him ... and he shall judge ... the kings and the mighty." (1 Enoch 62:2-9)
            OracleHarness.DivineAct(OracleHarness.Witness("the Elect One, the Son of Man, sits on the throne of glory and renders righteous judgment, delivering the oppressed and casting down the mighty", new Righteousness(), new JustRecompense(), new Deliverance()));
        }

        [Fact]
        public void TheRighteousAndElectShallDwellInLightAndPeaceForever_IsADivineAct()
        {
            // "the righteous and elect shall be saved on that day ... and with that Son of Man shall they eat and lie down and rise up for ever and ever." (1 Enoch 58:2-3; 62:14)
            OracleHarness.DivineAct(OracleHarness.Witness("God grants the righteous and elect an inheritance of unending light, peace, and joy in the presence of the Son of Man", new Peace(), new Joy(), new Goodness()));
        }
    }
}
