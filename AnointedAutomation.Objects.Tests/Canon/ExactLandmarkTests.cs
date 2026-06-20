// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// Gap 2: an exact-value landmark for each facet, so every facet of God's character has a test
    /// that pins the precise arithmetic (not just a threshold). A deed that upholds exactly one facet
    /// reads that facet at 1.0 and the whole at exactly 0.625, with no disorder.
    /// </summary>
    public class ExactLandmarkTests
    {
        [Fact]
        public void APureDeedOfLove_ReadsLoveFull_CoherenceExactlyPoint625()
        {
            Resolution r = OracleHarness.Witness("a pure kindness", new Kindness());
            OracleHarness.Facet(r, "Love", 1.0);
            OracleHarness.Exact(r, 0.625, 0.0);
        }

        [Fact]
        public void APureDeedOfJustice_ReadsJusticeFull_CoherenceExactlyPoint625()
        {
            Resolution r = OracleHarness.Witness("a pure recompense", new JustRecompense());
            OracleHarness.Facet(r, "Justice", 1.0);
            OracleHarness.Exact(r, 0.625, 0.0);
        }

        [Fact]
        public void APureDeedOfMercy_ReadsMercyFull_CoherenceExactlyPoint625()
        {
            Resolution r = OracleHarness.Witness("a pure pardon", new Pardon());
            OracleHarness.Facet(r, "Mercy", 1.0);
            OracleHarness.Exact(r, 0.625, 0.0);
        }

        [Fact]
        public void APureDeedOfFaithfulness_ReadsFaithfulnessFull_CoherenceExactlyPoint625()
        {
            Resolution r = OracleHarness.Witness("a kept covenant", new CovenantFaithfulness());
            OracleHarness.Facet(r, "Faithfulness", 1.0);
            OracleHarness.Exact(r, 0.625, 0.0);
        }
    }
}
