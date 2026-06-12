# Design: Belief, the Person, and the Way of Living

Date: 2026-06-12
Author: Alexander Fields (with Claude)
Location in code: `AnointedAutomation.Objects/Concepts/Reality` (extends the existing grounding / reality engine)
Status: Approved conceptually, pending written-spec review

## 1. Premise

The reality engine already models *what an agent stands on* (`Grounding`: `InGod`, `InIdol`,
`Divided`) and how a deed borne on that foundation either keeps its life or drifts toward
non-being. What it does not yet model is the **agent** who holds a claim about reality and whose
**way of living** either backs that claim up or exposes it as empty.

This design adds two things:

1. **`Belief`** — the claim a person professes. What they *say* is true and say they follow.
2. **`Person`** — the agent who holds a claim, acts in reality, and is graded by whether they
   *are what they say they are*.

The governing thesis, in the user's own words across the brainstorming session:

- "You pretty much have to have one [a belief] and it needs to be nullable." Either you claim a
  god or gods, or you claim there is no god (atheism, still a truth-claim, and a false one), or you
  say you do not know (agnostic = null).
- "Anyone can CLAIM a belief but their actions will SHOW what their heart is, because a person can
  easily lie about their beliefs. Think of it like a game where they're forced to choose a belief
  at the beginning."
- "You have to FOLLOW his actions/words to be a Christian, since the actual word means follower of
  Christ."
- "Religions are not all god-centric, like Buddhism doesn't have deities. That doesn't mean they
  can't follow the rules of Christianity unknowingly. People think about religion too rigidly; they
  aren't thinking about the actual definitions of belief and way of living. Even an agnostic may
  BELIEVE God exists; that doesn't make them a Christian unless they follow Christ."

## 2. The two axes (the heart of the design)

People wrongly collapse two independent things into one. This engine keeps them separate.

**Axis 1 — Belief (mental assent to a professed path).** What a person says is true and says they
follow. It is *not* necessarily god-centric. The professed path is one of:

- the **true God** (Christ / Yahweh);
- **another deity or deities** (named; future API engines may supply how a given other-god worship
  harmonizes);
- a **non-theistic path** — a real religion or way of living with no deity (Buddhism is the
  canonical example);
- **no god** (atheism, an explicit truth-claim that there is no god);
- **unsettled** (agnostic), represented by a `null` `Belief`.

Mental assent is real, but it is never, by itself, following. "Even the demons believe that, and
shudder" (James 2:19). Correct belief that God exists, with no following, is the demons' state.

