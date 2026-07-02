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
    /// The completeness guard: reflects over EVERY moral concept in the engine and proves none is a
    /// dead stub. Each must be instantiable, name itself, cite Scripture, and actually be wired to
    /// God's character (uphold or violate at least one facet). Vices must violate; virtues and
    /// mysteries must uphold. This is the automated answer to "is anything missing or half-built": if
    /// a concept exists but bears on nothing, this test fails.
    /// </summary>
    public class ConceptCoverageTests
    {
        private static IEnumerable<MoralConcept> AllConcepts()
        {
            Type baseType = typeof(MoralConcept);
            return baseType.Assembly.GetTypes()
                .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
                .Select(t => (MoralConcept)Activator.CreateInstance(t));
        }

        private static DivineAttribute[] Facets()
        {
            return new DivineAttribute[] { new LoveFacet(), new Justice(), new Mercy(), new Faithfulness() };
        }

        [Fact]
        public void TheEngineHasASubstantialBodyOfConcepts()
        {
            // Sanity floor: the canon census + per-book agents produced a large vocabulary.
            Assert.True(AllConcepts().Count() >= 100, "Expected at least 100 moral concepts.");
        }

        [Fact]
        public void EveryConcept_NamesItselfAndCitesScripture()
        {
            foreach (MoralConcept concept in AllConcepts())
            {
                Assert.False(string.IsNullOrWhiteSpace(concept.Name),
                    concept.GetType().Name + " has no Name.");
                Assert.False(string.IsNullOrWhiteSpace(concept.Scripture),
                    concept.GetType().Name + " cites no Scripture.");
            }
        }

        [Fact]
        public void EveryConcept_BearsOnGodsCharacter()
        {
            DivineAttribute[] facets = Facets();
            foreach (MoralConcept concept in AllConcepts())
            {
                bool wired = facets.Any(f => concept.Upholds(f) || concept.Violates(f));
                Assert.True(wired, concept.GetType().Name + " touches no facet of God's character.");
            }
        }

        [Fact]
        public void EveryVice_Violates_AndEveryVirtueOrMystery_Upholds()
        {
            DivineAttribute[] facets = Facets();
            foreach (MoralConcept concept in AllConcepts())
            {
                bool upholds = facets.Any(f => concept.Upholds(f));
                bool violates = facets.Any(f => concept.Violates(f));

                if (concept.Gravity == Gravity.None)
                {
                    // A virtue or a sacred mystery: it embodies God's character, it does not offend it.
                    Assert.True(upholds, concept.GetType().Name + " has no gravity but upholds nothing.");
                    Assert.False(violates, concept.GetType().Name + " has no gravity yet violates a facet.");
                }
                else
                {
                    // A sin: its gravity must answer to a real offence against God's character.
                    Assert.True(violates, concept.GetType().Name + " has gravity but violates nothing.");
                }
            }
        }

        [Fact]
        public void EveryConcept_ResolvesToTheRightSideOfTheVerdict()
        {
            // The deepest check: run each concept through the engine alone and confirm a virtue/mystery
            // never disorders reality, and a sin always does.
            foreach (MoralConcept concept in AllConcepts())
            {
                Resolution r = Reality.Revealed().Witness(new Act(concept.Name, concept));

                if (concept.Gravity == Gravity.None)
                {
                    Assert.Equal(0.0, r.Disorder);
                    Assert.True(r.Coherence >= 0.5, concept.GetType().Name + " is a virtue but reads incoherent.");
                }
                else
                {
                    Assert.True(r.Disorder > 0.0, concept.GetType().Name + " is a sin but disorders nothing.");
                    Assert.True(r.Coherence < 0.5, concept.GetType().Name + " is a sin but reads coherent.");
                }
            }
        }
    }
}
