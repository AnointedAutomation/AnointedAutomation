// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Reasoning.Language;

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// Deterministic first: returns the canon's answer whenever it is True or False. Only on Unknown
    /// does it ask the language engine, once per owner, query and model, record the answer in the
    /// owner's private <see cref="InterpretationLedger"/> and return it marked language-derived and
    /// unverified. Language answers are never inserted into <see cref="CanonFacts"/>.
    /// </summary>
    public sealed class HybridReasoner : IReasoner
    {
        private readonly CanonReasoner canon;
        private readonly LanguageReasoner language;
        private readonly InterpretationLedger ledger;
        private readonly OwnerScope owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="HybridReasoner"/> class for one owner's chat.
        /// </summary>
        /// <param name="canon">The deterministic engine.</param>
        /// <param name="language">The language engine.</param>
        /// <param name="ledger">Where language answers are kept.</param>
        /// <param name="owner">The user or chat the language answers are private to.</param>
        public HybridReasoner(CanonReasoner canon, LanguageReasoner language, InterpretationLedger ledger, OwnerScope owner)
        {
            this.canon = ReasoningGuard.NotNull(canon, nameof(canon));
            this.language = ReasoningGuard.NotNull(language, nameof(language));
            this.ledger = ReasoningGuard.NotNull(ledger, nameof(ledger));
            this.owner = ReasoningGuard.NotNull(owner, nameof(owner));
        }

        /// <inheritdoc/>
        public async Task<Answer> Ask(Query query, CancellationToken cancellationToken)
        {
            HybridResult result = await AskWithEntry(query, cancellationToken).ConfigureAwait(false);
            return result.Answer;
        }

        /// <summary>
        /// Answers like <see cref="Ask"/> and also returns the ledger entry when the answer is language-derived.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">Cancels the work.</param>
        /// <returns>The answer, plus the ledger entry for a language answer.</returns>
        public async Task<HybridResult> AskWithEntry(Query query, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(query, nameof(query));
            Answer decided = await canon.Ask(query, cancellationToken).ConfigureAwait(false);
            if (decided.Truth != TruthValue.Unknown)
            {
                return HybridResult.FromCanon(decided);
            }

            string queryKey = PromptComposer.Compose(query).Id;
            InterpretationEntry entry = await ledger.GetOrRecord(
                owner,
                queryKey,
                language.ModelName,
                ct => language.Interpret(query, ct),
                cancellationToken).ConfigureAwait(false);
            return HybridResult.FromLedger(entry);
        }
    }
}
