# Publishing AnointedAutomation NuGet Packages

The repository ships these packages to [NuGet.org](https://www.nuget.org/profiles/roku674):

- [AnointedAutomation.Algorithms](https://www.nuget.org/packages/AnointedAutomation.Algorithms/)
- [AnointedAutomation.APIMiddlewares](https://www.nuget.org/packages/AnointedAutomation.APIMiddlewares/)
- [AnointedAutomation.Concepts](https://www.nuget.org/packages/AnointedAutomation.Concepts/)
- [AnointedAutomation.Enums](https://www.nuget.org/packages/AnointedAutomation.Enums/)
- [AnointedAutomation.Imaging](https://www.nuget.org/packages/AnointedAutomation.Imaging/)
- [AnointedAutomation.Logging](https://www.nuget.org/packages/AnointedAutomation.Logging/)
- [AnointedAutomation.Mathematics](https://www.nuget.org/packages/AnointedAutomation.Mathematics/)
- [AnointedAutomation.Memory](https://www.nuget.org/packages/AnointedAutomation.Memory/)
- [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects/)
- [AnointedAutomation.Objects.API](https://www.nuget.org/packages/AnointedAutomation.Objects.API/)
- [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo/)
- [AnointedAutomation.Repository.MySql](https://www.nuget.org/packages/AnointedAutomation.Repository.MySql/)

`AnointedAutomation.Objects.Mongo` still exists on NuGet.org but no longer lives in this repository and is not
published from it.

## Release flow

1. Work lands on `develop`.
2. Open a pull request from `develop` to `master`. The **Auto-Increment Version on PR** workflow
   (`.github/workflows/version-increment.yml`) bumps the patch version of every package whose folder has changes
   other than its `.csproj`, unless the version was already raised by hand. Patch rolls into minor at 9
   (`1.0.9` becomes `1.1.0`); the major version is never changed automatically.
3. Merge to `master`. The **Publish NuGet Packages** workflow (`.github/workflows/nuget-publish.yml`) builds and
   tests the solution in Release, then packs and pushes each package whose `<Version>` differs from the latest
   version on NuGet.org. Unchanged packages are skipped.

Publishing uses [NuGet Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing) (OIDC
through `NuGet/login`), so no long lived API key is stored in the repository secrets.

## Bumping a version by hand

For a minor or major release, edit `<Version>` in the package's `.csproj` before opening the pull request. The
automation leaves a version alone once it differs from `master`.

## Adding a new package

1. Create the project with the same package metadata as its siblings (`Title`, `Description`, `PackageTags`,
   `Version`, `Authors`, `PackageLicenseExpression`, `PackageReadmeFile`) and pack its `README.md`:

   ```xml
   <ItemGroup>
     <None Include="README.md" Pack="true" PackagePath="/" />
   </ItemGroup>
   ```

   `Directory.Build.props` adds the shared settings (XML documentation file, repository metadata) to every
   project except the tests automatically.
2. Add the project to the package lists in `version-increment.yml`, `nuget-publish.yml` and
   `publish-packages.sh`.
3. Add a row to the package table in `README.md` and to the list above.

The README is rendered on the NuGet.org package page, where relative links do not resolve. Use absolute
`https://` links in package READMEs.

## Manual publishing

Use this only when the workflow is unavailable. It needs the .NET 10 SDK and a NuGet.org API key.

```bash
export NUGET_API_KEY=your_api_key
./publish-packages.sh
```

The script restores, builds and tests in Release, then pushes only the packages whose version differs from
NuGet.org.

To publish one package without the script:

```bash
dotnet pack AnointedAutomation.Logging/AnointedAutomation.Logging.csproj --configuration Release --output ./nupkgs
dotnet nuget push ./nupkgs/AnointedAutomation.Logging.*.nupkg --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json
```
