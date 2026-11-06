# Changelog: AnointedAutomation.Repository.Mongo

## 3.4.0

Cancellation for the page-numbered reads, additive. New overloads instead of an optional parameter, so existing
callers and `IMongoHelper` implementers keep binary and source compatibility; the new members have default interface
bodies, and `MongoHelper` and `DelegatingMongoHelper` implement them explicitly.

- `GetPageAsync(collection, filter, sort, page, pageSize, cancellationToken)` and
  `GetDocumentIdPageAsync(collection, page, pageSize, cancellationToken)`, plus matching `MongoPaging` overloads. A
  cancelled token throws `OperationCanceledException` before any read; the token is passed to the Find,
  `CountDocumentsAsync` and `EstimatedDocumentCountAsync`. The tokenless overloads pass `CancellationToken.None`.
- Tests only, no behaviour change: boundary tests for the oversized-page guard that 3.3.0 already had (skip is
  computed in `long`; a skip past `int.MaxValue` throws `ArgumentOutOfRangeException`).

## 3.3.0

Page-numbered reads that return `AnointedAutomation.Objects.PaginatedResponse<T>`, additive. Default interface bodies
keep outside implementations compiling; `MongoHelper` and `DelegatingMongoHelper` have explicit versions.

- `GetPageAsync(collection, filter, sort, page, pageSize)`: 1-based page plus `totalItems`/`totalPages`; the page Find and
  a server-side `CountDocumentsAsync` of the same filter run concurrently.
- `GetDocumentIdPageAsync(collection, page, pageSize)`: ids only (projected to `_id`, ordered by `_id`); its total comes from `EstimatedDocumentCountAsync` (O(1) metadata count), so `totalItems` is an estimate. Named
  apart from the skip/limit `GetDocumentIdsPageAsync`, which has the same parameter types.
- `MongoPaging` (shared implementation, `MaxPageSize` = 1000). page < 1 or pageSize outside 1..1000 throws
  `ArgumentOutOfRangeException`. The skip/limit methods stay and are not obsolete.

## 3.2.0

Collation overloads, additive (#253). Each new `IMongoHelper` member has a default interface body that runs through
`database`, so implementations outside this package keep compiling; `MongoHelper` and `DelegatingMongoHelper` have
explicit versions.

- `GetSingleAsync(collection, filter, collation)`, `GetFilteredDocumentsAsync(collection, filter, sort, limit, collation)`
  (bounded: limit 1 or more), `CountAsync(collection, filter, collation)` and `ExistsAsync(collection, filter, collation)`.
  A null collation throws `ArgumentNullException`.
- `GetAllDocumentsAsync` and `MongoRepository<TDoc>.GetAllAsync` stay `[Obsolete]` (warning); the docs and message now
  say they load the whole collection into RAM and name `GetDocumentIdsPageAsync`, `GetPagedAsync` and the bounded
  `GetFilteredDocumentsAsync`. Nothing in this repository calls them.

## 3.1.0

Opt-in write durability, read preference and transactions. Additive only: `IMongoHelper` is unchanged, and a helper
built without options behaves exactly as in 3.0.0 (no `WithWriteConcern`, no `WithReadPreference`, no `hello`).

- `MongoDurabilityOptions` (immutable, validated): `WriteConcern` (`Default`, `W1`, `Majority`), `Journal` (`bool?`,
  null = server default), `Transactions` (`Off`, `Auto`, `Required`), and per-collection overrides through
  `ForCollections(names, policy)`. `MongoDurabilityPolicy.MoneyWrites` is majority + journaled.
- `MongoReadOptions(ReadPreference)`: a separate read option, not part of durability.
- `new MongoHelper(durability)`, `new MongoHelper(durability, readOptions)`, and the same overloads on
  `MongoHelperFactory`. `MongoHelper.GetCollection<T>(name)` is now public and returns the handle with the options applied.
- `MongoHelper.ValidateAsync()` runs `hello` once and throws `InvalidOperationException` when `Required` or any
  `Majority` policy meets a standalone server; it runs automatically on first use when the options need it. Settings are
  never silently downgraded. `DetectedTopology` and `EffectiveTransactionMode` expose the result.
- `MongoHelper.RunInTransactionAsync(callback)` honors the mode: Off and Auto-on-standalone run the callback with a null
  session; Required (or Auto on a replica set or sharded cluster) runs it in a session transaction. The
  transaction is started with the strictest configured policy (global or any `ForCollections` override): its write
  concern, read concern majority when that policy is majority, and read preference primary, because the driver ignores
  per-collection write concerns inside a transaction. No policy configured: driver default transaction options.
  `MongoHelper.BuildTransactionOptions()` returns those options for code that starts its own session.
- Write concern errors (`MongoWriteConcernException`) propagate to the caller unchanged.
