// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Reasoning;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Canon
{
    /// <summary>
    /// Regression: the canon reasoner must agree with the oracle. Every indexed concept and sin is
    /// witnessed directly and through the reasoner, and the single-concept cases of the canonical
    /// corpus give the verdicts the corpus pins.
    /// </summary>
    public class CanonReasonerRegressionTests
    {
        private static readonly CanonReasoner Reasoner = CanonKit.Reasoner();

        private static TruthValue Witnessed(MoralConcept concept)
        {
            return Reasoner.Decide(CanonKit.Ask(ConceptRef.Of(concept), RelationKind.Upholds, CanonFacts.RevealedCharacter)).Truth;
        }

        /// <summary>The oracle's verdict, by the thresholds of <see cref="OracleHarness"/>.</summary>
        private static TruthValue OracleVerdict(MoralConcept concept)
        {
            var r = OracleHarness.Witness(concept.Name, concept);
            if (r.Coherence > 0.5 && r.Disorder == 0.0)
            {
                return TruthValue.True;
            }

            if (r.Coherence < 0.5 && r.Disorder > 0.0)
            {
                return TruthValue.False;
            }

            return TruthValue.Unknown;
        }

        [Fact]
        public void EveryCanonConcept_WitnessVerdict_AgreesWithTheOracle()
        {
            int checkedCount = 0;
            var disagreements = new List<string>();
            var seen = new HashSet<ConceptRef>();
            foreach (var fact in CanonFacts.Canon.All)
            {
                var subject = fact.Assertion.Subject;
                if (!seen.Add(subject) || !CanonFacts.Canon.TryGetConcept(subject, out var concept))
                {
                    continue;
                }

                if (OracleVerdict(concept) != Witnessed(concept))
                {
                    disagreements.Add(subject.ToString());
                }

                checkedCount++;
            }

            Assert.Empty(disagreements);
            Assert.True(checkedCount > 500, "Expected every catalogued sin and moral concept to be checked.");
        }

        [Fact]
        public void EveryCataloguedSin_ViolatesExactlyTheFacetsItsViceViolates_AndNeverFalse()
        {
            var disagreements = new List<string>();
            foreach (var entry in SinCatalog.All)
            {
                if (entry.IsOmitted)
                {
                    continue;
                }

                var sin = SinCatalog.Concept(entry.Id);
                foreach (var facet in Reality.Revealed().Character.Facets)
                {
                    var truth = Reasoner.Decide(CanonKit.Ask(ConceptRef.Of(entry.Id), RelationKind.Violates, CanonFacts.Attribute(facet))).Truth;
                    if ((sin.Violates(facet) ? TruthValue.True : TruthValue.Unknown) != truth)
                    {
                        disagreements.Add(entry.Name + " / " + facet.Name);
                    }
                }
            }

            Assert.Empty(disagreements);
        }

        [Fact] public void TheGoldenCalf() => Assert.Equal(TruthValue.False, Witnessed(new Idolatry()));

        [Fact] public void TheIncarnation() => Assert.Equal(TruthValue.True, Witnessed(new Incarnation()));

        [Fact] public void TheResurrection() => Assert.Equal(TruthValue.True, Witnessed(new Resurrection()));

        [Fact] public void ThePentecost() => Assert.Equal(TruthValue.True, Witnessed(new Pentecost()));

        [Fact] public void TheNewCreation() => Assert.Equal(TruthValue.True, Witnessed(new NewCreation()));
    }
}
