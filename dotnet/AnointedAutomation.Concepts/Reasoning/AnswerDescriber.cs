// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Renders any <see cref="Answer"/> (canon or language) as clear, human-readable text: the
    /// question, the truth, where it came from, the reasoning steps and the scripture.
    /// </summary>
    public static class AnswerDescriber
    {
        /// <summary>
        /// Describes an answer. The output is deterministic for equal answers.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <returns>Multi-line text separated by "\n".</returns>
        public static string Describe(Answer answer)
        {
            ReasoningGuard.NotNull(answer, nameof(answer));
            Assertion asked = answer.Query.Assertion;
            Provenance from = answer.Provenance;
            StringBuilder text = new StringBuilder();
            text.Append("Question: does ").Append(asked.Subject).Append(' ').Append(asked.Relation).Append(' ').Append(asked.Object).Append(" hold?\n");
            if (answer.Query.Context.Count > 0)
            {
                List<string> context = new List<string>();
                foreach (Circumstance circumstance in answer.Query.Context)
                {
                    context.Add(circumstance.Name);
                }

                text.Append("Context: ").Append(string.Join("; ", context)).Append('\n');
            }

            text.Append("Answer: ").Append(answer.Truth)
                .Append(" (confidence ").Append(answer.Confidence.ToString("0.###", CultureInfo.InvariantCulture)).Append(")\n");
            text.Append("Source: ").Append(from.Source).Append(" engine, model ").Append(from.Model)
                .Append(", ").Append(from.Status).Append(", from ").Append(from.RuleOrPromptId)
                .Append(" at ").Append(from.At.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture)).Append('\n');

            List<string> classes = new List<string>();
            foreach (ConceptRef concept in answer.Classification)
            {
                classes.Add(concept.ToString());
            }

            text.Append("Classified as: ").Append(classes.Count == 0 ? "none" : string.Join(", ", classes)).Append('\n');
            text.Append("Reasoning:\n");
            if (answer.Why.Steps.Count == 0)
            {
                text.Append("  none\n");
            }

            foreach (ProofStep step in answer.Why.Steps)
            {
                text.Append("  ").Append(step.Number).Append(". ").Append(step.Reading).Append(" [").Append(step.SymbolicForm).Append("]\n");
            }

            if (answer.Why.FactsUsed.Count > 0)
            {
                text.Append("Facts used:\n");
                foreach (Assertion fact in answer.Why.FactsUsed)
                {
                    text.Append("  - ").Append(fact.Subject).Append(' ').Append(fact.Relation).Append(' ').Append(fact.Object).Append('\n');
                }
            }

            text.Append("Explanation: ").Append(answer.Why.Explanation).Append('\n');
            List<string> verses = new List<string>();
            foreach (ScriptureReference reference in answer.Why.Scripture)
            {
                verses.Add(reference.ToString());
            }

            text.Append("Scripture: ").Append(verses.Count == 0 ? "none" : string.Join("; ", verses)).Append('\n');
            return text.ToString();
        }
    }
}
