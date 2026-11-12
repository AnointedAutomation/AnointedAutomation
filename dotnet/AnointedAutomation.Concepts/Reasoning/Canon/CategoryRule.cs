// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Answers "is this sin in this category?" from <see cref="SinEntry.Category"/>. A catalogued sin has
    /// exactly one category, so a different category is proven False. Any other subject is True only on
    /// a stated InCategory fact, otherwise Unknown.
    /// </summary>
    public sealed class CategoryRule : ICanonRule
    {
        /// <inheritdoc />
        public string Id => "canon.category";

        /// <inheritdoc />
        public RelationKind Relation => RelationKind.InCategory;

        /// <inheritdoc />
        public TruthValue Apply(Query query, CanonFacts facts, out ProofStep[] steps, out CanonFact[] grounds)
        {
            Assertion asked = ReasoningGuard.NotNull(query, nameof(query)).Assertion;
            ReasoningGuard.NotNull(facts, nameof(facts));
            if (!asked.Object.Kind.Equals(CanonFacts.CategoryKind, System.StringComparison.Ordinal))
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            facts.TryGetConcept(asked.Subject, out MoralConcept concept);
            CataloguedSin sin = concept as CataloguedSin;
            if (sin == null)
            {
                if (facts.TryGetFact(asked, out CanonFact stated))
                {
                    steps = CanonProof.Steps(CanonProof.Describe(asked), "The canon states it (" + stated.Scripture[0] + ").");
                    grounds = new[] { stated };
                    return TruthValue.True;
                }

                return CanonProof.Unknown(out steps, out grounds);
            }

            ConceptRef actual = CanonFacts.Category(sin.Entry.Category.Value);
            if (!facts.TryGetFact(new Assertion(asked.Subject, RelationKind.InCategory, actual), out CanonFact categoryFact))
            {
                return CanonProof.Unknown(out steps, out grounds);
            }

            grounds = new[] { categoryFact };
            if (actual.Equals(asked.Object))
            {
                steps = CanonProof.Steps(
                    CanonProof.Describe(asked),
                    sin.Entry.Name + " is catalogued under " + actual.Id + ".");
                return TruthValue.True;
            }

            steps = CanonProof.Steps(
                CanonProof.Describe(categoryFact.Assertion),
                sin.Entry.Name + " is catalogued under " + actual.Id + ".",
                actual + " ≠ " + asked.Object,
                "A catalogued sin has exactly one category, so it is not in " + asked.Object.Id + ".");
            return TruthValue.False;
        }
    }
}
