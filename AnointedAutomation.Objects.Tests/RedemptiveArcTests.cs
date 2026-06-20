// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    /// <summary>
    /// The whole arc of Scripture as a regression oracle: creation and the fall, covenant, the
    /// exodus and the law, Israel as a light to the nations, the line that brings forth the Messiah,
    /// and Jesus who saves the entire world. The engine must return the verdict the Biblical record
    /// gives at every step. The acts of God and of Jesus read coherent and never disorder reality;
    /// alignment with God flourishes, rebellion brings disorder; the deeds that set Israel apart in
    /// antiquity (not sacrificing children, honoring the vulnerable and the stranger) read coherent,
    /// and the surrounding nations' practices read as sin. If any step disagrees with Scripture, the
    /// code is wrong.
    /// </summary>
    public class RedemptiveArcTests
    {
        // ===== Creation and the Fall (Genesis 1-3) =====

        [Fact]
        public void GodCreatesAndOrdersTheWorld_IsVeryGood()
        {
            // "God saw all that he had made, and it was very good." (Genesis 1:31)
            AssertDivineAct(Witness(new Act("creation", new CreationOrder(), new CovenantFaithfulness())));
        }

        [Fact]
        public void AdamAndEveWalkWithGod_IsCoherent()
        {
            // Before the fall, they dwell with God in the garden, aligned with Him (Genesis 2).
            AssertRighteous(Witness(new Act("walking with God in the garden", new ObedienceToGod())));
        }

        [Fact]
        public void TheFall_IsSin_AndDisordersCreation()
        {
            // Eating what God forbade is rebellion; death and the curse enter (Genesis 3:6, 3:17-19;
            // Romans 5:12). Disorder enters reality.
            AssertSin(Witness(new Act("the fall", new Disobedience(), new Rebellion())));
        }

        // ===== Covenant (Noah, Abraham) =====

        [Fact]
        public void GodMakesAndKeepsCovenant_IsFaithful()
        {
            // The covenants with Noah and Abraham (Genesis 9; 15; 17). God keeps His word.
            AssertDivineAct(Witness(new Act("the covenant",
                new CovenantFaithfulness(), new Trustworthiness())));
        }

        [Fact]
        public void AbrahamTrustsGod_IsCreditedAsRighteousness()
        {
            // "Abram believed the LORD, and he credited it to him as righteousness." (Genesis 15:6)
            AssertRighteous(Witness(new Act("Abraham believes God",
                new ObedienceToGod(), new CovenantFaithfulness())));
        }

        // ===== Exodus: deliverance, and judgment on the oppressor =====

        [Fact]
        public void GodDeliversIsraelFromEgypt_IsMercyAndFaithfulness()
        {
            // "I have come down to rescue them from the hand of the Egyptians." (Exodus 3:8)
            AssertDivineAct(Witness(new Act("the exodus", new Deliverance(), new CovenantFaithfulness())));
        }

        [Fact]
        public void PharaohEnslavesAndOppresses_IsSin_AndDisordersReality()
        {
            // Pharaoh oppresses the Hebrews and orders their sons killed (Exodus 1).
            AssertSin(Witness(new Act("Pharaoh's oppression", new Oppression(), new Wrongdoing())));
        }

        // ===== The Law: safe when aligned, disciplined when not =====

        [Fact]
        public void IsraelAlignedWithGod_IsKeptSafe()
        {
            // "If you fully obey the LORD your God ... you will be blessed." (Deuteronomy 28:1-2)
            AssertRighteous(Witness(new Act("Israel keeps the covenant", new ObedienceToGod())));
        }

        [Fact]
        public void IsraelRebelsAgainstGod_BringsDisorder()
        {
            // The golden calf and the wilderness rebellions (Exodus 32; Numbers 14).
            AssertSin(Witness(new Act("Israel rebels", new Disobedience(), new Rebellion())));
        }

        // ===== The Law set Israel apart in antiquity =====

        [Fact]
        public void SacrificingChildren_AsTheNationsDid_IsGraveSin()
        {
            // "Do not give any of your children to be sacrificed to Molek." (Leviticus 18:21;
            // Deuteronomy 12:31). The practice of the surrounding nations is detestable.
            AssertSin(Witness(new Act("child sacrifice to Molek", new ChildSacrifice(), new Bloodshed())));
        }

        [Fact]
        public void ForbiddingChildSacrificeAndProtectingTheChild_IsCoherent()
        {
            // The law that forbids it protects the vulnerable.
            AssertRighteous(Witness(new Act("the child is protected",
                new HonoringTheVulnerable(), new Protection())));
        }

        [Fact]
        public void WelcomingTheStrangerAndHonoringTheVulnerable_IsCoherent()
        {
            // "Love the foreigner ... for you were foreigners in Egypt." (Deuteronomy 10:18-19);
            // care for the widow and the orphan (Exodus 22:21-22).
            AssertRighteous(Witness(new Act("the stranger welcomed",
                new Hospitality(), new HonoringTheVulnerable())));
        }

        // ===== Israel brings forth the Messiah =====

        [Fact]
        public void GodPreservesTheLineToBringForthTheMessiah_IsFaithful()
        {
            // From the promise in Eden (Genesis 3:15) through the line of David to Christ
            // (Matthew 1:1-17). God is faithful to bring it about.
            AssertDivineAct(Witness(new Act("the messianic line preserved",
                new CovenantFaithfulness(), new PromiseFulfilled())));
        }

        // ===== Jesus saves the entire world: every recorded act is verifiable =====

        [Fact]
        public void TheIncarnation_TheWordMadeFlesh_IsCoherent()
        {
            // "The Word became flesh and made his dwelling among us." (John 1:14)
            AssertDivineAct(Witness(new Act("the incarnation", new Compassion(), new PromiseFulfilled())));
        }

        [Fact]
        public void JesusHealsTheSick_IsCoherent_AndDestabilizesNothing()
        {
            // He healed every disease and sickness (Matthew 9:35).
            AssertDivineAct(Witness(new Act("Jesus heals the sick", new Healing(), new Compassion())));
        }

        [Fact]
        public void JesusForgivesSins_IsMercy()
        {
            // "Son, your sins are forgiven." (Mark 2:5)
            AssertDivineAct(Witness(new Act("Jesus forgives sins", new Forgiveness(), new Pardon())));
        }

        [Fact]
        public void JesusTeachesTheTruth_IsCoherent()
        {
            // "You will know the truth, and the truth will set you free." (John 8:32)
            AssertDivineAct(Witness(new Act("Jesus teaches the truth", new TeachingTruth())));
        }

        [Fact]
        public void JesusCleansesTheTemple_IsRighteousZeal_NotDisorder()
        {
            // Driving out those who made His Father's house a market (John 2:13-17). Righteous zeal,
            // not sin: it renders what is due and defends the wronged.
            AssertDivineAct(Witness(new Act("Jesus cleanses the temple",
                new JustRecompense(), new DefendingTheWeak())));
        }

        [Fact]
        public void JesusRaisesTheDead_RestoresLifeAndOrder()
        {
            // "Lazarus, come out!" (John 11:43); the resurrection (Matthew 28). Death is reversed:
            // the very opposite of disorder.
            AssertDivineAct(Witness(new Act("Jesus raises the dead",
                new RestoringLife(), new CreationOrder())));
        }

        [Fact]
        public void TheCross_SavesTheEntireWorld_PerfectlyCoherent()
        {
            // "For God so loved the world." (John 3:16); the fullness of God's character in one act.
            Resolution r = Witness(new Act("the cross saves the world",
                new SelfSacrifice(), new Atonement(), new Pardon(), new CovenantFaithfulness()));

            Assert.Equal(1.0, r.Coherence);
            Assert.Equal(0.0, r.Disorder);
        }

        // ===== Oracle helpers =====

        private static Resolution Witness(Act act)
        {
            return Reality.Revealed().Witness(act);
        }

        private static void AssertDivineAct(Resolution r)
        {
            Assert.True(r.Coherence >= 0.5, "An act of God can never read incoherent.");
            Assert.Equal(0.0, r.Disorder);
        }

        private static void AssertRighteous(Resolution r)
        {
            Assert.True(r.Coherence > 0.5, "A righteous deed must read coherent.");
            Assert.Equal(0.0, r.Disorder);
        }

        private static void AssertSin(Resolution r)
        {
            Assert.True(r.Coherence < 0.5, "Sin must read incoherent.");
            Assert.True(r.Disorder > 0.0, "Sin must introduce disorder into reality.");
        }
    }
}
