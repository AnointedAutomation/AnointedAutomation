# Anointed Automation JavaScript / TypeScript packages

Jesus is King ✝️

JavaScript and TypeScript packages from Anointed Automation. Each folder is an independent npm package with
its own README, tests and CI workflow. The .NET libraries live in [`../dotnet`](../dotnet).

| Package | What it does |
|---|---|
| [@anointedautomation/sso](./anointed-sso) | "Sign in with Anointed Automation": a dependency-free OpenID Connect client (code + PKCE) for Node 18+, Deno (Base44) and edge runtimes, with Express and Web (Next.js, Base44) adapters |

## Publishing

Same flow as the NuGet packages:

- Work lands on `develop`. A `develop` to `master` pull request auto increments the `version` in each changed
  package's `package.json` (`.github/workflows/version-increment.yml`; patch rolls into minor at 9).
- Merging to `master` runs `.github/workflows/npm-publish.yml`: every non-private `js/*/package.json` whose
  version is not on npm yet is tested and published with `npm publish --access public`.
- Auth is npm Trusted Publishing (OIDC, provenance included). No npm token is stored in GitHub.

One-time setup per package (npm cannot do a package's FIRST publish over OIDC):

1. Create the `anointedautomation` organization on npmjs.com (owns the `@anointedautomation` scope).
2. Publish the first version by hand: `cd js/<package> && npm test && npm publish --access public`.
3. On npmjs.com > the package > Settings > Trusted Publisher, add GitHub Actions with organization
   `AnointedAutomation`, repository `AnointedAutomation`, workflow `npm-publish.yml`.

After that, every merge to `master` with a new version publishes automatically.
