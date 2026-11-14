// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-13 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Threading;
using System.Threading.Tasks;

namespace AnointedAutomation.Concepts.Reasoning.Language.FreeText
{
    /// <summary>
    /// Answers a free-text question with exactly one call to a structured language model, constrained to
    /// <see cref="FreeTextPromptComposer.ReplySchema"/>, and strictly parses the reply. It never caches: callers store the
    /// answer by <see cref="FreeTextAnswer.PromptId"/> and model, and reuse it instead of asking again.
    /// </summary>
    public sealed class FreeTextReasoner
    {
        private readonly IStructuredLanguageModel model;
        private readonly FreeTextAnswerParser parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeTextReasoner"/> class.
        /// </summary>
        /// <param name="model">The language model.</param>
        /// <param name="parser">The reply parser.</param>
        public FreeTextReasoner(IStructuredLanguageModel model, FreeTextAnswerParser parser)
        {
            this.model = ReasoningGuard.NotNull(model, nameof(model));
            this.parser = ReasoningGuard.NotNull(parser, nameof(parser));
        }

        /// <summary>
        /// Gets the model name (part of the cache key).
        /// </summary>
        public string ModelName => model.Name;

        /// <summary>
        /// Asks the question: one paid call, one strictly parsed reply.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="cancellationToken">Cancellation.</param>
        /// <returns>The answer.</returns>
        /// <exception cref="InterpretationRejected">The reply failed validation.</exception>
        public async Task<FreeTextAnswer> Ask(FreeTextQuestion question, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(question, nameof(question));
            ComposedPrompt prompt = FreeTextPromptComposer.Compose(question);
            string reply = await model.Complete(prompt.Text, FreeTextPromptComposer.ReplySchema, cancellationToken).ConfigureAwait(false);
            return parser.Parse(reply, prompt.Id, model.Name);
        }
    }
}
