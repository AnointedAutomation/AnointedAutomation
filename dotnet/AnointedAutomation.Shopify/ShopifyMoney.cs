// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Readers for a Shopify GraphQL MoneyBag ("<setName>": { "shopMoney": { "amount", "currencyCode" } }). Moved from the
// Anointed API (Services/Common/ShopMoney.cs) and widened with the double-typed variants the order-sync code uses.
// Every reader is guarded at every level. Since 2.0.0 the decimal readers never fabricate a value: a miss is a false
// TryRead/TryReadAmount or a FormatException from Read naming the missing field.

using System;
using System.Globalization;
using System.Text.Json;
using AnointedAutomation.Serialization.Json;

namespace AnointedAutomation.Shopify
{
    /// <summary>Shopify MoneyBag (<c>amountSet.shopMoney</c>) extraction over System.Text.Json.</summary>
    public static class ShopifyMoney
    {
        /// <summary>
        /// Reads (<c>amount</c> as decimal, <c>currencyCode</c>) of <c>parent.setName.shopMoney</c>. A string amount is
        /// parsed with <see cref="NumberStyles.Number"/> invariant; a number amount is read exactly. Returns false (amount 0,
        /// currency null, both meaningless) when the set or shopMoney is missing or not an object, the amount is missing or
        /// unparseable, or the currency is missing or empty. Never fabricates a value.
        /// </summary>
        public static bool TryRead(JsonElement parent, string setName, out decimal amount, out string currency)
        {
            return TryReadCore(parent, setName, out amount, out currency) == null;
        }

        /// <summary>
        /// Reads (<c>amount</c> as decimal, <c>currencyCode</c>) of <c>parent.setName.shopMoney</c>, like
        /// <see cref="TryRead"/>, but throws a <see cref="FormatException"/> naming the missing or invalid field
        /// (for example <c>totalSet.shopMoney.amount</c>) instead of returning false.
        /// </summary>
        /// <exception cref="FormatException">The MoneyBag, its amount or its currency is missing or invalid.</exception>
        public static (decimal Amount, string Currency) Read(JsonElement parent, string setName)
        {
            string failure = TryReadCore(parent, setName, out decimal amount, out string currency);
            if (failure != null)
            {
                throw new FormatException(failure);
            }

            return (amount, currency);
        }

        /// <summary>
        /// Reads <c>parent.setName.shopMoney.amount</c> as a decimal (JSON number, or a string parsed with
        /// <see cref="NumberStyles.Number"/> invariant). Returns false when it is missing or unparseable. Currency is not read.
        /// </summary>
        public static bool TryReadAmount(JsonElement parent, string setName, out decimal amount)
        {
            decimal? parsed = ShopMoneyElement(parent, setName).GetDecimalCoercedOrNull("amount", NumberStyles.Number);
            amount = parsed.HasValue ? parsed.Value : 0m;
            return parsed.HasValue;
        }

        private static string TryReadCore(JsonElement parent, string setName, out decimal amount, out string currency)
        {
            amount = 0m;
            currency = null;
            JsonElement money = ShopMoneyElement(parent, setName);
            if (money.ValueKind != JsonValueKind.Object)
            {
                return "Shopify MoneyBag '" + setName + ".shopMoney' is missing or not an object.";
            }

            if (money.GetElementOrDefault("amount").ValueKind == JsonValueKind.Undefined)
            {
                return "Shopify MoneyBag field '" + setName + ".shopMoney.amount' is missing.";
            }

            decimal? parsed = money.GetDecimalCoercedOrNull("amount", NumberStyles.Number);
            if (!parsed.HasValue)
            {
                return "Shopify MoneyBag field '" + setName + ".shopMoney.amount' is not a parseable decimal.";
            }

            string code = money.GetStringOrNull("currencyCode");
            if (string.IsNullOrEmpty(code))
            {
                return "Shopify MoneyBag field '" + setName + ".shopMoney.currencyCode' is missing or empty.";
            }

            amount = parsed.Value;
            currency = code;
            return null;
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
