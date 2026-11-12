// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Answers "does this concept uphold God's revealed character?" (object
    /// <see cref="CanonFacts.RevealedCharacter"/>) by running <see cref="Reality.Witness(Act)"/> on an act
    /// of the concept in the query's context. It maps the reading with the oracle's verdict thresholds:
    /// righteous (coherence above 0.5, no disorder) is True, sin (coherence below 0.5, some disorder) is
    /// False, and anything in between is Unknown.
    /// </summary>
    public sealed class WitnessRule : ICanonRule
    {
        /// <summary>The coherence line: above it is coherent, below it incoherent.</summary>
        public const double CoherenceLine = 0.5;

        /// <inheritdoc />
        public string Id => "canon.witness";

        /// <inheritdoc />
        public RelationKind Relation => RelationKind.Upholds;

        /// <inheritdoc />
        public TruthValue Apply(Query query, CanonFacts facts, out ProofStep[] steps, out CanonFact[] grounds)
        {
            ReasoningGuard.NotNull(query, nameof(query));
            ReasoningGuard.NotNull(facts, nameof(facts));
            Assertion asked = query.Assertion;
            if (!asked.Object.Equals(CanonFacts.RevealedCharacter) || !facts.TryGetConcept(asked.Subject, out MoralConcept concept))
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            CanonFact[] cited = new List<CanonFact>(facts.About(asked.Subject, RelationKind.GroundedIn)).ToArray();
            if (cited.Length == 0)
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            Circumstance[] context = new List<Circumstance>(query.Context).ToArray();
            Resolution reading = Reality.Revealed().Witness(new Act(asked.Subject.ToString(), concept).InContext(context));
            string witnessed = "witness(" + asked.Subject + ") = (coherence " + CanonProof.Number(reading.Coherence)
                + ", disorder " + CanonProof.Number(reading.Disorder) + ")";
            if (reading.Coherence > CoherenceLine && reading.Disorder == 0.0)
            {
                steps = CanonProof.Steps(
                    witnessed,
                    "Witnessed under God's revealed character.",
                    "coherence > 0.5 ∧ disorder = 0",
                    "It reads coherent and disorders nothing, so it upholds God's character.");
                grounds = cited;
                return TruthValue.True;
            }

            if (reading.Coherence < CoherenceLine && reading.Disorder > 0.0)
            {
                steps = CanonProof.Steps(
                    witnessed,
                    "Witnessed under God's revealed character.",
                    "coherence < 0.5 ∧ disorder > 0",
                    "It reads incoherent and brings disorder, so it does not uphold God's character.");
                grounds = cited;
                return TruthValue.False;
            }

            return CanonProof.Unknown(out steps, out grounds);
        }
    }
}
