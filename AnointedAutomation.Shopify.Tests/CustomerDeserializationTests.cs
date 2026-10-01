// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System.IO;
using System.Linq;
using AnointedAutomation.Shopify;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Shopify.Tests
{
    /// <summary>
    /// Deserializes a real Admin REST <c>customers.json</c> customer (PII scrubbed) into <see cref="Customer"/>.
    /// </summary>
    public class CustomerDeserializationTests
    {
        private static Customer Load()
        {
            string json = File.ReadAllText(Path.Combine("Fixtures", "customer_rest.json"));
            return JsonConvert.DeserializeObject<Customer>(json);
        }

        [Fact]
        public void Customer_CoreFields_Deserialize()
        {
            Customer customer = Load();

            Assert.Equal(1000000000001L, customer.Id);
            Assert.Equal("gid://shopify/Customer/1000000000001", customer.AdminGraphQLAPIId);
            Assert.Equal("Jane", customer.FirstName);
            Assert.Equal("Doe", customer.LastName);
            Assert.Equal("jane.doe@example.com", customer.Email);
            Assert.Equal("enabled", customer.State);
            Assert.Equal(3, customer.OrdersCount);
            Assert.Equal(346.27m, customer.TotalSpent);
            Assert.Equal(2000000000001L, customer.LastOrderId);
            Assert.Equal("#1025", customer.LastOrderName);
            Assert.True(customer.VerifiedEmail);
            Assert.False(customer.TaxExempt);
            Assert.Equal("USD", customer.Currency);
            Assert.Null(customer.Phone);
            Assert.NotNull(customer.CreatedAt);
            Assert.Contains("newsletter", customer.Tags);
        }

        [Fact]
        public void Customer_Addresses_Deserialize()
        {
            Customer customer = Load();

            Address address = customer.Addresses.Single();
            Assert.Equal(3000000000001L, address.Id);
            Assert.Equal("Springfield", address.City);
            Assert.Equal("GA", address.ProvinceCode);
            Assert.Equal("US", address.CountryCode);
            Assert.True(address.Default);
            Assert.Equal("Springfield", customer.DefaultAddress.City);
        }

        [Fact]
        public void Customer_MarketingConsent_Deserializes()
        {
            Customer customer = Load();

            Assert.Equal("subscribed", customer.EmailMarketingConsent.State);
            Assert.Equal("single_opt_in", customer.EmailMarketingConsent.OptInLevel);
            Assert.NotNull(customer.EmailMarketingConsent.ConsentUpdatedAt);
            Assert.Null(customer.SmsMarketingConsent);
        }

        [Fact]
        public void Customer_RoundTrips_ThroughJson()
        {
            Customer customer = Load();

            Customer copy = JsonConvert.DeserializeObject<Customer>(JsonConvert.SerializeObject(customer));

            Assert.Equal(customer.Id, copy.Id);
            Assert.Equal(customer.Email, copy.Email);
            Assert.Equal(customer.DefaultAddress.Zip, copy.DefaultAddress.Zip);
        }

        [Fact]
        public void Order_WithNestedCustomerAndLineItems_Deserializes()
        {
            string json = "{\"id\":5,\"name\":\"#1001\",\"total_price\":\"12.50\",\"financial_status\":\"paid\","
                + "\"customer\":{\"id\":7,\"email\":\"a@example.com\"},"
                + "\"line_items\":[{\"id\":9,\"variant_id\":11,\"quantity\":2,\"price\":\"6.25\",\"sku\":\"SKU-1\"}]}";

            Order order = JsonConvert.DeserializeObject<Order>(json);

            Assert.Equal(5L, order.Id);
            Assert.Equal(12.50m, order.TotalPrice);
            Assert.Equal(7L, order.Customer.Id);
            LineItem line = order.LineItems.Single();
            Assert.Equal(11L, line.VariantId);
            Assert.Equal(2, line.Quantity);
            Assert.Equal("SKU-1", line.SKU);
        }
    }
}
