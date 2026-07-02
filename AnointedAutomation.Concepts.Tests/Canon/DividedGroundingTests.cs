// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    /// <summary>
    /// Gap 4: grounding is no longer only the binary of God-or-idol. A divided heart, lukewarm and
    /// double-minded, stands between them. "How long will you waver between two opinions?" (1 Kings
    /// 18:21); "a double-minded man, unstable in all he does" (James 1:8); "because you are lukewarm"
    /// (Revelation 3:16).
    /// </summary>
    public class DividedGroundingTests
    {
        [Fact]
        public void ADividedHeart_IsNeitherInGodNorAnIdol()
        {
            Assert.False(Grounding.Divided().IsInGod);
            Assert.Equal("a divided heart", Grounding.Divided().Name);
        }

        [Fact]
        public void ADeedOnADividedHeart_DriftsPartway_BetweenGodAndIdol()
        {
            Resolution deed = new Resolution(1.0, 0.0);

            Resolution onGod = Grounding.InGod().Bear(deed);
            Resolution onDivided = Grounding.Divided().Bear(deed);
            Resolution onIdol = Grounding.InIdol("Baal").Bear(deed);

            // A divided heart keeps three-quarters of the deed's life and drifts a quarter.
            Assert.Equal(0.75, onDivided.Coherence);
            Assert.Equal(0.25, onDivided.Disorder);

            // Strictly between standing on God and standing on a dead idol.
            Assert.True(onDivided.Coherence < onGod.Coherence);
            Assert.True(onDivided.Coherence > onIdol.Coherence);
            Assert.True(onDivided.Disorder > onGod.Disorder);
            Assert.True(onDivided.Disorder < onIdol.Disorder);
        }
    }
}
