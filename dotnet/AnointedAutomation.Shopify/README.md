# AnointedAutomation.Shopify

Jesus is King ✝️

Plain POCO models for the Shopify Admin REST API, with Newtonsoft.Json attributes. Customers, addresses, marketing consent, orders, line items, refunds, transactions, products, variants, images, fulfillments, fulfillment orders, inventory items and levels, locations, webhooks, price rules, gift cards, payouts, disputes and the rest of the REST resource set.

Models plus two small helpers: `ShopifyGid` (format and parse `gid://shopify/<Resource>/<id>`) and `ShopifyMoney` (read a GraphQL MoneyBag from a System.Text.Json `JsonElement`). Deserialize the JSON you already have:

```csharp
using AnointedAutomation.Shopify;
using Newtonsoft.Json;

Customer customer = JsonConvert.DeserializeObject<Customer>(json);
long? id = customer.Id;
```

- Target framework: `net10.0`
- Dependencies: `Newtonsoft.Json`, `Microsoft.Extensions.Logging.Abstractions`, `AnointedAutomation.Serialization`
- Namespace: `AnointedAutomation.Shopify` (converters in `AnointedAutomation.Shopify.Converters`)

## GraphQL client (`AnointedAutomation.Shopify.GraphQL`)

A transport-neutral Admin GraphQL client over your own `HttpClient`. Tenancy and token storage stay in your code: implement `IShopifyStoreContext` (shop domain, API version, access-token provider).

```csharp
using AnointedAutomation.Shopify.GraphQL;

ShopifyGraphQlClient client = new ShopifyGraphQlClient(httpClient, storeContext, logger);
ShopifyGraphQlResponse shop = await client.QueryAsync(new ShopifyGraphQlRequest("query Shop { shop { name } }", null), ct);

await foreach (JsonElement order in ShopifyPager.EnumerateNodesAsync(client,
    "query Orders($first: Int!, $after: String) { orders(first: $first, after: $after) { nodes { id } pageInfo { hasNextPage endCursor } } }",
    null, data => data.GetProperty("orders"), 250, ct))
{
}
```

- `SendAsync` returns the raw envelope; `QueryAsync` throws on top-level errors without data; `MutateAsync` throws on `userErrors` and returns the payload.
- Pacing: `ShopifyCostBucket` (one per shop, `ShopifyCostBucketRegistry`) waits on `extensions.cost.throttleStatus` before each send.
- Retries (`ShopifyRetryPolicy`): 429 honors `Retry-After`; HTTP 200 `THROTTLED` waits what the bucket needs, on its own round budget; 5xx and network failures back off exponentially.
- A mutation is resent after a 5xx or network failure ONLY when it carries an `@idempotent` key (`ShopifyIdempotency`). Otherwise the client throws `ShopifyGraphQlException` with `Outcome = Indeterminate`: it may have applied, so read the store before acting again.
- `IShopifyRequestScheduler` wraps each send so you can plug in your own queue (default: run now). Cancellation is rethrown; every failure is logged with the operation name and status, never the token.

## Bulk operations (`AnointedAutomation.Shopify.Bulk`)

```csharp
ShopifyBulkOperationRunner bulk = new ShopifyBulkOperationRunner(client, plainHttpClient, logger);
ShopifyBulkOperation op = await bulk.RunQueryAsync("{ products { edges { node { id title } } } }", ct);
await foreach (JsonElement line in bulk.ReadResultsAsync(op, ct)) { }        // streamed, never loaded whole

ShopifyBulkOperation m = await bulk.RunMutationAsync(mutationText, jsonlStream, "updates.jsonl", ct);
```

- `ShopifyStagedUploads` is the one `stagedUploadsCreate` builder; the JSONL is uploaded once and its `key` becomes `stagedUploadPath`.
- The start mutations are unkeyed, so an unknown outcome throws `Indeterminate` and is never resent. Only a definite "already in progress" refusal is retried, after a backoff wait.
- Polling reads `node(id:)` of YOUR operation with backoff (2 s doubling to 30 s, 25 h budget). The terminal state is returned: check `IsCompleted`; FAILED, CANCELED and EXPIRED carry `ErrorCode`, and `ResultUrl` falls back to `partialDataUrl`.

## Webhooks (`AnointedAutomation.Shopify.Webhooks`)

- `ShopifyWebhookHmac.Verify(rawBodyBytes, request.Headers["X-Shopify-Hmac-Sha256"], secret)`: base64 HMAC-SHA256 of the RAW body, constant-time compare. A missing or malformed header is `false`; a missing secret throws.
- `ShopifyWebhookRegistrar.SyncAsync(wanted, managedUriPrefix, ct)`: lists every subscription, keeps matches, creates missing ones and deletes stale ones ONLY under your prefix. `Diff` is the pure plan. Uses `WebhookSubscription.uri` (2025-04 and newer).

## Typed operations and models (`AnointedAutomation.Shopify.Operations`, `.GraphQL.Models`)

```csharp
await client.ExecuteAsync(ShopifyCatalogOperations.TagsAdd(productGid, new[] { "sale" }), ct);
ShopifyProduct p = await client.ExecuteAsync(ShopifyCatalogOperations.ProductById(productGid), ct); // null when not found
await foreach (ShopifyOrder o in ShopifyCatalogOperations.EnumerateOrdersAsync(client, "status:open", 250, ct)) { }
```

- Builders: `TagsAdd`, `TagsRemove`, `MetafieldsSet`, `ProductUpdate`, `ProductSet`, `ProductVariantsBulkUpdate`, `CustomerUpdate`, `InventoryActivate`, `InventorySetQuantities`, `ProductById`, `OrderById`, `CustomerById`, plus paged product/order/customer enumeration. Each pins its query text (tests assert it byte for byte).
- Money paths (`orderCancel`, `refundCreate`, `orderCapture`, `orderEdit*`, `draftOrderCreate`) are deliberately not included.
- Schema target 2026-10. On older versions: `productUpdate(product:)` needs 2024-10+; `@idempotent` is accepted from 2026-01 and required for the inventory mutations from 2026-04; `changeFromQuantity` replaces `ignoreCompareQuantity` from 2026-04.
- Models (`ShopifyProduct`, `ShopifyProductVariant`, `ShopifyMetafield`, `ShopifyCustomer`, `ShopifyOrder`, `ShopifyUserError`, `ShopifyConnection<T>` with `ShopifyPageInfo`, `ShopifyMoneyV2`/`ShopifyMoneyBag`; cost stays `ShopifyQueryCost`) read with `ShopifyGraphQlJson.Options`. Money is `decimal`; `NumericId` uses `ShopifyGid`.
- Hand-written, not schema-generated (decision): a generated Admin schema is thousands of types and changes every quarter, while consumers only need the fields these operations select. Small hand-written models stay stable, reviewable and dependency-free. Revisit if the operation set grows past what is reasonable to keep by hand.

## License

MIT. The models are derived from [ShopifySharp](https://github.com/nozzlegear/ShopifySharp) (Copyright (c) 2015 Joshua Harms, MIT). See `THIRD-PARTY-NOTICES.md`.