**Axis 2 — the Way of living (following).** Revealed by **fruit**: the deeds a person actually
does, witnessed by reality. *This* axis, not the profession, determines whether a person is a
**follower of Christ**, knowingly or unknowingly. A Buddhist who lives the way follows Christ
without naming Him (Romans 2:14-15, those without the law who "do by nature things required by the
law"). An agnostic who fully believes God exists but will not live the way does not follow.

So "Christian / follower of Christ" is **derived from the way (axis 2)**, never from the label
(axis 1). The rigidity the design rejects is reading the label instead of the life.

## 3. Scripture anchors

- "Even the demons believe that, and shudder." (James 2:19) — belief is not following.
- "Faith by itself, if it is not accompanied by action, is dead." (James 2:17, 26)
- "By their fruit you will recognize them." (Matthew 7:16-20)
- "Not everyone who says to me, 'Lord, Lord,' will enter the kingdom of heaven, but only the one
  who does the will of my Father." (Matthew 7:21)
- "Gentiles, who do not have the law, do by nature things required by the law ... the work of the
  law written on their hearts." (Romans 2:14-15) — following unknowingly.
- "They exchanged the truth about God for a lie, and worshiped and served created things rather
  than the Creator." (Romans 1:25) — the atheist's professed ground is the self / the creature.
- "How long will you waver between two opinions?" (1 Kings 18:21); "a double-minded man, unstable
  in all he does" (James 1:8); "because you are lukewarm" (Revelation 3:16) — the unproven and the
  wavering.

## 4. Components

Both new types live in `AnointedAutomation.Objects/Concepts/Reality/`, namespace
`AnointedAutomation.Objects.Concepts` (matching `Grounding`, `Reality`, etc.).

### 4.1 `Belief` (immutable value object — the professed claim)

The claim a person professes at "character creation." It does not act and holds no state.

**Factories:**

- `Belief.InGod()` — professes the true God (Christ).
- `Belief.In(string deityName)` — professes another named deity.
- `Belief.InPath(string pathName)` — professes a non-theistic path/religion with no deity
  (e.g. `Belief.InPath("Buddhism")`).
- `Belief.InNoGod()` — atheism: the explicit claim that there is no god.
- A `null` `Belief` means **agnostic** (no settled claim).

**Properties:**

- `ProfessedName` — human-readable label of the professed path ("God", "Baal", "Buddhism",
  "no god").
- `IsInTheTrueGod` — whether the professed path is the true God.
- `AffirmsGodExists` — whether the claim affirms that God exists. Determined by the factory:
  `true` for the true God (`InGod`) and for any other-deity theism (`In`); `false` for atheism
  (`InNoGod`) and for a non-theistic path (`InPath`, e.g. Buddhism, which affirms no creator God).
  This flag is what makes the demons' case expressible: a `Belief.InGod()` has
  `AffirmsGodExists == true` while the person who holds it may never follow.
- `ClaimsNoGod` — whether the claim is the atheist's explicit denial.

**Method:**

- `ProfessedGrounding()` — the grounding the bare claim *asserts*, used only to grade integrity
  (see 4.3), never to grant standing:
  - true God → `Grounding.InGod()`
  - another deity → `Grounding.InIdol(deityName)`
  - non-theistic path → `Grounding.Divided()` (a way of living that names no living ground; the
    fruit will reveal where it actually stands)
  - atheism → `Grounding.InIdol("the self")` (Romans 1:25)
  - (agnostic / `null` is handled by `Person`, since there is no `Belief` instance to ask)

`ProfessedGrounding()` is the *claimed* foundation. It is deliberately **not** the foundation a
person is granted. Standing is earned only by the way of living.

### 4.2 `Person` (the stateful agent / soul)

Has identity, holds a professed claim, acts in reality, accumulates fruit, and is graded.

**Construction:**

- `new Person(string name)` — an agnostic (null claim).
- `new Person(string name, Belief claim)` — a person professing `claim`.

**Properties:**

- `Name` — identity.
- `Claim` — the professed `Belief` (nullable; `null` = agnostic).
- `Grounding` — the person's **current revealed grounding** (see 4.3). Everyone starts **unproven**
  (`Grounding.Divided()`), regardless of profession, because belief is never following.
- `FollowsChrist` — derived from the **way** (axis 2), not the claim: `true` when the revealed
  grounding is `InGod` (the way of living coheres with God's whole character). A Buddhist or atheist
  professor with godly fruit reads `true` (following unknowingly); a professed Christian with rotten
  fruit reads `false`.

**Methods:**

- `Do(Act act, Reality reality)` — the person acts. Reality witnesses the deed **on the person's
  current grounding** (`reality.Witness(act, this.Grounding)`), which records it on the cosmic
  tablets. The person also records the resolution on its own life-record, then **re-derives its
  revealed grounding from the updated fruit**. Returns the `Resolution`.
- `Integrity()` — the grade: "are they what they say they are?", `0.0`–`1.0` (see 4.3).

### 4.3 The one uniform rule (revealed grounding + integrity)

This single rule replaces every earlier per-case table.

**Fruit.** A person keeps its own life-record of resolutions (the same `Resolution` objects reality
produced). The person's **fruit coherence** `F` is read off that record using the *same*
aggregation `HeavenlyTablets.Coherence()` already uses:

```
F = 1.0
foreach resolution in record:
    F = F * (1.0 - resolution.Disorder)
    F = F + resolution.Restoration * (1.0 - F)
```

A record with no deeds reads `F = 1.0` by that formula, so fruit coherence is **not** used as the
starting grounding (see below); it only governs movement once deeds exist.

**Starting grounding (no fruit yet).** Everyone starts **unproven**: `Grounding.Divided()`.
Profession does not grant standing. The demons believe; the demons do not follow. A brand-new
person has demonstrated no way of living, so they waver until their deeds speak.

**Revealed grounding (once deeds exist).** The way of living moves the person across the three
existing grounding states, by documented "two opinions" thresholds (1 Kings 18:21):

- `F` high (deeds follow God's character) → `Grounding.InGod()`
- `F` middling (mixed, divided fruit) → `Grounding.Divided()`
- `F` low (deeds betray any profession of God) → `Grounding.InIdol(idolName)`

`idolName` comes from the claim, so the revealed idol is named truthfully: an atheist's is
`"the self"`, an other-deity professor's is that deity, a fallen Christian-professor's defaults to
`"the self"`. Reusing `InGod` / `Divided` / `InIdol` means no new grounding machinery is invented;
the way of living simply relocates the person among the foundations the engine already has.

Thresholds (named constants, documented, not scenario lookups): `F >= 0.75` → follows (`InGod`);
`0.40 <= F < 0.75` → wavering (`Divided`); `F < 0.40` → betrays (`InIdol`). These are the
boundaries between the "two opinions"; they are tunable in one place.

**Integrity.** How close the person's **professed** grounding stands to their **revealed**
grounding on the life-scale (the `lifelessness` the `Grounding` already encodes, surfaced as a
read-only value for comparison):

```
Integrity = 1.0 - | professedLifelessness - revealedLifelessness |
```

- `1.0` = exactly what they say (a faithful believer revealed `InGod`; an honest selfish atheist
  revealed `the self`).
- Lower = a gap between claim and life (the hypocrite who professes Christ but lives for self; the
  "good" atheist who lays down his life and so lives *higher* than his claim).
- An agnostic (`null` claim) has no professed grounding; their professed grounding for integrity is
  taken as `Divided` (the claim is itself a wavering), so a wavering agnostic reads `1.0` and a
  secretly-following agnostic reads below `1.0`.

To support this comparison, `Grounding` exposes a read-only `Lifelessness` (currently a private
field). This is the one small change to an existing type, and it is additive (a public getter over
the existing field). No behavior of `Grounding` changes.

### 4.4 Worked cases (all fall out of the one rule)

| Person | Professes | After godly/rotten fruit, reveals | FollowsChrist | Integrity |
|---|---|---|---|---|
| Faithful believer | true God | `InGod` | true | 1.0 (is what they say) |
| Hypocrite (Lord, Lord) | true God | `InIdol(self)` | false | 0.5 (worse than they say) |
| Honest atheist (lives for self) | no god | `InIdol(self)` | false | 1.0 (consistent) |
| "Good" atheist (lays down his life) | no god | `InGod` | true | 0.5 (better than he says) |
| Buddhist who lives the way | non-theistic path | `InGod` | true (unknowingly) | < 1.0 (lives above claim) |
| Demon (affirms God, follows nothing) | true God, `AffirmsGodExists` | stays `Divided` / drifts `InIdol` | false | low |
| Wavering agnostic | null | `Divided` | false | 1.0 |
| Fresh person (no deeds) | anything | `Divided` (unproven) | false | varies by claim |

## 5. Files

New:

- `AnointedAutomation.Objects/Concepts/Reality/Belief.cs`
- `AnointedAutomation.Objects/Concepts/Reality/Person.cs`

Changed (additive only):

- `AnointedAutomation.Objects/Concepts/Reality/Grounding.cs` — add a public read-only
  `Lifelessness` getter over the existing private field. No behavior change.

Tests (xUnit, mirroring the existing `AnointedAutomation.Objects.Tests` layout; no new code without
tests):

- `AnointedAutomation.Objects.Tests/BeliefTests.cs` — factories, `ProfessedName`,
  `AffirmsGodExists`, `ClaimsNoGod`, `IsInTheTrueGod`, `ProfessedGrounding()` for every path,
  null/empty-name guards.
- `AnointedAutomation.Objects.Tests/PersonTests.cs` — every row of the 4.4 table: fresh person
  starts `Divided`; godly fruit reveals `InGod` and `FollowsChrist == true`; rotten fruit reveals
  `InIdol(self)` and `FollowsChrist == false`; a non-theistic professor with godly fruit follows
  unknowingly; the demon case (affirms God, no following, never reaches `InGod`); integrity for the
  faithful, the hypocrite, the honest atheist, the "good" atheist, the wavering agnostic; null-claim
  (agnostic) construction; `Do` records on both the cosmic tablets and the person's record and
  returns the resolution; null-argument guards on `Do`.

Demo:

- `AnointedAutomation.Objects.Demo/PersonDemo.cs`, called from `Program.cs` — the game flow: create
  a person, choose a claim, act, and watch the grounding, `FollowsChrist`, and `Integrity` reveal
  the heart over a sequence of deeds. Includes the Buddhist-who-follows-unknowingly and the
  professed-Christian-hypocrite to show the two axes diverging.

## 6. Code-standard conformance

Per the project rules: explicit types (no `var`); `.Equals(...)` for all string comparisons (no
`==`/`!=` on strings); fail fast on null/empty arguments with `ArgumentNullException` /
`ArgumentException` (no fallback defaults, no `?.`, no `??`); fully-qualified `System.*` usage
matching the surrounding files; copyright/steward header on every new file; XML doc comments on
every public member with Scripture anchors where the existing engine does; no try-catch in any hot
path. `Person` keeps its life-record in a private `System.Collections.Generic.List<Resolution>` and
exposes a read-only view, mirroring `HeavenlyTablets`.

## 7. Out of scope (v1, YAGNI)

- API engines for harmonizing other-deity or non-theistic worship through their own character
  models. `Belief.In(...)` / `Belief.InPath(...)` reserve the names; the harmonization plugs in
  later.
- Repentance/redemption arcs that *restore* a fallen person's grounding over time beyond what fruit
  coherence already expresses (the engine already carries `Restoration` on resolutions; a richer
  per-person redemption arc is a later design).
- Continuous (non-discrete) grounding. v1 maps the way of living onto the three existing grounding
  states; an arbitrary-`lifelessness` grounding constructor is deferred.
- Any `Agent`/social model beyond a single `Person` acting in `Reality`.
