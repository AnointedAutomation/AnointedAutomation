// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class PresenceTests
    {
        private static Act Mercy()
        {
            return new Act("a mercy", new Compassion());
        }

        [Fact]
        public void Witness_GroundedConformedAndEmpowered_Coheres()
        {
            Reality reality = Reality.Revealed();
            Resolution plain = Reality.Revealed().Witness(Mercy(), Grounding.InGod());

            Resolution triune = reality.Witness(Mercy(), Grounding.InGod(), Presence.HolySpirit());

            Assert.True(triune.Coherence > 0.0);
            Assert.Equal(plain.Coherence, triune.Coherence);
            Assert.Single(reality.Tablets.History());
        }

        [Fact]
        public void Witness_WithoutTheSpirit_HasNoCoherence()
        {
            Resolution resolution = Reality.Revealed().Witness(Mercy(), Grounding.InGod(), Presence.Absent());

            Assert.Equal(0.0, resolution.Coherence);
        }

        [Fact]
        public void Witness_WithoutGround_HasNoCoherence()
        {
            Resolution resolution = Reality.Revealed().Witness(Mercy(), Grounding.Groundless(), Presence.HolySpirit());

            Assert.Equal(0.0, resolution.Coherence);
        }

        [Fact]
        public void Witness_NotConformedToTheWord_HasNoCoherence()
        {
            // An act wholly against God's character has zero conformity to the Word.
            Reality reality = new Reality(new DivineCharacter(new Justice()));
            Resolution resolution = reality.Witness(new Act("a theft", new Plunder()), Grounding.InGod(), Presence.HolySpirit());

            Assert.Equal(0.0, resolution.Coherence);
            Assert.True(resolution.Disorder > 0.0);
        }

        [Fact]
        public void Witness_OnAnIdol_MultipliesLife()
        {
            Resolution god = Reality.Revealed().Witness(Mercy(), Grounding.InGod(), Presence.HolySpirit());
            Resolution idol = Reality.Revealed().Witness(Mercy(), Grounding.InIdol("Baal"), Presence.HolySpirit());

            Assert.Equal(god.Coherence * 0.5, idol.Coherence, 10);
        }

        [Fact]
        public void Word_SpeaksUnderTheTriadicRule()
        {
            Word word = new Word(Reality.Revealed());

            Assert.Equal(0.0, word.Speak(Mercy(), Grounding.InGod(), Presence.Absent()).Coherence);
            Assert.True(word.Speak(Mercy(), Grounding.InGod(), Presence.HolySpirit()).Coherence > 0.0);
        }

        [Fact]
        public void Presence_EmpowermentIsBinary()
        {
            Assert.Equal(1.0, Presence.HolySpirit().Empowerment);
            Assert.Equal(0.0, Presence.Absent().Empowerment);
            Assert.Throws<System.ArgumentNullException>(() => Presence.HolySpirit().Complete(null));
        }

        [Fact]
        public void Grounding_LifeMatchesFoundation()
        {
            Assert.Equal(1.0, Grounding.InGod().Life);
            Assert.Equal(0.5, Grounding.InIdol("Baal").Life);
            Assert.Equal(0.75, Grounding.Divided().Life);
            Assert.Equal(0.0, Grounding.Groundless().Life);
        }

        [Fact]
        public void Witness_RejectsNullPresence()
        {
            Assert.Throws<System.ArgumentNullException>(() => Reality.Revealed().Witness(Mercy(), Grounding.InGod(), null));
        }
    }
}
