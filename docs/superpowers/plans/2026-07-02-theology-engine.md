# Theology Engine (Epistemics) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the epistemic ledger engine from the approved spec: a consistency-mapping engine that examines theological claims against foundational claims (survived-falsification regularities) and each other, with three-valued (`bool?`) truth, four-state verdicts, and contradictions as data.

**Architecture:** New folder `AnointedAutomation.Objects/Concepts/Epistemics/` with immutable value types (`Proposition`, `FoundationalClaim`, `TheologicalClaim`, `Examination`, `Tension`) and one engine class (`EpistemicLedger`). Claims interact only through a shared proposition vocabulary; checking is set logic over propositions. Tests in `AnointedAutomation.Objects.Tests/Epistemics/`.

**Tech Stack:** C# / net10.0, xUnit 2.9.3, no new dependencies.

**Spec:** `docs/superpowers/specs/2026-07-02-theology-engine-design.md` (binding; read its "Epistemological commitments" section first).

## Global Constraints

- Explicit types everywhere; NO `var`.
- NO `==` / `!=` for string comparison; use `.Equals()` (ordinal).
- NO fallback values; `null` standings propagate as `null`; invalid input throws immediately.
- `ImplicitUsings` and `Nullable` are disabled project-wide: fully qualify or `using System...;` explicitly; `bool?` is fine (it is `System.Nullable<bool>`).
- `TreatWarningsAsErrors` is on; every public member needs XML docs (project style documents everything).
- Every file starts with this exact header (two lines):
  `// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02`
  `// Stewarded by Alexander Fields`
- Namespace for all new code: `AnointedAutomation.Objects.Concepts.Epistemics`. Test namespace: `AnointedAutomation.Objects.Tests.Epistemics`.
- Commit messages: short and generic, NO AI attribution of any kind.
- NO em dashes or en dashes in any prose, docs, or comments.
- Contradiction is never an exception; exceptions are reserved for misuse (null args, empty propositions, out-of-range weights, duplicate foundational names).
- Build/test from repo root: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj`.

---

### Task 1: Proposition and Testability

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/Testability.cs`
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/Proposition.cs`
- Test: `AnointedAutomation.Objects.Tests/Epistemics/PropositionTests.cs`

**Interfaces:**
- Consumes: nothing.
- Produces: `enum Testability { EmpiricallyTestable, BeyondObservation }`; `class Proposition` with ctor `Proposition(string name, string description, Testability testability)` and ctor `Proposition(string name, string description, Testability testability, bool? standing)`, properties `string Name`, `string Description`, `Testability Testability`, `bool? Standing`; equality and hash code by ordinal `Name`.

- [ ] **Step 1: Write the failing tests**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class PropositionTests
    {
        [Fact]
        public void Proposition_DefaultsStandingToNull()
        {
            // null is the honest state: unknown, untested, or unknowable from inside the universe.
            Proposition creator = new Proposition(
                "Creator",
                "A creator of the universe exists.",
                Testability.BeyondObservation);

            Assert.Null(creator.Standing);
            Assert.Equal(Testability.BeyondObservation, creator.Testability);
        }

        [Fact]
        public void Proposition_CarriesExplicitStanding()
        {
            Proposition entropy = new Proposition(
                "EntropyIncreases",
                "Entropy of a closed system does not decrease.",
                Testability.EmpiricallyTestable,
                true);

            Assert.True(entropy.Standing);
        }

        [Fact]
        public void Proposition_EqualityIsByName()
        {
            Proposition a = new Proposition("Creator", "one wording", Testability.BeyondObservation);
            Proposition b = new Proposition("Creator", "another wording", Testability.BeyondObservation);

            Assert.Equal(a, b);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void Proposition_ThrowsOnNullOrEmptyName()
        {
            Assert.Throws<System.ArgumentException>(
                () => new Proposition(null, "d", Testability.EmpiricallyTestable));
            Assert.Throws<System.ArgumentException>(
                () => new Proposition("", "d", Testability.EmpiricallyTestable));
        }

        [Fact]
        public void Proposition_ThrowsOnNullDescription()
        {
            Assert.Throws<System.ArgumentNullException>(
                () => new Proposition("Creator", null, Testability.BeyondObservation));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~PropositionTests"`
Expected: build FAILURE, `Proposition` and `Testability` do not exist.

- [ ] **Step 3: Write the implementation**

`Testability.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// Whether a proposition can, even in principle, be tested from inside the universe. This
    /// drives the <see cref="Verdict.Unfalsifiable"/> verdict: origin-of-universe and
    /// outside-the-universe subject matter cannot be checked by observers who cannot step outside
    /// to look.
    /// </summary>
    public enum Testability
    {
        /// <summary>
        /// Observation or experiment inside the universe can bear on it.
        /// </summary>
        EmpiricallyTestable,

        /// <summary>
        /// No observation from inside the universe can ever settle it.
        /// </summary>
        BeyondObservation
    }
}
```

`Proposition.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// One entry in the shared vocabulary through which claims interact. The engine does not parse
    /// natural language; claims declare which propositions they assert and deny, and checking is
    /// set logic over these. Standing is three-valued: true (asserted, so far unfalsified), false
    /// (falsified), null (unknown, untested, or unknowable). Null is the honest state for most of
    /// theology and is never defaulted or guessed away.
    /// </summary>
    public class Proposition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Proposition"/> class with an unknown
        /// (null) standing.
        /// </summary>
        /// <param name="name">The vocabulary name, e.g. "CreatedUniverse".</param>
        /// <param name="description">What the proposition asserts, in plain words.</param>
        /// <param name="testability">Whether it can be tested from inside the universe.</param>
        public Proposition(string name, string description, Testability testability)
            : this(name, description, testability, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Proposition"/> class with an explicit
        /// three-valued standing.
        /// </summary>
        /// <param name="name">The vocabulary name, e.g. "CreatedUniverse".</param>
        /// <param name="description">What the proposition asserts, in plain words.</param>
        /// <param name="testability">Whether it can be tested from inside the universe.</param>
        /// <param name="standing">True (so far unfalsified), false (falsified), or null (unknown).</param>
        public Proposition(string name, string description, Testability testability, bool? standing)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("A proposition requires a name.", nameof(name));
            }

            if (description == null)
            {
                throw new System.ArgumentNullException(nameof(description));
            }

            Name = name;
            Description = description;
            Testability = testability;
            Standing = standing;
        }

        /// <summary>
        /// The vocabulary name. Propositions are equal when their names match ordinally, so two
        /// claims touch the same proposition by using the same name.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// What the proposition asserts, in plain words.
        /// </summary>
        public string Description
        {
            get;
        }

        /// <summary>
        /// Whether the proposition can be tested from inside the universe.
        /// </summary>
        public Testability Testability
        {
            get;
        }

        /// <summary>
        /// The three-valued standing: true (so far unfalsified), false (falsified), or null
        /// (unknown, untested, or unknowable).
        /// </summary>
        public bool? Standing
        {
            get;
        }

        /// <summary>
        /// Two propositions are the same vocabulary entry when their names match ordinally.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True when <paramref name="obj"/> is a proposition with the same name.</returns>
        public override bool Equals(object obj)
        {
            Proposition other = obj as Proposition;
            if (other == null)
            {
                return false;
            }

            return Name.Equals(other.Name, System.StringComparison.Ordinal);
        }

        /// <summary>
        /// Hash code derived from the name, matching <see cref="Equals(object)"/>.
        /// </summary>
        /// <returns>The ordinal hash of the name.</returns>
        public override int GetHashCode()
        {
            return System.StringComparer.Ordinal.GetHashCode(Name);
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~PropositionTests"`
Expected: 5 passed.

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Epistemics/Testability.cs AnointedAutomation.Objects/Concepts/Epistemics/Proposition.cs AnointedAutomation.Objects.Tests/Epistemics/PropositionTests.cs
git commit -m "Add epistemics proposition vocabulary"
```

---

### Task 2: LawDomain and FoundationalClaim

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/LawDomain.cs`
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/FoundationalClaim.cs`
- Test: `AnointedAutomation.Objects.Tests/Epistemics/FoundationalClaimTests.cs`

**Interfaces:**
- Consumes: `Proposition` (Task 1).
- Produces: `enum LawDomain { IntraUniverse, Unrestricted }`; `class FoundationalClaim` with ctor `FoundationalClaim(string name, string statement, LawDomain domain, System.Collections.Generic.IEnumerable<Proposition> asserts, System.Collections.Generic.IEnumerable<Proposition> denies, double survivedFalsificationWeight)`, properties `string Name`, `string Statement`, `LawDomain Domain`, `System.Collections.Generic.IReadOnlyCollection<Proposition> Asserts`, `System.Collections.Generic.IReadOnlyCollection<Proposition> Denies`, `bool Falsifiable` (always true), `double SurvivedFalsificationWeight`; methods `bool AssertsProposition(Proposition proposition)`, `bool DeniesProposition(Proposition proposition)`.

- [ ] **Step 1: Write the failing tests**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class FoundationalClaimTests
    {
        private static Proposition Causality()
        {
            return new Proposition(
                "EffectsHaveCauses",
                "Within the universe, effects have causes.",
                Testability.EmpiricallyTestable,
                true);
        }

        [Fact]
        public void FoundationalClaim_IsFalsifiableByDefinition()
        {
            // Even laws are held by faith: asserted, and open to being proven wrong. That is what
            // makes them scientific rather than decreed.
            FoundationalClaim causality = new FoundationalClaim(
                "Causality",
                "Within the universe, effects have causes.",
                LawDomain.IntraUniverse,
                new List<Proposition> { Causality() },
                new List<Proposition>(),
                0.99);

            Assert.True(causality.Falsifiable);
            Assert.Equal(0.99, causality.SurvivedFalsificationWeight);
            Assert.True(causality.AssertsProposition(Causality()));
            Assert.False(causality.DeniesProposition(Causality()));
        }

        [Fact]
        public void FoundationalClaim_ThrowsOnWeightOutOfRange()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new FoundationalClaim(
                "Causality",
                "s",
                LawDomain.IntraUniverse,
                new List<Proposition> { Causality() },
                new List<Proposition>(),
                1.5));
        }

        [Fact]
        public void FoundationalClaim_ThrowsWhenItTouchesNoPropositions()
        {
            Assert.Throws<System.ArgumentException>(() => new FoundationalClaim(
                "Empty",
                "s",
                LawDomain.IntraUniverse,
                new List<Proposition>(),
                new List<Proposition>(),
                0.5));
        }

        [Fact]
        public void FoundationalClaim_ThrowsOnNullArguments()
        {
            Assert.Throws<System.ArgumentException>(() => new FoundationalClaim(
                null, "s", LawDomain.IntraUniverse,
                new List<Proposition> { Causality() }, new List<Proposition>(), 0.5));
            Assert.Throws<System.ArgumentNullException>(() => new FoundationalClaim(
                "Causality", "s", LawDomain.IntraUniverse,
                null, new List<Proposition>(), 0.5));
            Assert.Throws<System.ArgumentNullException>(() => new FoundationalClaim(
                "Causality", "s", LawDomain.IntraUniverse,
                new List<Proposition> { Causality() }, null, 0.5));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~FoundationalClaimTests"`
