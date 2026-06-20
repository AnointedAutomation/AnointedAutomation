// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// Gap 9: the two axes of a verdict are independent and can diverge. A deed can carry much good
    /// and still unleash total destruction. These tests pin that behavior so it is a documented,
    /// intended property and not a surprise: coherence measures distance from God's character;
    /// disorder measures the destruction the gravest wrong unleashes; one good does not cancel the
    /// other.
    /// </summary>
    public class DivergenceTests
    {
        [Fact]
        public void MuchGoodWithOneCapitalSin_ReadsHalfCoherentButFullyDisordering()
        {
            // Compassion, generosity, and worship, alongside a murder. The good lifts coherence past
            // the midpoint, yet the murder unleashes total disorder. "Whoever keeps the whole law and
            // yet stumbles at just one point is guilty of breaking all of it." (James 2:10)
            Resolution r = OracleHarness.Witness("a benefactor who is also a murderer",
                new Compassion(), new Generosity(), new Worship(), new Murder());

            OracleHarness.Exact(r, 0.625, 1.0);
            Assert.True(r.Coherence > 0.5);   // it does not look like simple sin ...
            Assert.Equal(1.0, r.Disorder);    // ... yet it destabilizes reality utterly.
        }

        [Fact]
        public void TheTwoAxesAreIndependent_DisorderTracksTheWorstWrong_NotTheAverage()
        {
            // Adding more good raises coherence but does not lower disorder: the gravest wrong still
            // governs the destruction unleashed.
            Resolution oneVirtue = OracleHarness.Witness("a kindness beside a theft", new Kindness(), new Theft());
            Resolution manyVirtues = OracleHarness.Witness("many kindnesses beside the same theft",
                new Kindness(), new Compassion(), new Generosity(), new Theft());

            Assert.True(manyVirtues.Coherence > oneVirtue.Coherence);
            Assert.Equal(oneVirtue.Disorder, manyVirtues.Disorder);
        }
    }
}
