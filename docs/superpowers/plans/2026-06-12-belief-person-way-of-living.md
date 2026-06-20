# Belief, Person, and the Way of Living — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a reusable, game-ready `Person` cluster to the reality engine where a person professes a `Belief` but is grounded only by the way they live, revealed by the fruit of their deeds.

**Architecture:** Two independent axes. A `Belief` is the professed claim (true God, another deity, a non-theistic path, atheism, or null = agnostic). A `Person` holds a claim, owns a `Life` (a personal fruit-record), and is read by a stateless `Heart` that applies one uniform rule: everyone starts unproven (`Divided`), and only fruit moves them to `InGod`, `Divided`, or `InIdol`. `FollowsChrist` and `Integrity` derive from the way, not the label.

**Tech Stack:** C# (.NET), xUnit. New code lives in `AnointedAutomation.Objects/Concepts/Reality/` (namespace `AnointedAutomation.Objects.Concepts`). Tests in `AnointedAutomation.Objects.Tests/`. Demo in `AnointedAutomation.Objects.Demo/`.

**Spec:** `docs/superpowers/specs/2026-06-12-belief-person-way-of-living-design.md`

**House rules (apply to every new file):** explicit types (no `var`); `.Equals(...)` for string comparisons (no `==`/`!=` on strings); fail fast with `System.ArgumentNullException` / `System.ArgumentException` (no fallback defaults, no `?.`, no `??`); fully-qualified `System.*`; the two-line steward header at the top of every file; XML doc comments on every public member; no try-catch in hot paths.

**Standard file header (copy verbatim into every new `.cs` file):**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields
```

---

## Task 1: Expose `Grounding.Lifelessness`

The `Heart` needs to measure how far a professed grounding stands from a revealed one. `Grounding`
already holds a private `lifelessness` field (0.0 for God, 0.25 for Divided, 0.5 for an idol); add a
public read-only getter over it. Additive only, no behavior change.

**Files:**
- Modify: `AnointedAutomation.Objects/Concepts/Reality/Grounding.cs`
- Test: `AnointedAutomation.Objects.Tests/GroundingTests.cs`

- [ ] **Step 1: Add failing tests**

Append these three facts inside the `GroundingTests` class in
`AnointedAutomation.Objects.Tests/GroundingTests.cs` (before the closing brace of the class):

```csharp
        [Fact]
        public void InGod_HasNoLifelessness()
        {
            Assert.Equal(0.0, Grounding.InGod().Lifelessness);
        }

        [Fact]
        public void Divided_IsPartlyLifeless()
        {
            Assert.Equal(0.25, Grounding.Divided().Lifelessness);
        }

        [Fact]
        public void InIdol_IsHalfLifeless()
        {
            Assert.Equal(0.5, Grounding.InIdol("Baal").Lifelessness);
        }
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~GroundingTests.InGod_HasNoLifelessness"`
Expected: BUILD FAILURE — `'Grounding' does not contain a definition for 'Lifelessness'`.

- [ ] **Step 3: Add the public getter**

In `AnointedAutomation.Objects/Concepts/Reality/Grounding.cs`, add this property immediately after
the existing `IsInGod` property's closing brace:

