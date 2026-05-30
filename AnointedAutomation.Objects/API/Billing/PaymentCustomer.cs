// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a customer profile stored with a payment provider.
    /// Maps to Stripe Customer, PayPal Customer, Braintree Customer, and Checkout.com Customer.
    /// </summary>
    [System.Serializable]
    public class PaymentCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentCustomer"/> class.
        /// </summary>
        public PaymentCustomer()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CreatedAt = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentCustomer"/> class with specified parameters.
        /// </summary>
        /// <param name="provider">The payment provider.</param>
        /// <param name="email">Customer email address.</param>
        /// <param name="name">Customer full name.</param>
        public PaymentCustomer(PaymentProvider provider, string email, string name)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Provider = provider;
            this.Email = email ?? throw new System.ArgumentNullException(nameof(email));
            this.Name = name;
            this.CreatedAt = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets the internal unique identifier.
        /// </summary>
        [DataMember]
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the payment provider.
        /// </summary>
        [DataMember]
        public PaymentProvider Provider
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the provider's customer ID.
        /// For example, Stripe's cus_xxx, PayPal's customer ID, etc.
        /// </summary>
        [DataMember]
        public string ProviderCustomerId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer's email address.
        /// </summary>
        [DataMember]
        public string Email
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer's full name.
        /// </summary>
        [DataMember]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer's phone number.
        /// </summary>
        [DataMember]
        public string Phone
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer's description or notes.
        /// </summary>
        [DataMember]
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer's billing address.
        /// </summary>
        [DataMember]
        public Address BillingAddress
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer's shipping address.
        /// </summary>
        [DataMember]
        public Address ShippingAddress
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the default payment method ID.
        /// </summary>
        [DataMember]
        public string DefaultPaymentMethodId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets custom metadata as JSON string.
        /// </summary>
        [DataMember]
        public string Metadata
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer's preferred currency code.
        /// </summary>
        [DataMember]
        public string Currency
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer's preferred language/locale.
        /// </summary>
        [DataMember]
        public string Locale
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the account balance (in smallest currency unit).
        /// Positive values represent credit, negative represent amount owed.
        /// </summary>
        [DataMember]
        public long Balance
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is delinquent on payments.
        /// </summary>
        [DataMember]
        public bool IsDelinquent
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        [DataMember]
        public System.DateTime CreatedAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the last update timestamp.
        /// </summary>
        [DataMember]
        public System.DateTime UpdatedAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a live mode customer.
        /// </summary>
        [DataMember]
        public bool IsLiveMode
        {
            get; set;
        }
    }
}
