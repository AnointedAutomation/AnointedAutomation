// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// Gap 8: the Faithfulness facet is God's own attribute (emet, "abounding in love and
    /// faithfulness", Exodus 34:6-7), not a human virtue. The human concepts that touch it,
    /// obedience, trust, covenant-keeping, the kept order of creation, are RESPONSES that reflect and
    /// cohere with His faithfulness; they uphold the facet, they are not the facet. These tests pin
    /// that distinction: the facet names the divine attribute, and human fidelity reads as upholding
    /// it.
    /// </summary>
    public class FaithfulnessFacetTests
    {
        [Fact]
        public void TheFacet_IsGodsOwnAttribute()
        {
            Assert.Equal("Faithfulness", new Faithfulness().Name);
        }

        [Fact]
        public void HumanFidelity_ReflectsAndUpholds_TheDivineFaithfulness()
        {
            // Each human response to God's faithfulness reads as fully upholding the facet, because it
            // reflects His own steadfastness; none of them IS the attribute.
            Assert.Equal(1.0, OracleHarness.Witness("keeping covenant", new CovenantFaithfulness()).Reading("Faithfulness"));
            Assert.Equal(1.0, OracleHarness.Witness("obeying God", new ObedienceToGod()).Reading("Faithfulness"));
            Assert.Equal(1.0, OracleHarness.Witness("trusting God", new Trust()).Reading("Faithfulness"));
        }

        [Fact]
        public void TheKeptOrderOfCreation_IsAnExpressionOfHisFaithfulness_NotASeparateAttribute()
        {
            // "In him all things hold together" (Colossians 1:17) is downstream of His faithfulness,
            // so CreationOrder upholds the same one facet rather than a facet of its own.
            Assert.Equal(1.0, OracleHarness.Witness("the courses of creation kept", new CreationOrder()).Reading("Faithfulness"));
        }

        [Fact]
        public void BreakingFaith_OffendsTheDivineFaithfulness()
        {
            Assert.Equal(0.0, OracleHarness.Witness("breaking covenant", new Treachery()).Reading("Faithfulness"));
        }
    }
}
