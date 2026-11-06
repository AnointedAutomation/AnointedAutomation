# Changelog: AnointedAutomation.Shopify

## 2.0.0 (breaking)

Money readers no longer fabricate values. Missing or invalid financial data is an explicit failure.

- `ShopifyMoney.Read(parent, setName)` now throws `FormatException` naming the field (`<setName>.shopMoney`,
  `<setName>.shopMoney.amount` or `<setName>.shopMoney.currencyCode`) when the MoneyBag is missing, the amount is missing
  or unparseable, or the currency is missing or empty. Before it returned `(0, "")`, `0` or `""`.
- New `ShopifyMoney.TryRead(parent, setName, out decimal amount, out string currency)`: false on the same failures.
- New `ShopifyMoney.TryReadAmount(parent, setName, out decimal amount)`: decimal amount only, false when missing or unparseable.
- Unchanged: `ReadAmountDoubleOrZero`, `ReadAmountDoubleOrFlatOrZero` (zero is in the name) and `ReadCurrencyOrNull`.

Migration:

```csharp
// Before (1.x): a missing total silently became 0
(decimal amount, string currency) = ShopifyMoney.Read(order, "totalPriceSet");

// After (2.0.0): handle the miss explicitly
if (!ShopifyMoney.TryRead(order, "totalPriceSet", out decimal amount, out string currency))
{
    logger.LogError("Order {OrderId} has no readable totalPriceSet", orderId);
    return;
}

// Or let it throw when the data must be present
(decimal amount, string currency) = ShopifyMoney.Read(order, "totalPriceSet"); // FormatException on a miss
```
