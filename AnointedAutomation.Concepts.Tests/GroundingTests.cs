// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class GroundingTests
    {
        [Fact]
        public void InGod_IsGroundedInTheLivingGod()
        {
            Grounding grounding = Grounding.InGod();

            Assert.True(grounding.IsInGod);
        }

        [Fact]
        public void InIdol_IsNotGroundedInGod_AndIsNamed()
        {
            // The idols "do not truly live, they cannot give life." (1 Meqabyan 1)
            Grounding grounding = Grounding.InIdol("Baal");

            Assert.False(grounding.IsInGod);
            Assert.Equal("Baal", grounding.Name);
        }

        [Fact]
        public void GroundedInGod_BearsADeedUnchanged()
        {
            // What stands on the living God keeps its life (1 Meqabyan: God alone gives life).
            Grounding grounding = Grounding.InGod();
            Resolution deed = new Resolution(1.0, 0.0);

            Resolution borne = grounding.Bear(deed);

            Assert.Equal(1.0, borne.Coherence);
            Assert.Equal(0.0, borne.Disorder);
        }

        [Fact]
        public void GroundedInAnIdol_DriftsADeedTowardNonBeing()
        {
            // Even an outwardly good deed cannot hold when its foundation has no ground: its life
            // drains and disorder rises (1 Meqabyan).
            Grounding grounding = Grounding.InIdol("Baal");
            Resolution deed = new Resolution(1.0, 0.0);

            Resolution borne = grounding.Bear(deed);

            Assert.True(borne.Coherence < 1.0);
            Assert.True(borne.Disorder > 0.0);
        }
    }
}