Expected: build FAILURE, `FoundationalClaim` and `LawDomain` do not exist.

- [ ] **Step 3: Write the implementation**

`LawDomain.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// Where a foundational claim has authority. This boundary is what keeps the engine neutral: a
    /// regularity established inside the universe can never settle a claim about the origin of or
    /// outside the universe.
    /// </summary>
    public enum LawDomain
    {
        /// <summary>
        /// Authoritative only for what happens within the universe.
        /// </summary>
        IntraUniverse,

        /// <summary>
        /// Authoritative without restriction, e.g. the law of non-contradiction.
        /// </summary>
        Unrestricted
    }
}
```

`FoundationalClaim.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// The measuring stick of the ledger. Not an ontologically special "law": every foundational
    /// claim began as an assumption someone was willing to have proven wrong, so it is falsifiable
    /// by definition and functions as bedrock only by the weight of falsification it has survived.
    /// Examples: non-contradiction, causality, conservation of energy, entropy.
    /// </summary>
    public class FoundationalClaim
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FoundationalClaim"/> class.
        /// </summary>
        /// <param name="name">Unique name, e.g. "ConservationOfEnergy".</param>
        /// <param name="statement">The claim in plain words.</param>
        /// <param name="domain">Where the claim has authority.</param>
        /// <param name="asserts">Propositions the claim asserts.</param>
        /// <param name="denies">Propositions the claim denies.</param>
        /// <param name="survivedFalsificationWeight">How much testing it has survived, 0.0 to 1.0.</param>
        public FoundationalClaim(
            string name,
            string statement,
            LawDomain domain,
            System.Collections.Generic.IEnumerable<Proposition> asserts,
            System.Collections.Generic.IEnumerable<Proposition> denies,
            double survivedFalsificationWeight)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("A foundational claim requires a name.", nameof(name));
            }

            if (statement == null)
            {
                throw new System.ArgumentNullException(nameof(statement));
            }

            if (asserts == null)
            {
                throw new System.ArgumentNullException(nameof(asserts));
            }

            if (denies == null)
            {
                throw new System.ArgumentNullException(nameof(denies));
            }

            if (survivedFalsificationWeight < 0.0 || survivedFalsificationWeight > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(survivedFalsificationWeight),
                    "Survived-falsification weight must be between 0.0 and 1.0.");
            }

            System.Collections.Generic.List<Proposition> assertList =
                new System.Collections.Generic.List<Proposition>(asserts);
            System.Collections.Generic.List<Proposition> denyList =
                new System.Collections.Generic.List<Proposition>(denies);

            if (assertList.Count == 0 && denyList.Count == 0)
            {
                throw new System.ArgumentException(
                    "A foundational claim must assert or deny at least one proposition.",
                    nameof(asserts));
            }

            Name = name;
            Statement = statement;
            Domain = domain;
            Asserts = assertList;
            Denies = denyList;
            SurvivedFalsificationWeight = survivedFalsificationWeight;
        }

        /// <summary>
        /// Unique name of the foundational claim.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// The claim in plain words.
        /// </summary>
        public string Statement
        {
            get;
        }

        /// <summary>
        /// Where the claim has authority.
        /// </summary>
        public LawDomain Domain
        {
            get;
        }

        /// <summary>
        /// Propositions the claim asserts.
        /// </summary>
        public System.Collections.Generic.IReadOnlyCollection<Proposition> Asserts
        {
            get;
        }

        /// <summary>
        /// Propositions the claim denies.
        /// </summary>
        public System.Collections.Generic.IReadOnlyCollection<Proposition> Denies
        {
            get;
        }

        /// <summary>
        /// Always true. An unfalsifiable foundational claim would be a decree, not science; the
        /// faith step at the root of the method is explicit in the model.
        /// </summary>
        public bool Falsifiable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// How much falsification the claim has survived, 0.0 to 1.0. This weight, not type-level
        /// specialness, is why it functions as bedrock.
        /// </summary>
        public double SurvivedFalsificationWeight
        {
            get;
        }

        /// <summary>
        /// Whether this claim asserts the given proposition (by vocabulary name).
        /// </summary>
        /// <param name="proposition">The proposition to look for.</param>
        /// <returns>True when the proposition is asserted.</returns>
        public bool AssertsProposition(Proposition proposition)
        {
            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            return Contains(Asserts, proposition);
        }

        /// <summary>
        /// Whether this claim denies the given proposition (by vocabulary name).
        /// </summary>
        /// <param name="proposition">The proposition to look for.</param>
        /// <returns>True when the proposition is denied.</returns>
        public bool DeniesProposition(Proposition proposition)
        {
            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            return Contains(Denies, proposition);
        }

        private static bool Contains(
            System.Collections.Generic.IReadOnlyCollection<Proposition> propositions,
            Proposition proposition)
        {
            foreach (Proposition candidate in propositions)
            {
                if (candidate.Equals(proposition))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~FoundationalClaimTests"`
Expected: 4 passed.

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Epistemics/LawDomain.cs AnointedAutomation.Objects/Concepts/Epistemics/FoundationalClaim.cs AnointedAutomation.Objects.Tests/Epistemics/FoundationalClaimTests.cs
git commit -m "Add foundational claims with law domains"
```

---

### Task 3: TheologicalClaim

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/TheologicalClaim.cs`
- Test: `AnointedAutomation.Objects.Tests/Epistemics/TheologicalClaimTests.cs`

**Interfaces:**
- Consumes: `Proposition` (Task 1).
- Produces: `class TheologicalClaim` with ctor `TheologicalClaim(string statement, string source, double confidence, System.Collections.Generic.IEnumerable<Proposition> asserts, System.Collections.Generic.IEnumerable<Proposition> denies)`, properties `string Statement`, `string Source`, `double Confidence`, `System.Collections.Generic.IReadOnlyCollection<Proposition> Asserts`, `System.Collections.Generic.IReadOnlyCollection<Proposition> Denies`; methods `bool AssertsProposition(Proposition proposition)`, `bool DeniesProposition(Proposition proposition)` (same semantics as `FoundationalClaim`).

