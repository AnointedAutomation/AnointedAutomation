// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// A <see cref="HybridReasoner"/> answer and, when it is language-derived, its ledger entry.
    /// </summary>
    public sealed record HybridResult
    {
        private HybridResult(Answer answer, InterpretationEntry entry)
        {
            Answer = answer;
            Entry = entry;
        }

        /// <summary>Gets the answer.</summary>
        public Answer Answer { get; }

        /// <summary>Gets the ledger entry; null exactly when <see cref="IsLanguageDerived"/> is false.</summary>
        public InterpretationEntry Entry { get; }

        /// <summary>Gets whether the answer came from the language engine.</summary>
        public bool IsLanguageDerived => Answer.Provenance.Source == AnswerSource.Language;

        internal static HybridResult FromCanon(Answer answer)
        {
            ReasoningGuard.NotNull(answer, nameof(answer));
            if (answer.Provenance.Source != AnswerSource.Canon)
            {
                throw new System.InvalidOperationException("The canon engine returned a non-canon answer.");
            }

            return new HybridResult(answer, null);
        }

        internal static HybridResult FromLedger(InterpretationEntry entry)
        {
            ReasoningGuard.NotNull(entry, nameof(entry));
            Answer answer = entry.Answer;
            if (entry.Status == InterpretationStatus.Rejected)
            {
                Provenance p = answer.Provenance;
                answer = new Answer(answer.Query, answer.Truth, answer.Confidence, answer.Classification, answer.Why, new Provenance(p.Source, p.Model, p.At, VerificationStatus.Rejected, p.RuleOrPromptId));
            }

            return new HybridResult(answer, entry);
        }
    }
}
