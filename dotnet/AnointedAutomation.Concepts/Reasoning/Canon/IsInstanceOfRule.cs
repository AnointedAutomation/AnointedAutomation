// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Answers "is this an instance of that?" from stated IsInstanceOf facts (a catalogued sin is an
    /// instance of its coarse sin and of its grounding vice). A catalogued sin has at most one coarse
    /// sin, so asking for a different coarse sin is proven False; everything else unstated is Unknown.
    /// </summary>
    public sealed class IsInstanceOfRule : ICanonRule
    {
        /// <inheritdoc />
        public string Id => "canon.is-instance-of";

        /// <inheritdoc />
        public RelationKind Relation => RelationKind.IsInstanceOf;

        /// <inheritdoc />
        public TruthValue Apply(Query query, CanonFacts facts, out ProofStep[] steps, out CanonFact[] grounds)
        {
            Assertion asked = ReasoningGuard.NotNull(query, nameof(query)).Assertion;
            ReasoningGuard.NotNull(facts, nameof(facts));
            if (facts.TryGetFact(asked, out CanonFact stated))
            {
                steps = CanonProof.Steps(CanonProof.Describe(asked), "The canon states it (" + stated.Scripture[0] + ").");
                grounds = new[] { stated };
                return TruthValue.True;
            }

            facts.TryGetConcept(asked.Subject, out MoralConcept concept);
            CataloguedSin sin = concept as CataloguedSin;
            if (sin == null
                || !sin.Entry.CoarseSin.HasValue
                || !asked.Object.Kind.Equals(CanonFacts.CoarseSinKind, System.StringComparison.Ordinal))
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            ConceptRef coarse = CanonFacts.CoarseSin(sin.Entry.CoarseSin.Value);
            if (!facts.TryGetFact(new Assertion(asked.Subject, RelationKind.IsInstanceOf, coarse), out CanonFact coarseFact))
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            steps = CanonProof.Steps(
                CanonProof.Describe(coarseFact.Assertion),
                sin.Entry.Name + " falls under the coarse sin " + coarse.Id + ".",
                coarse + " ≠ " + asked.Object,
                "A catalogued sin has one coarse sin, so it is not an instance of " + asked.Object.Id + ".");
            grounds = new[] { coarseFact };
            return TruthValue.False;
        }
    }
}
