# Code Documentation Status

## Overview
All code files in the solution have been documented with XML documentation following C# standards. This includes classes, methods, properties, events, and interfaces.

## Documentation Coverage

### 1. AnointedAutomation.Enums
- PaymentType.cs
  - Enum value documentation for all payment methods
  - None, PayPalToken, MasterCard, Visa, ACH

- PaymentProvider.cs
  - Enum value documentation for payment gateway providers
  - None, Stripe, PayPal, Braintree, Checkout, Square, Adyen, AuthorizeNet

- TransactionStatus.cs
  - Comprehensive transaction status values
  - Pending, Processing, RequiresAction, Succeeded, Failed, etc.

- WebhookEventType.cs
  - Common webhook event types across payment providers
  - Payment, refund, dispute, customer, subscription, invoice events

- SubscriptionStatus.cs
  - Subscription lifecycle status values
  - None, Active, Trialing, PastDue, Cancelled, Suspended, Paused, Expired, Pending, NotRenewed

- PaymentOperation.cs
  - PCI-DSS audit logging operation types (25 operations)
  - CreateCharge, CreateRefund, CreateCustomer, CreateSubscription, etc.

- CardType.cs
  - Credit/debit card type detection values
  - Unknown, Visa, MasterCard, AmericanExpress, Discover, DinersClub, JCB, UnionPay, Maestro

### 2. AnointedAutomation.APIMiddlewares
- IPBlacklistMiddleware.cs
  - Class and method documentation
  - Event documentation
  - Parameter descriptions
  - Safety warnings for sensitive operations

- EndpointAccessMiddleware.cs
  - Class and method documentation
  - Memory management details
  - Configuration parameters
  - Event handling

- InvalidEndpointTrackerMiddleware.cs
  - Class and method documentation
  - Security tracking details
  - Configuration options

- APIKeyAttribute.cs
  - Full attribute documentation
  - Environment variable configuration
  - Security implications

### 3. AnointedAutomation.Memory
- GarbageCollection.cs
  - Complete class documentation
  - Memory management method details
  - Safety considerations
  - Exception documentation

### 4. AnointedAutomation.Objects
- ChristianEmoticons.cs - Static class with Christian-themed emoticon string constants
  - LatinCross, OrthodoxCross, OutlinedCross, MalteseCross, Dagger, PrayingHands, Church, Bible, Dove, Angel, Star, Fish, Candle, Heart

- Account/
  - User.cs - Full authentication model documentation
  - Profile.cs - Profile data model documentation
  - Credentials.cs - Security credential documentation
  - IPInfo.cs - IP tracking documentation

- Billing/
  - All billing models documented (CreditCard, Product, etc.)
  - Transaction models (Purchase, Sale)
  - Payment models (PaymentCredentials - PaymentType moved to Enums)
  - Address and contact models
  - PaymentIntent.cs - Standardized payment intent for Stripe, PayPal, Braintree, Checkout.com
  - PaymentCustomer.cs - Customer profile stored with payment providers
  - PaymentMethodToken.cs - Tokenized payment method storage
  - Refund.cs - Standardized refund across payment providers
  - Dispute.cs - Chargeback/dispute handling with DisputeStatus enum
  - WebhookEvent.cs - Webhook event handling for payment notifications
  - PaymentAuditLog.cs - PCI-DSS compliant audit trail with sensitive data masking
  - StatusHistoryEntry.cs - Status change tracking for audit purposes
  - SubscriptionUsage.cs - Usage tracking with limits, remaining, percentage calculations
  - CreditCard.cs - Enhanced with Luhn validation, card type detection, masking
  - Subscription.cs - Pause/resume/cancel lifecycle, usage tracking, status history
  - Purchase.cs - Order status and status history audit trail

NOTE: as of the migration below, the Concepts namespace (Love/Agape/Situation/Circumstances and the
Reality subtree) no longer lives in AnointedAutomation.Objects. It moved into the new
AnointedAutomation.Concepts package (see section 4a). AnointedAutomation.Objects was bumped to
2.0.0 as a breaking change reflecting the loss of that namespace.

