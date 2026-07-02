# AnointedAutomation.Concepts

A .NET library for abstract concept modeling, currently anchored by an epistemics engine.

## Overview

AnointedAutomation.Concepts provides types for modeling abstract concepts as first-class data. The initial feature set is an epistemics engine that maps the consistency of claims: propositions carry three-valued standings, foundational claims declare domain boundaries and testability, theological claims resolve to four-state verdicts, and tensions between claims are tracked as first-class data rather than being silently discarded.

## Installation

Install via NuGet:

```bash
dotnet add package AnointedAutomation.Concepts
```

## Features

- **Three-valued standings**: Propositions can be held as true, false, or undetermined rather than forcing a binary choice
- **Four-state verdicts**: Theological claims resolve to a richer verdict space than pass/fail
- **Foundational claims with domain boundaries**: Claims declare the domain in which they apply and whether they are testable
- **Tensions as first-class data**: Conflicts between claims are modeled explicitly, not swallowed or resolved by default
- **No external dependencies**: Pure modeling code with zero package dependencies

## Namespaces

- `AnointedAutomation.Concepts.Epistemics` - Propositions, foundational claims, theological claims, law domains, and testability

## License

Copyright © 2023 Anointed Automation, LLC. All rights reserved.

## Author

Stewarded by Alexander Fields
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)
Part of the [Anointed](https://anointed.company) family of ventures.
