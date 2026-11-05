// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Text.Json;
using AnointedAutomation.Shopify;
using Xunit;

namespace AnointedAutomation.Shopify.Tests
{
    public class ShopifyGidTests
    {
        [Fact]
        public void Formatting()
        {
            Assert.Equal("gid://shopify/Order/5", ShopifyGid.ToGid("Order", 5));
            Assert.Equal("gid://shopify/Order/ x", ShopifyGid.ToGid("Order", " x"));
            Assert.Equal("gid://shopify/Order/5", ShopifyGid.ToGidOrNull("Order", 5));
            Assert.Null(ShopifyGid.ToGidOrNull("Order", null));
            Assert.Equal("gid://shopify/Product/7", ShopifyGid.EnsureGid("Product", "7"));
            Assert.Equal("gid://shopify/Product/7", ShopifyGid.EnsureGid("Product", "gid://shopify/Product/7"));
            Assert.Equal("gid://shopify/Product/ 7", ShopifyGid.EnsureGid("Product", " 7"));
            Assert.Equal("gid://shopify/Product/GID://x", ShopifyGid.EnsureGid("Product", "GID://x"));
            Assert.Throws<ArgumentNullException>(() => ShopifyGid.EnsureGid("Product", null));
        }

        [Fact]
        public void EnsureGidTrimmed()
        {
            Assert.Equal("gid://shopify/Order/5", ShopifyGid.EnsureGidTrimmed("Order", " 5 "));
            Assert.Equal("gid://shopify/Order/5", ShopifyGid.EnsureGidTrimmed("Order", " gid://shopify/Order/5 "));
            Assert.Equal("gid://shopify/Publication/", ShopifyGid.EnsureGidTrimmed("Publication", null));
            Assert.Equal("gid://shopify/Order/GID://shopify/Order/5", ShopifyGid.EnsureGidTrimmed("Order", "GID://shopify/Order/5"));
            Assert.Equal("GID://shopify/Order/5", ShopifyGid.EnsureGidTrimmed("Order", "GID://shopify/Order/5", StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData(" 0042 ", "gid://shopify/ProductVariant/42")]
        [InlineData("gid://shopify/ProductVariant/42", "gid://shopify/ProductVariant/42")]
        [InlineData(" junk ", "junk")]
        [InlineData("-5", "-5")]
        [InlineData(null, null)]
        public void NormalizeNumericToGid(string input, string expected) =>
            Assert.Equal(expected, ShopifyGid.NormalizeNumericToGid("ProductVariant", input));

        [Theory]
        [InlineData("gid://shopify/Order/123", "Order", 123L)]
        [InlineData("gid://shopify/Order/123?x=1", "Order", 123L)]
        [InlineData("gid://shopify/DraftOrder/9", "DraftOrder", 9L)]
        [InlineData("gid://shopify/Order/12abc", null, null)]
        [InlineData("gid://shopify/Order/", null, null)]
        [InlineData("gid://shopify/Order/-5", null, null)]
        [InlineData("gid://shopify/Order/1/2", null, null)]
        [InlineData("gid://shopify//1", null, null)]
        [InlineData("gid://shopify/Or-der/1", null, null)]
        [InlineData("gid://shopify/Order/99999999999999999999", null, null)]
        [InlineData("gid://shopify/Order/١٢", null, null)]
        [InlineData("123", null, null)]
        [InlineData(" gid://shopify/Order/1", null, null)]
        [InlineData(null, null, null)]
        public void TryParse_IsStrict(string gid, string resource, long? id)
        {
            bool ok = ShopifyGid.TryParse(gid, out string r, out long n);
            Assert.Equal(resource != null, ok);
            Assert.Equal(resource, r);
            Assert.Equal(id ?? 0, n);
        }

        [Theory]
        [InlineData("gid://shopify/Order/123", 123L)]
        [InlineData("gid://shopify/Order/123?a=b", 123L)]
        [InlineData("gid://shopify/DraftOrder/9", null)]
        [InlineData("gid://shopify/OrderTransaction/9", null)]
        [InlineData("gid://shopify/Order/12abc", null)]
        [InlineData("x gid://shopify/Order/12", null)]
        [InlineData("123", null)]
        [InlineData("", null)]
        public void ParseResourceId_IsAnchoredAndResourceAware(string gid, long? expected)
        {
            Assert.Equal(expected, ShopifyGid.ParseResourceIdOrNull(gid, "Order"));
            Assert.Equal(expected.HasValue ? expected.Value.ToString() : null, ShopifyGid.ParseResourceIdStringOrNull(gid, "Order"));
        }

        [Theory]
        [InlineData("5", "gid://shopify/Fulfillment/5", 5L)]
        [InlineData(" 5 ", "gid://shopify/Fulfillment/5", 5L)]
        [InlineData("gid://shopify/Fulfillment/5", "gid://shopify/Fulfillment/5", 5L)]
        [InlineData("gid://shopify/Order/5", null, null)]
        [InlineData("gid://shopify/Fulfillment/5?x", null, null)]
        [InlineData("0", null, null)]
        [InlineData("-1", null, null)]
        [InlineData("+1", null, null)]
        [InlineData("abc", null, null)]
        [InlineData("12345678901234567890", null, null)]
        [InlineData(" ", null, null)]
        [InlineData(null, null, null)]
        public void NormalizePositive_MatchesFulfillmentGid(string input, string gid, long? id)
        {
            Assert.Equal(gid, ShopifyGid.NormalizePositive("Fulfillment", input));
            Assert.Equal(id, ShopifyGid.ToPositiveNumericIdOrNull("Fulfillment", input));
        }

        [Theory]
        [InlineData("gid://shopify/Product/123", "123", "123", 123L)]
        [InlineData("gid://shopify/Product/123?foo=bar", "123", "123", 123L)]
        [InlineData("gid://shopify/Product/abc", "abc", "abc", null)]
        [InlineData("gid://shopify/Product/", "", null, null)]
        [InlineData("gid://shopify/Product/ ", " ", null, null)]
        [InlineData("gid://shopify/Product/-1", "-1", "-1", null)]
        [InlineData("123", "", null, null)]
        [InlineData("", "", null, null)]
        [InlineData(null, "", null, null)]
        public void ParseNumericIdFamily(string gid, string raw, string orNull, long? int64)
        {
            Assert.Equal(raw, ShopifyGid.ParseNumericId(gid));
            Assert.Equal(orNull, ShopifyGid.ParseNumericIdOrNull(gid));
            Assert.Equal(int64, ShopifyGid.ParseNumericIdInt64OrNull(gid));
        }

        [Theory]
        [InlineData("gid://shopify/Order/123", 123L, 123L)]
        [InlineData("123", 123L, 0L)]
        [InlineData(" 123 ", 123L, 0L)]
        [InlineData("gid://shopify/Order/ 123 ", 123L, 123L)]
        [InlineData("gid://shopify/Order/123?x=1", null, 0L)]
        [InlineData("gid://shopify/Order/", null, 0L)]
        [InlineData("gid://shopify/Order/-5", -5L, -5L)]
        [InlineData("abc", null, 0L)]
        [InlineData("  ", null, 0L)]
        [InlineData(null, null, 0L)]
        public void TailParsers(string input, long? lenient, long slashRequired)
        {
            Assert.Equal(lenient, ShopifyGid.ParseTailInt64OrNull(input));
            Assert.Equal(slashRequired, ShopifyGid.ParseSlashTailInt64OrZero(input));
            bool ok = ShopifyGid.TryParseTailInt64(input, out long id);
            Assert.Equal(lenient.HasValue, ok);
            Assert.Equal(lenient ?? 0, id);
        }

        [Fact]
        public void TryParseTailInt64_Trims() =>
            Assert.True(ShopifyGid.TryParseTailInt64(" gid://shopify/Order/9 ", out long id) && id == 9);

        [Theory]
        [InlineData("gid://shopify/Product/123", "123", "123")]
        [InlineData("123", "123", "123")]
        [InlineData("gid://shopify/Product/12a", "12a", null)]
        [InlineData("gid://shopify/Product/", null, null)]
        [InlineData("gid://shopify/Product/1?x", "1?x", null)]
        [InlineData("", null, null)]
        [InlineData(null, null, null)]
        public void StringTails(string input, string tail, string digitTail)
        {
            Assert.Equal(tail, ShopifyGid.TailOrNull(input));
            Assert.Equal(digitTail, ShopifyGid.DigitTailOrNull(input));
        }
    }

    public class ShopifyMoneyTests
    {
        private static JsonElement P(string json) => JsonDocument.Parse(json).RootElement.Clone();

        [Fact]
        public void Read_ParsesStringOrNumber()
        {
            Assert.Equal((12.50m, "USD"), ShopifyMoney.Read(P("{\"totalSet\":{\"shopMoney\":{\"amount\":\"12.50\",\"currencyCode\":\"USD\"}}}"), "totalSet"));
            Assert.Equal((3m, "CAD"), ShopifyMoney.Read(P("{\"totalSet\":{\"shopMoney\":{\"amount\":3,\"currencyCode\":\"CAD\"}}}"), "totalSet"));
            Assert.Equal((1234.5m, "USD"), ShopifyMoney.Read(P("{\"totalSet\":{\"shopMoney\":{\"amount\":\"1,234.5\",\"currencyCode\":\"USD\"}}}"), "totalSet"));
            Assert.Equal((0m, "USD"), ShopifyMoney.Read(P("{\"totalSet\":{\"shopMoney\":{\"amount\":\"0.00\",\"currencyCode\":\"USD\"}}}"), "totalSet"));
        }

        [Theory]
        [InlineData("{\"totalSet\":{\"shopMoney\":{\"amount\":\"1,234.5\"}}}", "totalSet.shopMoney.currencyCode")]
        [InlineData("{\"totalSet\":{\"shopMoney\":{\"amount\":\"1\",\"currencyCode\":\"\"}}}", "totalSet.shopMoney.currencyCode")]
        [InlineData("{\"totalSet\":{\"shopMoney\":{\"amount\":\"x\",\"currencyCode\":\"USD\"}}}", "totalSet.shopMoney.amount")]
        [InlineData("{\"totalSet\":{\"shopMoney\":{\"amount\":null,\"currencyCode\":\"USD\"}}}", "totalSet.shopMoney.amount")]
        [InlineData("{\"totalSet\":{\"shopMoney\":{\"currencyCode\":\"USD\"}}}", "totalSet.shopMoney.amount")]
        [InlineData("{\"totalSet\":{\"shopMoney\":5}}", "totalSet.shopMoney")]
        [InlineData("{}", "totalSet.shopMoney")]
        [InlineData("[1]", "totalSet.shopMoney")]
        public void Read_MissingOrInvalid_ThrowsNamingField(string json, string field)
        {
            FormatException ex = Assert.Throws<FormatException>(() => ShopifyMoney.Read(P(json), "totalSet"));
            Assert.Contains("'" + field + "'", ex.Message);
            Assert.False(ShopifyMoney.TryRead(P(json), "totalSet", out decimal amount, out string currency));
            Assert.Equal(0m, amount);
            Assert.Null(currency);
        }

        [Fact]
        public void Read_DefaultElement_Throws()
        {
            Assert.Throws<FormatException>(() => ShopifyMoney.Read(default, "totalSet"));
            Assert.False(ShopifyMoney.TryRead(default, "totalSet", out decimal _, out string _));
        }

        [Fact]
        public void TryRead_Success()
        {
            Assert.True(ShopifyMoney.TryRead(P("{\"s\":{\"shopMoney\":{\"amount\":\"7.25\",\"currencyCode\":\"EUR\"}}}"), "s", out decimal amount, out string currency));
            Assert.Equal(7.25m, amount);
            Assert.Equal("EUR", currency);
        }

        [Fact]
        public void TryReadAmount_SuccessAndFailure()
        {
            Assert.True(ShopifyMoney.TryReadAmount(P("{\"s\":{\"shopMoney\":{\"amount\":\"4.10\"}}}"), "s", out decimal amount));
            Assert.Equal(4.10m, amount);
            Assert.False(ShopifyMoney.TryReadAmount(P("{\"s\":{\"shopMoney\":{\"amount\":\"abc\"}}}"), "s", out decimal bad));
            Assert.Equal(0m, bad);
            Assert.False(ShopifyMoney.TryReadAmount(P("{\"s\":{}}"), "s", out decimal _));
            Assert.False(ShopifyMoney.TryReadAmount(default, "s", out decimal _));
        }

        [Fact]
        public void DoubleAndCurrencyReaders()
        {
            JsonElement full = P("{\"s\":{\"shopMoney\":{\"amount\":\"9.5\",\"currencyCode\":\"EUR\"}},\"amount\":\"1\"}");
            JsonElement flatOnly = P("{\"s\":{\"shopMoney\":{}},\"amount\":2.25}");
            JsonElement broken = P("{\"s\":\"x\",\"amount\":\"no\"}");
            Assert.Equal(9.5, ShopifyMoney.ReadAmountDoubleOrZero(full, "s"));
            Assert.Equal(9.5, ShopifyMoney.ReadAmountDoubleOrFlatOrZero(full, "s"));
            Assert.Equal(0, ShopifyMoney.ReadAmountDoubleOrZero(flatOnly, "s"));
            Assert.Equal(2.25, ShopifyMoney.ReadAmountDoubleOrFlatOrZero(flatOnly, "s"));
            Assert.Equal(0, ShopifyMoney.ReadAmountDoubleOrFlatOrZero(broken, "s"));
            Assert.Equal("EUR", ShopifyMoney.ReadCurrencyOrNull(full, "s"));
            Assert.Null(ShopifyMoney.ReadCurrencyOrNull(flatOnly, "s"));
            Assert.Null(ShopifyMoney.ReadCurrencyOrNull(broken, "s"));
        }
    }
}
