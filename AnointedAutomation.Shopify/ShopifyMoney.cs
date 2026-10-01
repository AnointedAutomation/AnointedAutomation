// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Readers for a Shopify GraphQL MoneyBag ("<setName>": { "shopMoney": { "amount", "currencyCode" } }). Moved from the
// Anointed API (Services/Common/ShopMoney.cs) and widened with the double-typed variants the order-sync code uses.
// Every reader is guarded at every level: a non-object anywhere on the path is a miss, never a throw.

using System.Globalization;
using System.Text.Json;
using AnointedAutomation.Serialization.Json;

namespace AnointedAutomation.Shopify
{
    /// <summary>Shopify MoneyBag (<c>amountSet.shopMoney</c>) extraction over System.Text.Json.</summary>
    public static class ShopifyMoney
    {
        /// <summary>
        /// (<c>amount</c> as decimal, <c>currencyCode</c>) of <c>parent.setName.shopMoney</c>. A string amount is parsed
        /// with <see cref="NumberStyles.Number"/> invariant; a number amount is read exactly. A missing set/shopMoney gives
        /// (0, ""); an unparseable amount gives 0; a missing currency gives "".
        /// </summary>
        public static (decimal Amount, string Currency) Read(JsonElement parent, string setName)
        {
            JsonElement money = ShopMoneyElement(parent, setName);
            if (money.ValueKind != JsonValueKind.Object)
            {
                return (0m, string.Empty);
            }

            decimal amount = money.GetDecimalCoercedOrNull("amount", NumberStyles.Number) ?? 0m;
            string currency = money.GetStringOrNull("currencyCode") ?? string.Empty;
            return (amount, currency);
        }

        /// <summary>
        /// <c>parent.setName.shopMoney.amount</c> as a double: a JSON number, or a string parsed with
        /// <see cref="NumberStyles.Float"/> invariant; 0 otherwise.
        /// </summary>
        public static double ReadAmountDoubleOrZero(JsonElement parent, string setName)
        {
            JsonElement amount = ShopMoneyElement(parent, setName).GetElementOrDefault("amount");
            return amount.TryGetDoubleLenient(out double d) ? d : 0;
        }

        /// <summary>
        /// <see cref="ReadAmountDoubleOrZero"/>, but when the MoneyBag amount is missing or unparseable, falls back to a
        /// flat <c>parent.amount</c> (same lenient parse); 0 when neither parses.
        /// </summary>
        public static double ReadAmountDoubleOrFlatOrZero(JsonElement parent, string setName)
        {
            JsonElement amount = ShopMoneyElement(parent, setName).GetElementOrDefault("amount");
            if (amount.TryGetDoubleLenient(out double d))
            {
                return d;
            }

            return parent.GetElementOrDefault("amount").TryGetDoubleLenient(out double flat) ? flat : 0;
        }

        /// <summary><c>parent.setName.shopMoney.currencyCode</c> when it is a JSON string; otherwise null.</summary>
        public static string ReadCurrencyOrNull(JsonElement parent, string setName) =>
            ShopMoneyElement(parent, setName).GetStringOrNull("currencyCode");

        private static JsonElement ShopMoneyElement(JsonElement parent, string setName)
        {
            JsonElement set = parent.GetElementOrDefault(setName);
            return set.GetElementOrDefault("shopMoney");
        }
    }
}
