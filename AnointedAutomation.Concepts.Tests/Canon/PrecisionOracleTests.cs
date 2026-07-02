// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    /// <summary>
    /// Closes the "coarse assertions" gap: the bulk of the oracle suite only checks which side of
    /// 0.5 a verdict falls on. These tests pin the EXACT coherence, disorder, and per-facet readings
    /// for landmark cases, so any drift in the engine's arithmetic fails loudly here rather than
    /// hiding behind a forgiving threshold.
    /// </summary>
    public class PrecisionOracleTests
    {
        [Fact]
        public void TheCross_IsExactlyOneCoherence_ZeroDisorder_AllFacetsFull()
        {
            Resolution r = OracleHarness.Witness("the cross",
                new SelfSacrifice(), new Atonement(), new Pardon(), new CovenantFaithfulness());

            OracleHarness.FullyDivine(r);
            OracleHarness.Facet(r, "Love", 1.0);
            OracleHarness.Facet(r, "Justice", 1.0);
            OracleHarness.Facet(r, "Mercy", 1.0);
            OracleHarness.Facet(r, "Faithfulness", 1.0);
        }

        [Fact]
        public void Micah68_ActJustlyLoveMercyWalkHumbly_IsExactlyFullyCoherent()
        {
            // Justice, mercy, and humility together touch every facet of God's character at once.
            Resolution r = OracleHarness.Witness("act justly, love mercy, walk humbly",
                new JustRecompense(), new Compassion(), new Humility());

            OracleHarness.Exact(r, 1.0, 0.0);
        }

        [Fact]
        public void Murder_IsExactly_Quarter_Coherence_FullDisorder()
        {
            // Offends Justice and Love (0,0), leaves Mercy and Order neutral (0.5): mean 0.25.
            // Capital gravity drives disorder to 1.0.
            OracleHarness.Exact(OracleHarness.Witness("murder", new Murder()), 0.25, 1.0);
        }

        [Fact]
        public void Theft_IsExactly_ThreeEighths_Coherence_HalfDisorder()
        {
            // Offends Justice only (0), three facets neutral: mean 0.375. Serious gravity: disorder 0.5.
            OracleHarness.Exact(OracleHarness.Witness("theft", new Theft()), 0.375, 0.5);
        }

        [Fact]
        public void Rudeness_IsExactly_ThreeEighths_Coherence_QuarterDisorder()
        {
            // Offends Love only (0): mean 0.375. Minor gravity: disorder only 0.25, far below murder.
            OracleHarness.Exact(OracleHarness.Witness("a rude word", new Rudeness()), 0.375, 0.25);
        }

        [Fact]
        public void GravedSinsAreStrictlyOrdered_RudenessLessThanTheftLessThanMurder()
        {
            // Scripture grades sin: the disorder each unleashes is strictly ordered, not flat.
            double rudeness = OracleHarness.Witness("rude", new Rudeness()).Disorder;
            double theft = OracleHarness.Witness("theft", new Theft()).Disorder;
            double murder = OracleHarness.Witness("murder", new Murder()).Disorder;

            Assert.True(rudeness < theft);
            Assert.True(theft < murder);
            Assert.Equal(1.0, murder);
        }

        [Fact]
        public void TheGoodSamaritan_ReadsLoveAndMercyExactlyFull()
        {
            Resolution r = OracleHarness.Witness("the Good Samaritan", new Compassion(), new Protection());

            OracleHarness.Exact(r, 0.75, 0.0);
            OracleHarness.Facet(r, "Love", 1.0);
            OracleHarness.Facet(r, "Mercy", 1.0);
            OracleHarness.Facet(r, "Justice", 0.5);
        }

        [Fact]
        public void AnIndifferentAct_SitsExactlyNeutral()
        {
            // No moral concept at all: every facet neutral, nothing destabilized.
            OracleHarness.Exact(OracleHarness.Witness("watching a sunset"), 0.5, 0.0);
        }

        [Fact]
        public void IdolGrounding_HalvesCoherence_AndFloorsDisorderAtHalf()
        {
            // The same right act of worship, borne on a dead idol, drifts toward non-being (1 Meqabyan).
            Resolution onGod = OracleHarness.Grounded("worship", Grounding.InGod(), new Worship());
            Resolution onIdol = OracleHarness.Grounded("worship", Grounding.InIdol("a dead idol"), new Worship());

            Assert.Equal(0.0, onGod.Disorder);
            Assert.Equal(onGod.Coherence * 0.5, onIdol.Coherence);
            Assert.Equal(0.5, onIdol.Disorder);
        }
    }
}
