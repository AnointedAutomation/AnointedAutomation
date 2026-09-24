# Anointed Automation .NET Libraries

[![Build and Test](https://github.com/AnointedAutomation/AnointedAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/AnointedAutomation/AnointedAutomation/actions/workflows/build-and-test.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE)

Open source .NET libraries from [Anointed Automation](https://anointedautomation.net), published on
[NuGet.org](https://www.nuget.org/profiles/roku674). Every package targets **.NET 10**, is MIT licensed, and
ships its own README and XML IntelliSense docs.

Part of the [Anointed](https://anointed.company) family of ventures.

## Packages

| Package | NuGet | What it does |
|---|---|---|
| [AnointedAutomation.Enums](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Enums) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Enums.svg)](https://www.nuget.org/packages/AnointedAutomation.Enums) | Shared enumerations used across the other packages |
| [AnointedAutomation.Objects](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Objects) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Objects.svg)](https://www.nuget.org/packages/AnointedAutomation.Objects) | Common models (accounts, billing, payments, response envelopes) as plain POCOs |
| [AnointedAutomation.Objects.API](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Objects.API) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Objects.API.svg)](https://www.nuget.org/packages/AnointedAutomation.Objects.API) | ASP.NET Core helpers: a custom `IFormFile`, a JSON casing convention and GraphQL envelopes |
| [AnointedAutomation.Logging](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Logging) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Logging.svg)](https://www.nuget.org/packages/AnointedAutomation.Logging) | Structured log messages and levels |
| [AnointedAutomation.Memory](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Memory) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Memory.svg)](https://www.nuget.org/packages/AnointedAutomation.Memory) | Memory management helpers |
| [AnointedAutomation.APIMiddlewares](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.APIMiddlewares) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.APIMiddlewares.svg)](https://www.nuget.org/packages/AnointedAutomation.APIMiddlewares) | ASP.NET Core middleware: API key auth and an IP blacklist |
| [AnointedAutomation.Repository.Mongo](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Repository.Mongo) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Repository.Mongo.svg)](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo) | Generic MongoDB repository |
| [AnointedAutomation.Repository.MySql](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Repository.MySql) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Repository.MySql.svg)](https://www.nuget.org/packages/AnointedAutomation.Repository.MySql) | Generic MySQL repository over EF Core |
| [AnointedAutomation.Algorithms](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Algorithms) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Algorithms.svg)](https://www.nuget.org/packages/AnointedAutomation.Algorithms) | Algorithms with no dependencies, including an ISO/IEC 18004 QR encoder |
| [AnointedAutomation.Imaging](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Imaging) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Imaging.svg)](https://www.nuget.org/packages/AnointedAutomation.Imaging) | Render QR codes to SVG and PNG with no image library |
| [AnointedAutomation.Concepts](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Concepts) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Concepts.svg)](https://www.nuget.org/packages/AnointedAutomation.Concepts) | Abstract concept modeling and an epistemics engine |
| [AnointedAutomation.Mathematics](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/AnointedAutomation.Mathematics) | [![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Mathematics.svg)](https://www.nuget.org/packages/AnointedAutomation.Mathematics) | Catalog of mathematical and physical laws, theories and conjectures |

Each package folder has a README with installation, a quick start and an API overview. The same README is
shown on the package's NuGet.org page.

## Installation

```bash
dotnet add package AnointedAutomation.Logging
```

Swap in any package id from the table above. Dependencies between packages are resolved automatically.

## How the packages fit together

```
Enums  <-  Objects  <-  Objects.API
              ^
Logging  <----+----  Repository.Mongo
   ^  ^
   |  +------------  Repository.MySql
   |
   +---------------  APIMiddlewares  ->  Memory

Algorithms  <-  Imaging
Concepts    <-  Mathematics
```

An arrow points from a package to one it depends on.

## Building from source

```bash
git clone https://github.com/AnointedAutomation/AnointedAutomation.git
cd AnointedAutomation
dotnet build AnointedAutomation.sln
dotnet test AnointedAutomation.sln
```

Requires the .NET 10 SDK.

## Releases

- Work lands on `develop`; releases are cut by merging `develop` into `master`.
- Opening a pull request to `master` auto increments the version of every package whose code changed.
- Merging to `master` publishes each package whose version is newer than the one on NuGet.org.

See [PUBLISHING.md](./PUBLISHING.md) for details.

## Contributing

Issues and pull requests are welcome. Please target the `develop` branch and include tests for behavior
changes.

## Support This Project

These libraries are free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Every purchase helps fund continued development and maintenance of these packages.

See [SUPPORT.md](./SUPPORT.md) for help and support options.

## License

[MIT](./LICENSE)

## Contact

- **Anointed (umbrella):** [https://anointed.company](https://anointed.company)
- **Anointed Automation:** [https://anointedautomation.net](https://anointedautomation.net)
- **GitHub:** [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)

Stewarded by Alexander Fields: [https://www.alexanderfields.me](https://www.alexanderfields.me)
