// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// Gap 3: the world's standing order is no longer doomed to permanent decay. Repentance and
    /// atonement RESTORE the record. "If my people ... will humble themselves and pray ... then I
    /// will hear ... and will heal their land." (2 Chronicles 7:14); "where sin increased, grace
    /// increased all the more." (Romans 5:20)
    /// </summary>
    public class RedemptionTests
    {
        [Fact]
        public void RepentanceAfterSin_PartlyRestoresTheWorldsOrder()
        {
            Reality reality = Reality.Revealed();
            reality.Witness(new Act("a murder", new Murder()));
            Assert.Equal(0.0, reality.Tablets.Coherence());

            reality.Witness(new Act("the people repent", new Repentance()));

            Assert.Equal(0.5, reality.Tablets.Coherence());   // healed halfway back
        }

        [Fact]
        public void AtonementAfterSin_FullyRestoresTheWorldsOrder()
        {
            // The cross does what repentance alone cannot: it heals the breach completely (Rom 5:20).
            Reality reality = Reality.Revealed();
            reality.Witness(new Act("a murder", new Murder()));
            Assert.Equal(0.0, reality.Tablets.Coherence());

            reality.Witness(new Act("the atonement", new Atonement()));

            Assert.Equal(1.0, reality.Tablets.Coherence());
        }

        [Fact]
        public void ABareRighteousDeed_RestoresNothing()
        {
            // Only repentance, atonement, forgiveness, and grace restore. A merely-just deed does not
            // heal what sin has broken.
            Reality reality = Reality.Revealed();
            reality.Witness(new Act("a murder", new Murder()));

            reality.Witness(new Act("a just recompense", new JustRecompense()));

            Assert.Equal(0.0, reality.Tablets.Coherence());
        }
    }
}
