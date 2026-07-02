// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Josephas son of Bengorion (Ethiopic Zëna Ayhud / Sefer Yosippon, canon tag ETH) narrative oracle:
    // the engine must return the verdict Scripture/the chronicle gives at each step. Best-effort, Ethiopic-only.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class JosephasNarrativeTests
    {
        [Fact]
        public void AntiochusDefilesTheTempleAndOutlawsTheLaw_IsSin()
        {
            // Antiochus enters Jerusalem, plunders and profanes the sanctuary, forbids the Law and
            // covenant worship, and commands the people to sacrifice to idols. (Yosippon, on Antiochus' persecution)
            // Proud, sacrilegious tyranny that defiles God's house and forces apostasy.
            OracleHarness.Sin(OracleHarness.Witness("Antiochus defiles the temple, outlaws the Law, and commands idolatry",
                new Defilement(), new Idolatry(), new Blasphemy(), new Oppression(), new Lawlessness(), new Pride()));
            Assert.Equal(1.0, OracleHarness.Witness("Antiochus defiles the temple, outlaws the Law, and commands idolatry",
                new Defilement(), new Idolatry(), new Blasphemy(), new Oppression(), new Lawlessness(), new Pride()).Disorder);
        }

        [Fact]
        public void HannahAndHerSevenSonsChooseDeathRatherThanEatUncleanFood_IsRighteous()
        {
            // The mother and her seven sons are tortured and slain one by one, yet refuse to break God's
            // Law, encouraging one another to die in faith. (Yosippon, the mother of the seven martyrs)
            // Courageous, enduring, faithful covenant-keeping unto death.
            OracleHarness.Righteous(OracleHarness.Witness("Hannah and her seven sons choose death rather than forsake God's Law",
                new CourageousFaith(), new Endurance(), new Fidelity(), new ObedienceToGod(), new Purity(), new SelfSacrifice()));
        }

        [Fact]
        public void MattathiasRefusesToSacrificeAndRalliesTheZealousForTheCovenant_IsRighteous()
        {
            // Mattathias refuses to obey the king's decree, tears down the idol altar, and calls all who are
            // zealous for the Law to come out with him. (Yosippon, the rising of the Hasmoneans)
            // Steadfast covenant faithfulness and courageous refusal of idolatry.
            OracleHarness.Righteous(OracleHarness.Grounded("Mattathias refuses the idol altar and rallies the zealous for the covenant",
                Grounding.InGod(),
                new CovenantFaithfulness(), new CourageousFaith(), new ObedienceToGod(), new Worship(), new Fidelity()));
        }

        [Fact]
        public void GodDeliversJudasAndTheFaithfulAgainstTheGreaterArmy_IsADivineAct()
        {
            // Judas and the few cry that strength comes from Heaven, and the Lord routs the vast pagan host
            // and saves Israel. (Yosippon, the Maccabean victories)
            // Scripture's verdict is the deliverance Heaven works for the weak who trust Him.
            OracleHarness.DivineAct(OracleHarness.Grounded("God delivers Judas and the faithful few against the mighty pagan army",
                Grounding.InGod(),
                new Deliverance(), new Protection(), new DefendingTheWeak(), new JustRecompense(),
                new Fidelity(), new PromiseFulfilled()));
        }

        [Fact]
        public void JudasCleansesAndRededicatesTheSanctuaryWithJoy_IsRighteous()
        {
            // Judas casts out the defilement, restores the altar, and rededicates God's house with song,
            // sacrifice, and gladness. (Yosippon, the purification of the temple)
            // Joyful, worshipful restoration of covenant worship.
            OracleHarness.Righteous(OracleHarness.Grounded("Judas cleanses and rededicates the sanctuary with worship and joy",
                Grounding.InGod(),
                new Worship(), new Joy(), new CovenantFaithfulness(), new RejoicingInTruth(), new ObedienceToGod()));
        }

        [Fact]
        public void HerodMurdersHisOwnKinToSeizeAndHoldThePower_IsSin()
        {
            // Herod, suspicious and cruel, puts to death his own wife, sons, and rivals to secure his throne.
            // (Yosippon, the reign of Herod)
            // Jealous, treacherous bloodshed and murder for the sake of power.
            OracleHarness.Sin(OracleHarness.Witness("Herod murders his own wife and sons to secure his throne",
                new Murder(), new Bloodshed(), new Treachery(), new Jealousy(), new Pride()));
            Assert.Equal(1.0, OracleHarness.Witness("Herod murders his own wife and sons to secure his throne",
                new Murder(), new Bloodshed(), new Treachery(), new Jealousy(), new Pride()).Disorder);
        }

        [Fact]
        public void TheZealotFactionsTearJerusalemApartInCivilStrife_IsSin()
        {
            // Inside the besieged city the rival factions fight one another, burn the storehouses of grain,
            // and shed the blood of their own people. (Yosippon, the war within Jerusalem)
            // Faction, discord, and bloodshed that destroy the city from within.
            OracleHarness.Sin(OracleHarness.Witness("The zealot factions war on one another and shed blood within Jerusalem",
                new Faction(), new Discord(), new Bloodshed(), new SowingDiscord(), new Chaos()));
            Assert.Equal(1.0, OracleHarness.Witness("The zealot factions war on one another and shed blood within Jerusalem",
                new Faction(), new Discord(), new Bloodshed(), new SowingDiscord(), new Chaos()).Disorder);
        }

        [Fact]
        public void TheStarvingMotherSlaysAndDevoursHerOwnChildInTheSiege_IsSin()
        {
            // In the famine of the siege a desperate mother kills and eats her own infant. (Yosippon, the
            // horrors of the famine in besieged Jerusalem)
            // The unnatural slaughter of one's own child, the abyss of the city's ruin.
            OracleHarness.Sin(OracleHarness.Witness("A starving mother slays and devours her own child during the siege",
                new ChildSacrifice(), new Murder(), new Bloodshed(), new Defilement()));
            Assert.Equal(1.0, OracleHarness.Witness("A starving mother slays and devours her own child during the siege",
                new ChildSacrifice(), new Murder(), new Bloodshed(), new Defilement()).Disorder);
        }

        [Fact]
        public void TheRomansBurnTheTempleAndSackTheHolyCity_IsSin()
        {
            // The Roman army storms Jerusalem, sets the sanctuary aflame, slaughters the people, and plunders
            // the holy vessels. (Yosippon, the destruction of the Second Temple)
            // Sacrilegious bloodshed, plunder, and oppression against God's house and people.
            OracleHarness.Sin(OracleHarness.Witness("The Romans burn the temple and sack the holy city of Jerusalem",
                new Bloodshed(), new Defilement(), new Theft(), new Oppression(), new Murder()));
            Assert.Equal(1.0, OracleHarness.Witness("The Romans burn the temple and sack the holy city of Jerusalem",
                new Bloodshed(), new Defilement(), new Theft(), new Oppression(), new Murder()).Disorder);
        }

        [Fact]
        public void TheRemnantMournsTheRuinedSanctuaryAndTurnsBackToGod_IsRighteous()
        {
            // The surviving remnant weeps over the burned house of God, confesses, and turns again to the
            // Lord in humble sorrow. (Yosippon, the lament after Jerusalem's fall)
            // Humble mourning, repentance, and renewed trust in God amid the ruin.
            OracleHarness.Righteous(OracleHarness.Grounded("The remnant mourns the ruined sanctuary and turns back to God",
                Grounding.InGod(),
                new Mourning(), new Repentance(), new Humility(), new Trust(), new Fidelity()));
        }
    }
}
