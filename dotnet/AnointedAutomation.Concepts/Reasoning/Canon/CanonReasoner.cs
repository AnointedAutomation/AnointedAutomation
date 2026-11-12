// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// The deterministic, offline <see cref="IReasoner"/>: it answers from scripture-grounded canon facts
    /// and small rules, with no AI or network dependency. The same query over the same facts always gets
    /// the same answer. A query no fact or rule settles is <see cref="TruthValue.Unknown"/>, never False;
    /// an established answer has confidence 1.0 and cites the Scripture of the facts it rests on.
    /// </summary>
    public sealed class CanonReasoner : IReasoner
    {
        /// <summary>The model name recorded in <see cref="Provenance.Model"/>.</summary>
        public const string ModelName = "canon";

        /// <summary>The explanation given when no fact or rule settles a query: plainly admits it does not know.</summary>
        public const string UnknownExplanation = "I don't know. No scripture fact or rule in the catalog settles this question.";

        /// <summary>The rule id recorded when no rule settles a query.</summary>
        public const string NoRuleId = "canon.none";

        private readonly CanonFacts facts;
        private readonly TimeProvider clock;
        private readonly Dictionary<RelationKind, IReadOnlyList<ICanonRule>> rulesByRelation;

        /// <summary>
        /// Initializes a reasoner over <see cref="CanonFacts.Canon"/> with <see cref="CanonRules.Default"/>.
        /// </summary>
        /// <param name="clock">The clock that stamps each answer's provenance.</param>
        public CanonReasoner(TimeProvider clock)
            : this(CanonFacts.Canon, CanonRules.Default, clock)
        {
        }

        /// <summary>
        /// Initializes a reasoner over the given facts and rules.
        /// </summary>
        /// <param name="facts">The canon index.</param>
        /// <param name="rules">The rules, in evaluation order; ids must be unique.</param>
        /// <param name="clock">The clock that stamps each answer's provenance.</param>
        /// <exception cref="ArgumentNullException">An argument is null.</exception>
        /// <exception cref="ArgumentException">A rule is null, or two rules share an id.</exception>
        public CanonReasoner(CanonFacts facts, IReadOnlyList<ICanonRule> rules, TimeProvider clock)
        {
            this.facts = ReasoningGuard.NotNull(facts, nameof(facts));
            this.clock = ReasoningGuard.NotNull(clock, nameof(clock));
            IReadOnlyList<ICanonRule> ordered = ReasoningGuard.CopyOf(rules, nameof(rules));
            HashSet<string> ids = new HashSet<string>(StringComparer.Ordinal);
            Dictionary<RelationKind, List<ICanonRule>> grouped = new Dictionary<RelationKind, List<ICanonRule>>();
            foreach (ICanonRule rule in ordered)
            {
                if (!ids.Add(ReasoningGuard.NotBlank(rule.Id, nameof(rules))))
                {
                    throw new ArgumentException("Two rules share the id '" + rule.Id + "'.", nameof(rules));
                }

                RelationKind relation = ReasoningGuard.DefinedAndSet(rule.Relation, nameof(rules));
                if (!grouped.TryGetValue(relation, out List<ICanonRule> list))
                {
                    list = new List<ICanonRule>();
                    grouped.Add(relation, list);
                }

                list.Add(rule);
            }

            rulesByRelation = new Dictionary<RelationKind, IReadOnlyList<ICanonRule>>(grouped.Count);
            foreach (KeyValuePair<RelationKind, List<ICanonRule>> pair in grouped)
            {
                rulesByRelation.Add(pair.Key, pair.Value.AsReadOnly());
            }
        }

        /// <inheritdoc />
        public Task<Answer> Ask(Query query, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Decide(query));
        }

        /// <summary>
        /// Answers a query synchronously. Every rule for the query's relation runs; the first established
        /// answer wins, and two rules establishing opposite answers is a canon contradiction.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The answer.</returns>
        /// <exception cref="ArgumentNullException">The query is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// A rule returned an undefined value, established an answer without canon facts, or contradicted another rule.
        /// </exception>
        public Answer Decide(Query query)
        {
            ReasoningGuard.NotNull(query, nameof(query));
            ICanonRule chosen = null;
            TruthValue truth = TruthValue.Unknown;
            ProofStep[] steps = CanonProof.NoSteps;
            CanonFact[] grounds = CanonProof.NoGrounds;
            if (rulesByRelation.TryGetValue(query.Assertion.Relation, out IReadOnlyList<ICanonRule> rules))
            {
                foreach (ICanonRule rule in rules)
                {
                    TruthValue result = rule.Apply(query, facts, out ProofStep[] ruleSteps, out CanonFact[] ruleGrounds);
                    if (!Enum.IsDefined(typeof(TruthValue), result))
                    {
                        throw new InvalidOperationException("Rule '" + rule.Id + "' returned an undefined truth value " + (int)result + ".");
                    }

                    if (result == TruthValue.Unknown)
                    {
                        continue;
                    }

                    if (ruleGrounds == null || ruleGrounds.Length == 0 || ruleSteps == null || ruleSteps.Length == 0)
                    {
                        throw new InvalidOperationException(
                            "Rule '" + rule.Id + "' established " + result + " for " + CanonProof.Describe(query.Assertion)
                            + " without proof steps and canon facts.");
                    }

                    if (chosen == null)
                    {
                        chosen = rule;
                        truth = result;
                        steps = ruleSteps;
                        grounds = ruleGrounds;
                    }
                    else if (result != truth)
                    {
                        throw new InvalidOperationException(
                            "Canon contradiction on " + CanonProof.Describe(query.Assertion) + ": rule '" + chosen.Id + "' says "
                            + truth + " but rule '" + rule.Id + "' says " + result + ".");
                    }
                }
            }

            return Compose(query, chosen, truth, steps, grounds);
        }

        /// <summary>
        /// Builds the answer: classification from the subject's category and instance facts, Scripture
        /// from the grounding facts, and canon provenance (Verified when established, Unverified when Unknown).
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="chosen">The rule that settled it, or null when none did.</param>
        /// <param name="truth">The truth value.</param>
        /// <param name="steps">The proof steps.</param>
        /// <param name="grounds">The grounding facts.</param>
        /// <returns>The answer.</returns>
        private Answer Compose(Query query, ICanonRule chosen, TruthValue truth, ProofStep[] steps, CanonFact[] grounds)
        {
            ConceptRef subject = query.Assertion.Subject;
            List<ConceptRef> classification = new List<ConceptRef>();
            foreach (CanonFact fact in facts.About(subject, RelationKind.InCategory))
            {
                classification.Add(fact.Assertion.Object);
            }

            foreach (CanonFact fact in facts.About(subject, RelationKind.IsInstanceOf))
            {
                classification.Add(fact.Assertion.Object);
            }

            List<ScriptureReference> scripture = new List<ScriptureReference>();
            List<Assertion> used = new List<Assertion>(grounds.Length);
            foreach (CanonFact fact in grounds)
            {
                used.Add(fact.Assertion);
                foreach (ScriptureReference reference in fact.Scripture)
                {
                    if (!scripture.Contains(reference))
                    {
                        scripture.Add(reference);
                    }
                }
            }

            string described = CanonProof.Describe(query.Assertion);
            string explanation = chosen == null
                ? UnknownExplanation
                : described + " is " + truth + " by rule " + chosen.Id + ".";
            return new Answer(
                query,
                truth,
                chosen == null ? 0.0 : 1.0,
                classification,
                new Justification(steps, scripture, used, explanation),
                new Provenance(AnswerSource.Canon, ModelName, clock.GetUtcNow(), chosen == null ? VerificationStatus.Unverified : VerificationStatus.Verified, chosen == null ? NoRuleId : chosen.Id));
        }
    }
}
