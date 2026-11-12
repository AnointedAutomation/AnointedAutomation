// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Reasoning;
using AnointedAutomation.Concepts.Reasoning.Hybrid;
using AnointedAutomation.Concepts.Reasoning.Language;
using AnointedAutomation.Concepts.Tests.Reasoning.Canon;
using AnointedAutomation.Concepts.Tests.Reasoning.Language;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Hybrid
{
    public class HybridReasonerTests
    {
        private static readonly OwnerScope Alice = new OwnerScope("chat-alice");
        private static readonly OwnerScope Bob = new OwnerScope("chat-bob");
        private static readonly Reviewer Owner = new Reviewer("canon-owner");
        private static readonly Reviewer Stranger = new Reviewer("someone-else");

        private static Query Unsettled()
        {
            return CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Mercy);
        }

        private static Query Settled()
        {
            return CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Justice);
        }

        private sealed class Kit
        {
            public Kit()
            {
                Model = new LanguageKit.FakeLanguageModel(LanguageKit.Golden("reply-valid.json"));
                Language = new LanguageReasoner(Model, LanguageKit.Parser(new LanguageKit.CapturingLogger<InterpretationParser>()), new CanonKit.FixedClock(CanonKit.Now));
                Store = new InMemoryInterpretationStore();
                Ledger = new InterpretationLedger(Store, new OwnerOnlyPolicy(Owner), new CanonKit.FixedClock(CanonKit.Now));
            }

            public LanguageKit.FakeLanguageModel Model { get; }

            public LanguageReasoner Language { get; }

            public InMemoryInterpretationStore Store { get; }

            public InterpretationLedger Ledger { get; }

            public HybridReasoner For(OwnerScope owner)
            {
                return new HybridReasoner(CanonKit.Reasoner(), Language, Ledger, owner);
            }
        }

        private sealed class OwnerOnlyPolicy : IPromotionPolicy
        {
            private readonly Reviewer owner;

            public OwnerOnlyPolicy(Reviewer owner)
            {
                this.owner = owner;
            }

            public bool MayPromote(Reviewer reviewer, InterpretationEntry entry)
            {
                return reviewer == owner;
            }

            public bool MayReject(Reviewer reviewer, InterpretationEntry entry)
            {
                return reviewer == owner;
            }
        }

        private sealed class DenyAllPolicy : IPromotionPolicy
        {
            public bool MayPromote(Reviewer reviewer, InterpretationEntry entry)
            {
                return false;
            }

            public bool MayReject(Reviewer reviewer, InterpretationEntry entry)
            {
                return false;
            }
        }

        [Fact]
        public async Task Ask_CanonSettles_NeverCallsTheModel()
        {
            var kit = new Kit();

            Answer answer = await kit.For(Alice).Ask(Settled(), CancellationToken.None);

            Assert.Equal(TruthValue.True, answer.Truth);
            Assert.Equal(AnswerSource.Canon, answer.Provenance.Source);
            Assert.Empty(kit.Model.Prompts);
            Assert.Equal(0, (await kit.Ledger.List(Alice, 0, 10, CancellationToken.None)).Total);
        }

        [Fact]
        public async Task Ask_CanonUnknown_AsksTheModelOnceAndRecordsAnUnverifiedEntry()
        {
            var kit = new Kit();

            HybridResult result = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);

            Assert.Single(kit.Model.Prompts);
            Assert.True(result.IsLanguageDerived);
            Assert.Equal(AnswerSource.Language, result.Answer.Provenance.Source);
            Assert.Equal(LanguageKit.ModelName, result.Answer.Provenance.Model);
            Assert.Equal(CanonKit.Now, result.Answer.Provenance.At);
            Assert.Equal(VerificationStatus.Unverified, result.Answer.Provenance.Status);
            Assert.Equal(InterpretationStatus.Unverified, result.Entry.Status);
            Assert.Equal(Alice, result.Entry.Owner);
            Assert.Equal(PromptComposer.Compose(Unsettled()).Id, result.Entry.QueryKey);
            Assert.Equal(VerificationStatus.Unverified, result.Entry.Provenance.Status);
            Assert.Same(result.Entry.Answer, result.Answer);
        }

        [Fact]
        public async Task Ask_SameQueryTwice_CallsTheModelOnce()
        {
            var kit = new Kit();
            HybridReasoner reasoner = kit.For(Alice);

            HybridResult first = await reasoner.AskWithEntry(Unsettled(), CancellationToken.None);
            HybridResult second = await reasoner.AskWithEntry(Unsettled(), CancellationToken.None);

            Assert.Single(kit.Model.Prompts);
            Assert.Equal(first.Entry.Id, second.Entry.Id);
            Assert.Equal(1, (await kit.Ledger.List(Alice, 0, 10, CancellationToken.None)).Total);
        }

        [Fact]
        public async Task Ask_ConcurrentSameQuery_SharesOneModelCall()
        {
            var kit = new Kit();
            HybridReasoner reasoner = kit.For(Alice);

            HybridResult[] results = await Task.WhenAll(
                reasoner.AskWithEntry(Unsettled(), CancellationToken.None),
                reasoner.AskWithEntry(Unsettled(), CancellationToken.None));

            Assert.Single(kit.Model.Prompts);
            Assert.Equal(results[0].Entry.Id, results[1].Entry.Id);
        }

        [Fact]
        public async Task Ask_LanguageAnswer_NeverChangesALaterCanonAnswer()
        {
            var kit = new Kit();
            int factCount = CanonFacts.Canon.All.Count;
            Answer before = await CanonKit.Reasoner().Ask(Settled(), CancellationToken.None);

            await kit.For(Alice).Ask(Unsettled(), CancellationToken.None);
            Answer after = await kit.For(Alice).Ask(Settled(), CancellationToken.None);
            Answer stillUnknown = await CanonKit.Reasoner().Ask(Unsettled(), CancellationToken.None);

            Assert.Equal(CanonKit.Signature(before), CanonKit.Signature(after));
            Assert.Equal(AnswerSource.Canon, after.Provenance.Source);
            Assert.Equal(TruthValue.Unknown, stillUnknown.Truth);
            Assert.Equal(factCount, CanonFacts.Canon.All.Count);
        }

        [Fact]
        public async Task Ledger_OneOwnerCannotReadAnothersEntries()
        {
            var kit = new Kit();
            HybridResult alices = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => kit.Ledger.Get(Bob, alices.Entry.Id, CancellationToken.None));
            Assert.Null(await kit.Store.Find(Bob, alices.Entry.Id, CancellationToken.None));
            Assert.Equal(0, (await kit.Ledger.List(Bob, 0, 10, CancellationToken.None)).Total);
            Assert.Equal(alices.Entry, await kit.Ledger.Get(Alice, alices.Entry.Id, CancellationToken.None));
        }

        [Fact]
        public async Task Ask_OtherOwnerSameQuery_GetsTheirOwnPrivateEntry()
        {
            var kit = new Kit();

            HybridResult alices = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);
            HybridResult bobs = await kit.For(Bob).AskWithEntry(Unsettled(), CancellationToken.None);

            Assert.NotEqual(alices.Entry.Id, bobs.Entry.Id);
            Assert.Equal(Bob, bobs.Entry.Owner);
            Assert.Equal(2, kit.Model.Prompts.Count);
        }

        [Fact]
        public async Task Promote_ByOwner_ReturnsFactPromotionAndLeavesCanonFactsAlone()
        {
            var kit = new Kit();
            int factCount = CanonFacts.Canon.All.Count;
            HybridResult asked = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);

            FactPromotion promotion = await kit.Ledger.Promote(Alice, asked.Entry.Id, Owner, "Matches Micah 6:8.", CancellationToken.None);

            Assert.Equal(asked.Entry.Id, promotion.EntryId);
            Assert.Equal(Unsettled().Assertion, promotion.Assertion);
            Assert.Equal(asked.Answer.Truth, promotion.Truth);
            Assert.Equal(asked.Answer.Why.Scripture, promotion.Scripture);
            Assert.Equal(Owner, promotion.Reviewer);
            Assert.Equal(CanonKit.Now, promotion.At);
            Assert.Equal(InterpretationStatus.VerifiedPendingCatalog, (await kit.Ledger.Get(Alice, asked.Entry.Id, CancellationToken.None)).Status);
            Assert.Equal(factCount, CanonFacts.Canon.All.Count);
            Assert.Equal(TruthValue.Unknown, (await CanonKit.Reasoner().Ask(Unsettled(), CancellationToken.None)).Truth);
            await Assert.ThrowsAsync<InvalidOperationException>(() => kit.Ledger.Promote(Alice, asked.Entry.Id, Owner, "Again.", CancellationToken.None));
        }

        [Fact]
        public async Task Promote_PolicyDenies_ThrowsAndLeavesEntryUnverified()
        {
            var kit = new Kit();
            HybridResult asked = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => kit.Ledger.Promote(Alice, asked.Entry.Id, Stranger, "Looks right.", CancellationToken.None));
            var denying = new InterpretationLedger(kit.Store, new DenyAllPolicy(), new CanonKit.FixedClock(CanonKit.Now));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => denying.Promote(Alice, asked.Entry.Id, Owner, "Looks right.", CancellationToken.None));

            Assert.Equal(InterpretationStatus.Unverified, (await kit.Ledger.Get(Alice, asked.Entry.Id, CancellationToken.None)).Status);
        }

        [Fact]
        public async Task Promote_WrongOwnerScope_Throws()
        {
            var kit = new Kit();
            HybridResult asked = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => kit.Ledger.Promote(Bob, asked.Entry.Id, Owner, "Note.", CancellationToken.None));
        }

        [Fact]
        public async Task Reject_ByOwner_MarksRejectedAndCacheReturnsRejectedAnswer()
        {
            var kit = new Kit();
            HybridResult asked = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);

            InterpretationEntry rejected = await kit.Ledger.Reject(Alice, asked.Entry.Id, Owner, "Not supported by scripture.", CancellationToken.None);
            HybridResult again = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);

            Assert.Equal(InterpretationStatus.Rejected, rejected.Status);
            Assert.Equal("Not supported by scripture.", rejected.ReviewNote);
            Assert.Single(kit.Model.Prompts);
            Assert.Equal(VerificationStatus.Rejected, again.Answer.Provenance.Status);
            await Assert.ThrowsAsync<InvalidOperationException>(() => kit.Ledger.Promote(Alice, asked.Entry.Id, Owner, "Changed my mind.", CancellationToken.None));
        }

        [Fact]
        public async Task Reject_PolicyDenies_Throws()
        {
            var kit = new Kit();
            HybridResult asked = await kit.For(Alice).AskWithEntry(Unsettled(), CancellationToken.None);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => kit.Ledger.Reject(Alice, asked.Entry.Id, Stranger, "No.", CancellationToken.None));
            Assert.Equal(InterpretationStatus.Unverified, (await kit.Ledger.Get(Alice, asked.Entry.Id, CancellationToken.None)).Status);
        }

        [Fact]
        public async Task Ask_LanguageFails_RecordsNothingAndRetries()
        {
            var kit = new Kit();
            var badModel = new LanguageKit.FakeLanguageModel(LanguageKit.Golden("reply-bad-truth.json"));
            var badLanguage = new LanguageReasoner(badModel, LanguageKit.Parser(new LanguageKit.CapturingLogger<InterpretationParser>()), new CanonKit.FixedClock(CanonKit.Now));
            var reasoner = new HybridReasoner(CanonKit.Reasoner(), badLanguage, kit.Ledger, Alice);

            await Assert.ThrowsAsync<InterpretationRejected>(() => reasoner.Ask(Unsettled(), CancellationToken.None));
            await Assert.ThrowsAsync<InterpretationRejected>(() => reasoner.Ask(Unsettled(), CancellationToken.None));

            Assert.Equal(2, badModel.Prompts.Count);
            Assert.Equal(0, (await kit.Ledger.List(Alice, 0, 10, CancellationToken.None)).Total);
        }

        [Fact]
        public async Task List_PagesTheOwnersEntries()
        {
            var kit = new Kit();
            await kit.For(Alice).Ask(Unsettled(), CancellationToken.None);
            await kit.For(Alice).Ask(CanonKit.Ask(new ConceptRef("Nowhere", "Nothing"), RelationKind.Violates, CanonKit.Love), CancellationToken.None);

            InterpretationPage page = await kit.Ledger.List(Alice, 1, 1, CancellationToken.None);

            Assert.Equal(2, page.Total);
            Assert.Single(page.Entries);
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => kit.Ledger.List(Alice, -1, 1, CancellationToken.None));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => kit.Ledger.List(Alice, 0, 0, CancellationToken.None));
        }

        [Fact]
        public void Constructors_RequireEveryDependency()
        {
            var kit = new Kit();
            var clock = new CanonKit.FixedClock(CanonKit.Now);

            Assert.Throws<ArgumentNullException>(() => new HybridReasoner(null, kit.Language, kit.Ledger, Alice));
            Assert.Throws<ArgumentNullException>(() => new HybridReasoner(CanonKit.Reasoner(), null, kit.Ledger, Alice));
            Assert.Throws<ArgumentNullException>(() => new HybridReasoner(CanonKit.Reasoner(), kit.Language, null, Alice));
            Assert.Throws<ArgumentNullException>(() => new HybridReasoner(CanonKit.Reasoner(), kit.Language, kit.Ledger, null));
            Assert.Throws<ArgumentNullException>(() => new InterpretationLedger(null, new DenyAllPolicy(), clock));
            Assert.Throws<ArgumentNullException>(() => new InterpretationLedger(kit.Store, null, clock));
            Assert.Throws<ArgumentNullException>(() => new InterpretationLedger(kit.Store, new DenyAllPolicy(), null));
            Assert.Throws<ArgumentException>(() => new OwnerScope(" "));
            Assert.Throws<ArgumentException>(() => new Reviewer(""));
        }
    }
}
