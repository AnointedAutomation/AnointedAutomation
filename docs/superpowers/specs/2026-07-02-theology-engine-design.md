# Theology Engine (Epistemics) Design

Date: 2026-07-02
Status: Approved approach (Approach A: epistemic ledger engine)

## Purpose

A consistency-mapping engine that examines theological claims against the best-established
regularities of the universe and against each other. It does not decide theology. It records,
for every claim: what supports it, what contradicts it, what can never be tested from inside
the universe, and where traditions collide. Contradictions and undecidability are output data,
never errors. The engine never crashes on a contradiction; that is its central requirement.

## Epistemological commitments

These came out of the design conversation and are binding on the implementation:

1. **No layer of absolute truth.** Even scientific laws begin as faith: an asserted claim one
   is willing to have falsified. So there is no ontologically special "law" type that is
   simply true. There are only claims, differing in how much falsification they have survived.
2. **Three-valued truth.** Every standing is a `bool?`: `true` (asserted, so far unfalsified),
   `false` (falsified), `null` (unknown, untested, or unknowable from inside the universe).
   `null` is the honest state for most of theology. It is never defaulted, coalesced, or
   guessed away (per the global no-fallbacks rule).
3. **Domain boundaries enforce neutrality.** A regularity established inside the universe
   (conservation of energy) can never settle a claim about the origin of or outside the
   universe. "A creator exists", "no creator exists", and "matter is eternal and uncaused"
   must all receive the same verdict (`Unfalsifiable`), symmetrically. The engine must not
   favor or disfavor God; neutrality falls out of the domain mechanics, not hand-tuning.
4. **Claims interact only through a shared proposition vocabulary.** The engine does not
   parse natural language. Claims declare which propositions they assert and deny; checking
   is set logic over propositions.
5. **Self-honesty.** Asked to examine "the scientific method yields truth", the engine
   returns `null` standing and `Unfalsifiable`: the method cannot prove the method.

## Location

- Code: `AnointedAutomation.Objects/Concepts/Epistemics/` (sibling to `Concepts/Reality/`,
  deliberately separate from the God-grounded `Reality` model so neutrality is structural).
- Tests: `AnointedAutomation.Objects.Tests/Epistemics/`.
- House style: explicit types (no `var`), `.Equals()` for string comparison, fail-fast null
  checks, XML docs.

## Core types

### Proposition

The shared vocabulary. Immutable value type with:

- `Name` (e.g., `CreatedUniverse`, `EternalMatter`, `Creator`)
- `Description`
- `Testability` (enum): `EmpiricallyTestable` or `BeyondObservation` (origin-of-universe and
  outside-the-universe subject matter)
- `Standing` (`bool?`): current three-valued standing; `null` until something establishes it

### LawDomain (enum)

`IntraUniverse` or `Unrestricted`. An `IntraUniverse` foundational claim is skipped when
examining any `BeyondObservation` proposition, and the skip is recorded in the derivation so
neutrality is auditable.

### FoundationalClaim

The measuring stick, formerly conceived as "UniversalLaw" and renamed to reflect that even
laws are held by faith. Immutable, hand-curated small set (non-contradiction, causality,
conservation of energy, entropy). Fields:

- `Name`, `Statement`
- `Domain` (`LawDomain`)
- `Asserts` / `Denies` (propositions)
- `Falsifiable` (bool): always true for empirical claims; part of what makes it scientific
- `SurvivedFalsificationWeight` (double, 0-1): how much testing it has survived; this weight,
  not type-level specialness, is why it functions as bedrock

### TheologicalClaim

The theory layer. Plain data object:

- `Statement`
- `Source` (tradition plus citation, e.g., "Genesis 1:1", "materialist cosmology")
- `Confidence` (double, 0-1)
- `Asserts` / `Denies` (propositions)

Constructed in code for v1; JSON loading is a later bolt-on.

### Verdict (enum)

- `Consistent`: nothing falsified it; provisional, `true`-leaning
- `Contradicts`: collides with the current unfalsified set
- `Unfalsifiable`: can never be tested from inside the universe (`null` flavor 1)
- `Undetermined`: testable in principle, insufficient evidence (`null` flavor 2)

### Examination

Result of examining one claim:

- `Claim`
- `Verdict`
- `Standing` (`bool?`): `Consistent` maps toward `true`, `Contradicts` to `false`,
  `Unfalsifiable`/`Undetermined` to `null`
- `Confidence` (double): minimum confidence along the derivation chain (a conclusion is only
  as strong as its weakest premise)
- `Derivation`: ordered steps, each naming the foundational claim or peer claim and the
  proposition that produced the step, including recorded domain skips

### Tension

First-class record of a contradiction between two theological claims: both claims, the shared
proposition they disagree on, both sources. Neither claim is deleted or rejected.

### EpistemicLedger

The engine. Holds foundational claims and admitted theological claims. API:

- `Examine(TheologicalClaim)` returns `Examination` (pure; does not store)
- `Admit(TheologicalClaim)` examines, stores the claim, records any `Tension`s with already
  admitted claims, returns the `Examination`
- `Tensions`: all known tensions
- `ClaimsAbout(Proposition)`, `ClaimsFrom(source)`: query helpers

## Examine algorithm

Order encodes the epistemology:

1. **Foundational check.** For each foundational claim whose `Domain` applies to the
   proposition under test: assert/deny collision yields `Contradicts`, citing it. Domain
   mismatches are skipped and the skip recorded.
2. **Falsifiability check.** If the claim's decisive propositions are all `BeyondObservation`,
   verdict is `Unfalsifiable`. This is a statement about testability, not truth, and it is
   symmetric across theism and atheism.
3. **Peer check.** Against admitted claims, an assert/deny collision creates a `Tension`.
   Tensions do not change the verdict; both claims stand, source-tagged.
4. **Otherwise.** `Consistent` if foundational claims positively support it; else
   `Undetermined`.

## Error handling

Contradiction is never an exception. Exceptions are reserved for genuine misuse: null
arguments, a claim referencing no propositions, duplicate foundational claim names. No
fallback values anywhere; `null` standings propagate as `null`.

## Testing

Unit tests written alongside the code (house rule: code without tests is incomplete):

- Foundational contradiction: a claim denying causality inside the universe yields
  `Contradicts` with the foundational claim named in the derivation.
- Neutrality property: "creator exists", "no creator", "eternal uncaused matter" all yield
  `Unfalsifiable`, verified symmetric.
- Domain boundary: conservation of energy never settles an origin claim; derivation shows the
  recorded skip.
- Tension detection: two admitted contradicting claims produce one `Tension`; both remain
  queryable via `ClaimsAbout`.
- Confidence propagation: verdict confidence equals the weakest premise.
- `bool?` honesty: `null` standings propagate; nothing defaults them.
- Self-reference: "the scientific method yields truth" yields `Unfalsifiable`, `null`.
- Integration-style worked example from the design conversation: conservation of energy,
  eternal-matter claim, cosmological-argument claim, atheism claim; expected verdicts and the
  eternal-matter vs. cosmological-argument `Tension`.

## Out of scope for v1

Natural-language parsing, JSON claim loading, persistence (Mongo repository), any graph
store. All can bolt on later without changing the core types.
