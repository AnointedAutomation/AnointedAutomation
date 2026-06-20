// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    /// <summary>
    /// Scripture is the regression oracle. The acts of God, Jesus, and the Holy Spirit are the
    /// standard of reality, so the engine must ALWAYS agree with the Biblical record: a divine act
    /// can never read incoherent and can never introduce disorder, sin must always read incoherent
    /// and destabilize reality, and righteous deeds must cohere. If the engine ever returns a
    /// different verdict than Scripture, the code is wrong, not the Bible.
    /// </summary>
    public class BiblicalOracleTests
    {
        // ----- The acts of God / Jesus / the Spirit: the standard. Never disorder, never incoherent.

        [Fact]
        public void TheCross_IsTheFullnessOfGodsCharacter_PerfectlyCoherent()
        {
            // The supreme act: full Love (lays down His life, John 15:13), full Justice (the debt is
            // paid, Romans 3:25), full Mercy (the guilty go free), and faithful to the promise
            // (Genesis 3:15; Luke 24:44). Every facet of God is satisfied at once.
            Reality reality = Reality.Revealed();
            Act theCross = new Act("Christ crucified",
                new SelfSacrifice(), new Atonement(), new Pardon(), new CovenantFaithfulness());

            Resolution r = reality.Witness(theCross);

            Assert.Equal(1.0, r.Coherence);
            Assert.Equal(0.0, r.Disorder);
        }

        [Fact]
        public void GodSoLovedTheWorld_IsLove_AndDestabilizesNothing()
        {
            // "For God so loved the world that he gave his one and only Son." (John 3:16)
            Reality reality = Reality.Revealed();
            Act gift = new Act("the Father gives the Son", new SelfSacrifice(), new Compassion());

            Resolution r = reality.Witness(gift);

            AssertDivineAct(r);
            Assert.Equal(1.0, r.Reading("Love"));
        }

        [Fact]
        public void GodKeepsCreationsOrder_IsFaithful_AndDestabilizesNothing()
        {
            // God ordains and sustains the courses of creation (Genesis 1; 1 Enoch 72-82;
            // Colossians 1:17). His faithfulness never fails (Lamentations 3:22-23).
            Reality reality = Reality.Revealed();
            Act sustaining = new Act("the courses of the heavens kept",
                new CreationOrder(), new CovenantFaithfulness());

            Resolution r = reality.Witness(sustaining);

            AssertDivineAct(r);
            Assert.Equal(1.0, r.Reading("Faithfulness"));
        }

        // ----- Sin: always incoherent, always introduces disorder (1 Enoch 80).

        [Fact]
        public void CainMurdersAbel_IsIncoherent_AndDisordersReality()
        {
            // "Your brother's blood cries out to me from the ground." (Genesis 4:10) The first murder
            // wrongs the neighbour and curses the very ground: disorder enters creation.
            Reality reality = Reality.Revealed();
            Act murder = new Act("Cain kills Abel", new Murder());

            Resolution r = reality.Witness(murder);

            AssertSin(r);
        }

        [Fact]
        public void TheUnforgivingServant_IsIncoherent_AndDisordersReality()
        {
            // Forgiven much, he would not forgive (Matthew 18:23-35). Mercy refused is grievous.
            Reality reality = Reality.Revealed();
            Act cruelty = new Act("the unforgiving servant", new Unforgiveness(), new Condemnation());

            Resolution r = reality.Witness(cruelty);

            AssertSin(r);
        }

        [Fact]
        public void CovenantBetrayal_IsIncoherent_AndDisordersReality()
        {
            // Breaking faith transgresses the order God decreed (1 Enoch 80; Malachi 2:10).
            Reality reality = Reality.Revealed();
            Act betrayal = new Act("a covenant broken", new Treachery());

            Resolution r = reality.Witness(betrayal);

            AssertSin(r);
        }

        // ----- Righteous human deeds: coherent, no disorder. The priest who passes by is not.

        [Fact]
        public void TheGoodSamaritan_IsCoherent_AndDestabilizesNothing()
        {
            // "Go and do likewise." (Luke 10:37) The mercy shown is the coherent act.
            Reality reality = Reality.Revealed();
            Act mercy = new Act("the Samaritan binds the wounds", new Compassion(), new Protection());

            Resolution r = reality.Witness(mercy);

            Assert.True(r.Coherence > 0.5, "Mercy shown must read coherent.");
            Assert.Equal(0.0, r.Disorder);
        }

        [Fact]
        public void ThePriestPassesByOnTheOtherSide_IsIncoherent_AndDisordersReality()
        {
            // The priest and Levite "passed by on the other side." (Luke 10:31-32) Compassion withheld
            // for self is not neutral; it offends love and mercy.
            Reality reality = Reality.Revealed();
            Act passingBy = new Act("passing by the half-dead man", new SelfSeeking(), new Indifference());

            Resolution r = reality.Witness(passingBy);

            AssertSin(r);
        }

        // ----- Grounding: an idolatrous act drifts toward non-being (1 Meqabyan; Exodus 32).

        [Fact]
        public void TheGoldenCalf_EvenAsWorship_DriftsTowardNonBeing()
        {
            // Worship offered to an idol cannot give life (1 Meqabyan 1; Exodus 32). The same outward
            // act of devotion, grounded in an idol, cannot hold.
            Act worship = new Act("a sacrifice offered", new CovenantFaithfulness());

            Reality toGod = Reality.Revealed();
            Resolution faithful = toGod.Witness(worship, Grounding.InGod());

            Reality toCalf = Reality.Revealed();
            Resolution idolatrous = toCalf.Witness(worship, Grounding.InIdol("the golden calf"));

            Assert.Equal(0.0, faithful.Disorder);
            Assert.True(idolatrous.Disorder > 0.0, "Idolatry drifts reality toward non-being.");
            Assert.True(idolatrous.Coherence < faithful.Coherence);
        }

        // ----- The engine, not a lookup: scenarios never named in Scripture must resolve rightly.

        [Fact]
        public void NovelMercy_NeverNamedInScripture_StillReadsCoherent()
        {
            // A stranger online talks someone off the ledge. Never in the Bible by name, yet love is
            // love: the engine must judge it by principle, not by a catalogue of stories.
            Reality reality = Reality.Revealed();
            Act rescue = new Act("a stranger talks someone out of suicide",
                new Compassion(), new Protection());

            Resolution r = reality.Witness(rescue);

            Assert.True(r.Coherence > 0.5);
            Assert.Equal(0.0, r.Disorder);
        }

        [Fact]
        public void NovelCruelty_NeverNamedInScripture_StillReadsAsSin()
        {
            // Doxxing someone for revenge. Not in Scripture by name, but it wrongs the neighbour,
            // keeps a record of wrongs, and condemns the broken: the engine must still read it as sin.
            Reality reality = Reality.Revealed();
            Act revenge = new Act("doxxing someone for revenge",
                new Wrongdoing(), new Grudge(), new Condemnation());

            Resolution r = reality.Witness(revenge);

            AssertSin(r);
        }

        // ----- Oracle helpers: the verdicts Scripture requires.

        private static void AssertDivineAct(Resolution r)
        {
            // God is the standard of reality: His acts never read incoherent and never destabilize it.
            Assert.True(r.Coherence >= 0.5, "An act of God can never read incoherent.");
            Assert.Equal(0.0, r.Disorder);
        }

        private static void AssertSin(Resolution r)
        {
            // Sin offends God's character and warps the order of reality (1 Enoch 80).
            Assert.True(r.Coherence < 0.5, "Sin must read incoherent.");
            Assert.True(r.Disorder > 0.0, "Sin must introduce disorder into reality.");
        }
    }
}
