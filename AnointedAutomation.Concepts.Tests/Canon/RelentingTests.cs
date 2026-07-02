// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    /// <summary>
    /// Gap 7: God's dealings are conditional on response. A pronounced judgment relents when the
    /// guilty repent, and a standing order is forfeited when the faithful turn away. "If that nation
    /// ... repents ... then I will relent. ... And if it does evil ... then I will reconsider the
    /// good I had intended." (Jeremiah 18:7-10); "When God saw ... that they turned from their evil
    /// ways, he relented." (Jonah 3:10). The conditionality emerges from witnessing the RESPONSE: the
    /// world's standing order recovers on repentance and falls on apostasy.
    /// </summary>
    public class RelentingTests
    {
        [Fact]
        public void JudgmentRelentsWhenTheGuiltyRepent()
        {
            // Nineveh: wickedness brings the world to ruin, then the city repents and God relents.
            Reality reality = Reality.Revealed();
            reality.Witness(new Act("Nineveh's violence", new Bloodshed()));
            Assert.Equal(0.0, reality.Tablets.Coherence());

            reality.Witness(new Act("Nineveh repents in sackcloth", new Repentance()));

            Assert.True(reality.Tablets.Coherence() > 0.0, "On repentance, the LORD relents.");
        }

        [Fact]
        public void JudgmentStandsWhenThereIsNoRepentance()
        {
            // Where wickedness only deepens, the ruin remains (Nahum: Nineveh, generations later).
            Reality reality = Reality.Revealed();
            reality.Witness(new Act("violence", new Bloodshed()));
            reality.Witness(new Act("more violence, unrepentant", new Oppression()));

            Assert.Equal(0.0, reality.Tablets.Coherence());
        }

        [Fact]
        public void TheStandingOrderIsForfeitedOnApostasy()
        {
            // "If it does evil ... I will reconsider the good I had intended." (Jeremiah 18:10)
            Reality reality = Reality.Revealed();
            reality.Witness(new Act("a faithful generation", new CovenantFaithfulness()));
            Assert.Equal(1.0, reality.Tablets.Coherence());

            reality.Witness(new Act("the next generation turns to idols", new Idolatry(), new Rebellion()));

            Assert.True(reality.Tablets.Coherence() < 1.0, "Apostasy forfeits the standing order.");
        }
    }
}
