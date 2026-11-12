// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Answers a query straight from the index: True when the canon states the assertion, False when
    /// it states the contrary (God forbids what the subject would uphold, or forbids what is asked as
    /// commanded and the reverse), and Unknown when it states neither or both.
    /// </summary>
    public sealed class FactRule : ICanonRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactRule"/> class.
        /// </summary>
        /// <param name="relation">The relation this instance answers.</param>
        public FactRule(RelationKind relation)
        {
            Relation = ReasoningGuard.DefinedAndSet(relation, nameof(relation));
            Id = "canon.fact." + relation.ToString().ToLowerInvariant();
        }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public RelationKind Relation { get; }

        /// <inheritdoc />
        public TruthValue Apply(Query query, CanonFacts facts, out ProofStep[] steps, out CanonFact[] grounds)
        {
            Assertion asked = ReasoningGuard.NotNull(query, nameof(query)).Assertion;
            ReasoningGuard.NotNull(facts, nameof(facts));
            bool stated = facts.TryGetFact(asked, out CanonFact fact);
            bool contradicted = TryContrary(asked, facts, out CanonFact contrary);
            if (stated && !contradicted)
            {
                steps = CanonProof.Steps(
                    CanonProof.Describe(asked),
                    "The canon states it (" + fact.Scripture[0] + ").");
                grounds = new[] { fact };
                return TruthValue.True;
            }

            if (contradicted && !stated)
            {
                steps = CanonProof.Steps(
                    CanonProof.Describe(contrary.Assertion),
                    "The canon states the contrary (" + contrary.Scripture[0] + ").",
                    "¬(" + CanonProof.Describe(asked) + ")",
                    "So the assertion is false.");
                grounds = new[] { contrary };
                return TruthValue.False;
            }

            return CanonProof.Unknown(out steps, out grounds);
        }

        /// <summary>
        /// Finds the stated fact that contradicts an assertion, when the relation has a contrary.
        /// </summary>
        /// <param name="asked">The assertion.</param>
        /// <param name="facts">The canon index.</param>
        /// <param name="contrary">The contrary fact, when found.</param>
        /// <returns>True when a contrary fact is stated.</returns>
        private static bool TryContrary(Assertion asked, CanonFacts facts, out CanonFact contrary)
        {
            switch (asked.Relation)
            {
                case RelationKind.Upholds:
                    return facts.TryGetFact(new Assertion(asked.Object, RelationKind.Forbids, asked.Subject), out contrary);
                case RelationKind.Commands:
                    return facts.TryGetFact(new Assertion(asked.Subject, RelationKind.Forbids, asked.Object), out contrary);
                case RelationKind.Forbids:
                    return facts.TryGetFact(new Assertion(asked.Subject, RelationKind.Commands, asked.Object), out contrary);
                default:
                    contrary = null;
                    return false;
            }
        }
    }
}