- [ ] **Step 1: Write the failing tests**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class TheologicalClaimTests
    {
        private static Proposition CreatedUniverse()
        {
            return new Proposition(
                "CreatedUniverse",
                "The universe was brought into being by something outside itself.",
                Testability.BeyondObservation);
        }

        [Fact]
        public void TheologicalClaim_CarriesSourceAndConfidence()
        {
            TheologicalClaim genesis = new TheologicalClaim(
                "In the beginning God created the heavens and the earth.",
                "Genesis 1:1",
                0.9,
                new List<Proposition> { CreatedUniverse() },
                new List<Proposition>());

            Assert.Equal("Genesis 1:1", genesis.Source);
            Assert.Equal(0.9, genesis.Confidence);
            Assert.True(genesis.AssertsProposition(CreatedUniverse()));
            Assert.False(genesis.DeniesProposition(CreatedUniverse()));
        }

        [Fact]
        public void TheologicalClaim_ThrowsWhenItTouchesNoPropositions()
        {
            Assert.Throws<System.ArgumentException>(() => new TheologicalClaim(
                "s", "src", 0.5, new List<Proposition>(), new List<Proposition>()));
        }

        [Fact]
        public void TheologicalClaim_ThrowsOnConfidenceOutOfRange()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new TheologicalClaim(
                "s", "src", -0.1,
                new List<Proposition> { CreatedUniverse() }, new List<Proposition>()));
        }

        [Fact]
        public void TheologicalClaim_ThrowsOnNullArguments()
        {
            Assert.Throws<System.ArgumentException>(() => new TheologicalClaim(
                null, "src", 0.5, new List<Proposition> { CreatedUniverse() }, new List<Proposition>()));
            Assert.Throws<System.ArgumentException>(() => new TheologicalClaim(
                "s", null, 0.5, new List<Proposition> { CreatedUniverse() }, new List<Proposition>()));
            Assert.Throws<System.ArgumentNullException>(() => new TheologicalClaim(
                "s", "src", 0.5, null, new List<Proposition>()));
            Assert.Throws<System.ArgumentNullException>(() => new TheologicalClaim(
                "s", "src", 0.5, new List<Proposition> { CreatedUniverse() }, null));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~TheologicalClaimTests"`
Expected: build FAILURE, `TheologicalClaim` does not exist.

- [ ] **Step 3: Write the implementation**

`TheologicalClaim.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// A claim in the theory layer: a theological or metaphysical statement made by a tradition,
    /// carrying its source and a confidence. Claims are data; they interact with foundational
    /// claims and with one another only through the shared proposition vocabulary. "Materialist
    /// cosmology" claims sit here on exactly the same footing as "Genesis 1:1" claims.
    /// </summary>
    public class TheologicalClaim
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheologicalClaim"/> class.
        /// </summary>
        /// <param name="statement">The claim in plain words.</param>
        /// <param name="source">Tradition plus citation, e.g. "Genesis 1:1" or "materialist cosmology".</param>
        /// <param name="confidence">How firmly the source holds it, 0.0 to 1.0.</param>
        /// <param name="asserts">Propositions the claim asserts.</param>
        /// <param name="denies">Propositions the claim denies.</param>
        public TheologicalClaim(
            string statement,
            string source,
            double confidence,
            System.Collections.Generic.IEnumerable<Proposition> asserts,
            System.Collections.Generic.IEnumerable<Proposition> denies)
        {
            if (string.IsNullOrWhiteSpace(statement))
            {
                throw new System.ArgumentException("A claim requires a statement.", nameof(statement));
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new System.ArgumentException("A claim requires a source.", nameof(source));
            }

            if (asserts == null)
            {
                throw new System.ArgumentNullException(nameof(asserts));
            }

            if (denies == null)
            {
                throw new System.ArgumentNullException(nameof(denies));
            }

            if (confidence < 0.0 || confidence > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(confidence),
                    "Confidence must be between 0.0 and 1.0.");
            }

            System.Collections.Generic.List<Proposition> assertList =
                new System.Collections.Generic.List<Proposition>(asserts);
            System.Collections.Generic.List<Proposition> denyList =
                new System.Collections.Generic.List<Proposition>(denies);

            if (assertList.Count == 0 && denyList.Count == 0)
            {
                throw new System.ArgumentException(
                    "A claim must assert or deny at least one proposition.",
                    nameof(asserts));
            }

            Statement = statement;
            Source = source;
            Confidence = confidence;
            Asserts = assertList;
            Denies = denyList;
        }

        /// <summary>
        /// The claim in plain words.
        /// </summary>
        public string Statement
        {
            get;
        }

        /// <summary>
        /// Tradition plus citation, e.g. "Genesis 1:1" or "materialist cosmology".
        /// </summary>
        public string Source
        {
            get;
        }

        /// <summary>
        /// How firmly the source holds the claim, 0.0 to 1.0.
        /// </summary>
        public double Confidence
        {
            get;
        }

        /// <summary>
        /// Propositions the claim asserts.
        /// </summary>
        public System.Collections.Generic.IReadOnlyCollection<Proposition> Asserts
        {
            get;
        }

        /// <summary>
        /// Propositions the claim denies.
        /// </summary>
        public System.Collections.Generic.IReadOnlyCollection<Proposition> Denies
        {
            get;
        }

        /// <summary>
        /// Whether this claim asserts the given proposition (by vocabulary name).
        /// </summary>
        /// <param name="proposition">The proposition to look for.</param>
        /// <returns>True when the proposition is asserted.</returns>
        public bool AssertsProposition(Proposition proposition)
        {
            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            return Contains(Asserts, proposition);
        }

        /// <summary>
        /// Whether this claim denies the given proposition (by vocabulary name).
        /// </summary>
        /// <param name="proposition">The proposition to look for.</param>
        /// <returns>True when the proposition is denied.</returns>
        public bool DeniesProposition(Proposition proposition)
        {
            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            return Contains(Denies, proposition);
        }

        private static bool Contains(
            System.Collections.Generic.IReadOnlyCollection<Proposition> propositions,
            Proposition proposition)
        {
            foreach (Proposition candidate in propositions)
            {
                if (candidate.Equals(proposition))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~TheologicalClaimTests"`
Expected: 4 passed.

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Epistemics/TheologicalClaim.cs AnointedAutomation.Objects.Tests/Epistemics/TheologicalClaimTests.cs
git commit -m "Add theological claims"
```

---

### Task 4: Verdict, DerivationStep, Examination

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/Verdict.cs`
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/DerivationStep.cs`
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/Examination.cs`
- Test: `AnointedAutomation.Objects.Tests/Epistemics/ExaminationTests.cs`

**Interfaces:**
- Consumes: `TheologicalClaim` (Task 3).
- Produces:
  - `enum Verdict { Consistent, Contradicts, Unfalsifiable, Undetermined }`
  - `class DerivationStep` with ctor `DerivationStep(string authority, string propositionName, string outcome)` and properties `string Authority`, `string PropositionName`, `string Outcome`.
  - `class Examination` with ctor `Examination(TheologicalClaim claim, Verdict verdict, double confidence, System.Collections.Generic.IReadOnlyList<DerivationStep> derivation)`, properties `TheologicalClaim Claim`, `Verdict Verdict`, `bool? Standing` (computed: `Consistent` yields true, `Contradicts` yields false, otherwise null), `double Confidence`, `System.Collections.Generic.IReadOnlyList<DerivationStep> Derivation`.

- [ ] **Step 1: Write the failing tests**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class ExaminationTests
    {
        private static TheologicalClaim AnyClaim()
        {
            Proposition creator = new Proposition(
                "Creator", "A creator exists.", Testability.BeyondObservation);
            return new TheologicalClaim(
                "A creator exists.", "theism", 0.8,
                new List<Proposition> { creator }, new List<Proposition>());
        }

        [Fact]
        public void Examination_MapsConsistentToTrueStanding()
        {
            Examination examination = new Examination(
                AnyClaim(), Verdict.Consistent, 0.8, new List<DerivationStep>());

            Assert.True(examination.Standing);
        }

        [Fact]
        public void Examination_MapsContradictsToFalseStanding()
        {
            Examination examination = new Examination(
                AnyClaim(), Verdict.Contradicts, 0.8, new List<DerivationStep>());

            Assert.False(examination.Standing);
        }

        [Fact]
        public void Examination_MapsBothNullFlavorsToNullStanding()
        {
            // Unfalsifiable and Undetermined are the two flavors of null: can never test from
            // inside, versus could test but have not sufficiently.
            Examination unfalsifiable = new Examination(
                AnyClaim(), Verdict.Unfalsifiable, 0.8, new List<DerivationStep>());
            Examination undetermined = new Examination(
                AnyClaim(), Verdict.Undetermined, 0.8, new List<DerivationStep>());

            Assert.Null(unfalsifiable.Standing);
            Assert.Null(undetermined.Standing);
        }

        [Fact]
        public void Examination_CarriesDerivation()
        {
            DerivationStep step = new DerivationStep(
                "Causality", "EffectsHaveCauses", "claim denies what the foundational claim asserts");
            Examination examination = new Examination(
                AnyClaim(), Verdict.Contradicts, 0.8, new List<DerivationStep> { step });

            Assert.Single(examination.Derivation);
            Assert.Equal("Causality", examination.Derivation[0].Authority);
            Assert.Equal("EffectsHaveCauses", examination.Derivation[0].PropositionName);
        }

        [Fact]
        public void Examination_ThrowsOnNullArguments()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Examination(
                null, Verdict.Consistent, 0.8, new List<DerivationStep>()));
            Assert.Throws<System.ArgumentNullException>(() => new Examination(
                AnyClaim(), Verdict.Consistent, 0.8, null));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~ExaminationTests"`
Expected: build FAILURE, types do not exist.

- [ ] **Step 3: Write the implementation**

`Verdict.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// What examining a claim yields. Four states rather than a boolean, because "contradicted"
    /// and "untestable" are different findings, and both differ from "not yet determined".
    /// Contradiction is a verdict, never an exception.
    /// </summary>
    public enum Verdict
    {
        /// <summary>
        /// Nothing falsified it; provisional and true-leaning, like every scientific claim.
        /// </summary>
        Consistent,

        /// <summary>
        /// Collides with the current unfalsified set.
        /// </summary>
        Contradicts,

        /// <summary>
        /// Can never be tested from inside the universe. A statement about testability, not truth,
        /// and symmetric across theism and atheism.
        /// </summary>
        Unfalsifiable,

        /// <summary>
        /// Testable in principle, but the evidence is insufficient.
        /// </summary>
        Undetermined
    }
}
```

`DerivationStep.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// One step in how a verdict was reached: which authority (foundational claim or peer claim)
    /// touched which proposition, and what happened, including recorded domain skips so the
    /// engine's neutrality is auditable.
    /// </summary>
    public class DerivationStep
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DerivationStep"/> class.
        /// </summary>
        /// <param name="authority">Name of the foundational or peer claim involved.</param>
        /// <param name="propositionName">The proposition the step turned on.</param>
        /// <param name="outcome">What happened, in plain words.</param>
        public DerivationStep(string authority, string propositionName, string outcome)
        {
            if (string.IsNullOrWhiteSpace(authority))
            {
                throw new System.ArgumentException("A step requires an authority.", nameof(authority));
            }

            if (string.IsNullOrWhiteSpace(propositionName))
            {
                throw new System.ArgumentException("A step requires a proposition.", nameof(propositionName));
            }

            if (string.IsNullOrWhiteSpace(outcome))
            {
                throw new System.ArgumentException("A step requires an outcome.", nameof(outcome));
            }

            Authority = authority;
            PropositionName = propositionName;
            Outcome = outcome;
        }

        /// <summary>
        /// Name of the foundational or peer claim involved.
        /// </summary>
        public string Authority
        {
            get;
        }

        /// <summary>
        /// The proposition the step turned on.
        /// </summary>
        public string PropositionName
        {
            get;
        }

        /// <summary>
        /// What happened, in plain words.
        /// </summary>
        public string Outcome
        {
            get;
        }
    }
}
```

`Examination.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// The result of examining one claim: a verdict, its three-valued standing, the confidence it
    /// was reached with (a conclusion is only as strong as its weakest premise), and the derivation
    /// showing its work.
    /// </summary>
    public class Examination
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Examination"/> class.
        /// </summary>
        /// <param name="claim">The claim that was examined.</param>
        /// <param name="verdict">The verdict reached.</param>
        /// <param name="confidence">Minimum confidence along the derivation chain, 0.0 to 1.0.</param>
        /// <param name="derivation">The ordered steps that produced the verdict.</param>
        public Examination(
            TheologicalClaim claim,
            Verdict verdict,
            double confidence,
            System.Collections.Generic.IReadOnlyList<DerivationStep> derivation)
        {
            if (claim == null)
            {
                throw new System.ArgumentNullException(nameof(claim));
            }

            if (derivation == null)
            {
                throw new System.ArgumentNullException(nameof(derivation));
            }

            if (confidence < 0.0 || confidence > 1.0)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(confidence),
                    "Confidence must be between 0.0 and 1.0.");
            }

            Claim = claim;
            Verdict = verdict;
            Confidence = confidence;
            Derivation = derivation;
        }

        /// <summary>
        /// The claim that was examined.
        /// </summary>
        public TheologicalClaim Claim
        {
            get;
        }

        /// <summary>
        /// The verdict reached.
        /// </summary>
        public Verdict Verdict
        {
            get;
        }

        /// <summary>
        /// The three-valued standing of the claim. Consistent leans true (provisional, like every
        /// scientific claim), Contradicts is false relative to the current unfalsified set, and
        /// both Unfalsifiable and Undetermined are honestly null. Null is never defaulted away.
        /// </summary>
        public bool? Standing
        {
            get
            {
                if (Verdict == Verdict.Consistent)
                {
                    return true;
                }

                if (Verdict == Verdict.Contradicts)
                {
                    return false;
                }

                return null;
            }
        }

        /// <summary>
        /// Minimum confidence along the derivation chain, 0.0 to 1.0.
        /// </summary>
        public double Confidence
        {
            get;
        }

        /// <summary>
        /// The ordered steps that produced the verdict, including recorded domain skips.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<DerivationStep> Derivation
        {
            get;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~ExaminationTests"`
Expected: 5 passed.

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Epistemics/Verdict.cs AnointedAutomation.Objects/Concepts/Epistemics/DerivationStep.cs AnointedAutomation.Objects/Concepts/Epistemics/Examination.cs AnointedAutomation.Objects.Tests/Epistemics/ExaminationTests.cs
git commit -m "Add verdicts and examinations"
```

---

### Task 5: Tension

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/Tension.cs`
- Test: `AnointedAutomation.Objects.Tests/Epistemics/TensionTests.cs`

**Interfaces:**
- Consumes: `TheologicalClaim`, `Proposition` (Tasks 1, 3).
- Produces: `class Tension` with ctor `Tension(TheologicalClaim first, TheologicalClaim second, Proposition proposition)`, properties `TheologicalClaim First`, `TheologicalClaim Second`, `Proposition Proposition`.

- [ ] **Step 1: Write the failing tests**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class TensionTests
    {
        [Fact]
        public void Tension_LinksBothClaimsAndTheSharedProposition()
        {
            Proposition created = new Proposition(
                "CreatedUniverse",
                "The universe was brought into being by something outside itself.",
                Testability.BeyondObservation);
            TheologicalClaim theist = new TheologicalClaim(
                "The universe was created.", "Genesis 1:1", 0.9,
                new List<Proposition> { created }, new List<Proposition>());
            TheologicalClaim materialist = new TheologicalClaim(
                "The universe is eternal and uncaused.", "materialist cosmology", 0.6,
                new List<Proposition>(), new List<Proposition> { created });

            Tension tension = new Tension(theist, materialist, created);

            // Neither claim is deleted or rejected; the contradiction is data.
            Assert.Same(theist, tension.First);
            Assert.Same(materialist, tension.Second);
            Assert.Equal("CreatedUniverse", tension.Proposition.Name);
        }

        [Fact]
        public void Tension_ThrowsOnNullArguments()
        {
            Proposition created = new Proposition(
                "CreatedUniverse", "d", Testability.BeyondObservation);
            TheologicalClaim claim = new TheologicalClaim(
                "s", "src", 0.5, new List<Proposition> { created }, new List<Proposition>());

            Assert.Throws<System.ArgumentNullException>(() => new Tension(null, claim, created));
            Assert.Throws<System.ArgumentNullException>(() => new Tension(claim, null, created));
            Assert.Throws<System.ArgumentNullException>(() => new Tension(claim, claim, null));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~TensionTests"`
Expected: build FAILURE, `Tension` does not exist.

- [ ] **Step 3: Write the implementation**

`Tension.cs`:

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// A first-class record of a contradiction between two theological claims: both claims and the
    /// shared proposition they disagree on. Neither claim is deleted or rejected; the ledger holds
    /// both, source-tagged, plus this record. Contradictions are output, never a crash.
    /// </summary>
    public class Tension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tension"/> class.
        /// </summary>
        /// <param name="first">One of the disagreeing claims.</param>
        /// <param name="second">The other disagreeing claim.</param>
        /// <param name="proposition">The proposition one asserts and the other denies.</param>
        public Tension(TheologicalClaim first, TheologicalClaim second, Proposition proposition)
        {
            if (first == null)
            {
                throw new System.ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new System.ArgumentNullException(nameof(second));
            }

            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            First = first;
            Second = second;
            Proposition = proposition;
        }

        /// <summary>
        /// One of the disagreeing claims.
        /// </summary>
        public TheologicalClaim First
        {
            get;
        }

        /// <summary>
        /// The other disagreeing claim.
        /// </summary>
        public TheologicalClaim Second
        {
            get;
        }

        /// <summary>
        /// The proposition one claim asserts and the other denies.
        /// </summary>
        public Proposition Proposition
        {
            get;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~TensionTests"`
Expected: 2 passed.

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Epistemics/Tension.cs AnointedAutomation.Objects.Tests/Epistemics/TensionTests.cs
git commit -m "Add tensions between claims"
```

---

### Task 6: EpistemicLedger.Examine

**Files:**
- Create: `AnointedAutomation.Objects/Concepts/Epistemics/EpistemicLedger.cs`
- Test: `AnointedAutomation.Objects.Tests/Epistemics/EpistemicLedgerExamineTests.cs`

**Interfaces:**
- Consumes: everything from Tasks 1 to 5.
- Produces: `class EpistemicLedger` with ctor `EpistemicLedger(System.Collections.Generic.IEnumerable<FoundationalClaim> foundations)` (throws `System.ArgumentException` on duplicate foundational names) and method `Examination Examine(TheologicalClaim claim)` (pure; stores nothing). Task 7 adds `Admit`, `Tensions`, and queries to this same class.

**Examine algorithm (spec section "Examine algorithm"; order encodes the epistemology):**
1. Foundational check. For every proposition the claim asserts or denies, for every foundational claim: if the foundation's `Domain` is `IntraUniverse` and the proposition's `Testability` is `BeyondObservation`, record a skip step ("domain skip: intra-universe authority does not reach beyond observation") and move on. Otherwise an assert/deny collision (claim asserts what foundation denies, or claim denies what foundation asserts) yields `Contradicts`, confidence `System.Math.Min(claim.Confidence, foundation.SurvivedFalsificationWeight)`, with a step recording the collision. First collision wins; remaining checks still run only to finish recording steps for the same proposition? No: stop scanning on the first collision (record it and return). Simplicity beats exhaustiveness in v1.
2. Falsifiability check. If every proposition the claim touches is `BeyondObservation`, verdict `Unfalsifiable`, confidence `claim.Confidence`, step recording "unfalsifiable from inside the universe".
3. Support check. If any applicable (non-skipped) foundational claim asserts a proposition the claim asserts, or denies one the claim denies, verdict `Consistent`, confidence `System.Math.Min(claim.Confidence, foundation.SurvivedFalsificationWeight)`, step recording the support.
4. Otherwise verdict `Undetermined`, confidence `claim.Confidence`, step recording "no applicable foundational claim bears on it" with authority "Ledger" and the first touched proposition's name.

- [ ] **Step 1: Write the failing tests**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using System.Linq;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class EpistemicLedgerExamineTests
    {
        private static readonly Proposition EffectsHaveCauses = new Proposition(
            "EffectsHaveCauses",
            "Within the universe, effects have causes.",
            Testability.EmpiricallyTestable,
            true);

        private static readonly Proposition EnergyConserved = new Proposition(
            "EnergyConserved",
            "Within the universe, energy is neither created nor destroyed.",
            Testability.EmpiricallyTestable,
            true);

        private static readonly Proposition CreatedUniverse = new Proposition(
            "CreatedUniverse",
            "The universe was brought into being by something outside itself.",
            Testability.BeyondObservation);

        private static readonly Proposition Creator = new Proposition(
            "Creator",
            "A creator of the universe exists.",
            Testability.BeyondObservation);

        private static EpistemicLedger NewLedger()
        {
            FoundationalClaim causality = new FoundationalClaim(
                "Causality",
                "Within the universe, effects have causes.",
                LawDomain.IntraUniverse,
                new List<Proposition> { EffectsHaveCauses },
                new List<Proposition>(),
                0.99);
            FoundationalClaim conservation = new FoundationalClaim(
                "ConservationOfEnergy",
                "Within the universe, energy is neither created nor destroyed.",
                LawDomain.IntraUniverse,
                new List<Proposition> { EnergyConserved },
                new List<Proposition>(),
                0.99);
            return new EpistemicLedger(new List<FoundationalClaim> { causality, conservation });
        }

        [Fact]
        public void Examine_ClaimDenyingCausalityInsideTheUniverse_Contradicts()
        {
            TheologicalClaim uncausedMiracles = new TheologicalClaim(
                "Events inside the universe routinely happen with no cause at all.",
                "test tradition",
                0.7,
                new List<Proposition>(),
                new List<Proposition> { EffectsHaveCauses });

            Examination examination = NewLedger().Examine(uncausedMiracles);

            Assert.Equal(Verdict.Contradicts, examination.Verdict);
            Assert.False(examination.Standing);
            // The foundational claim is named in the derivation.
            Assert.Contains(examination.Derivation,
                (DerivationStep step) => step.Authority.Equals("Causality"));
            // Weakest premise: min(0.7, 0.99).
            Assert.Equal(0.7, examination.Confidence);
        }

        [Fact]
        public void Examine_OriginClaims_AreUnfalsifiableSymmetrically()
        {
            // Neutrality: creator, no creator, and eternal uncaused matter all get the same
            // verdict, because none can be tested from inside the universe.
            EpistemicLedger ledger = NewLedger();
            TheologicalClaim theism = new TheologicalClaim(
                "A creator exists.", "theism", 0.9,
                new List<Proposition> { Creator }, new List<Proposition>());
            TheologicalClaim atheism = new TheologicalClaim(
                "No creator exists.", "atheism", 0.9,
                new List<Proposition>(), new List<Proposition> { Creator });
            TheologicalClaim eternalMatter = new TheologicalClaim(
                "The universe is eternal and uncaused.", "materialist cosmology", 0.9,
                new List<Proposition>(), new List<Proposition> { CreatedUniverse });

            Assert.Equal(Verdict.Unfalsifiable, ledger.Examine(theism).Verdict);
            Assert.Equal(Verdict.Unfalsifiable, ledger.Examine(atheism).Verdict);
            Assert.Equal(Verdict.Unfalsifiable, ledger.Examine(eternalMatter).Verdict);
            Assert.Null(ledger.Examine(theism).Standing);
            Assert.Null(ledger.Examine(atheism).Standing);
        }

        [Fact]
        public void Examine_IntraUniverseLaw_NeverSettlesAnOriginClaim_AndTheSkipIsRecorded()
        {
            // Conservation of energy is proven inside the universe; it cannot rule on the origin
            // of the universe. The skip is recorded so neutrality is auditable.
            TheologicalClaim eternalMatter = new TheologicalClaim(
                "The universe is eternal and uncaused.", "materialist cosmology", 0.9,
                new List<Proposition>(), new List<Proposition> { CreatedUniverse });

            Examination examination = NewLedger().Examine(eternalMatter);

            Assert.Equal(Verdict.Unfalsifiable, examination.Verdict);
            Assert.Contains(examination.Derivation,
                (DerivationStep step) => step.Outcome.Contains("domain skip"));
        }

        [Fact]
        public void Examine_ClaimAssertingWhatAFoundationAsserts_IsConsistent()
        {
            TheologicalClaim orderlyWorld = new TheologicalClaim(
                "The world runs on cause and effect.", "test tradition", 0.8,
                new List<Proposition> { EffectsHaveCauses }, new List<Proposition>());

            Examination examination = NewLedger().Examine(orderlyWorld);

            Assert.Equal(Verdict.Consistent, examination.Verdict);
            Assert.True(examination.Standing);
            Assert.Equal(0.8, examination.Confidence);
        }

        [Fact]
        public void Examine_TestableClaimNoFoundationBearsOn_IsUndetermined()
        {
            Proposition prayerHeals = new Proposition(
                "IntercessoryPrayerAffectsRecovery",
                "Intercessory prayer measurably affects medical recovery.",
                Testability.EmpiricallyTestable);
            TheologicalClaim claim = new TheologicalClaim(
                "Prayer heals the sick.", "test tradition", 0.6,
                new List<Proposition> { prayerHeals }, new List<Proposition>());

            Examination examination = NewLedger().Examine(claim);

            Assert.Equal(Verdict.Undetermined, examination.Verdict);
            Assert.Null(examination.Standing);
        }

        [Fact]
        public void Examine_ScientificMethodItself_IsUnfalsifiable()
        {
            // Faith at the root, formally acknowledged: the method cannot prove the method.
            Proposition methodYieldsTruth = new Proposition(
                "ScientificMethodYieldsTruth",
                "The scientific method yields truth about reality.",
                Testability.BeyondObservation);
            TheologicalClaim claim = new TheologicalClaim(
                "The scientific method yields truth.", "scientism", 0.9,
                new List<Proposition> { methodYieldsTruth }, new List<Proposition>());

            Examination examination = NewLedger().Examine(claim);

            Assert.Equal(Verdict.Unfalsifiable, examination.Verdict);
            Assert.Null(examination.Standing);
        }

        [Fact]
        public void Ledger_ThrowsOnDuplicateFoundationalNames()
        {
            FoundationalClaim a = new FoundationalClaim(
                "Causality", "s", LawDomain.IntraUniverse,
                new List<Proposition> { EffectsHaveCauses }, new List<Proposition>(), 0.9);
            FoundationalClaim b = new FoundationalClaim(
                "Causality", "s2", LawDomain.IntraUniverse,
                new List<Proposition> { EffectsHaveCauses }, new List<Proposition>(), 0.8);

            Assert.Throws<System.ArgumentException>(
                () => new EpistemicLedger(new List<FoundationalClaim> { a, b }));
        }

        [Fact]
        public void Examine_ThrowsOnNullClaim()
        {
            Assert.Throws<System.ArgumentNullException>(() => NewLedger().Examine(null));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~EpistemicLedgerExamineTests"`
Expected: build FAILURE, `EpistemicLedger` does not exist.

- [ ] **Step 3: Write the implementation**

`EpistemicLedger.cs` (Task 7 will extend this file; write it now with only the ctor, fields, and `Examine`):

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts.Epistemics
{
    /// <summary>
    /// The engine. Holds foundational claims (the measuring stick) and admitted theological claims
    /// (the theory layer), examines claims against both, and records tensions where traditions
    /// collide. It does not decide theology: contradiction and undecidability are output data,
    /// never errors, and the domain boundaries on foundational claims keep it neutral about
    /// anything that cannot be tested from inside the universe.
    /// </summary>
    public class EpistemicLedger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EpistemicLedger"/> class over a set of
        /// foundational claims.
        /// </summary>
        /// <param name="foundations">The hand-curated foundational claims.</param>
        public EpistemicLedger(System.Collections.Generic.IEnumerable<FoundationalClaim> foundations)
        {
            if (foundations == null)
            {
                throw new System.ArgumentNullException(nameof(foundations));
            }

            this.foundations = new System.Collections.Generic.List<FoundationalClaim>();
            System.Collections.Generic.HashSet<string> names =
                new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
            foreach (FoundationalClaim foundation in foundations)
            {
                if (!names.Add(foundation.Name))
                {
                    throw new System.ArgumentException(
                        "Duplicate foundational claim name: " + foundation.Name,
                        nameof(foundations));
                }

                this.foundations.Add(foundation);
            }

            admitted = new System.Collections.Generic.List<TheologicalClaim>();
            tensions = new System.Collections.Generic.List<Tension>();
        }

        private readonly System.Collections.Generic.List<FoundationalClaim> foundations;
        private readonly System.Collections.Generic.List<TheologicalClaim> admitted;
        private readonly System.Collections.Generic.List<Tension> tensions;

        /// <summary>
        /// Examines a claim against the foundational claims. Pure: nothing is stored. Order
        /// encodes the epistemology: foundational collisions first, then the falsifiability
        /// boundary, then support, else undetermined.
        /// </summary>
        /// <param name="claim">The claim to examine.</param>
        /// <returns>The examination, with verdict, standing, confidence, and derivation.</returns>
        public Examination Examine(TheologicalClaim claim)
        {
            if (claim == null)
            {
                throw new System.ArgumentNullException(nameof(claim));
            }

            System.Collections.Generic.List<DerivationStep> derivation =
                new System.Collections.Generic.List<DerivationStep>();
            System.Collections.Generic.List<Proposition> touched =
                new System.Collections.Generic.List<Proposition>(claim.Asserts);
            touched.AddRange(claim.Denies);

            // 1. Foundational check: collisions with the measuring stick, honoring domains.
            foreach (Proposition proposition in touched)
            {
                foreach (FoundationalClaim foundation in foundations)
                {
                    if (foundation.Domain == LawDomain.IntraUniverse
                        && proposition.Testability == Testability.BeyondObservation)
                    {
                        if (foundation.AssertsProposition(proposition)
                            || foundation.DeniesProposition(proposition))
                        {
                            derivation.Add(new DerivationStep(
                                foundation.Name,
                                proposition.Name,
                                "domain skip: intra-universe authority does not reach beyond observation"));
                        }

                        continue;
                    }

                    bool collision =
                        (claim.AssertsProposition(proposition) && foundation.DeniesProposition(proposition))
                        || (claim.DeniesProposition(proposition) && foundation.AssertsProposition(proposition));
                    if (collision)
                    {
                        derivation.Add(new DerivationStep(
                            foundation.Name,
                            proposition.Name,
                            "collision: the claim and the foundational claim cannot both stand"));
                        double contradictionConfidence = System.Math.Min(
                            claim.Confidence, foundation.SurvivedFalsificationWeight);
                        return new Examination(
                            claim, Verdict.Contradicts, contradictionConfidence, derivation);
                    }
                }
            }

            // 2. Falsifiability boundary: if nothing the claim touches can ever be observed from
            // inside the universe, the honest verdict is unfalsifiable, symmetrically for all
            // traditions.
            bool anyTestable = false;
            foreach (Proposition proposition in touched)
            {
                if (proposition.Testability == Testability.EmpiricallyTestable)
                {
                    anyTestable = true;
                    break;
                }
            }

            if (!anyTestable)
            {
                derivation.Add(new DerivationStep(
                    "Ledger",
                    touched[0].Name,
                    "unfalsifiable from inside the universe"));
                return new Examination(claim, Verdict.Unfalsifiable, claim.Confidence, derivation);
            }

            // 3. Support: an applicable foundational claim agreeing with the claim.
            foreach (Proposition proposition in touched)
            {
                foreach (FoundationalClaim foundation in foundations)
                {
                    if (foundation.Domain == LawDomain.IntraUniverse
                        && proposition.Testability == Testability.BeyondObservation)
                    {
                        continue;
                    }

                    bool support =
                        (claim.AssertsProposition(proposition) && foundation.AssertsProposition(proposition))
                        || (claim.DeniesProposition(proposition) && foundation.DeniesProposition(proposition));
                    if (support)
                    {
                        derivation.Add(new DerivationStep(
                            foundation.Name,
                            proposition.Name,
                            "support: the foundational claim agrees"));
                        double supportConfidence = System.Math.Min(
                            claim.Confidence, foundation.SurvivedFalsificationWeight);
                        return new Examination(
                            claim, Verdict.Consistent, supportConfidence, derivation);
                    }
                }
            }

            // 4. Testable in principle, but nothing on the ledger bears on it.
            derivation.Add(new DerivationStep(
                "Ledger",
                touched[0].Name,
                "no applicable foundational claim bears on it"));
            return new Examination(claim, Verdict.Undetermined, claim.Confidence, derivation);
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~EpistemicLedgerExamineTests"`
Expected: 8 passed.

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Epistemics/EpistemicLedger.cs AnointedAutomation.Objects.Tests/Epistemics/EpistemicLedgerExamineTests.cs
git commit -m "Add epistemic ledger examine"
```

---

### Task 7: Admit, Tensions, and queries

**Files:**
- Modify: `AnointedAutomation.Objects/Concepts/Epistemics/EpistemicLedger.cs` (add members after `Examine`)
- Test: `AnointedAutomation.Objects.Tests/Epistemics/EpistemicLedgerAdmitTests.cs`

**Interfaces:**
- Consumes: `EpistemicLedger` from Task 6 (fields `admitted` and `tensions` already exist).
- Produces, on `EpistemicLedger`:
  - `Examination Admit(TheologicalClaim claim)`: examines, records tensions against already admitted claims (assert/deny collision on a shared proposition), stores the claim, returns the examination.
  - `System.Collections.Generic.IReadOnlyList<Tension> Tensions { get; }`
  - `System.Collections.Generic.IReadOnlyList<TheologicalClaim> ClaimsAbout(Proposition proposition)`: admitted claims asserting or denying it.
  - `System.Collections.Generic.IReadOnlyList<TheologicalClaim> ClaimsFrom(string source)`: admitted claims whose `Source` equals it ordinally.

- [ ] **Step 1: Write the failing tests**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    public class EpistemicLedgerAdmitTests
    {
        private static readonly Proposition CreatedUniverse = new Proposition(
            "CreatedUniverse",
            "The universe was brought into being by something outside itself.",
            Testability.BeyondObservation);

        private static EpistemicLedger EmptyLedger()
        {
            Proposition effectsHaveCauses = new Proposition(
                "EffectsHaveCauses",
                "Within the universe, effects have causes.",
                Testability.EmpiricallyTestable,
                true);
            FoundationalClaim causality = new FoundationalClaim(
                "Causality",
                "Within the universe, effects have causes.",
                LawDomain.IntraUniverse,
                new List<Proposition> { effectsHaveCauses },
                new List<Proposition>(),
                0.99);
            return new EpistemicLedger(new List<FoundationalClaim> { causality });
        }

        private static TheologicalClaim Theist()
        {
            return new TheologicalClaim(
                "The universe was created.", "Genesis 1:1", 0.9,
                new List<Proposition> { CreatedUniverse }, new List<Proposition>());
        }

        private static TheologicalClaim Materialist()
        {
            return new TheologicalClaim(
                "The universe is eternal and uncaused.", "materialist cosmology", 0.6,
                new List<Proposition>(), new List<Proposition> { CreatedUniverse });
        }

        [Fact]
        public void Admit_TwoContradictingClaims_RecordsOneTension_BothClaimsStand()
        {
            EpistemicLedger ledger = EmptyLedger();
            TheologicalClaim theist = Theist();
            TheologicalClaim materialist = Materialist();

            ledger.Admit(theist);
            ledger.Admit(materialist);

            Tension tension = Assert.Single(ledger.Tensions);
            Assert.Equal("CreatedUniverse", tension.Proposition.Name);
            // Neither claim was deleted; both remain queryable.
            System.Collections.Generic.IReadOnlyList<TheologicalClaim> about =
                ledger.ClaimsAbout(CreatedUniverse);
            Assert.Equal(2, about.Count);
            Assert.Contains(theist, about);
            Assert.Contains(materialist, about);
        }

        [Fact]
        public void Admit_TensionDoesNotChangeTheVerdict()
        {
            // Peer collisions create tensions; they do not falsify either claim. Both origin
            // claims stay Unfalsifiable.
            EpistemicLedger ledger = EmptyLedger();
            ledger.Admit(Theist());

            Examination examination = ledger.Admit(Materialist());

            Assert.Equal(Verdict.Unfalsifiable, examination.Verdict);
            Assert.Null(examination.Standing);
        }

        [Fact]
        public void ClaimsFrom_FiltersBySourceOrdinally()
        {
            EpistemicLedger ledger = EmptyLedger();
            TheologicalClaim theist = Theist();
            ledger.Admit(theist);
            ledger.Admit(Materialist());

            System.Collections.Generic.IReadOnlyList<TheologicalClaim> fromGenesis =
                ledger.ClaimsFrom("Genesis 1:1");

            TheologicalClaim only = Assert.Single(fromGenesis);
            Assert.Same(theist, only);
        }

        [Fact]
        public void Admit_ThrowsOnNullClaim_AndQueriesThrowOnNull()
        {
            EpistemicLedger ledger = EmptyLedger();

            Assert.Throws<System.ArgumentNullException>(() => ledger.Admit(null));
            Assert.Throws<System.ArgumentNullException>(() => ledger.ClaimsAbout(null));
            Assert.Throws<System.ArgumentException>(() => ledger.ClaimsFrom(null));
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~EpistemicLedgerAdmitTests"`
Expected: build FAILURE, `Admit`, `Tensions`, `ClaimsAbout`, `ClaimsFrom` do not exist.

- [ ] **Step 3: Add the members to EpistemicLedger**

Append inside the `EpistemicLedger` class, after `Examine`:

```csharp
        /// <summary>
        /// All recorded tensions between admitted claims. A tension is a contradiction held as
        /// data: both claims stand, source-tagged, and the disagreement is queryable.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<Tension> Tensions
        {
            get
            {
                return tensions;
            }
        }

        /// <summary>
        /// Examines a claim, records any tensions with already admitted claims, stores the claim,
        /// and returns the examination. Peer collisions create tensions; they never change the
        /// verdict, because a disagreement between two theories falsifies neither.
        /// </summary>
        /// <param name="claim">The claim to admit.</param>
        /// <returns>The examination the claim received.</returns>
        public Examination Admit(TheologicalClaim claim)
        {
            Examination examination = Examine(claim);

            System.Collections.Generic.List<Proposition> touched =
                new System.Collections.Generic.List<Proposition>(claim.Asserts);
            touched.AddRange(claim.Denies);
            foreach (TheologicalClaim peer in admitted)
            {
                foreach (Proposition proposition in touched)
                {
                    bool collision =
                        (claim.AssertsProposition(proposition) && peer.DeniesProposition(proposition))
                        || (claim.DeniesProposition(proposition) && peer.AssertsProposition(proposition));
                    if (collision)
                    {
                        tensions.Add(new Tension(peer, claim, proposition));
                    }
                }
            }

            admitted.Add(claim);
            return examination;
        }

        /// <summary>
        /// Admitted claims that assert or deny the given proposition.
        /// </summary>
        /// <param name="proposition">The proposition to query.</param>
        /// <returns>The claims touching it, in admission order.</returns>
        public System.Collections.Generic.IReadOnlyList<TheologicalClaim> ClaimsAbout(Proposition proposition)
        {
            if (proposition == null)
            {
                throw new System.ArgumentNullException(nameof(proposition));
            }

            System.Collections.Generic.List<TheologicalClaim> matches =
                new System.Collections.Generic.List<TheologicalClaim>();
            foreach (TheologicalClaim claim in admitted)
            {
                if (claim.AssertsProposition(proposition) || claim.DeniesProposition(proposition))
                {
                    matches.Add(claim);
                }
            }

            return matches;
        }

        /// <summary>
        /// Admitted claims from the given source, compared ordinally.
        /// </summary>
        /// <param name="source">The source to query, e.g. "Genesis 1:1".</param>
        /// <returns>The claims from that source, in admission order.</returns>
        public System.Collections.Generic.IReadOnlyList<TheologicalClaim> ClaimsFrom(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new System.ArgumentException("A source is required.", nameof(source));
            }

            System.Collections.Generic.List<TheologicalClaim> matches =
                new System.Collections.Generic.List<TheologicalClaim>();
            foreach (TheologicalClaim claim in admitted)
            {
                if (claim.Source.Equals(source, System.StringComparison.Ordinal))
                {
                    matches.Add(claim);
                }
            }

            return matches;
        }
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj --filter "FullyQualifiedName~EpistemicLedgerAdmitTests"`
Expected: 4 passed.

- [ ] **Step 5: Commit**

```bash
git add AnointedAutomation.Objects/Concepts/Epistemics/EpistemicLedger.cs AnointedAutomation.Objects.Tests/Epistemics/EpistemicLedgerAdmitTests.cs
git commit -m "Add ledger admit, tensions, and queries"
```

---

### Task 8: Integration test, the worked example

**Files:**
- Test: `AnointedAutomation.Objects.Tests/Epistemics/WorkedExampleTests.cs`

**Interfaces:**
- Consumes: everything from Tasks 1 to 7. No new production code; this task locks the design conversation's worked example in as an executable specification.

- [ ] **Step 1: Write the test (expected to pass immediately; it is an integration lock, not TDD)**

```csharp
// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-07-02
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Objects.Concepts.Epistemics;

namespace AnointedAutomation.Objects.Tests.Epistemics
{
    /// <summary>
    /// The worked example from the design conversation, locked in as executable specification:
    /// conservation of energy, the eternal-matter claim, the cosmological argument, and atheism.
    /// The engine must stay neutral: it never declares "therefore God exists" and never lets an
    /// intra-universe law settle an origin claim, but it shows that "eternal uncaused energy" and
    /// "no creator" are theories with the same epistemic status as theism.
    /// </summary>
    public class WorkedExampleTests
    {
        private static readonly Proposition EnergyConserved = new Proposition(
            "EnergyConserved",
            "Within the universe, energy is neither created nor destroyed.",
            Testability.EmpiricallyTestable,
            true);

        private static readonly Proposition CreatedUniverse = new Proposition(
            "CreatedUniverse",
            "The universe was brought into being by something outside itself.",
            Testability.BeyondObservation);

        private static readonly Proposition Creator = new Proposition(
            "Creator",
            "A creator of the universe exists.",
            Testability.BeyondObservation);

        [Fact]
        public void TheConversation_PlaysOutOnTheLedger()
        {
            FoundationalClaim conservation = new FoundationalClaim(
                "ConservationOfEnergy",
                "Within the universe, energy is neither created nor destroyed.",
                LawDomain.IntraUniverse,
                new List<Proposition> { EnergyConserved },
                new List<Proposition>(),
                0.99);
            // An intra-universe extrapolation that, if illegitimately applied, would falsify the
            // cosmological argument. The engine must refuse: this is the neutrality property under
            // maximum pressure, and the refusal is recorded as a domain skip.
            FoundationalClaim conservationExtrapolation = new FoundationalClaim(
                "ConservationExtrapolation",
                "Extrapolation: conservation implies the total system needs no origin.",
                LawDomain.IntraUniverse,
                new List<Proposition>(),
                new List<Proposition> { CreatedUniverse },
                0.4);
            EpistemicLedger ledger = new EpistemicLedger(
                new List<FoundationalClaim> { conservation, conservationExtrapolation });

            TheologicalClaim eternalMatter = new TheologicalClaim(
                "Energy and the universe are eternal and uncaused.",
                "materialist cosmology",
                0.7,
                new List<Proposition>(),
                new List<Proposition> { CreatedUniverse });
            TheologicalClaim cosmological = new TheologicalClaim(
                "The universe began, so it has a cause outside itself.",
                "cosmological argument",
                0.7,
                new List<Proposition> { CreatedUniverse },
                new List<Proposition>());
            TheologicalClaim atheism = new TheologicalClaim(
                "No creator exists.",
                "atheism",
                0.7,
                new List<Proposition>(),
                new List<Proposition> { Creator });

            Examination eternalMatterExam = ledger.Admit(eternalMatter);
            Examination cosmologicalExam = ledger.Admit(cosmological);
            Examination atheismExam = ledger.Admit(atheism);

            // Origin claims are unfalsifiable from inside the universe, all of them alike.
            Assert.Equal(Verdict.Unfalsifiable, eternalMatterExam.Verdict);
            Assert.Equal(Verdict.Unfalsifiable, cosmologicalExam.Verdict);
            Assert.Equal(Verdict.Unfalsifiable, atheismExam.Verdict);
            Assert.Null(eternalMatterExam.Standing);
            Assert.Null(cosmologicalExam.Standing);
            Assert.Null(atheismExam.Standing);

            // Eternal matter and the cosmological argument collide on CreatedUniverse: one
            // tension, both claims still on the ledger.
            Tension tension = Assert.Single(ledger.Tensions);
            Assert.Equal("CreatedUniverse", tension.Proposition.Name);
            Assert.Equal(2, ledger.ClaimsAbout(CreatedUniverse).Count);

            // The intra-universe extrapolation touched CreatedUniverse but was refused authority
            // over an origin claim; the skip was recorded, and the cosmological argument was NOT
            // ruled Contradicts by it.
            Assert.Contains(cosmologicalExam.Derivation,
                (DerivationStep step) => step.Outcome.Contains("domain skip"));
            Assert.Contains(eternalMatterExam.Derivation,
                (DerivationStep step) => step.Outcome.Contains("domain skip"));
        }
    }
}
```

Note: the domain-skip step only appears when a foundational claim actually touches the proposition being skipped. That is why the setup includes `ConservationExtrapolation`, an intra-universe foundational claim that denies `CreatedUniverse`: the engine must refuse it authority over an origin claim, record the skip, and still return `Unfalsifiable` (never `Contradicts`) for the cosmological argument.

- [ ] **Step 2: Run the full test suite**

Run: `dotnet test AnointedAutomation.Objects.Tests/AnointedAutomation.Objects.Tests.csproj`
Expected: all tests pass, including all pre-existing (non-Epistemics) tests.

- [ ] **Step 3: Build the whole solution to confirm nothing else broke**

Run: `dotnet build AnointedAutomation.sln`
Expected: Build succeeded, 0 errors, 0 warnings.

- [ ] **Step 4: Commit**

```bash
git add AnointedAutomation.Objects.Tests/Epistemics/WorkedExampleTests.cs
git commit -m "Add epistemics worked example test"
```

---

### Task 9: Documentation updates

**Files:**
- Modify: `PROJECT_STRUCTURE_DICTIONARY.md` (add the `Concepts/Epistemics/` folder and its files)
- Modify: `PROJECT_STRUCTURE_CODE.md` (describe the Epistemics area: purpose, types, Examine algorithm summary)
- Modify: `PROJECT_WORK.md` (record the completed task; this file is never committed)

**Interfaces:**
- Consumes: the finished code from Tasks 1 to 8.
- Produces: updated documentation.

- [ ] **Step 1: Update PROJECT_STRUCTURE_DICTIONARY.md**

Read the file first and follow its existing format exactly. Add entries for:
- `AnointedAutomation.Objects/Concepts/Epistemics/` and each file in it (`Testability.cs`, `Proposition.cs`, `LawDomain.cs`, `FoundationalClaim.cs`, `TheologicalClaim.cs`, `Verdict.cs`, `DerivationStep.cs`, `Examination.cs`, `Tension.cs`, `EpistemicLedger.cs`)
- `AnointedAutomation.Objects.Tests/Epistemics/` and its five test files

- [ ] **Step 2: Update PROJECT_STRUCTURE_CODE.md**

Read the file first and follow its existing format. Add a short section describing the Epistemics engine: consistency mapping over a shared proposition vocabulary, three-valued standing, four-state verdicts, domain-bounded foundational claims, tensions as data, and a pointer to the spec at `docs/superpowers/specs/2026-07-02-theology-engine-design.md`.

- [ ] **Step 3: Update PROJECT_WORK.md**

Mark the theology engine implementation as completed with today's date. Do NOT stage or commit this file.

- [ ] **Step 4: Commit the structure docs**

```bash
git add PROJECT_STRUCTURE_DICTIONARY.md PROJECT_STRUCTURE_CODE.md
git commit -m "Document epistemics engine"
```

---

## Amendment (2026-07-02, mid-execution)

User decisions during execution changed the packaging architecture. Binding overrides:

- The epistemics engine lives in a new package `AnointedAutomation.Concepts` (done, commit e120727). Wherever Tasks 4 to 8 say `AnointedAutomation.Objects/Concepts/Epistemics/` read `AnointedAutomation.Concepts/Epistemics/`; namespace `AnointedAutomation.Concepts.Epistemics`; tests in `AnointedAutomation.Concepts.Tests/Epistemics/`, namespace `AnointedAutomation.Concepts.Tests.Epistemics`. Test command: `dotnet test AnointedAutomation.Concepts.Tests/AnointedAutomation.Concepts.Tests.csproj`.
- Execution order: Tasks 5, 6, 7, 8, then 10, 11, 12, then 9 (docs last, covering everything).

### Task 10: Migrate all remaining concepts from Objects to Concepts

Move `AnointedAutomation.Objects/Concepts/` (everything: Love, Agape, SacrificialLove, SelfSeekingLove, LoveAction, Deed, Situation, Circumstance(s), Condition, Selector, Sequence, BehaviorNode, BehaviorResult, and the whole `Reality/` subtree including `Morals/`) into `AnointedAutomation.Concepts/`, preserving subfolder structure (`AnointedAutomation.Concepts/Love.cs` etc. at package root mirroring current layout under `Concepts/`). Namespace `AnointedAutomation.Objects.Concepts` becomes `AnointedAutomation.Concepts`. Move the matching test files (`MoralConceptTests.cs`, `Canon/` concept tests, and any other test file whose usings reference `AnointedAutomation.Objects.Concepts`) from `AnointedAutomation.Objects.Tests` to `AnointedAutomation.Concepts.Tests`, updating namespaces and usings. Update ALL references across the solution (`grep -rn "Objects.Concepts" --include=*.cs`). If Objects.Demo or Objects.API reference concept types, add a ProjectReference to `AnointedAutomation.Concepts` where needed. Breaking change: bump `AnointedAutomation.Objects` csproj Version major (2.0.0) and `AnointedAutomation.Concepts` stays 1.0.0 (unreleased). Whole solution must build with 0 errors; all tests pass. One commit: `Move concepts into Concepts package`.

### Task 11: EpistemicStatus

Add `AnointedAutomation.Concepts/Epistemics/EpistemicStatus.cs`: `public enum EpistemicStatus { Law, Theory, Conjecture }` with XML docs explaining the split (Law: survived so much falsification it functions as bedrock; Theory: well supported, still contested at the edges; Conjecture: asserted and unproven, standing null, e.g. the Collatz conjecture). Add a `EpistemicStatus Status` property to `FoundationalClaim` via a new constructor parameter (after `survivedFalsificationWeight`), keeping the existing constructor overload which defaults to `EpistemicStatus.Law` for backward compatibility within this branch. TDD, tests in `FoundationalClaimTests`. Commit: `Add epistemic status`.

### Task 12: AnointedAutomation.Mathematics package

New project `AnointedAutomation.Mathematics` (+ `AnointedAutomation.Mathematics.Tests`), modeled on the Concepts csproj (Version 1.0.0, Description mentioning curated laws, theories, and conjectures of mathematics and physics; PackageTags `mathematics laws epistemics`; README.md). ProjectReference to `AnointedAutomation.Concepts`. Wire into sln and ALL NuGet runners (nuget-publish.yml PACKAGES, version-increment.yml PROJECTS, publish-packages.sh PACKAGES, build-and-test.yml test step), exactly as done for Concepts in commit e120727.

Contents, namespace `AnointedAutomation.Mathematics`:
- `UniversalPropositions.cs`: static class exposing the shared `Proposition` instances the catalogs use (NonContradiction, Identity, ExcludedMiddle, EffectsHaveCauses, EnergyConserved, EntropyIncreases, MassEnergyEquivalent, SpeedOfLightConstant, CollatzTerminates, GoldbachHolds, RiemannZerosOnCriticalLine), each with correct `Testability` (logic and intra-universe physics: `EmpiricallyTestable`; nothing here is `BeyondObservation`) and standing (`true` for laws' propositions, `null` for conjectures').
- `UniversalLaws.cs`: static class with static readonly `FoundationalClaim` members and an `All` read-only list. Entries (status `Law`): NonContradiction (Unrestricted, 1.0), Identity (Unrestricted, 1.0), ExcludedMiddle (Unrestricted, 0.98), Causality (IntraUniverse, 0.99), ConservationOfEnergy (IntraUniverse, 0.99), EntropyIncrease (IntraUniverse, 0.99).
- `PhysicalTheories.cs`: static class, status `Theory`: MassEnergyEquivalence (IntraUniverse, 0.95), InvariantLightSpeed (IntraUniverse, 0.95). Plus `All`.
- `Conjectures.cs`: static class, status `Conjecture`, weight 0.0 (survived no proof), propositions standing null: Collatz, Goldbach, RiemannHypothesis. Plus `All`.
- Tests: every catalog member non-null, correct domain/status/weight ranges, `All` counts, laws usable to construct an `EpistemicLedger` that examines a claim; a conjecture-backed claim examines as `Undetermined` not `Consistent` (weight 0.0 must never support); no duplicate proposition names across `UniversalPropositions`.

Commit: `Add mathematics package with law catalog`.