```csharp
        /// <summary>
        /// How little life this foundation can give, from 0.0 (the living God) to 1.0 (an utterly
        /// dead idol). Exposed so a heart can measure how far a professed grounding stands from the
        /// grounding a life actually reveals.
        /// </summary>
        public double Lifelessness
        {
            get
            {
                return lifelessness;
            }
        }
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~GroundingTests"`
Expected: PASS (all GroundingTests, old and new).

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Reality/Grounding.cs AnointedAutomation.Objects.Tests/GroundingTests.cs
git commit -m "Expose Grounding.Lifelessness"
```

---

## Task 2: `Belief` — the professed claim

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Reality/Belief.cs`
- Test: `AnointedAutomation.Objects.Tests/BeliefTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `AnointedAutomation.Objects.Tests/BeliefTests.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class BeliefTests
    {
        [Fact]
        public void InGod_ProfessesTheTrueGod_AndAffirmsHeExists()
        {
            Belief belief = Belief.InGod();

            Assert.Equal("God", belief.ProfessedName);
            Assert.True(belief.IsInTheTrueGod);
            Assert.True(belief.AffirmsGodExists);
            Assert.False(belief.ClaimsNoGod);
        }

        [Fact]
        public void In_ProfessesAnotherDeity_ThatAffirmsAGodButNotTheTrueGod()
        {
            Belief belief = Belief.In("Baal");

            Assert.Equal("Baal", belief.ProfessedName);
            Assert.False(belief.IsInTheTrueGod);
            Assert.True(belief.AffirmsGodExists);
            Assert.False(belief.ClaimsNoGod);
        }

        [Fact]
        public void InPath_ProfessesANonTheisticPath_ThatAffirmsNoGod()
        {
            Belief belief = Belief.InPath("Buddhism");

            Assert.Equal("Buddhism", belief.ProfessedName);
            Assert.False(belief.IsInTheTrueGod);
            Assert.False(belief.AffirmsGodExists);
            Assert.False(belief.ClaimsNoGod);
        }

        [Fact]
        public void InNoGod_IsTheAtheistsExplicitDenial()
        {
            Belief belief = Belief.InNoGod();

            Assert.Equal("no god", belief.ProfessedName);
            Assert.False(belief.IsInTheTrueGod);
            Assert.False(belief.AffirmsGodExists);
            Assert.True(belief.ClaimsNoGod);
        }

        [Fact]
        public void ProfessedGrounding_TrueGod_IsInGod()
        {
            Assert.True(Belief.InGod().ProfessedGrounding().IsInGod);
        }

        [Fact]
        public void ProfessedGrounding_AnotherDeity_IsThatNamedIdol()
        {
            Grounding grounding = Belief.In("Baal").ProfessedGrounding();

            Assert.False(grounding.IsInGod);
            Assert.Equal("Baal", grounding.Name);
        }

        [Fact]
        public void ProfessedGrounding_Atheism_IsTheIdolOfTheSelf()
        {
            Grounding grounding = Belief.InNoGod().ProfessedGrounding();

            Assert.False(grounding.IsInGod);
            Assert.Equal("the self", grounding.Name);
        }

        [Fact]
        public void ProfessedGrounding_NonTheisticPath_IsDivided()
        {
            Grounding grounding = Belief.InPath("Buddhism").ProfessedGrounding();

            Assert.Equal(0.25, grounding.Lifelessness);
        }

        [Fact]
        public void In_RejectsAnEmptyName()
        {
            Assert.Throws<System.ArgumentNullException>(() => Belief.In(""));
        }

        [Fact]
        public void InPath_RejectsAnEmptyName()
        {
            Assert.Throws<System.ArgumentNullException>(() => Belief.InPath(""));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~BeliefTests"`
Expected: BUILD FAILURE — `The type or namespace name 'Belief' could not be found`.

- [ ] **Step 3: Implement `Belief`**

Create `AnointedAutomation.Objects/Concepts/Reality/Belief.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// The claim a person professes at "character creation": what they say is true and say they
    /// follow. Mental assent is real, but it is never by itself following. "Even the demons believe
    /// that, and shudder" (James 2:19). A <c>null</c> <see cref="Belief"/> is the agnostic, who has
    /// made no settled claim. A belief does not act and holds no state.
    /// </summary>
    public class Belief
    {
        private Belief(string professedName, bool isInTheTrueGod, bool affirmsGodExists, bool claimsNoGod)
        {
            ProfessedName = professedName;
            IsInTheTrueGod = isInTheTrueGod;
            AffirmsGodExists = affirmsGodExists;
            ClaimsNoGod = claimsNoGod;
        }

        /// <summary>
        /// The human-readable label of the professed path ("God", "Baal", "Buddhism", "no god").
        /// </summary>
        public string ProfessedName
        {
            get;
        }

        /// <summary>
        /// Whether the professed path is the true God (Christ).
        /// </summary>
        public bool IsInTheTrueGod
        {
            get;
        }

        /// <summary>
        /// Whether the claim affirms that God exists. True for the true God and for any other-deity
        /// theism; false for atheism and for a non-theistic path. This is what makes the demons'
        /// case expressible: a claim can affirm God exists while the person who holds it never follows.
        /// </summary>
        public bool AffirmsGodExists
        {
            get;
        }

        /// <summary>
        /// Whether the claim is the atheist's explicit denial that there is any god.
        /// </summary>
        public bool ClaimsNoGod
        {
            get;
        }

        /// <summary>
        /// Professes the true God, Christ (John 14:6).
        /// </summary>
        /// <returns>A belief in the true God.</returns>
        public static Belief InGod()
        {
            return new Belief("God", true, true, false);
        }

        /// <summary>
        /// Professes another named deity (a god that is not the true God). Future API engines may
        /// supply how that deity's worship harmonizes.
        /// </summary>
        /// <param name="deityName">The name of the professed deity.</param>
        /// <returns>A belief in another deity.</returns>
        public static Belief In(string deityName)
        {
            if (string.IsNullOrEmpty(deityName))
            {
                throw new System.ArgumentNullException(nameof(deityName));
            }

            return new Belief(deityName, false, true, false);
        }

        /// <summary>
        /// Professes a non-theistic path: a real religion or way of living that names no deity
        /// (Buddhism, for example).
        /// </summary>
        /// <param name="pathName">The name of the professed path.</param>
        /// <returns>A belief in a non-theistic path.</returns>
        public static Belief InPath(string pathName)
        {
            if (string.IsNullOrEmpty(pathName))
            {
                throw new System.ArgumentNullException(nameof(pathName));
            }

            return new Belief(pathName, false, false, false);
        }

        /// <summary>
        /// Atheism: the explicit truth-claim that there is no god (Romans 1:25, serving the creature
        /// rather than the Creator).
        /// </summary>
        /// <returns>A belief that there is no god.</returns>
        public static Belief InNoGod()
        {
            return new Belief("no god", false, false, true);
        }

        /// <summary>
        /// The grounding the bare claim <em>asserts</em>. Used only to grade integrity, never to
        /// grant standing: the true God asserts <see cref="Grounding.InGod"/>; another deity asserts
        /// that named idol; atheism asserts the idol of the self (Romans 1:25); a non-theistic path
        /// names no living ground, so it asserts a divided footing and the fruit reveals the rest.
        /// </summary>
        /// <returns>The grounding the claim professes.</returns>
        public Grounding ProfessedGrounding()
        {
            if (IsInTheTrueGod)
            {
                return Grounding.InGod();
            }

            if (ClaimsNoGod)
            {
                return Grounding.InIdol("the self");
            }

            if (AffirmsGodExists)
            {
                return Grounding.InIdol(ProfessedName);
            }

            return Grounding.Divided();
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~BeliefTests"`
Expected: PASS (all 10 facts).

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Reality/Belief.cs AnointedAutomation.Objects.Tests/BeliefTests.cs
git commit -m "Add Belief value object"
```

---

## Task 3: `Beliefs` — the preset catalog

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Reality/Beliefs.cs`
- Test: `AnointedAutomation.Objects.Tests/BeliefsTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `AnointedAutomation.Objects.Tests/BeliefsTests.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class BeliefsTests
    {
        [Fact]
        public void Christianity_IsBeliefInTheTrueGod()
        {
            Assert.True(Beliefs.Christianity.IsInTheTrueGod);
        }

        [Fact]
        public void Buddhism_IsANonTheisticPath()
        {
            Belief belief = Beliefs.Buddhism;

            Assert.Equal("Buddhism", belief.ProfessedName);
            Assert.False(belief.AffirmsGodExists);
        }

        [Fact]
        public void Atheism_ClaimsNoGod()
        {
            Assert.True(Beliefs.Atheism.ClaimsNoGod);
        }

        [Fact]
        public void Agnosticism_IsNoSettledClaim()
        {
            Assert.Null(Beliefs.Agnosticism);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~BeliefsTests"`
Expected: BUILD FAILURE — `The type or namespace name 'Beliefs' could not be found`.

- [ ] **Step 3: Implement `Beliefs`**

Create `AnointedAutomation.Objects/Concepts/Reality/Beliefs.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A small catalog of preset claims for a "choose your belief" menu at character creation. Kept
    /// minimal on purpose: only paths whose grounding is theologically settled here. Other deities
    /// and other religions are reached through the open factories <see cref="Belief.In"/> and
    /// <see cref="Belief.InPath"/>, and the catalog grows as those mappings are decided.
    /// </summary>
    public static class Beliefs
    {
        /// <summary>
        /// Belief in the true God (Christ).
        /// </summary>
        public static Belief Christianity
        {
            get
            {
                return Belief.InGod();
            }
        }

        /// <summary>
        /// A non-theistic path that names no deity.
        /// </summary>
        public static Belief Buddhism
        {
            get
            {
                return Belief.InPath("Buddhism");
            }
        }

        /// <summary>
        /// The explicit claim that there is no god.
        /// </summary>
        public static Belief Atheism
        {
            get
            {
                return Belief.InNoGod();
            }
        }

        /// <summary>
        /// The agnostic: no settled claim. Modeled as the absence of a belief (<c>null</c>).
        /// </summary>
        public static Belief Agnosticism
        {
            get
            {
                return null;
            }
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~BeliefsTests"`
Expected: PASS (all 4 facts).

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Reality/Beliefs.cs AnointedAutomation.Objects.Tests/BeliefsTests.cs
git commit -m "Add Beliefs preset catalog"
```

---

## Task 4: `Life` — the personal fruit-record

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Reality/Life.cs`
- Test: `AnointedAutomation.Objects.Tests/LifeTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `AnointedAutomation.Objects.Tests/LifeTests.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class LifeTests
    {
        [Fact]
        public void AnEmptyLife_ReadsFullyCoherent()
        {
            Life life = new Life();

            Assert.Equal(1.0, life.Coherence());
            Assert.Empty(life.History());
        }

        [Fact]
        public void RecordedDeeds_AreKeptInOrder()
        {
            Life life = new Life();
            Resolution first = new Resolution(1.0, 0.0);
            Resolution second = new Resolution(0.5, 0.5);

            life.Record(first);
            life.Record(second);

            Assert.Equal(2, life.History().Count);
            Assert.Same(first, life.History()[0]);
            Assert.Same(second, life.History()[1]);
        }

        [Fact]
        public void DisorderingDeeds_DrainTheFruitCoherence()
        {
            Life life = new Life();
            life.Record(new Resolution(0.0, 0.5));

            Assert.Equal(0.5, life.Coherence());
        }

        [Fact]
        public void Record_RejectsNull()
        {
            Life life = new Life();

            Assert.Throws<System.ArgumentNullException>(() => life.Record(null));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~LifeTests"`
Expected: BUILD FAILURE — `The type or namespace name 'Life' could not be found`.

- [ ] **Step 3: Implement `Life`**

Create `AnointedAutomation.Objects/Concepts/Reality/Life.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A person's own record of what their deeds came to: their fruit. A per-person mirror of the
    /// <see cref="HeavenlyTablets"/>. "By their fruit you will recognize them" (Matthew 7:16-20). The
    /// coherence of a life is read off its whole record the same way the standing order of reality is.
    /// </summary>
    public class Life
    {
        private readonly System.Collections.Generic.List<Resolution> record =
            new System.Collections.Generic.List<Resolution>();

        /// <summary>
        /// Writes a witnessed deed onto this life's record.
        /// </summary>
        /// <param name="resolution">The resolution of a deed this person has done.</param>
        public void Record(Resolution resolution)
        {
            if (resolution == null)
            {
                throw new System.ArgumentNullException(nameof(resolution));
            }

            record.Add(resolution);
        }

        /// <summary>
        /// The whole record of this person's deeds, in the order they happened.
        /// </summary>
        /// <returns>A read-only view of every recorded resolution.</returns>
        public System.Collections.Generic.IReadOnlyList<Resolution> History()
        {
            return record;
        }

        /// <summary>
        /// The fruit coherence of this life, from 0.0 (wholly disordered) to 1.0 (fully ordered). An
        /// empty life reads 1.0; disordering deeds drain it and restorative deeds heal it.
        /// </summary>
        /// <returns>The coherence of this life's fruit.</returns>
        public double Coherence()
        {
            double coherence = 1.0;
            foreach (Resolution resolution in record)
            {
                coherence = coherence * (1.0 - resolution.Disorder);
                coherence = coherence + resolution.Restoration * (1.0 - coherence);
            }

            return coherence;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~LifeTests"`
Expected: PASS (all 4 facts).

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Reality/Life.cs AnointedAutomation.Objects.Tests/LifeTests.cs
git commit -m "Add Life fruit-record"
```

---

## Task 5: `Heart` — the uniform way-of-living rule

This is the engine's core judgment, kept stateless and tested in isolation with hand-built
`Resolution`s so the thresholds are exact and independent of the witnessing pipeline.

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Reality/Heart.cs`
- Test: `AnointedAutomation.Objects.Tests/HeartTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `AnointedAutomation.Objects.Tests/HeartTests.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class HeartTests
    {
        private static Life LifeWithFruit(double disorder)
        {
            // A single deed whose disorder sets the life's fruit coherence to (1 - disorder).
            Life life = new Life();
            life.Record(new Resolution(1.0 - disorder, disorder));
            return life;
        }

        [Fact]
        public void AnEmptyLife_IsUnprovenAndWavers_WhateverIsProfessed()
        {
            // The demons believe, but the demons do not follow. No deeds means no standing.
            Heart heart = new Heart();
            Grounding grounding = heart.Reveal(Belief.InGod(), new Life());

            Assert.False(grounding.IsInGod);
            Assert.Equal(0.25, grounding.Lifelessness);
        }

        [Fact]
        public void GodlyFruit_RevealsAGroundingInGod()
        {
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.0); // fruit coherence 1.0

            Assert.True(heart.Reveal(Belief.InNoGod(), life).IsInGod);
            Assert.True(heart.FollowsChrist(Belief.InNoGod(), life));
        }

        [Fact]
        public void MixedFruit_RevealsAWaveringGrounding()
        {
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.5); // fruit coherence 0.5

            Grounding grounding = heart.Reveal(Belief.InGod(), life);

            Assert.False(grounding.IsInGod);
            Assert.Equal(0.25, grounding.Lifelessness);
        }

        [Fact]
        public void BetrayingFruit_RevealsTheIdolNamedByTheClaim()
        {
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.9); // fruit coherence 0.1

            Grounding asAtheist = heart.Reveal(Belief.InNoGod(), life);
            Grounding asBaalWorshipper = heart.Reveal(Belief.In("Baal"), life);

            Assert.Equal("the self", asAtheist.Name);
            Assert.Equal("Baal", asBaalWorshipper.Name);
        }

        [Fact]
        public void AFallenChristianProfessor_RevealsTheIdolOfTheSelf()
        {
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.9);

            Assert.Equal("the self", heart.Reveal(Belief.InGod(), life).Name);
            Assert.False(heart.FollowsChrist(Belief.InGod(), life));
        }

        [Fact]
        public void FaithfulBeliever_HasFullIntegrity()
        {
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.0);

            Assert.Equal(1.0, heart.Integrity(Belief.InGod(), life));
        }

        [Fact]
        public void Hypocrite_HasLowIntegrity()
        {
            // Professes the true God (lifelessness 0.0) but lives for self (0.5).
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.9);

            Assert.Equal(0.5, heart.Integrity(Belief.InGod(), life));
        }

        [Fact]
        public void HonestSelfishAtheist_HasFullIntegrity()
        {
            // Professes the idol of self (0.5) and lives it (0.5): exactly what he says.
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.9);

            Assert.Equal(1.0, heart.Integrity(Belief.InNoGod(), life));
        }

        [Fact]
        public void GoodAtheist_LivesAboveHisClaim()
        {
            // Professes the idol of self (0.5) but lays down his life (revealed InGod, 0.0).
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.0);

            Assert.Equal(0.5, heart.Integrity(Belief.InNoGod(), life));
        }

        [Fact]
        public void WaveringAgnostic_IsWhatTheySay()
        {
            // A null claim is itself a wavering; revealed wavering matches it.
            Heart heart = new Heart();
            Life life = LifeWithFruit(0.5);

            Grounding grounding = heart.Reveal(null, life);

            Assert.Equal(0.25, grounding.Lifelessness);
            Assert.Equal(1.0, heart.Integrity(null, life));
        }

        [Fact]
        public void AnEmptyLife_DoesNotFollowChrist()
        {
            Heart heart = new Heart();

            Assert.False(heart.FollowsChrist(Belief.InGod(), new Life()));
        }

        [Fact]
        public void Reveal_RejectsANullLife()
        {
            Heart heart = new Heart();

            Assert.Throws<System.ArgumentNullException>(() => heart.Reveal(Belief.InGod(), null));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~HeartTests"`
Expected: BUILD FAILURE — `The type or namespace name 'Heart' could not be found`.

- [ ] **Step 3: Implement `Heart`**

Create `AnointedAutomation.Objects/Concepts/Reality/Heart.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// What the LORD looks at (1 Samuel 16:7). The heart carries no state: it reads a <see cref="Life"/>
    /// in light of a professed <see cref="Belief"/> and yields the grounding the person actually
    /// stands on, whether they follow Christ, and how closely their life matches their claim. "Out of
    /// the overflow of the heart ... a good man brings good things out of the good stored up in his
    /// heart" (Luke 6:45). Profession never grants standing; only the way of living does.
    /// </summary>
    public class Heart
    {
        /// <summary>
        /// Fruit at or above this coherence follows God's character and stands in God.
        /// </summary>
        private const double FollowsThreshold = 0.75;

        /// <summary>
        /// Fruit at or above this coherence (but below following) wavers between two opinions
        /// (1 Kings 18:21); below it, the fruit betrays any profession of God.
        /// </summary>
        private const double WaversThreshold = 0.40;

        /// <summary>
        /// Reveals the grounding a person actually stands on. An empty life is unproven and wavers,
        /// whatever is professed (the demons believe but do not follow). Once deeds exist, the fruit
        /// moves the person to God (high), to wavering (mixed), or to the idol named by their claim
        /// (low).
        /// </summary>
        /// <param name="claim">The professed belief (may be <c>null</c> for the agnostic).</param>
        /// <param name="life">The person's life and its fruit.</param>
        /// <returns>The grounding the way of living reveals.</returns>
        public Grounding Reveal(Belief claim, Life life)
        {
            if (life == null)
            {
                throw new System.ArgumentNullException(nameof(life));
            }

            if (life.History().Count == 0)
            {
                return Grounding.Divided();
            }

            double fruit = life.Coherence();
            if (fruit >= FollowsThreshold)
            {
                return Grounding.InGod();
            }

            if (fruit >= WaversThreshold)
            {
                return Grounding.Divided();
            }

            return Grounding.InIdol(IdolName(claim));
        }

        /// <summary>
        /// Whether the person follows Christ, knowingly or unknowingly: true when their way of living
        /// reveals a grounding in God. Derived from the way, never from the label.
        /// </summary>
        /// <param name="claim">The professed belief (may be <c>null</c>).</param>
        /// <param name="life">The person's life and its fruit.</param>
        /// <returns><c>true</c> if the revealed grounding is in God.</returns>
        public bool FollowsChrist(Belief claim, Life life)
        {
            return Reveal(claim, life).IsInGod;
        }

        /// <summary>
        /// How close the person's professed grounding stands to the grounding their life reveals,
        /// from 0.0 to 1.0. 1.0 means they are exactly what they say (a faithful believer, or an
        /// honest self-serving atheist); a gap marks the hypocrite who lives below his claim or the
        /// quiet follower who lives above it.
        /// </summary>
        /// <param name="claim">The professed belief (may be <c>null</c> for the agnostic).</param>
        /// <param name="life">The person's life and its fruit.</param>
        /// <returns>The integrity of claim and life.</returns>
        public double Integrity(Belief claim, Life life)
        {
            Grounding professed = ProfessedGrounding(claim);
            Grounding revealed = Reveal(claim, life);

            double gap = professed.Lifelessness - revealed.Lifelessness;
            if (gap < 0.0)
            {
                gap = -gap;
            }

            return 1.0 - gap;
        }

        /// <summary>
        /// The grounding the claim professes, treating a null claim (the agnostic) as a wavering.
        /// </summary>
        /// <param name="claim">The professed belief (may be <c>null</c>).</param>
        /// <returns>The professed grounding.</returns>
        private static Grounding ProfessedGrounding(Belief claim)
        {
            if (claim == null)
            {
                return Grounding.Divided();
            }

            return claim.ProfessedGrounding();
        }

        /// <summary>
        /// The name of the idol a betraying life falls to: another deity keeps that deity's name;
        /// every other claim (atheism, a non-theistic path, a fallen profession of the true God, or
        /// the agnostic) falls to the idol of the self (Romans 1:25).
        /// </summary>
        /// <param name="claim">The professed belief (may be <c>null</c>).</param>
        /// <returns>The idol's name.</returns>
        private static string IdolName(Belief claim)
        {
            if (claim == null)
            {
                return "the self";
            }

            if (claim.AffirmsGodExists && !claim.IsInTheTrueGod)
            {
                return claim.ProfessedName;
            }

            return "the self";
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~HeartTests"`
Expected: PASS (all 12 facts).

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Reality/Heart.cs AnointedAutomation.Objects.Tests/HeartTests.cs
git commit -m "Add Heart way-of-living rule"
```

---

## Task 6: `Person` — the reusable agent

Wires `Belief` + `Life` + `Heart` and acts in `Reality`. Tested end-to-end through the real
witnessing pipeline.

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Reality/Person.cs`
- Test: `AnointedAutomation.Objects.Tests/PersonTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `AnointedAutomation.Objects.Tests/PersonTests.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class PersonTests
    {
        private static Act AGodlyDeed()
        {
            return new Act("binding a stranger's wounds", new Compassion(), new Protection());
        }

        private static Act AWickedDeed()
        {
            return new Act("the shedding of innocent blood", new Murder());
        }

        [Fact]
        public void AFreshPerson_StartsUnprovenAndWavering()
        {
            Person person = new Person("Newborn", Belief.InGod());

            Assert.Equal(0.25, person.Grounding.Lifelessness);
            Assert.False(person.FollowsChrist);
        }

        [Fact]
        public void AnAgnostic_HasNoClaim()
        {
            Person person = new Person("Seeker");

            Assert.Null(person.Claim);
        }

        [Fact]
        public void GodlyFruit_GroundsAProfessedBelieverInGod()
        {
            Reality reality = Reality.Revealed();
            Person person = new Person("Paul", Belief.InGod());

            person.Do(AGodlyDeed(), reality);

            Assert.True(person.Grounding.IsInGod);
            Assert.True(person.FollowsChrist);
        }

        [Fact]
        public void RottenFruit_DropsAProfessedBelieverToTheIdolOfTheSelf()
        {
            Reality reality = Reality.Revealed();
            Person person = new Person("Judas", Belief.InGod());

            person.Do(AWickedDeed(), reality);

            Assert.False(person.FollowsChrist);
            Assert.Equal("the self", person.Grounding.Name);
        }

        [Fact]
        public void ABuddhistWhoLivesTheWay_FollowsChristUnknowingly()
        {
            Reality reality = Reality.Revealed();
            Person person = new Person("Siddhartha", Belief.InPath("Buddhism"));

            person.Do(AGodlyDeed(), reality);

            Assert.True(person.FollowsChrist);
        }

        [Fact]
        public void Do_RecordsOnBothTheCosmicTabletsAndThePersonsLife()
        {
            Reality reality = Reality.Revealed();
            Person person = new Person("Ruth", Belief.InGod());

            Resolution resolution = person.Do(AGodlyDeed(), reality);

            Assert.NotNull(resolution);
            Assert.Single(person.Life.History());
            Assert.Single(reality.Tablets.History());
        }

        [Fact]
        public void Do_RejectsANullAct()
        {
            Reality reality = Reality.Revealed();
            Person person = new Person("Ruth", Belief.InGod());

            Assert.Throws<System.ArgumentNullException>(() => person.Do(null, reality));
        }

        [Fact]
        public void Do_RejectsANullReality()
        {
            Person person = new Person("Ruth", Belief.InGod());

            Assert.Throws<System.ArgumentNullException>(() => person.Do(AGodlyDeed(), null));
        }

        [Fact]
        public void Constructor_RejectsAnEmptyName()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Person(""));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~PersonTests"`
Expected: BUILD FAILURE — `The type or namespace name 'Person' could not be found`.

- [ ] **Step 3: Implement `Person`**

Create `AnointedAutomation.Objects/Concepts/Reality/Person.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A soul who professes a claim and acts in reality. A person is deliberately thin: it holds a
    /// name and a <see cref="Belief"/>, owns a <see cref="Life"/>, and is read by a <see cref="Heart"/>.
    /// The way-of-living judgment lives in the heart, not here, so a person stays a clean, reusable
    /// handle. "Not everyone who says to me, 'Lord, Lord,' will enter the kingdom of heaven, but only
    /// the one who does the will of my Father" (Matthew 7:21).
    /// </summary>
    public class Person
    {
        private readonly Life life = new Life();
        private readonly Heart heart = new Heart();

        /// <summary>
        /// Initializes an agnostic person: one who has made no settled claim (a null belief).
        /// </summary>
        /// <param name="name">The person's name.</param>
        public Person(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a person professing a given claim.
        /// </summary>
        /// <param name="name">The person's name.</param>
        /// <param name="claim">The professed belief (<c>null</c> for the agnostic).</param>
        public Person(string name, Belief claim)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentNullException(nameof(name));
            }

            Name = name;
            Claim = claim;
        }

        /// <summary>
        /// The person's name.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// The belief the person professes (<c>null</c> for the agnostic).
        /// </summary>
        public Belief Claim
        {
            get;
        }

        /// <summary>
        /// The person's life and its fruit.
        /// </summary>
        public Life Life
        {
            get
            {
                return life;
            }
        }

        /// <summary>
        /// The grounding the person actually stands on, revealed by the way they live. Everyone
        /// starts unproven (wavering) until their deeds speak.
        /// </summary>
        public Grounding Grounding
        {
            get
            {
                return heart.Reveal(Claim, life);
            }
        }

        /// <summary>
        /// Whether the person follows Christ, knowingly or unknowingly, judged by the way they live.
        /// </summary>
        public bool FollowsChrist
        {
            get
            {
                return heart.FollowsChrist(Claim, life);
            }
        }

        /// <summary>
        /// The person acts. Reality witnesses the deed on the person's current grounding (recording
        /// it on the cosmic tablets), the deed is written onto the person's own life, and the next
        /// reading of the person's grounding reflects the new fruit.
        /// </summary>
        /// <param name="act">The deed the person does.</param>
        /// <param name="reality">The reality that witnesses it.</param>
        /// <returns>The resolution of the deed.</returns>
        public Resolution Do(Act act, Reality reality)
        {
            if (act == null)
            {
                throw new System.ArgumentNullException(nameof(act));
            }

            if (reality == null)
            {
                throw new System.ArgumentNullException(nameof(reality));
            }

            Resolution resolution = reality.Witness(act, Grounding);
            life.Record(resolution);
            return resolution;
        }

        /// <summary>
        /// How closely the person's life matches their claim, from 0.0 to 1.0: are they what they
        /// say they are?
        /// </summary>
        /// <returns>The integrity of claim and life.</returns>
        public double Integrity()
        {
            return heart.Integrity(Claim, life);
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~PersonTests"`
Expected: PASS (all 9 facts).

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Reality/Person.cs AnointedAutomation.Objects.Tests/PersonTests.cs
git commit -m "Add Person agent"
```

---

## Task 7: `Deeds` — a starter catalog of acts

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Reality/Deeds.cs`
- Test: `AnointedAutomation.Objects.Tests/DeedsTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `AnointedAutomation.Objects.Tests/DeedsTests.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class DeedsTests
    {
        [Fact]
        public void BindAStrangersWounds_IsCompassionAndProtection()
        {
            Act act = Deeds.BindAStrangersWounds();

            Assert.NotNull(act);
            Assert.Contains(act.Concepts, concept => concept is Compassion);
            Assert.Contains(act.Concepts, concept => concept is Protection);
        }

        [Fact]
        public void LayDownYourLife_IsSelfSacrifice()
        {
            Act act = Deeds.LayDownYourLife();

            Assert.Contains(act.Concepts, concept => concept is SelfSacrifice);
        }

        [Fact]
        public void OfferWorship_IsCovenantFaithfulness()
        {
            Act act = Deeds.OfferWorship();

            Assert.Contains(act.Concepts, concept => concept is CovenantFaithfulness);
        }

        [Fact]
        public void ShedInnocentBlood_IsMurder()
        {
            Act act = Deeds.ShedInnocentBlood();

            Assert.Contains(act.Concepts, concept => concept is Murder);
        }

        [Fact]
        public void Steal_IsTheft()
        {
            Act act = Deeds.Steal();

            Assert.Contains(act.Concepts, concept => concept is Theft);
        }

        [Fact]
        public void RefuseMercy_IsUnforgivenessAndCondemnation()
        {
            Act act = Deeds.RefuseMercy();

            Assert.Contains(act.Concepts, concept => concept is Unforgiveness);
            Assert.Contains(act.Concepts, concept => concept is Condemnation);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~DeedsTests"`
Expected: BUILD FAILURE — `The type or namespace name 'Deeds' could not be found`.

- [ ] **Step 3: Implement `Deeds`**

Create `AnointedAutomation.Objects/Concepts/Reality/Deeds.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A starter catalog of concrete deeds a person can choose to act out, composed from the moral
    /// concepts already in the engine. Small on purpose; it grows as the game needs more choices.
    /// </summary>
    public static class Deeds
    {
        /// <summary>
        /// Binding a stranger's wounds: compassion and protection (Luke 10:34).
        /// </summary>
        /// <returns>The deed.</returns>
        public static Act BindAStrangersWounds()
        {
            return new Act("binding a stranger's wounds", new Compassion(), new Protection());
        }

        /// <summary>
        /// Laying down your life for a friend: the greater love (John 15:13).
        /// </summary>
        /// <returns>The deed.</returns>
        public static Act LayDownYourLife()
        {
            return new Act("laying down your life for a friend", new SelfSacrifice(), new Compassion());
        }

        /// <summary>
        /// Offering true worship: covenant faithfulness.
        /// </summary>
        /// <returns>The deed.</returns>
        public static Act OfferWorship()
        {
            return new Act("a sacrifice offered", new CovenantFaithfulness());
        }

        /// <summary>
        /// Shedding innocent blood: murder (Genesis 4).
        /// </summary>
        /// <returns>The deed.</returns>
        public static Act ShedInnocentBlood()
        {
            return new Act("the shedding of innocent blood", new Murder());
        }

        /// <summary>
        /// Taking what is not yours: theft.
        /// </summary>
        /// <returns>The deed.</returns>
        public static Act Steal()
        {
            return new Act("taking what is not yours", new Theft());
        }

        /// <summary>
        /// Refusing mercy to one in your debt: the unforgiving servant (Matthew 18).
        /// </summary>
        /// <returns>The deed.</returns>
        public static Act RefuseMercy()
        {
            return new Act("mercy refused", new Unforgiveness(), new Condemnation());
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~DeedsTests"`
Expected: PASS (all 6 facts).

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Reality/Deeds.cs AnointedAutomation.Objects.Tests/DeedsTests.cs
git commit -m "Add Deeds starter catalog"
```

---

## Task 8: `PersonDemo` — the game flow

A console walkthrough showing the two axes diverging. No unit test; verified by building and running
the demo and reading its output.

**Files:**
- Create: `AnointedAutomation.Objects.Demo/PersonDemo.cs`
- Modify: `AnointedAutomation.Objects.Demo/Program.cs`

- [ ] **Step 1: Create the demo**

Create `AnointedAutomation.Objects.Demo/PersonDemo.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using System;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Demo
{
    /// <summary>
    /// Demonstrates the two axes of the Person cluster: a person professes a belief, but their
    /// grounding, whether they follow Christ, and their integrity are revealed only by the way they
    /// live. A professed Christian who will not follow drifts to the idol of the self; a Buddhist who
    /// lives the way follows Christ unknowingly.
    /// </summary>
    public static class PersonDemo
    {
        public static void Run()
        {
            Console.WriteLine("=== Belief is professed; the way of living reveals the heart ===");
            Console.WriteLine();

            Reality reality = Reality.Revealed();

            Person paul = new Person("Paul", Beliefs.Christianity);
            Show("Paul professes Christ, before acting", paul);
            paul.Do(Deeds.BindAStrangersWounds(), reality);
            Show("Paul binds a stranger's wounds", paul);

            Person nominal = new Person("Demas", Beliefs.Christianity);
            nominal.Do(Deeds.ShedInnocentBlood(), reality);
            Show("Demas professes Christ but sheds innocent blood", nominal);

            Person sid = new Person("Siddhartha", Beliefs.Buddhism);
            sid.Do(Deeds.LayDownYourLife(), reality);
            Show("Siddhartha (a non-theistic path) lays down his life", sid);

            Person seeker = new Person("Seeker");
            Show("An agnostic who has not acted", seeker);

            Console.WriteLine();
        }

        private static void Show(string title, Person person)
        {
            Console.WriteLine(title);
            Console.WriteLine("  grounding: " + person.Grounding.Name
                + " | follows Christ: " + person.FollowsChrist
                + " | integrity: " + Format(person.Integrity()));
            Console.WriteLine();
        }

        private static string Format(double value)
        {
            return value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
```

- [ ] **Step 2: Wire it into `Program.cs`**

In `AnointedAutomation.Objects.Demo/Program.cs`, add a call to `PersonDemo.Run()` immediately after
the existing `RealityDemo.Run();` line (near the end of `Main`):

```csharp
            Console.WriteLine();
            RealityDemo.Run();
            PersonDemo.Run();
```

- [ ] **Step 3: Build and run the demo**

Run: `dotnet run --project AnointedAutomation.Objects.Demo/AnointedAutomation.Objects.Demo.csproj`
Expected output includes the Person section, with these readings:
- "Paul professes Christ, before acting" → grounding `a divided heart`, follows Christ `False`.
- "Paul binds a stranger's wounds" → grounding `God`, follows Christ `True`.
- "Demas ... sheds innocent blood" → grounding `the self`, follows Christ `False`.
- "Siddhartha ... lays down his life" → follows Christ `True`.
- "An agnostic who has not acted" → grounding `a divided heart`, follows Christ `False`.

- [ ] **Step 4: Commit**

```bash
git add AnointedAutomation.Objects.Demo/PersonDemo.cs AnointedAutomation.Objects.Demo/Program.cs
git commit -m "Add PersonDemo game flow"
```

---

## Task 9: Full suite + documentation

- [ ] **Step 1: Run the whole Objects test suite**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj`
Expected: PASS (all tests, including the pre-existing ones, with the new Belief/Beliefs/Life/Heart/Person/Deeds facts).

- [ ] **Step 2: Update PROJECT documentation**

Update `PROJECT_STRUCTURE_CODE.md` to list the new `Concepts/Reality` types (`Belief`, `Beliefs`,
`Life`, `Heart`, `Person`, `Deeds`) and the `Grounding.Lifelessness` addition. Update
`PROJECT_STRUCTURE_DICTIONARY.md` if it enumerates files. Record the completed work in
`PROJECT_WORK.md`.

- [ ] **Step 3: Commit**

```bash
git add PROJECT_STRUCTURE_CODE.md PROJECT_STRUCTURE_DICTIONARY.md PROJECT_WORK.md
git commit -m "Document Person cluster"
```

---

## Notes for the implementer

- **Why the empty-life check in `Heart.Reveal` comes before the coherence check:** an empty `Life`
  reads `Coherence() == 1.0` by the product formula, which would falsely look like perfect following.
  The explicit `History().Count == 0` guard is what makes "everyone starts unproven" true.
- **The 0.75 threshold and one godly deed:** a fresh person acts on `Divided` grounding (lifelessness
  0.25), so a clean good deed is borne with disorder `0.25`, giving fruit coherence exactly `0.75`,
  which meets `FollowsThreshold`. This is deterministic (0.25 and 0.75 are exact in double); the
  `PersonTests` rely on it.
- **Property names `Grounding` and `Life` on `Person`** intentionally match their types; C# resolves
  the member in expression context, so `reality.Witness(act, Grounding)` uses the property.
- **`Heart` is tested in isolation** with hand-built `Resolution`s (Task 5) and **through the real
  pipeline** via `Person` (Task 6); both layers matter.
