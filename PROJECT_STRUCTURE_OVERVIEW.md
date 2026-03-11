[← Back to Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)

# PROJECT STRUCTURE OVERVIEW

## Architecture
This is a .NET solution containing multiple NuGet package libraries developed by Anointed Automation.

**GitHub Organization:** [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)

## Technology Stack
- **.NET**: Target framework for all projects
- **C#**: Primary programming language
- **Visual Studio**: Solution format v12.00
- **NuGet**: Package distribution
- **GitHub Actions**: CI/CD for automatic publishing
- **MongoDB**: Database integration via Repository pattern

## Solution Structure
The solution `AnointedAutomation.sln` contains the following project organization:

### Main Libraries
1. **AnointedAutomation.Logging** (`AnointedAutomation.Logging/`)
2. **AnointedAutomation.APIMiddlewares** (`AnointedAutomation.APIMiddlewares/`)
3. **AnointedAutomation.Repository.Mongo** (`AnointedAutomation.Repository.Mongo/`)
4. **AnointedAutomation.Objects.API** (`AnointedAutomation.Objects.API/`)
5. **AnointedAutomation.Memory** (`AnointedAutomation.Memory/`)
6. **AnointedAutomation.Objects** (`AnointedAutomation.Objects/`)
7. **AnointedAutomation.Objects.Mongo** (`AnointedAutomation.Objects.Mongo/`) - MongoDB-specific data models with BSON attributes

### Test Projects
All test projects are organized under a "Tests" solution folder:
- **AnointedAutomation.Logging.Tests** (`AnointedAutomation.Logging.Tests/`)
- **AnointedAutomation.Memory.Tests** (`AnointedAutomation.Memory.Tests/`)
- **AnointedAutomation.APIMiddlewares.Tests** (`AnointedAutomation.APIMiddlewares.Tests/`)
- **AnointedAutomation.Repository.Mongo.Tests** (`AnointedAutomation.Repository.Mongo.Tests/`)

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