### 4a. AnointedAutomation.Concepts (new package, moved out of Objects)
Namespace `AnointedAutomation.Concepts`. Two areas live here: the original concept modeling classes
(Love, the Reality subtree) migrated from Objects, and a brand new epistemics engine.

- Concept modeling (Love, Reality): Love modeled as a principle-driven DECISION ENGINE (behavior
  tree), not hardcoded scenario lookups. You feed in a situation (arbitrary facts) and a love returns
  the fitting deed, even for situations Scripture never named. Love.cs holds the shared character (the
  sixteen 1 Corinthians 13:4-8 bool virtues/vices, sourced in God 1 John 4:8, directed Lover to Beloved
  Matthew 22:37-39, with a sacrificial dimension John 3:16/John 15:13), with helpers IsPerfect(),
  Completeness() (0-17), Bears()/Believes()/Hopes()/Endures() (1 Cor 13:7), GreaterLove(), Abides(),
  Describe(), and an Agape() factory; the public `LoveAction Decide(Situation)` lazily builds and walks
  the love's behavior tree via the protected abstract `BuildBehavior()`. Agape.cs is the concrete
  perfect love (a Selector repertoire covering forgiveness, feeding an enemy, the six works of mercy
  from Matthew 25, meeting need, mourning, rejoicing, and a fallback of patience and kindness).
  SelfSeekingLove.cs is the priest/Levite (a single pass-by deed, Luke 10:31-32). SacrificialLove.cs
  extends Agape with one prepended branch (friendsLifeAtStake, John 15:13). Situation.cs,
  Circumstance.cs, and Circumstances.cs supply the decision input as first-class concepts rather than
  strings, and BehaviorNode/BehaviorResult/Selector/Sequence/Condition/Deed provide the underlying
  behavior tree machinery. LoveAction.cs is the deed a love performs (acts, Deed, Virtue, Reference,
  Exhortation). The Reality/ subfolder is the divine grounding and reality engine: Reality.cs (the one
  thing agents address, `Witness(Act[, Grounding])`), HeavenlyTablets.cs (the one record where state
  and truth are the same), Concept.cs (root of the concept model), MoralConcept.cs plus
  Reality/Morals/Virtues.cs and Reality/Morals/Vices.cs (the concrete moral concepts and their
  gravity), Gravity.cs, Act.cs, DivineAttribute.cs, Justice.cs/Mercy.cs/Order.cs, LoveFacet.cs,
  DivineCharacter.cs, Resolution.cs, Grounding.cs, and Word.cs. Design spec for this subtree:
  `docs/design/specs/2026-06-11-divine-grounding-reality-engine-design.md`.

- Sin catalog (additive): Reality/Morals/Sins/ holds SinId.cs and SinCatalog.Entries.cs (the coded catalog: one
  SinId member and one SinEntry per checklist number, 668 entries; #367 omitted placeholder, the duplicate #566 is
  id 668), SinEntry.cs, SinCategory.cs (no Unknown; real categories from 1), BibleBook.cs (Unknown=0, 66 books), ScriptureReference.cs
  (validated book/chapter/verse range), SinCatalog.cs (Get/TryGet/InCategory/UnderCoarseSin/Concept, precomputed
  dictionaries), CataloguedSin.cs (a MoralConcept over an entry, grounded in the engine vice for its coarse
  Enums.Sin or its category, so Reality.Witness reads it), and ISinConsequence.cs/SinConsequenceRegistry.cs (the
  consumer extension point for effects). Concepts now references AnointedAutomation.Enums.

