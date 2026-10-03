# AnointedAutomation.Concepts

Jesus is King ✝️

Models abstract, Biblically grounded concepts as first class .NET types: love as a behavior tree that decides what to do in a situation, a moral "reality" that reads acts against facets of God's character, and an epistemics engine that checks claims for consistency against foundational laws. It is for developers building faith oriented apps, games, moderation tools, or teaching material who want these ideas as testable code instead of prose.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Concepts.svg)](https://www.nuget.org/packages/AnointedAutomation.Concepts) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Concepts.svg)](https://www.nuget.org/packages/AnointedAutomation.Concepts)

## Installation

```bash
dotnet add package AnointedAutomation.Concepts
```

- Target framework: `net10.0`
- Dependencies: none

## Quick start

Love decides what to do by walking its behavior tree over the circumstances of a situation (the Good Samaritan, Luke 10:33-35):

```csharp
using System;
using AnointedAutomation.Concepts;

Love agape = Love.Agape();

Situation situation = new Situation("A stranger lies beaten by the road.")
    .With(new Need())
    .With(new Means());

LoveAction action = agape.Decide(situation);

Console.WriteLine(action.acts);        // True
Console.WriteLine(action.Deed);        // what love does, e.g. meet the need
Console.WriteLine(action.Reference);   // the Scripture behind it (Luke 10)
Console.WriteLine(action.Exhortation); // "Go and do likewise."
```

## Love and behavior trees

Namespace: `AnointedAutomation.Concepts`

| Type | Description |
|---|---|
| `Love` | Abstract. The 1 Corinthians 13:4-8 attributes as properties (`patient`, `kind`, `envious`, `boastful`, `selfSeeking`, `sacrificial`, `neverFails`, and more), plus `Lover`, `Beloved`, `Source`, `Scripture`, `Reference`. Methods: `Decide(Situation)`, `IsPerfect()`, `Completeness()` (0 to `MaxCompleteness`, which is 17), `Bears()`, `Believes()`, `Hopes()`, `Endures()`, `GreaterLove()`, `Abides()`, `Describe()`, and the factory `Love.Agape(string lover = null, string beloved = null)`. |
| `Agape` | Perfect love: every virtue present, every vice absent. |
| `SacrificialLove` | Derives from `Agape`. |
| `SelfSeekingLove` | The absence of love: passes by on the other side (Luke 10:31-32). |
| `Situation` | A description plus a list of `Circumstance`s. `With(Circumstance)` and `Has(Circumstance)`. |
| `Circumstance` | Base class for circumstances, compared by `Name`. Built in: `Hunger`, `Thirst`, `Estrangement`, `Nakedness`, `Sickness`, `Imprisonment`, `Need`, `Means`, `Grievance`, `Enmity`, `Grief`, `Gladness`, `MortalPeril`. |
| `LoveAction` | The result of a decision: `acts`, `Deed`, `Virtue`, `Reference`, `Exhortation`. |
| `BehaviorNode` | Abstract node with `BehaviorResult Tick(Situation)`. |
| `Selector`, `Sequence` | Composite nodes. `Selector` returns the first child that succeeds; `Sequence` succeeds only if every child succeeds, in order. |
| `Condition` | Leaf that tests the situation. `Condition.For(Circumstance)`, `And`, `Or`, `Not`. |
| `Deed` | Leaf that always succeeds with a `LoveAction`. |
| `BehaviorResult` | `succeeded` and `Action`, with `Succeed(LoveAction)` and `Fail()` factories. |

To define your own kind of love, derive from `Love` and override `protected abstract BehaviorNode BuildBehavior()`:

```csharp
using AnointedAutomation.Concepts;

public class Encouragement : Love
{
    public Encouragement() : base("me", "a friend")
    {
        kind = true;
    }

    protected override BehaviorNode BuildBehavior()
    {
        return new Selector(
            new Sequence(
                Condition.For(new Grief()),
                new Deed(new LoveAction(true, "Sit with them and listen.", "compassion", "Romans 12:15", null))),
            new Deed(new LoveAction(true, "Send a word of encouragement.", "kindness", "1 Thessalonians 5:11", null)));
    }
}
```

## Reality: acts read against God's character

Namespace: `AnointedAutomation.Concepts` (source folder `Reality/`)

