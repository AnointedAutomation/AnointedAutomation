// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Shopify
{
    /// <summary>
    /// The Shopify customer link for a user. Shopify sign-in has no token/profile split (it is a customer
    /// link, not an OIDC login), so this container holds a single nested <see cref="ShopifyCustomer"/>,
    /// keeping the container-of-objects shape consistent with
    /// <see cref="AnointedAutomation.Objects.Google.GoogleObjects"/>. This is the object stored at
    /// <c>User.SSO.Shopify</c> and is the source of truth for a user's Shopify customer id. The legacy
    /// <c>Meta["ShopifyCustomerId"]</c> key is still written as a back-compat mirror for readers not yet
    /// repointed here. The stable link key is <see cref="ShopifyCustomer.customerId"/>.
    /// </summary>
    public class ShopifyObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyObjects"/> class.
        /// </summary>
        public ShopifyObjects()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopifyObjects"/> class with a customer.
        /// </summary>
        /// <param name="customer">The linked Shopify customer.</param>
        public ShopifyObjects(ShopifyCustomer customer)
        {
            Customer = customer ?? new ShopifyCustomer();
        }

        /// <summary>Gets or sets the linked Shopify customer record.</summary>
        public ShopifyCustomer Customer { get; set; }

        /// <summary>When this Shopify link was first established.</summary>
        public System.DateTime? linkedAt { get; set; }
    }
}
