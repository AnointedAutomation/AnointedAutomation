# AnointedAutomation (open source libraries)

## What this is
MIT licensed libraries from Anointed Automation: .NET 10 NuGet packages (`dotnet/`), the `@anointedautomation/sso`
npm package (`js/anointed-sso/`) and the AnointedSSO Swift package (`swift/`, `Package.swift` at root).
Org: AnointedAutomation on GitHub. Umbrella site: anointed.company.

## Inputs
- `PROJECT_WORK.md` (active tasks, gitignored; history in `docs/archive/PROJECT_WORK_HISTORY.md`, do not auto-read).
- GitHub issues and Dependabot PRs. Partner/portal docs: `docs/`.

## Process
- .NET (run from `dotnet/`): `dotnet restore`, `dotnet build --configuration Release`,
  `dotnet test <Project>.Tests/<Project>.Tests.csproj --configuration Release` (solution: `dotnet/AnointedAutomation.sln`).
- JS (`js/anointed-sso/`): `npm test` (runs `node --test test/*.test.js`).
- Swift (repo root): `swift test`.
- Every source file starts with the Anointed copyright header and ends it with "Jesus is King" (see `CONTRIBUTING.md`).
- New code needs unit tests. Test projects sit next to each package as `<Package>.Tests`.

## Outputs
- Work on `develop`, PR to `master`. Version-increment workflow bumps patch versions on the PR.
- Push to `master` runs `nuget-publish.yml` (NuGet Trusted Publishing, OIDC, no stored key). Unchanged packages are skipped.
- npm publish: `npm-publish.yml`. Swift: released by semver tag (`swift-sso-release.yml`).

## Verification
- `cd dotnet && dotnet build --configuration Release && dotnet test --configuration Release`
- `cd js/anointed-sso && npm test`; `swift test`.
- CI: `.github/workflows/build-and-test.yml`, `js-anointed-sso.yml`, `swift-anointed-sso.yml`, `codeql-analysis.yml`.

## Read only when needed
| Doing | Read |
|---|---|
| Find which structure doc to open | `PROJECT_STRUCTURE_DICTIONARY.md` |
| Architecture, package list | `PROJECT_STRUCTURE_OVERVIEW.md` |
| One library's API and deps | `PROJECT_STRUCTURE_LIBRARIES.md` |
| CI/CD, publishing | `PROJECT_STRUCTURE_BUILD.md`, `dotnet/PUBLISHING.md` |
| Writing/running tests | `PROJECT_STRUCTURE_TESTING.md` |
| XML doc standards | `PROJECT_STRUCTURE_CODE.md` |
| Diagrams | `PROJECT_STRUCTURE_VISUAL.md` |
| Add/rename a package, header rules | `CONTRIBUTING.md`, `dotnet/PUBLISHING.md` |
| Partner portal, Sign in with Anointed Automation | `docs/PARTNER_PORTAL.md`, `docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md` |
| AI-facing package index | `llms.txt`, `llms.md`, `llms.json` |

## Landmines
- Any change under `js/` bumps and republishes the npm package; do not touch `js/anointed-sso/README.md` casually.
- Adding a package: update the lists in `version-increment.yml`, `nuget-publish.yml`, `dotnet/publish-packages.sh`, root README, and `dotnet/PUBLISHING.md`.
- When packages, versions, or public types change, update `llms.txt`, `llms.md`, `llms.json` in the same change.
- Publish order matters: Serialization, Shopify, Objects, Repository.Mongo, Objects.API.
- NuGet README links must be absolute (relative links do not resolve on NuGet.org).
- Consumers use TreatWarningsAsErrors: do not mark shims `[Obsolete]`.
