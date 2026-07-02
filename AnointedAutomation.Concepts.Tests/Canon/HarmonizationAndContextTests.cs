// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    /// <summary>
    /// Closes audit gaps in the harmonization machinery and the context mechanism: the empty-character
    /// guard, single-facet harmonization, the EXACT graded disorder of every sin (not just the three
    /// spot-checked in PrecisionOracleTests), and the boundaries of context (it must not excuse a
    /// capital crime, and an unrelated context must not suppress a sin).
    /// </summary>
    public class HarmonizationAndContextTests
    {
        [Fact]
        public void DivineCharacter_WithNoFacets_Throws()
        {
            // God's character is never empty; the engine refuses to model it as such.
            Assert.Throws<ArgumentException>(() => new DivineCharacter());
        }

        [Fact]
        public void ASingleFacetCharacter_ReadsThatFacetAlone()
        {
            DivineCharacter justice = new DivineCharacter(new Justice());
            Resolution r = justice.Harmonize(new Act("a just recompense", new JustRecompense()));

            Assert.Equal(1.0, r.Coherence);
            Assert.Equal(0.0, r.Disorder);
        }

        [Fact]
        public void EverySin_UnleashesDisorderExactlyEqualToItsGravity()
        {
            // The graded-severity behavior, verified for ALL sins, not just murder/theft/rudeness:
            // capital 1.0, grave 0.75, serious 0.5, minor 0.25. Catches any mis-tiered concept or a
            // broken gravity-to-disorder mapping.
            foreach (MoralConcept vice in Vices())
            {
                double expected = (double)(int)vice.Gravity / (double)(int)Gravity.Capital;
                Resolution r = Reality.Revealed().Witness(new Act(vice.Name, vice));

                Assert.Equal(expected, r.Disorder);
            }
        }

        [Fact]
        public void Context_DoesNotExcuseACapitalCrime()
        {
            // Context can recontextualize a deception (Rahab), but it must not turn murder into a
            // righteous act. A capital wrong remains capital whatever the framing.
            Resolution r = OracleHarness.WitnessInContext("a murder done 'to protect'",
                new[] { new Circumstance("protectingInnocentLife") }, new Murder());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void AnUnrelatedContext_DoesNotSuppressDeceit()
        {
            // Only the defense of innocent life recontextualizes a deception. A deception in some
            // other circumstance (say, war for gain) is still the lie God condemns.
            Resolution r = OracleHarness.WitnessInContext("a deception for advantage",
                new[] { new Circumstance("war") }, new Deceit());

            OracleHarness.Sin(r);
        }

        [Fact]
        public void DeceitWithNoContext_IsStillSin()
        {
            // The default path: without any mitigating context, deceit offends the God of truth.
            OracleHarness.Sin(OracleHarness.Witness("a bare deception", new Deceit()));
        }

        private static IEnumerable<MoralConcept> Vices()
        {
            Type baseType = typeof(MoralConcept);
            return baseType.Assembly.GetTypes()
                .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
                .Select(t => (MoralConcept)Activator.CreateInstance(t))
                .Where(c => c.Gravity != Gravity.None);
        }
    }
}
