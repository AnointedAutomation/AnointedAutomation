// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Meqabyan (Ethiopic Maccabees) narrative oracle: the engine must return the verdict
    // Scripture gives at each step. Canon tag: ETH (Ethiopic-only; best-effort).
    // These three books (1, 2, 3 Meqabyan) of the Ethiopian Orthodox Tewahedo canon are
    // distinct from the Greek Maccabees. Verses follow common ETH versification.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class MeqabyanNarrativeTests
    {
        [Fact]
        public void MeqabisOfBenjaminExaltsHimselfAndSetsUpIdols_IsSin()
        {
            // Meqabis (Tseerutsaydaan), a Midianite king of Benjamin's land, exalts himself,
            // forsakes the living God, and sets up carved idols to be worshipped. (1 Meqabyan 1:6-11)
            // Proud, lawless idolatry that abandons the covenant of the LORD.
            OracleHarness.Sin(OracleHarness.Grounded("Meqabis exalts himself and sets up idols to be worshipped",
                Grounding.InIdol("Meqabis"),
                new Idolatry(), new Pride(), new Lawlessness(), new CovenantBreaking(), new Blasphemy()));
        }

        [Fact]
        public void MeqabisCommandsAllToWorshipHisIdolsAndOppressesTheFaithful_IsSin()
        {
            // The king decrees that every people must bow to his gods, oppressing and afflicting
            // those who cling to the God of Abraham, Isaac, and Jacob. (1 Meqabyan 1:12-20)
            // Tyrannical compulsion to idolatry and oppression of the righteous.
            OracleHarness.Sin(OracleHarness.Witness("Meqabis commands all peoples to worship his idols and oppresses the faithful",
                new Idolatry(), new Oppression(), new Lawlessness(), new Pride()));
        }

        [Fact]
        public void MeqabisSlaughtersThoseWhoWorshipTheTrueGod_IsSin()
        {
            // Meqabis puts to death those who refuse his idols and worship the living God,
            // shedding the blood of the innocent who keep the covenant. (1 Meqabyan 1:21-28)
            // Bloodshed and murder of the righteous; reality is driven to full disorder.
            OracleHarness.Sin(OracleHarness.Witness("Meqabis slaughters those who worship the true God",
                new Murder(), new Bloodshed(), new Oppression(), new Idolatry(), new Hatred()));
            Assert.Equal(1.0, OracleHarness.Witness("Meqabis slaughters those who worship the true God",
                new Murder(), new Bloodshed(), new Oppression(), new Idolatry(), new Hatred()).Disorder);
        }

        [Fact]
        public void TheFiveSonsOfMeqabyanRefuseIdolatryAndConfessTheLivingGod_IsRighteous()
        {
            // Abya, Seela, and Fentos with their brethren refuse the king's idols, confessing,
            // "We worship the God who made heaven and earth," though it means death. (1 Meqabyan 2:1-12)
            // Courageous covenant faithfulness and steadfast confession of the true God.
            OracleHarness.Righteous(OracleHarness.Grounded("The five sons of Meqabyan refuse idolatry and confess the living God",
                Grounding.InGod(),
                new CourageousFaith(), new CovenantFaithfulness(), new ObedienceToGod(),
                new Fidelity(), new Worship()));
        }

        [Fact]
        public void TheSonsOfMeqabyanEndureMartyrdomRatherThanDenyGod_IsRighteous()
        {
            // The brethren are tortured and slain, yet they endure to the end rather than deny the LORD,
            // laying down their lives for His name. (1 Meqabyan 2:13-23)
            // Faithful endurance unto death; self-sacrifice that keeps the covenant whole.
            OracleHarness.Righteous(OracleHarness.Grounded("The sons of Meqabyan endure martyrdom rather than deny God",
                Grounding.InGod(),
                new Endurance(), new SelfSacrifice(), new CourageousFaith(), new Fidelity(),
                new ObedienceToGod(), new Purity()));
        }

        [Fact]
        public void GodAvengesTheMartyrsAndDestroysTheTyrantMeqabis_IsADivineAct()
        {
            // The LORD hears the blood of His servants and brings the proud king to ruin,
            // delivering His people and rendering the tyrant his due. (1 Meqabyan 3:1-12)
            // Scripture's verdict is the just deliverance Heaven works for the oppressed righteous.
            OracleHarness.DivineAct(OracleHarness.Grounded("God avenges the martyrs and destroys the tyrant Meqabis",
                Grounding.InGod(),
                new JustRecompense(), new Deliverance(), new Protection(), new DefendingTheWeak(),
                new Righteousness(), new Fidelity()));
        }

        [Fact]
        public void GodAsCreatorOfHeavenAndEarthIsProclaimed_IsADivineAct()
        {
            // Meqabyan extols the LORD who established the heavens, the sea, and the dry land in order,
            // ruling all creation by His word. (2 Meqabyan 1:1-9; 3:1-6)
            // The ordered work of the Creator who upholds reality.
            OracleHarness.DivineAct(OracleHarness.Grounded("God the Creator establishes the heavens and the earth in order",
                Grounding.InGod(),
                new CreationOrder(), new Goodness(), new Fidelity()));
        }

        [Fact]
        public void GodPromisesToRaiseTheRighteousDeadAtTheResurrection_IsADivineAct()
        {
            // The book proclaims that God will raise the dead and restore life to the righteous who
            // died for His name, fulfilling His promise of resurrection. (2 Meqabyan 5:28-35; 3 Meqabyan 6)
            // Scripture's verdict is the life-restoring faithfulness of God to His own.
            OracleHarness.DivineAct(OracleHarness.Grounded("God promises to raise the righteous dead and restore them to life",
                Grounding.InGod(),
                new RestoringLife(), new PromiseFulfilled(), new Fidelity(), new Deliverance(),
                new CovenantFaithfulness()));
        }

        [Fact]
        public void TheBookCallsSinnersToRepentBeforeTheJudgment_IsRighteous()
        {
            // Meqabyan exhorts the wicked to turn from idols and wickedness, humble themselves,
            // and seek the mercy of God before the day of judgment. (2 Meqabyan 2; 3 Meqabyan 2-4)
            // A righteous call to repentance, humility, and trust in God's mercy.
            OracleHarness.Righteous(OracleHarness.Grounded("Meqabyan calls sinners to repent and humble themselves before the judgment",
                Grounding.InGod(),
                new Repentance(), new Humility(), new Trust(), new ObedienceToGod()));
        }

        [Fact]
        public void GodRewardsTheRighteousAndJudgesTheWickedAtTheLastDay_IsADivineAct()
        {
            // At the last day God gives the righteous their reward and renders the wicked their due,
            // delivering His faithful into rest. (3 Meqabyan 5-7)
            // Scripture's verdict is the just, faithful recompense of the righteous Judge.
            OracleHarness.DivineAct(OracleHarness.Grounded("God rewards the righteous and judges the wicked at the last day",
                Grounding.InGod(),
                new JustRecompense(), new Righteousness(), new Deliverance(), new Fidelity(),
                new PromiseFulfilled()));
        }
    }
}