- Trinity, love triad, and creed (1.1.0, additive): Trinity/ holds Trinity.cs (one God in three
  Persons over a Reality: one shared Essence instance = Reality.Character, three distinct
  DivinePerson objects, undivided Work/WorkThrough), DivinePerson.cs, DivinePersonKind.cs (Unknown=0),
  Presence.cs (the Holy Spirit factor, binary), and Triad.cs (Exists/Product: any factor zero collapses
  the whole). Reality.Witness(Act, Grounding, Presence) and Word.Speak(Act, Grounding, Presence) apply
  the triadic rule coherence = Grounding.Life x harmonized conformity x Presence.Empowerment; the old
  overloads are unchanged. Bond.cs (love itself; commitment gate Commit/Withdraw with auditable
  History of CommitmentEvent; condilectio via ShareWith/IsCondilectio), Commitment.cs (Unknown=0,
  Committed, Withdrawn), CommitmentEvent.cs. Love gained Bond, Commit, Withdraw, Exists, IsComplete;
  constructed loves are committed so legacy Decide behavior is unchanged, and a withdrawn love's Decide
  returns acts=false (Deuteronomy 30:19; Joshua 24:15). Creed.cs reflects over the assembly to emit
  a deterministic JSON constitution (ToJson) and a Markdown system prompt (ToPrompt). Tests:
  TriadTests, TrinityTests, PresenceTests, BondTests, LoveTriadTests, CreedTests.

- Epistemics engine (Epistemics/, namespace `AnointedAutomation.Concepts.Epistemics`): a new engine
  that checks the consistency of theological claims against foundational claims over a shared
  proposition vocabulary. Proposition.cs defines the shared vocabulary; Testability.cs and
  LawDomain.cs classify claims (law domains are bounded so intra-universe laws never settle origin
  claims); FoundationalClaim.cs and TheologicalClaim.cs assert and deny propositions whose standing is a
  three-valued Reasoning.TruthValue (Unknown/True/False); EpistemicStatus.cs and Verdict.cs carry the four-state verdict
  (Consistent/Contradicts/Unfalsifiable/Undetermined); DerivationStep.cs and Examination.cs record how
  a verdict was reached; Tension.cs holds contradicting claims as data rather than resolving them
  away; EpistemicLedger.cs is the aggregate that admits claims, runs examinations, and tracks tensions,
  treating zero-weight foundations as never counting toward support. Full design spec:
  `docs/design/specs/2026-07-02-theology-engine-design.md`.
  Proof.cs, ProofSymbol.cs and ProofStep.cs model theoretical proofs stated in first-order logic:
  a Proof carries a plain statement, a symbolic form (∃, ∀, ∧, ⊕, → live in the BMP, so a plain
  UTF-16 string holds them, no UTF-32 needed), a glossary of ProofSymbol tokens, and an ordered
  ProofStep derivation. TheoreticalProofs.cs is a static factory exposing the canonical Agnosticism
  and Christianity proofs.

- Reasoning contracts (Reasoning/, namespace `AnointedAutomation.Concepts.Reasoning`): the shared
  contracts for the dual reasoning engine (deterministic canon engine, language engine, hybrid) behind
  one API, PR 1 of 5 (contracts only). TruthValue.cs (Unknown = 0, True, False; Unknown is "not
  established", never False) and TruthLogic.cs (strong Kleene And/Or/Not); RelationKind.cs (None = 0,
  rejected by Assertion); ConceptRef.cs (Kind + Id, Of(SinId) and Of(MoralConcept), a CataloguedSin
  resolving to its SinId); Assertion.cs, Query.cs, Justification.cs (ProofStep, ScriptureReference,
  facts used, explanation); AnswerSource.cs and VerificationStatus.cs (Unknown = 0, rejected by
  Provenance); Provenance.cs (every field required); Answer.cs; IReasoner.cs (Ask(Query,
  CancellationToken)). ReasoningGuard.cs holds the shared argument checks. Proposition.Standing and
  Examination.Standing are TruthValue (changed in place from bool?). Owner rule: only the owner can
  approve promoting an AI interpretation into a permanent fact. Tests: Reasoning/TruthLogicTests,
  Reasoning/ReasoningContractsTests.
