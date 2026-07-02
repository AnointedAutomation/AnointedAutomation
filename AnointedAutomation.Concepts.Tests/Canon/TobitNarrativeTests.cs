// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Tobit narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class TobitNarrativeTests
    {
        [Fact]
        public void TobitGivesAlmsAndFeedsTheHungryInExile_IsRighteous()
        {
            // "I gave many alms to my kindred ... I would give my bread to the hungry and my garments to the naked." (Tobit 1:3,16-17)
            // Tobit's lifelong charity to the poor and exiled.
            OracleHarness.Righteous(OracleHarness.Witness("Tobit gives alms, feeds the hungry, clothes the naked",
                new Generosity(), new FeedingTheHungry(), new ClothingTheNaked(),
                new Compassion(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void TobitWalksInCovenantFaithfulnessWhileOthersApostatize_IsRighteous()
        {
            // While the tribes sacrificed to Baal, Tobit alone went to Jerusalem to worship the LORD. (Tobit 1:4-8)
            // Steadfast obedience to the Law and worship of the one true God in a faithless generation.
            OracleHarness.Righteous(OracleHarness.Witness("Tobit alone keeps the Law and worships at Jerusalem",
                new ObedienceToGod(), new Worship(), new CovenantFaithfulness(),
                new Fidelity(), new CourageousFaith()));
        }

        [Fact]
        public void TobitRisksHisLifeToBuryTheDead_IsRighteous()
        {
            // "If the king ... slew any of the Jews, I buried them privily." (Tobit 1:17-19; 2:3-8)
            // At peril of death Tobit honors the abandoned slain by burying them.
            OracleHarness.Righteous(OracleHarness.Witness("Tobit secretly buries the murdered dead despite the king's wrath",
                new CourageousFaith(), new HonoringTheVulnerable(), new Compassion(),
                new SelfSacrifice(), new Righteousness()));
        }

        [Fact]
        public void TheDemonAsmodeusSlaysSarahsSevenHusbands_IsSin()
        {
            // "Asmodeus the evil spirit had slain" Sarah's seven husbands before they came to her. (Tobit 3:8)
            // The demon's murderous jealousy is capital bloodshed and disorders reality utterly.
            Resolution r = OracleHarness.Grounded("the demon Asmodeus murders Sarah's seven bridegrooms",
                Grounding.InIdol("Asmodeus"),
                new Murder(), new Bloodshed(), new Jealousy(), new Malice());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TobitPraysInHisAfflictionAndBlindness_IsRighteous()
        {
            // Blinded and grieved, Tobit prays: "Deal with me according to thy pleasure ... command my spirit to be taken." (Tobit 3:1-6)
            // Humble, repentant trust that justifies God even in suffering.
            OracleHarness.Righteous(OracleHarness.Witness("Tobit prays in his blindness, trusting God's justice",
                new Repentance(), new Trust(), new Humility(), new Worship(), new Endurance()));
        }

        [Fact]
        public void SarahPraysInHerDistressRefusingToCurseOrDespair_IsRighteous()
        {
            // Reproached and grieved to death, Sarah will not take her life but prays toward heaven. (Tobit 3:11-15)
            // She blesses God and pleads her innocence with patient trust.
            OracleHarness.Righteous(OracleHarness.Witness("Sarah prays toward heaven in her grief and blesses God",
                new Worship(), new Trust(), new Purity(), new Humility(), new Endurance()));
        }

        [Fact]
        public void GodSendsRaphaelToHealTobitAndSarah_IsADivineAct()
        {
            // "The prayers of them both were heard ... Raphael was sent to heal them both." (Tobit 3:16-17)
            // God hears the afflicted and dispatches His angel to deliver and heal.
            OracleHarness.DivineAct(OracleHarness.Witness("God sends the angel Raphael to heal Tobit and Sarah",
                new Healing(), new Deliverance(), new Compassion(),
                new CovenantFaithfulness(), new Goodness()));
        }

        [Fact]
        public void TobitTeachesTobiasToFearGodAndGiveAlms_IsRighteous()
        {
            // "Give alms of thy substance ... for alms deliver from death." (Tobit 4:5-11,16)
            // The father instructs his son in righteousness, charity, and the fear of the Lord.
            OracleHarness.Righteous(OracleHarness.Witness("Tobit instructs Tobias to fear God and give alms",
                new TeachingTruth(), new Generosity(), new ObedienceToGod(),
                new Righteousness(), new FeedingTheHungry()));
        }

        [Fact]
        public void RaphaelGuidesAndProtectsTobiasOnTheJourney_IsADivineAct()
        {
            // The angel goes with Tobias as a faithful guide and guardian on the road to Media. (Tobit 5:4-6:1)
            // God's messenger protects and leads the son safely.
            OracleHarness.DivineAct(OracleHarness.Witness("Raphael guides and guards Tobias on the journey",
                new Protection(), new Trustworthiness(), new Fidelity(),
                new Deliverance(), new Goodness()));
        }

        [Fact]
        public void TobiasBindsTheDemonAndMarriesSarahInPrayer_IsRighteous()
        {
            // Tobias burns the fish heart so the demon flees, then prays with Sarah: "not for lust but in truth." (Tobit 8:1-9)
            // Pure, prayerful marriage that drives out evil and honors God.
            OracleHarness.Righteous(OracleHarness.Witness("Tobias drives off the demon and weds Sarah in pure prayer",
                new Purity(), new Worship(), new SelfControl(),
                new CourageousFaith(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TobiasHealsHisFathersBlindnessWithTheGall_IsRighteous()
        {
            // Tobias anoints Tobit's eyes with the fish gall and his sight is restored. (Tobit 11:10-14)
            // The son, by God's appointed remedy, heals his blind father and restores him.
            OracleHarness.Righteous(OracleHarness.Witness("Tobias anoints and heals his father's blind eyes",
                new Healing(), new CaringForTheSick(), new Compassion(), new Goodness()));
        }

        [Fact]
        public void RaphaelRevealsHePresentedTheirPrayersBeforeGod_IsADivineAct()
        {
            // "I am Raphael, one of the seven holy angels, which ... go in before the glory of the Holy One." (Tobit 12:6-15)
            // The angel reveals he bore their prayers and almsdeeds to God, who heard and delivered them.
            OracleHarness.DivineAct(OracleHarness.Witness("Raphael reveals he carried their prayers before God's glory",
                new Deliverance(), new Healing(), new Fidelity(),
                new RejoicingInTruth(), new Goodness()));
        }

        [Fact]
        public void TobitSingsHisHymnBlessingTheGodOfIsrael_IsRighteous()
        {
            // "Blessed be God that liveth for ever ... let all men praise him." (Tobit 13:1-18)
            // Restored Tobit pours out a hymn of joyful, faithful praise to the Lord.
            OracleHarness.Righteous(OracleHarness.Witness("Tobit sings his hymn of praise to the everlasting God",
                new Worship(), new Joy(), new RejoicingInTruth(),
                new Fidelity(), new Goodness()));
        }
    }
}
