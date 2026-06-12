// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// Gap 6: a sacrament in itself is holy and reads as a divine mystery, but it can be profaned by
    /// the sin of the one who handles it. The engine represents this by composition: the mystery
    /// together with the sin of unworthy reception or simony no longer reads as a pure divine act, it
    /// carries the disorder of the profanation. "Whoever eats the bread or drinks the cup in an
    /// unworthy manner ... eats and drinks judgment on themselves." (1 Corinthians 11:27-29)
    /// </summary>
    public class ProfanedSacramentsTests
    {
        [Fact]
        public void TheSacramentInItself_IsAHolyMystery()
        {
            OracleHarness.FullyDivine(OracleHarness.Witness("the Lord's table", new Eucharist()));
        }

        [Fact]
        public void UnworthyCommunion_IsProfaned_NotFullyDivine()
        {
            // The same table approached with sacrilege brings judgment, not pure blessing (1 Cor 11:29).
            Resolution r = OracleHarness.Witness("the table approached unworthily", new Eucharist(), new Sacrilege());

            Assert.True(r.Disorder > 0.0, "An unworthily-received sacrament carries judgment.");
            Assert.True(r.Coherence < 1.0, "It is no longer a pure divine act.");
        }

        [Fact]
        public void SimoniacalOrdination_IsProfaned()
        {
            // "May your money perish with you, because you thought you could buy the gift of God!"
            // (Acts 8:20)
            Resolution r = OracleHarness.Witness("ordination bought with money", new Ordination(), new Simony());

            Assert.True(r.Disorder > 0.0);
            Assert.True(r.Coherence < 1.0);
        }

        [Fact]
        public void TheSinOfElisSons_AtTheAltar_IsProfanation()
        {
            // Eli's sons "were treating the LORD's offering with contempt." (1 Samuel 2:17)
            Resolution r = OracleHarness.Witness("contempt for the LORD's offering", new Worship(), new Sacrilege(), new Greed());

            Assert.True(r.Disorder > 0.0);
        }
    }
}
