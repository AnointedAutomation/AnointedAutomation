# Design: First-class Google identity on User (GoogleObjects)

Date: 2026-07-26
Status: Approved design, pending spec review

## Problem

Google sign-in data is currently stuffed into the generic `User.Meta` JSON bag as
flat keys (`GoogleSubject`, `Picture`). This has several downsides:

- Google identity shares one untyped JSON string with unrelated data (Shopify linkage
  keys live in the same `Meta` bag).
- The Backend overwrites the whole `Meta` string on Google login
  (`AuthService.cs:339`, `user.Meta = metaJson`), which can clobber Shopify keys.
- The Google account `sub` is not a typed, first-class field, so it cannot be reasoned
  about or indexed cleanly.

There is already an orphaned `Google/` folder in the open-source repo root
(`GoogleObjects`, `GoogleTokenInfo`, `UserProfile`) that models Google token + profile
data with **only a `Newtonsoft.Json` dependency and no `Google.Apis.Auth` SDK
dependency**. It is not wired into any project.

## Goal

Move Google identity out of `Meta` into its own typed object hanging off `User`,
reusing the existing orphaned POCOs, without adding any Google SDK dependency to the
open-source Objects package.

## Decisions (locked)

1. **Property on base `User`**, not a subclass and not `Meta`. `User` gets
   `public GoogleObjects Google { get; set; }`.
2. **POCOs live in the base `AnointedAutomation.Objects` package.** They are
   `Newtonsoft.Json`-only (already a dependency of that package, v13.0.4), so this adds
   no Google SDK and no new package. No separate opt-in dll.
3. **Namespace `AnointedAutomation.Objects.Google`** for all three POCOs (the two
   currently in `Google.Integrations` are renamed to match `GoogleObjects.cs`).
4. **Backend refactor included** in this effort: `AuthService` and `GoogleAuthProvider`
   populate `user.Google` instead of writing Google keys into `Meta`.

## Open-source library changes (`AnointedAutomation.Objects`)

### Move the POCOs in
- Move `Google/GoogleObjects.cs`, `Google/GoogleTokenInfo.cs`, `Google/UserProfile.cs`
  into `AnointedAutomation.Objects/API/Google/` (mirrors the existing `API/Account/`
  layout).
- Standardize namespace to `AnointedAutomation.Objects.Google` in all three.
- Delete the orphaned root `Google/` folder (and its `README.md`, folding relevant
  content into the package README if useful).

### Add the property to `User`
- In `AnointedAutomation.Objects/API/Account/User.cs`:
  - `using AnointedAutomation.Objects.Google;`
  - Add `[DataMember] public GoogleObjects Google { get; set; }`.
  - Remove the stale commented `//this.GoogleObjects = googleObjects;` line in the ctor.
- `Meta` stays on `User` (still used for Shopify linkage). Only Google leaves `Meta`.

### Versioning
- Bump `AnointedAutomation.Objects` package `Version` (currently `2.0.1`) and publish
  via the existing `publish-packages.sh` flow. The Backend then consumes the new
  version (currently references `2.0.0`).

## Backend changes (`AnointedAutomation.API`)

### `NewUserData` (in `Services/Auth/IAuthProvider.cs`)
- Replace `string? MetaJson` with `GoogleObjects? Google` (Google is the only thing that
  was ever put in `MetaJson`; `EmailAuthProvider` sets it null).

### `GoogleAuthProvider.GetNewUserData()`
- Build a `GoogleObjects` from the validated `GoogleJsonWebSignature.Payload`:
  - `UserProfile`: `id`/`sub` = `payload.Subject`, `email` = `payload.Email`,
    `verifiedEmail` = `payload.EmailVerified`, `name` = `payload.Name`,
    `givenName` = `payload.GivenName`, `familyName` = `payload.FamilyName`,
    `picture` = `payload.Picture`, `locale` = `payload.Locale`,
    `hd` = `payload.HostedDomain`.
  - `GoogleTokenInfo`: fill the identity fields available from the ID-token payload
    (`sub`, `aud` = `payload.Audience`, `azp`, `exp`, `email`, `emailVerified`). Fields
    that come only from the OAuth token-endpoint response (`access_token`, `id_token`,
    `scope`, `access_type`) are left null here; they are not available from ID-token
    validation.
- Return `NewUserData { ..., Google = googleObjects }` (no `MetaJson`).

### `AuthService`
- Provider path (around line 174-190): set `user.Google = userData.Google` instead of
  `Meta = userData.MetaJson`.
- Direct `AuthenticateGoogleUser` path (lines 254-341): build the same `GoogleObjects`
  from the payload; set `user.Google = googleObjects` on both the new-user branch
  (was line 278 `Meta = metaJson`) and the existing-user branch (was line 339
  `user.Meta = metaJson`). Remove the hand-rolled `metaJson` serialization.
- This removes the `Meta` overwrite, so Shopify keys in `Meta` are no longer clobbered
  on Google login.

### Shopify services (verify only, likely no change)
- `ShopifyCustomerProvisionService` / `ShopifyCustomerSyncService` read/write only the
  Shopify keys in `Meta`; they merely preserved `GoogleSubject` incidentally. After this
  change they keep working on `Meta` for Shopify data. Confirm no logic actually depends
  on reading `GoogleSubject` from `Meta`; if any does, repoint it to `user.Google`.

## Data / backward compatibility

- Existing user documents keep their legacy flat `GoogleSubject`/`Picture` inside `Meta`.
  Nothing reads those for logic, so they are harmless dead keys. No migration required
  for correctness.
- Optional (out of scope for this pass): a one-time backfill that moves legacy
  `Meta.GoogleSubject`/`Meta.Picture` into `user.Google` and strips them from `Meta`.
  Flagged, not implemented here.

## Testing

Open-source package (`AnointedAutomation.Objects.Tests`):
- `GoogleObjects` / `GoogleTokenInfo` / `UserProfile` serialize and deserialize with the
  expected `JsonProperty` names.
- `User` round-trips with a populated `Google` property.

Backend (`AnointedAutomation.API.Tests`):
- `GoogleAuthProvider.GetNewUserData` maps payload fields into `Google` correctly.
- `AuthService.AuthenticateGoogleUser`: new user gets `user.Google` populated and `Meta`
  is not overwritten; existing user with Shopify keys in `Meta` keeps those keys after a
  Google login (regression test for the clobber bug).
- Update existing auth tests that assert on `Meta = "{\"GoogleSubject\":...}"` to assert
  on `user.Google` instead.

## Out of scope

- Separate `AnointedAutomation.Objects.Google` NuGet package (rejected; POCOs go in base).
- `GoogleUser : User` subclass / polymorphic Mongo storage (rejected).
- Changing the Google login lookup key from email to `sub` (separate concern).
- Backfilling/cleaning legacy `Meta` Google keys on existing documents.
</content>
</invoke>