| Type | Description |
|---|---|
| `Reality` | Witnesses acts and records them. `Witness(Act)`, `Witness(Act, Grounding)`, `Tablets`, and `Reality.Revealed()`, a reality over Love, Justice, Mercy and Faithfulness. |
| `DivineCharacter` | Harmonizes a set of facets into one `Resolution` via `Harmonize(Act)`. |
| `DivineAttribute` | Abstract facet with `Name` and `Read(Act)`. Built in: `LoveFacet`, `Justice`, `Mercy`, `Faithfulness`. |
| `Act` | A described deed made of one or more `MoralConcept`s. `InContext(params Circumstance[])`, `InContextOf(Circumstance)`. |
| `MoralConcept` | Abstract base for virtues, vices and mysteries: `Name`, `Scripture`, `Gravity`, `Restoration`, `Upholds(...)`, `Violates(...)`. |
| `SacredMystery` | Abstract `MoralConcept` for redemptive acts and sacraments; upholds every facet. |
| `Gravity` | Enum: `None`, `Minor`, `Serious`, `Grave`, `Capital`. |
| `Resolution` | What reality gives back: `Coherence`, `Disorder`, `Restoration`, `Readings`, `Reading(string facet)`. |
| `Grounding` | What the actor stands on: `Grounding.InGod()`, `Grounding.InIdol(string)`, `Grounding.Divided()`. |
| `HeavenlyTablets` | The record of witnessed resolutions: `Record`, `History()`, `Coherence()`. |
| `Word` | The medium between an agent and reality: `new Word(reality).Speak(Act, Grounding)`. |
| `Concept` | Abstract base with `Name`, shared by `Circumstance` and `MoralConcept`. |

The package ships 136 concrete `MoralConcept`s, for example `Kindness`, `Forgiveness`, `Atonement`, `Pardon`, `Humility`, `Theft`, `Murder`, `Pride`, `Idolatry`, and `SacredMystery` types such as `Incarnation`, `Resurrection` and `Eucharist`.

```csharp
using System;
using AnointedAutomation.Concepts;

Reality reality = Reality.Revealed();

Resolution cross = reality.Witness(new Act("the cross", new Atonement(), new Pardon()));
Resolution theft = reality.Witness(new Act("a theft", new Theft()));

Console.WriteLine(cross.Coherence);
Console.WriteLine(theft.Disorder);
Console.WriteLine(reality.Tablets.Coherence()); // drops as disorder is recorded
```

## Trinity, love triad, and the AI creed (1.1.0)

Namespace: `AnointedAutomation.Concepts` (source folder `Trinity/` plus `Bond.cs`, `Commitment.cs`, `Creed.cs`). All additive; nothing existing was removed or changed in behavior.