- CanonReasoner (Reasoning/Canon/, same namespace, dual engine PR 2 of 5): the deterministic, offline
  IReasoner. CanonFacts.cs is the immutable index built once (Lazy) from SinCatalog.All, every public
  MoralConcept type (reflection, ordered by name) and Reality.Revealed() facets: GroundedIn per reference,
  Upholds/Violates per facet with the mirrored Commands/Forbids, InCategory and IsInstanceOf (coarse sin,
  grounding vice) per sin; lookups by (subject, relation) or whole assertion, With() extends a copy.
  CanonFact.cs (Assertion + at least one ScriptureReference). ICanonRule.cs; rules SinViolatesRule,
  CategoryRule, IsInstanceOfRule, WitnessRule (oracle thresholds), FactRule (stated or contrary);
  CanonRules.Default; CanonProof.cs (internal step helpers). CanonReasoner.cs runs every rule for the
  relation, first established answer wins, opposite answers throw; Unknown when nothing settles it.
  ScriptureReference.Parse reads display references. Tests: Reasoning/Canon/ (rule unit tests, FsCheck
  properties, oracle regression).
- LanguageReasoner (Reasoning/Language/, namespace AnointedAutomation.Concepts.Reasoning.Language, dual engine
  PR 3 of 5): ILanguageModel.cs (the port; no AI SDK or network in core). PromptComposer.cs (deterministic prompt
  plus the exact ReplySchema; ComposedPrompt id = SHA-256 prefix). InterpretationParser.cs (strict JSON into
  Interpretation.cs; any invalid reply throws InterpretationRejected.cs, logged at Error in full via
  Microsoft.Extensions.Logging.Abstractions). LanguageReasoner.cs (one model call per query; Answer is Language and
  Unverified; Interpret returns LanguageResult.cs with the whole interpretation). Reasoning/AnswerDescriber.cs renders
  any Answer as text. Tests: Reasoning/Language/ with golden prompt, reply-fixture and Describe files in Golden/.
- HybridReasoner (Reasoning/Hybrid/, namespace AnointedAutomation.Concepts.Reasoning.Hybrid, dual engine PR 4 of 5):
  HybridReasoner.cs (canon first; on Unknown one LanguageReasoner.Interpret call per owner, query and model, recorded
  in the ledger; HybridResult.cs carries the entry). InterpretationLedger.cs (owner-scoped cache, Get, paged List,
  Promote and Reject through an injected IPromotionPolicy.cs with no default) over IInterpretationStore.cs
  (InMemoryInterpretationStore.cs in core). InterpretationEntry.cs, InterpretationStatus.cs, OwnerScope.cs,
  Reviewer.cs, InterpretationPage.cs, FactPromotion.cs (never mutates CanonFacts). Tests: Reasoning/Hybrid/.

### 4a-1. AnointedAutomation.Concepts.Anthropic (optional adapter, dual engine PR 5 of 5)
Namespace `AnointedAutomation.Concepts.Anthropic`; references Concepts and the Anthropic SDK (Concepts never references it).
AnthropicLanguageModel.cs (ILanguageModel over AnthropicClient with SDK retries off; one structured-output call per
question; own bounded backoff for 429/408/5xx/overload/IO/timeout; 401/403 to AnthropicConfigurationException.cs;
exhausted retries to AnthropicTransientException.cs; other API errors logged and rethrown). AnthropicLanguageModelOptions.cs
(all settings required, Problems()/ThrowIfInvalid()). ReplySchemaFormat.cs (internal; core ReplySchema minus
minimum/maximum/minLength). RetryAfterRecorder.cs (internal DelegatingHandler capturing Retry-After per call). AnthropicServiceCollectionExtensions.cs
(AddAnthropicLanguageModel(section), named HttpClient). Tests: AnointedAutomation.Concepts.Anthropic.Tests (RecordedHandler replays recorded HTTP responses, no
network; ArchitectureTests check the dependency direction).

