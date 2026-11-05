# Changelog: AnointedAutomation.Objects

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
