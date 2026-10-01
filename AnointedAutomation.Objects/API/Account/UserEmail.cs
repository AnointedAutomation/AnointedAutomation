// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Account
{
    /// <summary>
    /// One email address attached to a <see cref="User"/>. A user may hold several; exactly one is
    /// <see cref="IsPrimary"/> when the list is non-empty, and <see cref="User.Email"/> mirrors it.
    /// </summary>
    [System.Serializable]
    public class UserEmail
    {
        /// <summary>
        /// Gets or sets the email address. Stored lower-cased and trimmed.
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets whether ownership of the address has been proven.
        /// </summary>
        [DataMember]
        public bool Verified { get; set; }

        /// <summary>
        /// Gets or sets when the address was verified, or null if it never was.
        /// </summary>
        [DataMember]
        public System.DateTime? VerifiedAt { get; set; }

        /// <summary>
        /// Gets or sets whether this is the primary address (the one <see cref="User.Email"/> mirrors).
        /// </summary>
        [DataMember]
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets how the address was added.
        /// </summary>
        [DataMember]
        public UserEmailSource Source { get; set; }

        /// <summary>
        /// Gets or sets the id of the Shopify customer record this address maps to, or null when no Shopify
        /// customer is known for it. One account may hold several addresses, each linked to its own Shopify
        /// customer. Stored as <c>shopifyCustomerId</c> under the hybrid casing (value type, camelCase).
        /// </summary>
        [DataMember]
        public long? ShopifyCustomerId { get; set; }

        /// <summary>
        /// Gets or sets when <see cref="ShopifyCustomerId"/> was linked, or null when it never was. Stored as
        /// <c>shopifyLinkedAt</c> under the hybrid casing.
        /// </summary>
        [DataMember]
        public System.DateTime? ShopifyLinkedAt { get; set; }

        /// <summary>
        /// Gets or sets when the address was added to the account.
        /// </summary>
        [DataMember]
        public System.DateTime AddedAt { get; set; }
    }
}
