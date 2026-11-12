// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// A subject-relation-object statement the engines evaluate, e.g. (Act, Violates, Justice).
    /// </summary>
    public sealed record Assertion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Assertion"/> record.
        /// </summary>
        /// <param name="subject">What the statement is about.</param>
        /// <param name="relation">The claimed relation; <see cref="RelationKind.None"/> is rejected.</param>
        /// <param name="object">What the subject is related to.</param>
        public Assertion(ConceptRef subject, RelationKind relation, ConceptRef @object)
        {
            Subject = ReasoningGuard.NotNull(subject, nameof(subject));
            Relation = ReasoningGuard.DefinedAndSet(relation, nameof(relation));
            Object = ReasoningGuard.NotNull(@object, nameof(@object));
        }

        /// <summary>
        /// What the statement is about.
        /// </summary>
        public ConceptRef Subject { get; }

        /// <summary>
        /// The claimed relation.
        /// </summary>
        public RelationKind Relation { get; }

        /// <summary>
        /// What the subject is related to.
        /// </summary>
        public ConceptRef Object { get; }
    }
}