| Type | Description |
|---|---|
| `Trinity` | One God in three Persons over a `Reality`. `Father`, `Son`, `HolySpirit`, `Persons`, one `Essence` (the reality's `DivineCharacter`), `IsOneEssence()`, `ArePersonsDistinct()`, `Work(Act)`, `WorkThrough(DivinePersonKind, Act)` (undivided: same reading for every Person), `Trinity.Revealed()`. |
| `DivinePerson` | A Person: `Kind`, `Name`, `Origin` (Nicene relation of origin), `Appropriation`, `Scripture`, `Essence`, `Attributes`, `IsSameEssenceAs`, `IsSamePersonAs`. |
| `DivinePersonKind` | Enum: `Unknown` (0), `Father`, `Son`, `HolySpirit`. |
| `Presence` | The Holy Spirit who applies and completes a work (John 14:16-17, 26): `Presence.HolySpirit()`, `Presence.Absent()`, `Empowerment`, `Complete(Resolution)`. |
| `Triad` | The multiplicative rule: `Exists(a, b, c)` (all non-null) and `Product(a, b, c)` (any factor 0 collapses the whole). |
| `Bond` | Love itself, the third member of lover x beloved x love (Augustine, De Trinitate VIII-IX). Carries the commitment gate: `Commit(reason)`, `Withdraw(reason)`, `State`, `IsCommitted`, `Gate` (1 or 0), auditable `History`, and condilectio via `ShareWith(third)` / `IsCondilectio(lover, beloved)` (Richard of St. Victor, De Trinitate III). |
| `Commitment` / `CommitmentEvent` | Enum `Unknown` (0), `Committed`, `Withdrawn`; one recorded choice with `Sequence`, `State`, `Reason`. |
| `Creed` | A constitution generated from the types: `Creed.ToJson()` (deterministic), `Creed.ToPrompt()` (Markdown system prompt), `MoralConcepts()`, `Attributes()`, `KindOf(concept)`, `DecisionRules`. |

Additions to existing types (marked "added in 1.1.0" in their XML docs): `Love.Bond`, `Love.Commit(string)`, `Love.Withdraw(string)`, `Love.Exists()`, `Love.IsComplete()`, `Love.ConstructedCommitment`; `Reality.Character`, `Reality.Witness(Act, Grounding, Presence)`; `Word.Speak(Act, Grounding, Presence)`; `Grounding.Life`, `Grounding.Groundless()`; `DivineCharacter.Facets`.

Rules:

- **Triadic coherence.** `Witness(Act, Grounding, Presence)` sets coherence to `Grounding.Life x conformity to the Word x Presence.Empowerment`. Conformity is the act's harmonized reading against God's character. Groundless, wholly against God's character, or without the Spirit: coherence 0. The two-argument `Witness` is unchanged.
- **Love triad.** `Love.Exists()` requires a named lover, a named beloved, and a committed bond. `Love.IsComplete()` is perfect agape that exists and is shared toward a third. `IsPerfect()` is unchanged (every virtue present).
- **Commitment gate.** Every love is committed when constructed, so existing code behaves exactly as before. After `Withdraw`, `Decide` returns a not-acting `LoveAction` citing Deuteronomy 30:19 and Joshua 24:15, however kind the love's properties are.

```csharp
using AnointedAutomation.Concepts;

Trinity god = Trinity.Revealed();
bool orthodox = god.IsOneEssence() && god.ArePersonsDistinct(); // true

Resolution noSpirit = Reality.Revealed().Witness(new Act("a mercy", new Compassion()), Grounding.InGod(), Presence.Absent());
// noSpirit.Coherence == 0.0

Love love = Love.Agape("husband", "wife");
love.Bond.ShareWith("child");             // love.IsComplete() == true
love.Withdraw("hardened heart");          // love.Decide(situation).acts == false

string json = Creed.ToJson();             // give to an AI as its base belief system
string prompt = Creed.ToPrompt();
```

Research note: the multiplicative triad (any factor zero instantiates nothing) and the binary commitment gate were suggested by the structure of Coty Austin Trout's "CAT'S Theory: Triadic Invariant Collapse". The theology rests only on Scripture, the Nicene Creed, Augustine, and Richard of St. Victor.

## Epistemics engine

Namespace: `AnointedAutomation.Concepts.Epistemics`

The ledger maps the consistency of claims without deciding theology. Contradiction and undecidability are returned as data, never thrown.

| Type | Description |
|---|---|
| `Proposition` | A named entry in the shared vocabulary, with `Testability` and a three valued `bool? Standing`. Equal by name. |
| `FoundationalClaim` | A measuring stick claim: `Name`, `Statement`, `Domain`, `Asserts`, `Denies`, `Falsifiable`, `SurvivedFalsificationWeight` (0.0 to 1.0), `Status`. |
| `TheologicalClaim` | A claim from a tradition: `Statement`, `Source`, `Confidence`, `Asserts`, `Denies`. |
| `EpistemicLedger` | The engine. `Examine(claim)` (pure), `Admit(claim)` (examines, stores, records tensions), `Tensions`, `ClaimsAbout(Proposition)`, `ClaimsFrom(string source)`. |
| `Examination` | `Claim`, `Verdict`, `Standing`, `Confidence`, `Derivation`. |
| `DerivationStep` | One step of reasoning: `Authority`, `PropositionName`, `Outcome`. |
| `Tension` | Two admitted claims that disagree on a `Proposition`: `First`, `Second`, `Proposition`. |
| `Verdict` | `Consistent`, `Contradicts`, `Unfalsifiable`, `Undetermined`. |
| `EpistemicStatus` | `Law`, `Theory`, `Conjecture`. |
| `LawDomain` | `IntraUniverse`, `Unrestricted`. Intra universe laws never rule on propositions beyond observation. |
| `Testability` | `EmpiricallyTestable`, `BeyondObservation`. |
| `Proof`, `ProofStep`, `ProofSymbol` | A symbolic proof with glossary and numbered steps. |
| `TheoreticalProofs` | Built in proofs: `Agnosticism()` and `Christianity()`. |

```csharp
using System;
using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;

Proposition created = new Proposition(
    "CreatedUniverse", "The universe was created.", Testability.BeyondObservation);
Proposition energy = new Proposition(
    "EnergyConserved", "Energy is neither created nor destroyed.", Testability.EmpiricallyTestable, true);

FoundationalClaim conservation = new FoundationalClaim(
    "ConservationOfEnergy", "Within a closed system, energy is conserved.",
    LawDomain.IntraUniverse, new List<Proposition> { energy }, new List<Proposition>(), 0.99);

EpistemicLedger ledger = new EpistemicLedger(new List<FoundationalClaim> { conservation });

Examination genesis = ledger.Admit(new TheologicalClaim(
    "In the beginning God created the heavens and the earth.", "Genesis 1:1", 0.9,
    new List<Proposition> { created }, new List<Proposition>()));

ledger.Admit(new TheologicalClaim(
    "The universe was not created.", "materialist cosmology", 0.9,
    new List<Proposition>(), new List<Proposition> { created }));

Console.WriteLine(genesis.Verdict);       // Unfalsifiable
Console.WriteLine(genesis.Standing == null); // True: honestly unknown, never guessed
Console.WriteLine(ledger.Tensions.Count); // 1: the two claims disagree, and both stand
```

For a ready made set of foundational claims (64 laws, 12 theories and 3 conjectures), see [AnointedAutomation.Mathematics](https://www.nuget.org/packages/AnointedAutomation.Mathematics).

## Related packages

- [AnointedAutomation.Mathematics](https://www.nuget.org/packages/AnointedAutomation.Mathematics): a curated catalog of laws, theories and conjectures built on this engine.
- [AnointedAutomation.Enums](https://www.nuget.org/packages/AnointedAutomation.Enums): shared enums, including the `Sin` moderation categories.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).
Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields.

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Pets:** [https://www.animaldander.com](https://www.animaldander.com)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue: [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues)
