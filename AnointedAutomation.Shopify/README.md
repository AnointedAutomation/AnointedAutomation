# AnointedAutomation.Shopify

Jesus is King ✝️

Plain POCO models for the Shopify Admin REST API, with Newtonsoft.Json attributes. Customers, addresses, marketing consent, orders, line items, refunds, transactions, products, variants, images, fulfillments, fulfillment orders, inventory items and levels, locations, webhooks, price rules, gift cards, payouts, disputes and the rest of the REST resource set.

Models plus two small helpers: `ShopifyGid` (format and parse `gid://shopify/<Resource>/<id>`) and `ShopifyMoney` (read a GraphQL MoneyBag from a System.Text.Json `JsonElement`). There is no HTTP client, no service layer, and no GraphQL types. Deserialize the JSON you already have:

```csharp
using AnointedAutomation.Shopify;
using Newtonsoft.Json;

Customer customer = JsonConvert.DeserializeObject<Customer>(json);
long? id = customer.Id;
```

- Target framework: `net10.0`
- Dependencies: `Newtonsoft.Json`, `AnointedAutomation.Serialization`
- Namespace: `AnointedAutomation.Shopify` (converters in `AnointedAutomation.Shopify.Converters`)

## License

MIT. The models are derived from [ShopifySharp](https://github.com/nozzlegear/ShopifySharp) (Copyright (c) 2015 Joshua Harms, MIT). See `THIRD-PARTY-NOTICES.md`.
