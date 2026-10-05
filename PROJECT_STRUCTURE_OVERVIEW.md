[← Back to Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)

# PROJECT STRUCTURE OVERVIEW

## Architecture
This is a .NET solution containing multiple NuGet package libraries developed by Anointed Automation, part of the [Anointed](https://anointed.company) family of ventures.

**Anointed (umbrella):** [https://anointed.company](https://anointed.company)
**GitHub Organization:** [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)

## Technology Stack
- **.NET**: Target framework for all projects
- **C#**: Primary programming language
- **Visual Studio**: Solution format v12.00
- **NuGet**: Package distribution
- **GitHub Actions**: CI/CD for automatic publishing
- **MongoDB**: Database integration via Repository pattern

## Solution Structure
Code is grouped by language: `dotnet/` holds the .NET solution and every NuGet package, `js/` holds the
JavaScript packages (see "JavaScript Packages" below).

The solution `dotnet/AnointedAutomation.sln` contains the following project organization:

### Main Libraries
1. **AnointedAutomation.Logging** (`dotnet/AnointedAutomation.Logging/`)
2. **AnointedAutomation.APIMiddlewares** (`dotnet/AnointedAutomation.APIMiddlewares/`)
3. **AnointedAutomation.Repository.Mongo** (`dotnet/AnointedAutomation.Repository.Mongo/`)
4. **AnointedAutomation.Objects.API** (`dotnet/AnointedAutomation.Objects.API/`)
5. **AnointedAutomation.Memory** (`dotnet/AnointedAutomation.Memory/`)
6. **AnointedAutomation.Objects** (`dotnet/AnointedAutomation.Objects/`) - v2.0.0; breaking change: the
   Concepts namespace (Love, Reality, etc.) was removed and migrated to AnointedAutomation.Concepts
7. **AnointedAutomation.Objects.Mongo** (`AnointedAutomation.Objects.Mongo/`) - MongoDB-specific data models with BSON attributes
8. **AnointedAutomation.Concepts** (`dotnet/AnointedAutomation.Concepts/`) - concept modeling (Love, Reality)
   migrated from Objects, plus a new Epistemics engine (`AnointedAutomation.Concepts.Epistemics`)
9. **AnointedAutomation.Mathematics** (`dotnet/AnointedAutomation.Mathematics/`) - curated mathematics and
   physics claim catalogs (UniversalLaws, PhysicalTheories, Conjectures) that feed the Epistemics
   engine; references AnointedAutomation.Concepts

### Test Projects
All test projects are organized under a "Tests" solution folder:
- **AnointedAutomation.Logging.Tests** (`dotnet/AnointedAutomation.Logging.Tests/`)
- **AnointedAutomation.Memory.Tests** (`dotnet/AnointedAutomation.Memory.Tests/`)
- **AnointedAutomation.APIMiddlewares.Tests** (`dotnet/AnointedAutomation.APIMiddlewares.Tests/`)
- **AnointedAutomation.Repository.Mongo.Tests** (`dotnet/AnointedAutomation.Repository.Mongo.Tests/`)
- **AnointedAutomation.Concepts.Tests** (`dotnet/AnointedAutomation.Concepts.Tests/`)
- **AnointedAutomation.Mathematics.Tests** (`dotnet/AnointedAutomation.Mathematics.Tests/`)

### JavaScript Packages (`js/`)
- **@anointedautomation/sso** (`js/anointed-sso/`) - "Sign in with Anointed Automation" OpenID Connect
  client kit for partner apps (Node 18+, Deno/Base44, edge). Dependency-free core, Express and Web adapters,
  runnable Express / Next.js / Base44 examples. Not published to npm.

### External Dependencies
- **Google/** - Contains Google OAuth integration objects

## Publishing Strategy
- **Stable Releases**: Published from `master` branch
- **Pre-releases**: Published from `develop` branch with `-develop.BUILD` suffix
- Automated via GitHub Actions on branch merges
- Manual publishing via PowerShell/shell scripts available

## Development Workflow
- Development occurs on `develop` branch
- Feature branches created from `develop`
- Merges to `master` trigger stable releases
- Clean build and test requirements before commits
---

**[← Back to Project Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)**
