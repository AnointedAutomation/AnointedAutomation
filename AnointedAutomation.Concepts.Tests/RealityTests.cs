// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class RealityTests
    {
        [Fact]
        public void Witness_ReturnsTheHarmonizedResolution()
        {
            Reality reality = new Reality(new DivineCharacter(new Justice(), new Mercy()));
            Act theCross = new Act("the cross", new Atonement(), new Pardon());

            Resolution resolution = reality.Witness(theCross);

            Assert.Equal(1.0, resolution.Coherence);
            Assert.Equal(0.0, resolution.Disorder);
        }

        [Fact]
        public void Witness_WritesTheDeedOntoTheTablets()
        {
            // State and truth are one system: witnessing both judges the act and records it.
            Reality reality = new Reality(new DivineCharacter(new Justice(), new Mercy()));
            Act deed = new Act("a kindness", new JustRecompense());

            reality.Witness(deed);

            Assert.Single(reality.Tablets.History());
        }

        [Fact]
        public void WitnessingInjustice_DegradesTheStandingOrderOfReality()
        {
            // As sinners multiply their wrong, the order of reality drifts (1 Enoch 80).
            Reality reality = new Reality(new DivineCharacter(new Justice(), new Mercy()));

            Assert.Equal(1.0, reality.Tablets.Coherence());

            reality.Witness(new Act("a theft", new Theft()));

            // Theft is serious (not capital), so it degrades the world's order without zeroing it.
            Assert.Equal(0.5, reality.Tablets.Coherence());
        }

        [Fact]
        public void WitnessBorneOnAnIdol_RecordsGreaterDisorderThanOnGod()
        {
            // The same outwardly-right deed cannot hold on a dead foundation (1 Meqabyan).
            Act deed = new Act("alms to be seen", new JustRecompense());

            Reality onGod = Reality.Revealed();
            onGod.Witness(deed, Grounding.InGod());

            Reality onIdol = Reality.Revealed();
            onIdol.Witness(deed, Grounding.InIdol("vainglory"));

            Assert.Equal(1.0, onGod.Tablets.Coherence());
            Assert.True(onIdol.Tablets.Coherence() < 1.0);
        }

        [Fact]
        public void Revealed_WiresTheStandardCharacter()
        {
            // The factory reveals reality already grounded in God's character; no facet is missing.
            Reality reality = Reality.Revealed();
            Act theCross = new Act("the cross", new Atonement(), new Pardon());

            Resolution resolution = reality.Witness(theCross);

            Assert.True(resolution.Reading("Justice") >= 0.5);
            Assert.True(resolution.Reading("Mercy") >= 0.5);
        }
    }
}
