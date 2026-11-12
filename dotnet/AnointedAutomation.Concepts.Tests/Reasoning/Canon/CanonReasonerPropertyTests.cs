// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Reasoning;
using FsCheck.Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Canon
{
    /// <summary>
    /// Property tests over a universe of queries drawn from the canon (every indexed subject, every
    /// relation, every facet, category and coarse sin, God's character) plus subjects and objects the
    /// canon has never heard of. FsCheck picks the indices.
    /// </summary>
    public class CanonReasonerPropertyTests
    {
        private static readonly IReadOnlyList<ConceptRef> Subjects = BuildSubjects();
        private static readonly IReadOnlyList<ConceptRef> Objects = BuildObjects();
        private static readonly RelationKind[] Relations =
        {
            RelationKind.Violates, RelationKind.Upholds, RelationKind.IsInstanceOf, RelationKind.InCategory,
            RelationKind.HasGravity, RelationKind.GroundedIn, RelationKind.Commands, RelationKind.Forbids,
        };

        private static readonly IReadOnlyList<Circumstance>[] Contexts =
        {
            new List<Circumstance>(),
            new List<Circumstance> { new Circumstance("Duress") },
        };

        private static readonly CanonReasoner Reasoner = CanonKit.Reasoner();

        private static int Pick(int seed, int count)
        {
            return (int)(((long)seed % count + count) % count);
        }

        private static Query QueryFor(int subject, int relation, int @object, bool inContext)
        {
            return new Query(
                new Assertion(Subjects[Pick(subject, Subjects.Count)], Relations[Pick(relation, Relations.Length)], Objects[Pick(@object, Objects.Count)]),
                Contexts[inContext ? 1 : 0]);
        }

        private static bool Established(TruthValue truth)
        {
            return truth != TruthValue.Unknown;
        }

        [Property(MaxTest = 300)]
        public bool SameQuery_SameAnswer_AcrossRunsReasonersAndThreads(int subject, int relation, int @object, bool inContext)
        {
            var query = QueryFor(subject, relation, @object, inContext);
            var expected = CanonKit.Signature(Reasoner.Decide(query));
            if (!expected.Equals(CanonKit.Signature(CanonKit.Reasoner().Decide(query)), StringComparison.Ordinal))
            {
                return false;
            }

            var parallel = new string[8];
            Parallel.For(0, parallel.Length, i => parallel[i] = CanonKit.Signature(Reasoner.Decide(query)));
            foreach (var signature in parallel)
            {
                if (!expected.Equals(signature, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        [Property(MaxTest = 300)]
        public bool Unknown_IsNeverReturnedAsFalse(int subject, int relation, int @object, bool inContext)
        {
            var query = QueryFor(subject, relation, @object, inContext);
            var answer = Reasoner.Decide(query);
            bool unknownSubject = !CanonFacts.Canon.TryGetConcept(query.Assertion.Subject, out _)
                && !CanonFacts.Canon.TryGetAttribute(query.Assertion.Subject, out _);
            if (unknownSubject && answer.Truth != TruthValue.Unknown)
            {
                return false;
            }

            if (answer.Truth != TruthValue.False)
            {
                return true;
            }

            // A False is always a proof: full confidence, steps, facts and a named rule.
            return answer.Confidence == 1.0
                && answer.Why.Steps.Count > 0
                && answer.Why.FactsUsed.Count > 0
                && !answer.Provenance.RuleOrPromptId.Equals(CanonReasoner.NoRuleId, StringComparison.Ordinal);
        }

        [Property(MaxTest = 300)]
        public bool AddingAnUnrelatedFact_NeverFlipsAnEstablishedAnswer(int subject, int relation, int @object, int extraRelation, int extraObject, int tag)
        {
            var query = QueryFor(subject, relation, @object, false);
            var before = Reasoner.Decide(query);
            var unrelated = new ConceptRef("Unrelated", "u" + Pick(tag, 1000));
            var extra = new CanonFact(
                new Assertion(unrelated, Relations[Pick(extraRelation, Relations.Length)], Objects[Pick(extraObject, Objects.Count)]),
                new[] { new ScriptureReference(BibleBook.Ecclesiastes, 1, 9, 0) });
            var extended = new CanonReasoner(CanonFacts.Canon.With(new[] { extra }), CanonRules.Default, new CanonKit.FixedClock(CanonKit.Now));
            var after = extended.Decide(query);
            return !Established(before.Truth) || before.Truth == after.Truth;
        }

        [Property(MaxTest = 300)]
        public bool EveryEstablishedAnswer_CitesScripture(int subject, int relation, int @object, bool inContext)
        {
            var answer = Reasoner.Decide(QueryFor(subject, relation, @object, inContext));
            return !Established(answer.Truth) || answer.Why.Scripture.Count > 0;
        }

        private static IReadOnlyList<ConceptRef> BuildSubjects()
        {
            var subjects = new List<ConceptRef>();
            var seen = new HashSet<ConceptRef>();
            foreach (var fact in CanonFacts.Canon.All)
            {
                if (seen.Add(fact.Assertion.Subject))
                {
                    subjects.Add(fact.Assertion.Subject);
                }
            }

            subjects.Add(new ConceptRef("Nowhere", "Nothing"));
            subjects.Add(CanonKit.Concept("NotACanonConcept"));
            return subjects;
        }

        private static IReadOnlyList<ConceptRef> BuildObjects()
        {
            var objects = new List<ConceptRef> { CanonKit.Love, CanonKit.Justice, CanonKit.Mercy, CanonKit.Faithfulness, CanonFacts.RevealedCharacter };
            foreach (SinCategory category in Enum.GetValues(typeof(SinCategory)))
            {
                objects.Add(CanonFacts.Category(category));
            }

            foreach (AnointedAutomation.Enums.Sin sin in Enum.GetValues(typeof(AnointedAutomation.Enums.Sin)))
            {
                objects.Add(CanonFacts.CoarseSin(sin));
            }

            objects.Add(CanonKit.Murder);
            objects.Add(CanonKit.Compassion);
            objects.Add(new ConceptRef("Nowhere", "Nothing"));
            return objects;
        }
    }
}
