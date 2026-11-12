// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Answers "does this catalogued sin violate this facet of God?" through
    /// <see cref="CataloguedSin.Violates(DivineAttribute)"/>, grounded in the sin's catalog references.
    /// A sin that does not violate the facet gives Unknown, never False.
    /// </summary>
    public sealed class SinViolatesRule : ICanonRule
    {
        /// <inheritdoc />
        public string Id => "canon.sin-violates";

        /// <inheritdoc />
        public RelationKind Relation => RelationKind.Violates;

        /// <inheritdoc />
        public TruthValue Apply(Query query, CanonFacts facts, out ProofStep[] steps, out CanonFact[] grounds)
        {
            Assertion asked = ReasoningGuard.NotNull(query, nameof(query)).Assertion;
            ReasoningGuard.NotNull(facts, nameof(facts));
            if (!facts.TryGetConcept(asked.Subject, out MoralConcept concept)
                || !facts.TryGetAttribute(asked.Object, out DivineAttribute facet))
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            CataloguedSin sin = concept as CataloguedSin;
            if (sin == null || !sin.Violates(facet))
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            CanonFact[] cited = new List<CanonFact>(facts.About(asked.Subject, RelationKind.GroundedIn)).ToArray();
            if (cited.Length == 0)
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            steps = CanonProof.Steps(
                asked.Subject + " ⊑ " + sin.GroundingVice.Name,
                sin.Entry.Name + " is grounded in the vice " + sin.GroundingVice.Name + ".",
                sin.GroundingVice.Name + " ⊗ " + facet.Name,
                sin.GroundingVice.Name + " violates God's " + facet.Name + ".",
                CanonProof.Describe(asked),
                "So the sin violates " + facet.Name + ".");
            grounds = cited;
            return TruthValue.True;
        }
    }
}
