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

## License

MIT. The models are derived from [ShopifySharp](https://github.com/nozzlegear/ShopifySharp) (Copyright (c) 2015 Joshua Harms, MIT). See `THIRD-PARTY-NOTICES.md`.
