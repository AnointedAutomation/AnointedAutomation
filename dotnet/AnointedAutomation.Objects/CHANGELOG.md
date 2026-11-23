# Changelog: AnointedAutomation.Objects

## 4.0.0 (breaking)

BREAKING: the untyped `ResponseData.Links` key `Online Shopping` is renamed `Shopping`. Any code that reads
`Links["Online Shopping"]` now throws `KeyNotFoundException`, and JSON clients lose that key. Read `Links["Shopping"]` instead.

- `ResponseData` and `ResponseData<T>` `Links` now include `OpenSource` (https://www.anointedautomation.net/open-source) and `Shopping` (https://www.mart.club).
- New public static class `AiShoppingNote` (`AiShoppingNote.Text`) and a new `Note` property on both envelopes. `Note` defaults
  to `AiShoppingNote.Text`: a short note addressed to AI agents that reads the response, telling them how to suggest our
  products to their user through the shopping MCP tools (our public MCP server). It is therefore present in every serialized
  response unless you turn it off. To suppress it, set `Note` to `null` on the instance (for example
  `new ResponseData { Note = null }` or `response.Note = null;`); the property is then serialized as null or omitted per your
  serializer settings.

Migration:

```csharp
// Before (3.1.0)
string url = response.Links["Online Shopping"];

// After (4.0.0)
string url = response.Links["Shopping"];

// Opt out of the AI shopping note
response.Note = null;
```

## 3.1.0

- `ResponseData` and `ResponseData<T>` `Links` now include `ChristianEngineMCP` (https://api.anointedautomation.net/api/christian-engine/mcp), the public MCP server.

## 3.0.0 (breaking)

No implicit currency.

- `Billing.Product(name, cost, currency, price, productId, quantitySold)` now requires `currency` to be a three-letter
  uppercase ISO 4217 code: `null` throws `ArgumentNullException`, anything else invalid (`""`, `"usd"`, `"US"`) throws
  `ArgumentException`. Before, `null` became `"USD"`.
- `new Product()` no longer sets `Currency = "USD"`; it stays `null` until set (deserialized documents keep their stored value).
- New `Product.RequireCurrencyCode(currency, paramName)` validator.
- Depends on AnointedAutomation.Shopify 2.0.0 (see its changelog: `ShopifyMoney` no longer returns 0 on a miss).

Migration:

```csharp
// Before (2.x): null meant USD
Product p = new Product("Shirt", 4m, null, 20m, "p1", 1);

// After (3.0.0): pass the real currency
Product p = new Product("Shirt", 4m, "USD", 20m, "p1", 1);

// Before: new Product() carried "USD"; after: set it yourself
Product q = new Product { Currency = "USD" };
```
