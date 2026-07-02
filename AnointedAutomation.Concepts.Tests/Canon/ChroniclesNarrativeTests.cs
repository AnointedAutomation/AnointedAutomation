// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // 1 & 2 Chronicles narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class ChroniclesNarrativeTests
    {
        [Fact]
        public void SaulDiesForHisUnfaithfulnessAndConsultingAMedium_IsSin()
        {
            // "Saul died for his breach of faith ... and also because he consulted a medium ... and did not
            // keep the command of the LORD." (1 Chronicles 10:13-14)
            // Covenant treachery joined to sorcery and disobedience.
            OracleHarness.Sin(OracleHarness.Witness("Saul breaks faith with God and consults a medium",
                new CovenantBreaking(), new Sorcery(), new Disobedience(), new Treachery()));
        }

        [Fact]
        public void DavidBringsUpTheArkWithWorshipAndRejoicing_IsRighteous()
        {
            // David brings the ark to Jerusalem with sacrifices, singing, and praise before God. (1 Chronicles 15-16)
            // "Oh give thanks to the LORD ... sing to him, sing praises to him." (1 Chronicles 16:8-9)
            OracleHarness.Righteous(OracleHarness.Witness("David brings up the ark with worship and rejoicing",
                new Worship(), new Joy(), new ObedienceToGod(), new RejoicingInTruth()));
        }

        [Fact]
        public void GodMakesAnEverlastingCovenantWithDavid_IsADivineAct()
        {
            // "I will raise up your offspring after you ... and I will establish his kingdom ... forever."
            // (1 Chronicles 17:11-14) God's covenant promise to build David a house.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD covenants to establish David's throne forever",
                new CovenantFaithfulness(), new PromiseFulfilled(), new Fidelity(), new Goodness()));
        }

        [Fact]
        public void DavidNumbersIsraelInProvocationFromSatan_IsSin()
        {
            // "Satan stood against Israel and incited David to number Israel." (1 Chronicles 21:1)
            // A proud, faithless census provoking God; "this thing ... was evil in the sight of God." (21:7-8)
            OracleHarness.Sin(OracleHarness.Witness("David proudly numbers Israel against God's will",
                new Pride(), new Disobedience(), new Wrongdoing()));
        }

        [Fact]
        public void DavidRepentsAndBuildsAnAltarToHaltThePlague_IsRighteous()
        {
            // "I have sinned greatly ... please take away the iniquity of your servant." (1 Chronicles 21:8)
            // David buys the threshing floor, refuses to offer what costs nothing, and the plague is stayed. (21:24-27)
            OracleHarness.Righteous(OracleHarness.Witness("David repents, builds the altar, and offers sacrifice",
                new Repentance(), new Worship(), new Humility(), new SelfSacrifice()));
        }

        [Fact]
        public void SolomonAsksForWisdomToGovernGodsPeople_IsRighteous()
        {
            // "Give me now wisdom and knowledge to go out and come in before this people." (2 Chronicles 1:10)
            // Solomon seeks not riches but discernment to serve; God commends the request. (1:11-12)
            OracleHarness.Righteous(OracleHarness.Witness("Solomon asks God for wisdom to lead the people",
                new Humility(), new Trust(), new ObedienceToGod(), new Worship()));
        }

        [Fact]
        public void TheGloryOfTheLordFillsTheDedicatedTemple_IsADivineAct()
        {
            // "The glory of the LORD filled the house of God." (2 Chronicles 5:14; 7:1-3)
            // God answers with fire and glory at the temple dedication.
            OracleHarness.DivineAct(OracleHarness.Witness("The glory of the LORD fills Solomon's temple",
                new CreationOrder(), new Goodness(), new PromiseFulfilled(), new Fidelity()));
        }

        [Fact]
        public void GodPromisesToHealTheLandIfThePeopleHumbleThemselves_IsADivineAct()
        {
            // "If my people ... humble themselves, and pray ... then I will hear ... and heal their land."
            // (2 Chronicles 7:14) God's gracious promise of pardon and healing for the repentant.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD promises to forgive and heal a repentant people",
                new Pardon(), new Healing(), new CovenantFaithfulness(), new Compassion()));
        }

        [Fact]
        public void AsaRemovesTheIdolsAndSeeksTheLord_IsRighteous()
        {
            // Asa "took away the foreign altars ... and commanded Judah to seek the LORD." (2 Chronicles 14:2-4)
            // Reform that tears down idolatry and turns the nation back to God.
            OracleHarness.Righteous(OracleHarness.Witness("Asa removes the idols and leads Judah to seek God",
                new Worship(), new ObedienceToGod(), new CourageousFaith(), new Righteousness()));
        }

        [Fact]
        public void JehoshaphatTrustsGodAndIsDeliveredFromTheInvadingArmies_IsRighteous()
        {
            // "We do not know what to do, but our eyes are on you." (2 Chronicles 20:12) Judah fasts, worships,
            // and stands still; the LORD sets ambushes and delivers them without a sword. (20:17-22)
            OracleHarness.Righteous(OracleHarness.Witness("Jehoshaphat trusts the LORD and worships before battle",
                new Trust(), new Worship(), new CourageousFaith(), new ObedienceToGod()));
        }

        [Fact]
        public void JoashMurdersZechariahThePriestWhoRebukedHim_IsSin()
        {
            // King Joash forgets the kindness of Jehoiada and has his son Zechariah stoned in the temple court.
            // "May the LORD see and avenge!" (2 Chronicles 24:20-22) Ingratitude crowned with bloodshed.
            Resolution r = OracleHarness.Witness("Joash has the priest Zechariah stoned to death",
                new Murder(), new Bloodshed(), new Ingratitude(), new Treachery());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void AhazSacrificesHisSonsAndBurnsIncenseToOtherGods_IsSin()
        {
            // Ahaz "made metal images for the Baals ... and burned his sons as an offering." (2 Chronicles 28:2-4)
            // Idolatry compounded by child sacrifice in the valley of Hinnom.
            Resolution r = OracleHarness.Grounded("Ahaz burns his sons as offerings to idols", Grounding.InIdol("Baal"),
                new ChildSacrifice(), new Idolatry(), new Bloodshed(), new Blasphemy());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void HezekiahCleansesTheTempleAndKeepsThePassover_IsRighteous()
        {
            // Hezekiah reopens and cleanses the house of the LORD and restores the Passover for all Israel.
            // (2 Chronicles 29-30) "He did what was good and right and faithful before the LORD his God." (31:20)
            OracleHarness.Righteous(OracleHarness.Witness("Hezekiah cleanses the temple and restores the Passover",
                new Worship(), new ObedienceToGod(), new Righteousness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void ManassehHumblesHimselfInPrisonAndIsRestored_IsRighteous()
        {
            // The captive Manasseh "humbled himself greatly ... and prayed to him ... and God ... brought him again."
            // (2 Chronicles 33:12-13) Even the worst king's repentance is received.
            OracleHarness.Righteous(OracleHarness.Witness("Manasseh humbles himself, repents, and turns back to God",
                new Repentance(), new Humility(), new Worship(), new Trust()));
        }

        [Fact]
        public void JosiahRenewsTheCovenantAtTheBookOfTheLaw_IsRighteous()
        {
            // Josiah hears the rediscovered Law, tears his clothes, and leads the people to keep the covenant.
            // (2 Chronicles 34:29-33) He turned to the LORD with all his heart.
            OracleHarness.Righteous(OracleHarness.Witness("Josiah renews the covenant and keeps the Law",
                new Repentance(), new CovenantFaithfulness(), new ObedienceToGod(), new Humility()));
        }

        [Fact]
        public void CyrusIsStirredToReleaseGodsPeopleToRebuildTheTemple_IsADivineAct()
        {
            // "The LORD stirred up the spirit of Cyrus ... 'let him go up.'" (2 Chronicles 36:22-23)
            // God keeps his word through Jeremiah and delivers the exiles home.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD stirs Cyrus to free the exiles to rebuild Jerusalem",
                new Deliverance(), new PromiseFulfilled(), new CovenantFaithfulness(), new Fidelity()));
        }
    }
}
