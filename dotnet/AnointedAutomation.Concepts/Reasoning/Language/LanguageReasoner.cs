// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Epistemics;

namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// The language engine: composes one prompt per query, sends it through an
    /// <see cref="ILanguageModel"/>, strictly parses the reply and returns an answer whose
    /// provenance is Language and Unverified. It never marks an answer Verified; only the owner
    /// promotes an interpretation into a fact.
    /// </summary>
    public sealed class LanguageReasoner : IReasoner
    {
        private readonly ILanguageModel model;
        private readonly InterpretationParser parser;
        private readonly TimeProvider clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageReasoner"/> class.
        /// </summary>
        /// <param name="model">The model port; required, there is no default model.</param>
        /// <param name="parser">The strict reply parser.</param>
        /// <param name="clock">Stamps each answer's provenance.</param>
        public LanguageReasoner(ILanguageModel model, InterpretationParser parser, TimeProvider clock)
        {
            this.model = ReasoningGuard.NotNull(model, nameof(model));
            this.parser = ReasoningGuard.NotNull(parser, nameof(parser));
            this.clock = ReasoningGuard.NotNull(clock, nameof(clock));
        }

        /// <inheritdoc/>
        public async Task<Answer> Ask(Query query, CancellationToken cancellationToken)
        {
            LanguageResult result = await Interpret(query, cancellationToken).ConfigureAwait(false);
            return result.Answer;
        }

        /// <summary>
        /// Answers a query and also returns the full interpretation, from one model call.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">Cancels the model call.</param>
        /// <returns>The answer and its interpretation.</returns>
        /// <exception cref="InterpretationRejected">The model reply was invalid (already logged in full).</exception>
        public async Task<LanguageResult> Interpret(Query query, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(query, nameof(query));
            ComposedPrompt prompt = PromptComposer.Compose(query);
            string reply = await model.Complete(prompt.Text, cancellationToken).ConfigureAwait(false);
            Interpretation interpretation = parser.Parse(reply, prompt.Id, model.Name);
            return new LanguageResult(Build(query, prompt, interpretation), interpretation);
        }

        private Answer Build(Query query, ComposedPrompt prompt, Interpretation interpretation)
        {
            Assertion asked = query.Assertion;
            string confidence = interpretation.Confidence.ToString("0.###", CultureInfo.InvariantCulture);
            List<ProofStep> steps = new List<ProofStep>
            {
                new ProofStep(1, "ask(" + prompt.Id + ", " + model.Name + ")", "Asked the language model " + model.Name + " with prompt " + prompt.Id + "."),
                new ProofStep(
                    2,
                    asked.Subject + " " + asked.Relation + " " + asked.Object + " = " + interpretation.Truth + " @ " + confidence,
                    "The model answered " + interpretation.Truth + " with confidence " + confidence + "; unverified until the owner promotes it."),
            };

            foreach (Assertion suggestion in interpretation.Assertions)
            {
                steps.Add(new ProofStep(
                    steps.Count + 1,
                    "suggest(" + suggestion.Subject + " " + suggestion.Relation + " " + suggestion.Object + ")",
                    "The model suggested " + suggestion.Subject + " " + suggestion.Relation + " " + suggestion.Object + " (unverified)."));
            }

            List<ConceptRef> classification = new List<ConceptRef>();
            foreach (ConceptRef concept in interpretation.Classification)
            {
                AddOnce(classification, concept);
            }

            foreach (SinId sin in interpretation.SinIds)
            {
                AddOnce(classification, ConceptRef.Of(sin));
            }

            return new Answer(
                query,
                interpretation.Truth,
                interpretation.Confidence,
                classification,
                new Justification(steps, interpretation.Scripture, new List<Assertion>(), interpretation.Explanation),
                new Provenance(AnswerSource.Language, model.Name, clock.GetUtcNow(), VerificationStatus.Unverified, prompt.Id));
        }

        private static void AddOnce(List<ConceptRef> list, ConceptRef concept)
        {
            if (!list.Contains(concept))
            {
                list.Add(concept);
            }
        }
    }
}
