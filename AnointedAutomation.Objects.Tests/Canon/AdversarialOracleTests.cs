// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// The adversarial sweep: the hard cases where a naive reading fights the text. The finding is
    /// twofold. (1) Most "hard cases" resolve once you assign the RIGHT concept rather than the
    /// shocking surface label: Abraham's act is obedience and trust, not child sacrifice; Jael's is
    /// deliverance, not murder; the imprecatory cry is an appeal to God's justice, not enacted
    /// murder. The engine judges the concepts you give it; it does not adjudicate the theology for
    /// you. (2) ONE case genuinely needs context, Rahab's deception, where the very same act-feature
    /// is present yet Scripture refuses to condemn it. That is why the engine now carries a Context
    /// dimension.
    /// </summary>
    public class AdversarialOracleTests
    {
        private static Circumstance[] ProtectingTheInnocent()
        {
            return new[] { new Circumstance("protectingInnocentLife") };
        }

        // ----- The case that genuinely needs context: the same deceit, read two ways. -----

        [Fact]
        public void RahabsDeception_ReadNaively_LooksLikeSin()
        {
            // Stripped of context, a deception offends the God of truth.
            OracleHarness.Sin(OracleHarness.Witness("a bare deception", new Deceit()));
        }

        [Fact]
        public void RahabsDeception_ToSaveTheSpies_IsRighteous()
        {
            // But Rahab hid the spies from those who would kill them, and Scripture commends her
            // faith (Joshua 2; Hebrews 11:31; James 2:25). In that context the deceit is not the lie
            // God condemns. The verdict FLIPS, which is exactly why context had to be modeled.
            OracleHarness.Righteous(OracleHarness.WitnessInContext(
                "Rahab hides the spies from those who would kill them",
                ProtectingTheInnocent(), new Deceit(), new Protection()));
        }

        [Fact]
        public void TheHebrewMidwives_DeceivingPharaohToSaveTheBabies_IsRighteous()
        {
            // "The midwives feared God ... so God was kind to them." (Exodus 1:17-21)
            OracleHarness.Righteous(OracleHarness.WitnessInContext(
                "the midwives deceive Pharaoh to spare the newborns",
                ProtectingTheInnocent(), new Deceit(), new CourageousFaith()));
        }

        // ----- Cases that resolve by assigning the right concept, not the surface label. -----

        [Fact]
        public void AbrahamOffersIsaac_IsObedientFaith_NotChildSacrifice()
        {
            // The act Scripture records is faith: Abraham trusted that God could raise the dead, and
            // God provided the ram (Genesis 22:13; Hebrews 11:17-19). The sacrifice was never carried
            // out. The right concepts are obedience and trust, and the verdict is righteous.
            OracleHarness.Righteous(OracleHarness.Witness("Abraham offers Isaac in faith",
                new ObedienceToGod(), new Trust()));
        }

        [Fact]
        public void JaelKillsSisera_IsDeliverance_NotMurder()
        {
            // "Most blessed of women be Jael." (Judges 5:24) The text frames the killing of the enemy
            // commander as the deliverance of an oppressed people, not as murder.
            OracleHarness.Righteous(OracleHarness.Witness("Jael delivers Israel from Sisera",
                new Deliverance(), new CourageousFaith()));
        }

        [Fact]
        public void TheImprecatoryCry_IsAnAppealToGodsJustice_NotEnactedMurder()
        {
            // "Repay her for what she has done." (Psalm 137:8) The psalmist hands vengeance to God
            // (Deuteronomy 32:35; Romans 12:19); he does not lift the sword himself. The shocking
            // words are lament committed to the Judge, an appeal to justice.
            OracleHarness.Righteous(OracleHarness.Witness("the cry for God to repay the oppressor",
                new JustRecompense()));
        }

        // ----- The genuine judgment call, stated as such. -----

        [Fact]
        public void TheConquest_FramedAsDivineJudgment_ReadsAsJustWhenSoAssigned()
        {
            // JUDGMENT CALL, flagged honestly: the herem can be framed as God's just judgment on
            // persistent wickedness (including the Canaanites' own child sacrifice, Deuteronomy 9:4-5;
            // 12:31), executed in obedience. Assigned those concepts, the engine reads it as just.
            // The engine does NOT settle the theology; it reports the verdict of whatever moral
            // concepts you assign, and this is the traditional framing.
            OracleHarness.DivineAct(OracleHarness.WitnessInContext(
                "the judgment on the Canaanites under divine command",
                new[] { new Circumstance("divineCommand") }, new JustRecompense(), new ObedienceToGod()));
        }

        [Fact]
        public void TheModelsLimit_IsHonest_NaiveLabelingCanMisjudge()
        {
            // The lesson of the sweep: the same outward facts can be labeled "child sacrifice" or
            // "obedient faith," "murder" or "deliverance." The engine is only as faithful as the
            // concepts assigned to a deed. Labeled crudely, it can misjudge; labeled as Scripture
            // frames the act, it agrees with Scripture. That dependency is the model's real boundary.
            Resolution crude = OracleHarness.Witness("Abraham raises the knife, labeled crudely", new ChildSacrifice());
            Resolution faithful = OracleHarness.Witness("Abraham offers Isaac in faith", new ObedienceToGod(), new Trust());

            OracleHarness.Sin(crude);
            OracleHarness.Righteous(faithful);
        }
    }
}
