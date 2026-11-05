# Changelog: AnointedAutomation.Repository.Mongo

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
  session; Required (or Auto on a replica set or sharded cluster) runs it in a session transaction.
- Write concern errors (`MongoWriteConcernException`) propagate to the caller unchanged.
