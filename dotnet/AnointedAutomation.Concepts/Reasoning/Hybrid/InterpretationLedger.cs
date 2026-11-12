// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Reasoning.Language;

namespace AnointedAutomation.Concepts.Reasoning.Hybrid
{
    /// <summary>
    /// Keeps language-engine answers apart from the canon, each private to its owner's chat, and lets
    /// the canon's owner review them. Promotion yields a <see cref="FactPromotion"/> for a reviewed
    /// catalog PR; it never changes the running <see cref="CanonFacts"/>.
    /// </summary>
    public sealed class InterpretationLedger
    {
        private readonly IInterpretationStore store;
        private readonly IPromotionPolicy policy;
        private readonly TimeProvider clock;
        private readonly object gate = new object();
        private readonly Dictionary<string, Task<InterpretationEntry>> inFlight = new Dictionary<string, Task<InterpretationEntry>>(StringComparer.Ordinal);

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpretationLedger"/> class.
        /// </summary>
        /// <param name="store">Where entries are kept.</param>
        /// <param name="policy">Who may promote or reject; required, there is no permissive default.</param>
        /// <param name="clock">The clock for review timestamps.</param>
        public InterpretationLedger(IInterpretationStore store, IPromotionPolicy policy, TimeProvider clock)
        {
            this.store = ReasoningGuard.NotNull(store, nameof(store));
            this.policy = ReasoningGuard.NotNull(policy, nameof(policy));
            this.clock = ReasoningGuard.NotNull(clock, nameof(clock));
        }