### 4b. AnointedAutomation.Mathematics (new package, references Concepts)
Namespace `AnointedAutomation.Mathematics`. Curated catalogs that feed the epistemics engine with
foundational and theological claims drawn from mathematics and physics. UniversalPropositions.cs
defines the shared proposition vocabulary (79 propositions) used by the catalogs below.
UniversalLaws.cs (with partial files UniversalLaws.Physics.cs and UniversalLaws.Chemistry.cs)
catalogs 64 laws with Law status: the logic laws (NonContradiction, Identity, ExcludedMiddle,
domain Unrestricted) plus the established laws of physics, chemistry, and astronomy (Newton's
laws, Kepler's laws, thermodynamics, conservation laws, Maxwell-era electromagnetism, optics
and radiation, fluids, gas laws, and the classical chemistry laws), weighted 0.99 for exact
laws and 0.9 for idealizations (Ohm, Hooke, ideal-gas family). PhysicalTheories.cs catalogs
12 well-established theories with Theory status at weight 0.95 (special and general relativity,
quantum mechanics, atomic theory, Big Bang cosmology, evolution by natural selection, germ
theory, cell theory, plate tectonics, kinetic theory of gases, MassEnergyEquivalence,
InvariantLightSpeed). Conjectures.cs catalogs open mathematical conjectures
with Conjecture status and zero weight (Collatz, Goldbach, RiemannHypothesis), so an unproven
conjecture never counts as support for or against a claim. See
`docs/design/specs/2026-07-02-theology-engine-design.md` for how these catalogs plug into the
epistemics engine.

### 5. AnointedAutomation.Logging
- LogMessage.cs
  - Class and method documentation
  - Event system documentation
  - Message type descriptions
  - Static factory methods documented

- MessageType.cs
  - Enum value documentation
  - Usage guidelines

### 6. AnointedAutomation.Repository.Mongo
- MongoDocument.cs
  - Base class for MongoDB documents (`MongoDocument`)
  - Timestamp hierarchy (`CreatedMongoDocument`, `TimestampedMongoDocument`, `HistoriedMongoDocument`, `UpdateStamp`)
- TimestampedMongoRepositories.cs
  - Stamping repositories (`CreatedMongoRepository`, `TimestampedMongoRepository`, `HistoriedMongoRepository`)
  - BSON attribute configuration for ObjectId handling
  - Full XML documentation

- MongoHelper.cs
  - Full CRUD operation documentation
  - Connection management
  - Event system
  - Error handling

- IMongoHelper.cs
  - Interface method documentation
  - Parameter descriptions
  - Return value documentation

- MongoHelperFactory.cs
  - Factory pattern documentation
  - Caching mechanism details

### 7. Google Integration
- GoogleObjects.cs
  - Authentication object documentation
  - Integration details
  - Token management

- GoogleTokenInfo.cs
  - Token data documentation
  - Security considerations

- UserProfile.cs
  - Profile data documentation
  - Integration details

## Testing Documentation

### Unit Tests
All test classes include documentation explaining:
- Test purpose
- Setup requirements
- Expected outcomes
- Test environment requirements

Key test files:
- GarbageCollectionTests.cs
- APIKeyAttributeTests.cs
- MongoHelperTests.cs
- LogMessageTests.cs

## Documentation Standards Applied

1. Class Documentation
   ```csharp
   /// <summary>
   /// Describes the purpose and functionality of the class
   /// </summary>
   ```

2. Method Documentation
   ```csharp
   /// <summary>
   /// Describes what the method does
   /// </summary>
   /// <param name="paramName">Parameter description</param>
   /// <returns>Description of return value</returns>
   /// <exception cref="ExceptionType">When exception is thrown</exception>
   ```

3. Property Documentation
   ```csharp
   /// <summary>
   /// Describes what the property represents
   /// </summary>
   ```

4. Interface Documentation
   ```csharp
   /// <summary>
   /// Describes the contract and purpose of the interface
   /// </summary>
   ```

5. Event Documentation
   ```csharp
   /// <summary>
   /// Describes when the event is triggered and its purpose
   /// </summary>
   ```

## Verification Status
- ✅ All classes documented
- ✅ All public methods documented
- ✅ All interfaces documented
- ✅ All properties documented
- ✅ All events documented
- ✅ All test classes documented
---

**[← Back to Project Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)**

## File Header Standard (2026-09-30)

Every source file starts with `// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️` (or the language's comment syntax). Every README has "Jesus is King ✝️" under its title. Enforced via `.editorconfig` `file_header_template` (IDE0073, suggestion) and documented in CONTRIBUTING.md.
