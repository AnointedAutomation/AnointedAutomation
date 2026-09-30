// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using Newtonsoft.Json;

namespace AnointedAutomation.Objects.Shopify
{
    /// <summary>
    /// The Shopify customer link for a user, holding the real known storefront-customer fields. This is
    /// the object stored at <c>User.SSO.Shopify</c> and is the source of truth for a user's Shopify
    /// customer id. The legacy <c>Meta["ShopifyCustomerId"]</c> key is still written as a back-compat
    /// mirror for readers not yet repointed here. The stable link key is <see cref="customerId"/>. Mirrors
    /// the structure style of <see cref="AnointedAutomation.Objects.Google.GoogleObjects"/>.
    /// </summary>
    public class ShopifyObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyObjects"/> class.
        /// </summary>
        public ShopifyObjects()
        {
        }

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

        /// <summary>When this Shopify link was first established.</summary>
        [JsonProperty("linkedAt")]
        public System.DateTime? linkedAt { get; set; }
    }
}
