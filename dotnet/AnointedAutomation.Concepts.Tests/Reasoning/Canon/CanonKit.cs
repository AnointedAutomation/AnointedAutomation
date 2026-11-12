// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Text;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Concepts.Reasoning;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Canon
{
    /// <summary>
    /// Shared fixtures for the canon reasoner tests: a fixed clock, common references and a
    /// value signature for comparing answers.
    /// </summary>
    internal static class CanonKit
    {
        public static readonly DateTimeOffset Now = new DateTimeOffset(2026, 11, 12, 0, 0, 0, TimeSpan.Zero);

        public static readonly ConceptRef Love = new ConceptRef(CanonFacts.AttributeKind, "Love");
        public static readonly ConceptRef Justice = new ConceptRef(CanonFacts.AttributeKind, "Justice");
        public static readonly ConceptRef Mercy = new ConceptRef(CanonFacts.AttributeKind, "Mercy");
        public static readonly ConceptRef Faithfulness = new ConceptRef(CanonFacts.AttributeKind, "Faithfulness");

        public static readonly ConceptRef Compassion = Concept("Compassion");
        public static readonly ConceptRef Murder = Concept("Murder");
        public static readonly ConceptRef Theft = Concept("Theft");
        public static readonly ConceptRef Abortion = ConceptRef.Of(SinId.Abortion);

        public static ConceptRef Concept(string name)
        {
            return new ConceptRef(ConceptRef.MoralConceptKind, name);
        }

        public static Query Ask(ConceptRef subject, RelationKind relation, ConceptRef @object)
        {
            return new Query(new Assertion(subject, relation, @object), new List<Circumstance>());
        }

        public static CanonReasoner Reasoner()
        {
            return new CanonReasoner(new FixedClock(Now));
        }

        /// <summary>Every value of an answer as one string, so two answers compare by value.</summary>
        public static string Signature(Answer answer)
        {
            var text = new StringBuilder();
            text.Append(answer.Query.Assertion.Subject).Append('|').Append(answer.Query.Assertion.Relation).Append('|')
                .Append(answer.Query.Assertion.Object).Append('|').Append(answer.Truth).Append('|')
                .Append(answer.Confidence.ToString("R", System.Globalization.CultureInfo.InvariantCulture)).Append('|');
            foreach (var c in answer.Classification)
            {
                text.Append(c).Append(',');
            }

            foreach (ProofStep step in answer.Why.Steps)
            {
                text.Append(step.Number).Append(step.SymbolicForm).Append(step.Reading).Append(';');
            }

            foreach (var reference in answer.Why.Scripture)
            {
                text.Append(reference).Append(',');
            }

            foreach (var fact in answer.Why.FactsUsed)
            {
                text.Append(fact).Append(',');
            }

            text.Append(answer.Why.Explanation).Append('|').Append(answer.Provenance.Source).Append(answer.Provenance.Model)
                .Append(answer.Provenance.At.ToString("O", System.Globalization.CultureInfo.InvariantCulture))
                .Append(answer.Provenance.Status).Append(answer.Provenance.RuleOrPromptId);
            return text.ToString();
        }

        /// <summary>A clock that always reads the same instant.</summary>
        internal sealed class FixedClock : TimeProvider
        {
            private readonly DateTimeOffset now;

            public FixedClock(DateTimeOffset now)
            {
                this.now = now;
            }

            public override DateTimeOffset GetUtcNow()
            {
                return now;
            }
        }
    }
}
