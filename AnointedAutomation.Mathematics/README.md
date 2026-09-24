# AnointedAutomation.Mathematics

A curated catalog of laws, theories, and conjectures from logic, mathematics, physics, and chemistry, expressed as data for the AnointedAutomation.Concepts epistemics engine. Use it as a ready made measuring stick for an `EpistemicLedger`, so you can check claims against well established knowledge without hand authoring every law yourself.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Mathematics.svg)](https://www.nuget.org/packages/AnointedAutomation.Mathematics) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Mathematics.svg)](https://www.nuget.org/packages/AnointedAutomation.Mathematics)

## Installation

```bash
dotnet add package AnointedAutomation.Mathematics
```

- Target framework: `net10.0`
- Dependencies: [AnointedAutomation.Concepts](https://www.nuget.org/packages/AnointedAutomation.Concepts) (installed automatically; provides `FoundationalClaim`, `Proposition`, `EpistemicLedger`)

## Quick start

```csharp
using System;
using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Mathematics;

EpistemicLedger ledger = new EpistemicLedger(UniversalLaws.All);

TheologicalClaim perpetualMotion = new TheologicalClaim(
    "A closed machine can produce energy forever.",
    "perpetual motion pitch",
    0.9,
    new List<Proposition>(),
    new List<Proposition> { UniversalPropositions.EnergyConserved });

Examination result = ledger.Examine(perpetualMotion);

Console.WriteLine(result.Verdict);    // Contradicts
Console.WriteLine(result.Standing);   // False
Console.WriteLine(result.Confidence); // 0.9 (the lower of the claim's 0.9 and the law's 0.99)
```

## Catalogs

Everything is in the `AnointedAutomation.Mathematics` namespace. Each catalog is a static class of `public static readonly` fields plus an `All` list.

| Class | Element type | Count | What it holds |
|---|---|---|---|
| `UniversalPropositions` | `Proposition` | 79 | The shared vocabulary the catalogs assert. All are `Testability.EmpiricallyTestable`. |
| `UniversalLaws` | `FoundationalClaim` | 64 | Bedrock claims with status `EpistemicStatus.Law`. |
| `PhysicalTheories` | `FoundationalClaim` | 12 | Well supported but still open claims with status `EpistemicStatus.Theory`. |
| `Conjectures` | `FoundationalClaim` | 3 | Unproven claims with status `EpistemicStatus.Conjecture` and a survived falsification weight of 0.0. |

### UniversalLaws

Logic (`LawDomain.Unrestricted`): `NonContradiction`, `Identity`, `ExcludedMiddle`.

Every other law is `LawDomain.IntraUniverse`:

- Mechanics: `Causality`, `NewtonsFirstLaw`, `NewtonsSecondLaw`, `NewtonsThirdLaw`, `HookesLaw`, `StokesLaw`
- Gravitation and astronomy: `NewtonsLawOfUniversalGravitation`, `KeplersFirstLaw`, `KeplersSecondLaw`, `KeplersThirdLaw`
- Conservation: `ConservationOfEnergy`, `ConservationOfMomentum`, `ConservationOfAngularMomentum`, `ConservationOfMass`
- Thermodynamics: `EntropyIncrease`, `ZerothLawOfThermodynamics`, `ThirdLawOfThermodynamics`
- Electromagnetism: `CoulombsLaw`, `GausssLawElectric`, `GausssLawMagnetism`, `FaradaysLawOfInduction`, `AmperesCircuitalLaw`, `LenzsLaw`, `BiotSavartLaw`, `OhmsLaw`, `KirchhoffsCurrentLaw`, `KirchhoffsVoltageLaw`, `KirchhoffsLawOfThermalRadiation`, `JoulesFirstLaw`, `JoulesSecondLaw`, `CuriesLaw`
- Optics and radiation: `SnellsLaw`, `LawOfReflection`, `InverseSquareLawOfRadiation`, `MalussLaw`, `StefanBoltzmannLaw`, `WiensDisplacementLaw`, `PlancksLawOfBlackBodyRadiation`, `BeerLambertLaw`
- Fluids: `PascalsLaw`, `ArchimedesPrinciple`, `BernoullisPrinciple`, `TorricellisLaw`
- Gas laws: `BoylesLaw`, `CharlessLaw`, `GayLussacsLaw`, `AvogadrosLaw`, `IdealGasLaw`, `DaltonsLawOfPartialPressures`, `GrahamsLawOfEffusion`, `HenrysLaw`, `RaoultsLaw`
- Chemistry: `LawOfDefiniteProportions`, `LawOfMultipleProportions`, `LawOfReciprocalProportions`, `HesssLaw`, `FaradaysLawsOfElectrolysis`, `LawOfMassAction`, `FicksFirstLawOfDiffusion`, `FicksSecondLawOfDiffusion`, `FouriersLawOfHeatConduction`

### PhysicalTheories

`MassEnergyEquivalence`, `InvariantLightSpeed`, `SpecialRelativity`, `GeneralRelativity`, `QuantumMechanics`, `AtomicTheory`, `BigBangCosmology`, `EvolutionByNaturalSelection`, `GermTheoryOfDisease`, `CellTheory`, `PlateTectonics`, `KineticTheoryOfGases`

### Conjectures

`Collatz`, `Goldbach`, `RiemannHypothesis`

Because their weight is 0.0, the ledger never treats a conjecture as support: a claim that only agrees with a conjecture examines as `Verdict.Undetermined`, never `Verdict.Consistent`.

## Building a wider ledger

The catalogs are plain lists, so you can combine them. `EpistemicLedger` rejects duplicate claim names, and the names are unique across the three claim catalogs.

```csharp
using System.Collections.Generic;
using AnointedAutomation.Concepts.Epistemics;
using AnointedAutomation.Mathematics;

List<FoundationalClaim> foundations = new List<FoundationalClaim>(UniversalLaws.All);
foundations.AddRange(PhysicalTheories.All);
foundations.AddRange(Conjectures.All);

EpistemicLedger ledger = new EpistemicLedger(foundations);
```

## Related packages

- [AnointedAutomation.Concepts](https://www.nuget.org/packages/AnointedAutomation.Concepts): the epistemics engine these catalogs are built on.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).
Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields.

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue: [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues)
