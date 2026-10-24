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
10. **AnointedAutomation.SSO** (`dotnet/AnointedAutomation.SSO/`) - Sign in with Anointed Automation for ASP.NET Core
11. **AnointedAutomation.SSO.UI** (`dotnet/AnointedAutomation.SSO.UI/`) - Tag Helper / Blazor buttons and BFF auth
    routes on top of SSO; sign-out also does RP-initiated logout
12. **AnointedAutomation.SSO.Owin** (`dotnet/AnointedAutomation.SSO.Owin/`) - .NET Framework 4.7.2 OWIN kit (MVC 5,
    Web Forms); tests target net10.0 everywhere and net48 on Windows
13. **AnointedAutomation.Templates** (`dotnet/templates/AnointedAutomation.Templates/`) - `dotnet new`
    `anointed-razor` and `anointed-blazor` templates pinned to SSO and SSO.UI

### Test Projects
All test projects are organized under a "Tests" solution folder:
- **AnointedAutomation.Logging.Tests** (`dotnet/AnointedAutomation.Logging.Tests/`)
- **AnointedAutomation.Memory.Tests** (`dotnet/AnointedAutomation.Memory.Tests/`)
- **AnointedAutomation.APIMiddlewares.Tests** (`dotnet/AnointedAutomation.APIMiddlewares.Tests/`)
- **AnointedAutomation.Repository.Mongo.Tests** (`dotnet/AnointedAutomation.Repository.Mongo.Tests/`)
- **AnointedAutomation.Concepts.Tests** (`dotnet/AnointedAutomation.Concepts.Tests/`)
- **AnointedAutomation.Mathematics.Tests** (`dotnet/AnointedAutomation.Mathematics.Tests/`)
- **AnointedAutomation.SSO.Tests**, **AnointedAutomation.SSO.UI.Tests** (xunit + bUnit), **AnointedAutomation.SSO.Owin.Tests**

### JavaScript Packages (`js/`)
- **@anointedautomation/sso** (`js/anointed-sso/`) - "Sign in with Anointed Automation" OpenID Connect
  client kit for partner apps (Node 18+, Deno/Base44, edge). Dependency-free core, Express and Web adapters,
  runnable Express / Next.js / Base44 examples.
- **@anointedautomation/sso-react** (`js/anointed-sso-react/`) - React sign-in button and `useAnointedSignIn` hook
  (browser safe, no secrets; the server uses `@anointedautomation/sso`).
- **@anointedautomation/sso-next** (`js/anointed-sso-next/`) - Next.js App Router `start` / `callback` /
  `backchannelLogout` route handlers built on `@anointedautomation/sso`.
- **@anointedautomation/sso-vue** (`js/anointed-sso-vue/`) - Vue 3 sign-in button and `useAnointedSignIn` composable
  (browser safe, no secrets).
- **@anointedautomation/sso-nuxt** (`js/anointed-sso-nuxt/`) - Nuxt 3/4 server routes (start, callback, logout,
  back-channel logout) built on `@anointedautomation/sso`.
- **@anointedautomation/sso-angular** (`js/anointed-sso-angular/`) - Angular 17+ standalone button,
  `AnointedSignInService`, `provideAnointedSignIn()`; built with ng-packagr.
- **@anointedautomation/sso-sveltekit** (`js/anointed-sso-sveltekit/`) - SvelteKit handle hook / `+server.js`
  handlers built on `@anointedautomation/sso`, plus a Svelte 5 button (`./client`).
- **@anointedautomation/sso-react-native** (`js/anointed-sso-react-native/`) - React Native / Expo button and
  `useAnointedSignIn` hook (system auth session, deep-link callback, no secrets in the app).

### Python Package (`python/`)
- **anointedautomation-sso** (`python/anointed-sso/`) - server-side OIDC kit (code + PKCE) for Python 3.9+ with
  Django, FastAPI/Starlette and Flask adapters. CI: `python-anointed-sso.yml`; publish: `pypi-publish.yml`.

### PHP Package (`php/`)
- **anointedautomation/sso** (`php/anointed-sso/`) - PHP 8.1+ and Laravel kit (code + PKCE, ID token checks,
  logout, back-channel logout). CI: `php-anointed-sso.yml`; split mirror: `php-split.yml`.

### Swift Package (`swift/`)
- **AnointedSSO** (`swift/AnointedSSO/`, `Package.swift` at the repo root) - "Sign in with Anointed Automation"
  for iOS 15+ / macOS 12+: system sign-in sheet against the partner's own server (which keeps the secret),
  state check, SwiftUI button. Released by semver tag, listed on Swift Package Index.

### Android Library (`android/`)
- **AnointedSSO** (`android/AnointedSSO/`, Gradle module `anointedsso`) - "Sign in with Anointed Automation" for
  Android 7.0+: Custom Tab against the partner's own server (which keeps the secret), state check, Compose button.
  Consumed from source or JitPack; not on Maven Central.

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
