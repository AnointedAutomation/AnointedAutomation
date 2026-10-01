// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Shopify
{
    /// <summary>
    /// A linked Shopify storefront customer. The fields are the REAL Shopify customer fields the backend
    /// already captures for a user (the customer id, email, first/last name and display name written by
    /// <c>ShopifyCustomerProvisionService</c> and read from the Shopify customer webhook payload), not a
    /// guessed shape. <see cref="customerId"/> is the durable link key; 0 means "not a real linked
    /// customer".
    /// </summary>
    public class ShopifyCustomer
    {
        /// <summary>The Shopify customer id (the durable link key). 0 means "not a real linked customer".</summary>
        [JsonProperty("customerId")]
        public long customerId { get; set; }

        /// <summary>The customer's email as known to the store.</summary>
        [JsonProperty("email")]
        public string email { get; set; }

        /// <summary>The customer's first name.</summary>
        [JsonProperty("firstName")]
        public string firstName { get; set; }

        /// <summary>The customer's last name.</summary>
        [JsonProperty("lastName")]
        public string lastName { get; set; }

        /// <summary>The customer's display name.</summary>
        [JsonProperty("displayName")]
        public string displayName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyCustomer"/> class.
        /// </summary>
        public ShopifyCustomer()
        {
        }
    }
}
