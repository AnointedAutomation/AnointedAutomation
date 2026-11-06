// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using AnointedAutomation.Objects.Billing;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_ValidCurrency_AssignsExactly()
        {
            Product p = new Product("Shirt", 4.25m, "EUR", 19.99m, "p1", 3);
            Assert.Equal("EUR", p.Currency);
            Assert.Equal(4.25m, p.cost);
            Assert.Equal(19.99m, p.price);
            Assert.Equal(3, p.quantitySold);
        }

        [Fact]
        public void Constructor_NullCurrency_Throws()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new Product("Shirt", 1m, null, 2m, "p1", 1));
            Assert.Equal("currency", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("US")]
        [InlineData("USDD")]
        [InlineData("usd")]
        [InlineData("U5D")]
        [InlineData(" US")]
        public void Constructor_InvalidCurrency_Throws(string currency)
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => new Product("Shirt", 1m, currency, 2m, "p1", 1));
            Assert.Equal("currency", ex.ParamName);
        }

        [Fact]
        public void Constructor_NullName_StillThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Product(null, 1m, "USD", 2m, "p1", 1));
        }

        [Fact]
        public void ParameterlessConstructor_LeavesCurrencyUnset()
        {
            Product p = new Product();
            Assert.Null(p.Currency);
            Assert.Equal(1, p.quantitySold);
        }

        [Fact]
        public void RequireCurrencyCode_ReturnsValidCode()
        {
            Assert.Equal("GBP", Product.RequireCurrencyCode("GBP", "c"));
        }
    }
}