        /// <summary>
        /// Returns the owner's entry for this query hash and model, or asks once and records the result.
        /// Concurrent callers for the same owner, query and model share one ask (run under the first caller's token).
        /// </summary>
        /// <param name="owner">The owner the entry is private to.</param>
        /// <param name="queryKey">The query hash (the composed prompt id).</param>
        /// <param name="model">The model name.</param>
        /// <param name="ask">Asks the language engine; called at most once per owner, query and model.</param>
        /// <param name="cancellationToken">Cancels the read, the ask and the write.</param>
        /// <returns>The cached or newly recorded entry.</returns>
        public async Task<InterpretationEntry> GetOrRecord(
            OwnerScope owner,
            string queryKey,
            string model,
            Func<CancellationToken, Task<LanguageResult>> ask,
            CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(owner, nameof(owner));
            ReasoningGuard.NotBlank(queryKey, nameof(queryKey));
            ReasoningGuard.NotBlank(model, nameof(model));
            ReasoningGuard.NotNull(ask, nameof(ask));
            string key = owner.OwnerId + "\n" + queryKey + "\n" + model;
            Task<InterpretationEntry> pending;
            lock (gate)
            {
                if (!inFlight.TryGetValue(key, out pending))
                {
                    pending = Load(owner, queryKey, model, ask, cancellationToken);
                    inFlight.Add(key, pending);
                }
            }

            try
            {
                return await pending.ConfigureAwait(false);
            }
            finally
            {
                lock (gate)
                {
                    if (inFlight.TryGetValue(key, out Task<InterpretationEntry> current) && ReferenceEquals(current, pending))
                    {
                        inFlight.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Finds one of the owner's entries.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="entryId">The entry id.</param>
        /// <param name="cancellationToken">Cancels the read.</param>
        /// <returns>The entry; throws <see cref="KeyNotFoundException"/> when the owner has none with that id.</returns>
        public async Task<InterpretationEntry> Get(OwnerScope owner, string entryId, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(owner, nameof(owner));
            ReasoningGuard.NotBlank(entryId, nameof(entryId));
            InterpretationEntry entry = await store.Find(owner, entryId, cancellationToken).ConfigureAwait(false);
            if (entry == null)
            {
                throw new KeyNotFoundException("No ledger entry '" + entryId + "' exists for this owner.");
            }

            return entry;
        }

        /// <summary>
        /// Lists one page of the owner's entries. There is no listing across owners.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="skip">Entries to skip.</param>
        /// <param name="limit">Page size.</param>
        /// <param name="cancellationToken">Cancels the read.</param>
        /// <returns>The page and the owner's total.</returns>
        public Task<InterpretationPage> List(OwnerScope owner, int skip, int limit, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(owner, nameof(owner));
            return store.List(owner, skip, limit, cancellationToken);
        }

        /// <summary>
        /// Approves an unverified entry for the canon. Throws <see cref="UnauthorizedAccessException"/>
        /// unless the policy allows the reviewer. Marks the entry
        /// <see cref="InterpretationStatus.VerifiedPendingCatalog"/> and returns what to add to the source
        /// catalog; the running <see cref="CanonFacts"/> are not touched.
        /// </summary>
        /// <param name="owner">The owner whose chat holds the entry.</param>
        /// <param name="entryId">The entry id.</param>
        /// <param name="reviewer">The reviewer.</param>
        /// <param name="note">Why it was approved.</param>
        /// <param name="cancellationToken">Cancels the review.</param>
        /// <returns>The assertion and scripture to add in a reviewed PR.</returns>
        public async Task<FactPromotion> Promote(OwnerScope owner, string entryId, Reviewer reviewer, string note, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(reviewer, nameof(reviewer));
            ReasoningGuard.NotBlank(note, nameof(note));
            InterpretationEntry entry = await Get(owner, entryId, cancellationToken).ConfigureAwait(false);
            if (!policy.MayPromote(reviewer, entry))
            {
                throw new UnauthorizedAccessException("Reviewer '" + reviewer.ReviewerId + "' may not promote entry '" + entryId + "'.");
            }

            if (entry.Answer.Truth == TruthValue.Unknown)
            {
                throw new InvalidOperationException("Entry '" + entryId + "' answered Unknown; there is no fact to promote.");
            }

            await Transition(entry, InterpretationStatus.VerifiedPendingCatalog, note, cancellationToken).ConfigureAwait(false);
            return new FactPromotion(entry.Id, entry.Query.Assertion, entry.Answer.Truth, entry.Answer.Why.Scripture, reviewer, note, clock.GetUtcNow());
        }

        /// <summary>
        /// Rejects an unverified entry. Throws <see cref="UnauthorizedAccessException"/> unless the policy
        /// allows the reviewer.
        /// </summary>
        /// <param name="owner">The owner whose chat holds the entry.</param>
        /// <param name="entryId">The entry id.</param>
        /// <param name="reviewer">The reviewer.</param>
        /// <param name="note">Why it was rejected.</param>
        /// <param name="cancellationToken">Cancels the review.</param>
        /// <returns>The rejected entry.</returns>
        public async Task<InterpretationEntry> Reject(OwnerScope owner, string entryId, Reviewer reviewer, string note, CancellationToken cancellationToken)
        {
            ReasoningGuard.NotNull(reviewer, nameof(reviewer));
            ReasoningGuard.NotBlank(note, nameof(note));
            InterpretationEntry entry = await Get(owner, entryId, cancellationToken).ConfigureAwait(false);
            if (!policy.MayReject(reviewer, entry))
            {
                throw new UnauthorizedAccessException("Reviewer '" + reviewer.ReviewerId + "' may not reject entry '" + entryId + "'.");
            }

            return await Transition(entry, InterpretationStatus.Rejected, note, cancellationToken).ConfigureAwait(false);
        }

        private async Task<InterpretationEntry> Transition(InterpretationEntry entry, InterpretationStatus status, string note, CancellationToken cancellationToken)
        {
            if (entry.Status != InterpretationStatus.Unverified)
            {
                throw new InvalidOperationException("Entry '" + entry.Id + "' is " + entry.Status + "; only unverified entries can be reviewed.");
            }

            InterpretationEntry reviewed = entry.Reviewed(status, note);
            if (!await store.Replace(reviewed, InterpretationStatus.Unverified, cancellationToken).ConfigureAwait(false))
            {
                throw new InvalidOperationException("Entry '" + entry.Id + "' was reviewed concurrently.");
            }

            return reviewed;
        }

        private async Task<InterpretationEntry> Load(
            OwnerScope owner,
            string queryKey,
            string model,
            Func<CancellationToken, Task<LanguageResult>> ask,
            CancellationToken cancellationToken)
        {
            InterpretationEntry cached = await store.FindByQuery(owner, queryKey, model, cancellationToken).ConfigureAwait(false);
            if (cached != null)
            {
                return cached;
            }

            LanguageResult result = await ask(cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                throw new InvalidOperationException("The language engine returned no result for query " + queryKey + ".");
            }

            Provenance provenance = result.Answer.Provenance;
            if (provenance.Source != AnswerSource.Language || provenance.Status != VerificationStatus.Unverified)
            {
                throw new InvalidOperationException("Only unverified language answers can be recorded (query " + queryKey + ").");
            }

            if (!string.Equals(provenance.Model, model, StringComparison.Ordinal) || !string.Equals(provenance.RuleOrPromptId, queryKey, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("The language answer's model or prompt id does not match the cache key for query " + queryKey + ".");
            }

            InterpretationEntry entry = new InterpretationEntry(
                Guid.NewGuid().ToString("N"),
                owner,
                queryKey,
                result.Answer.Query,
                result.Answer,
                result.Interpretation,
                provenance,
                InterpretationStatus.Unverified,
                string.Empty);
            await store.Add(entry, cancellationToken).ConfigureAwait(false);
            return entry;
        }
    }
}
