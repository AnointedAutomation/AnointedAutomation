// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// One deterministic rule of the <see cref="CanonReasoner"/>. A rule answers queries of one
    /// <see cref="Relation"/>. It returns <see cref="TruthValue.Unknown"/> whenever the canon does not
    /// settle the query, and <see cref="TruthValue.False"/> only when it proves the negation. An
    /// established answer (True or False) names the canon facts it rests on, which carry its Scripture.
    /// </summary>
    public interface ICanonRule
    {
        /// <summary>
        /// Gets the stable rule id recorded in <see cref="Provenance.RuleOrPromptId"/>.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the relation this rule answers.
        /// </summary>
        RelationKind Relation { get; }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="query">The query; its relation is <see cref="Relation"/>.</param>
        /// <param name="facts">The canon index.</param>
        /// <param name="steps">The proof steps (empty when Unknown).</param>
        /// <param name="grounds">The canon facts the answer rests on (empty when Unknown, never empty otherwise).</param>
        /// <returns>The truth value.</returns>
        TruthValue Apply(Query query, CanonFacts facts, out ProofStep[] steps, out CanonFact[] grounds);
    }
}
