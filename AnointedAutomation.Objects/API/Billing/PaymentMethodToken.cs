// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a tokenized payment method stored with a payment provider.
    /// Maps to Stripe PaymentMethod, PayPal Vault, Braintree PaymentMethod, and Checkout.com Instrument.
    /// </summary>
    [System.Serializable]
    public class PaymentMethodToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodToken"/> class.
        /// </summary>
        public PaymentMethodToken()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CreatedAt = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodToken"/> class with specified parameters.
        /// </summary>
        /// <param name="provider">The payment provider.</param>
        /// <param name="customerId">The customer ID this payment method belongs to.</param>
        /// <param name="paymentType">The type of payment method.</param>
        public PaymentMethodToken(PaymentProvider provider, string customerId, PaymentType paymentType)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Provider = provider;
            this.CustomerId = customerId;
            this.PaymentType = paymentType;
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
        /// Gets or sets the provider's payment method ID/token.
        /// For example, Stripe's pm_xxx, Braintree's payment method token, etc.
        /// </summary>
        [DataMember]
        public string ProviderPaymentMethodId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer ID this payment method belongs to.
        /// </summary>
        [DataMember]
        public string CustomerId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the type of payment method.
        /// </summary>
        [DataMember]
        public PaymentType PaymentType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card brand (Visa, MasterCard, Amex, etc.) for card payments.
        /// </summary>
        [DataMember]
        public string CardBrand
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the last four digits of the card or account.
        /// </summary>
        [DataMember]
        public string Last4
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card expiration month (1-12).
        /// </summary>
        [DataMember]
        public int ExpirationMonth
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card expiration year (4-digit).
        /// </summary>
        [DataMember]
        public int ExpirationYear
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the cardholder name.
        /// </summary>
        [DataMember]
        public string CardholderName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card funding type (credit, debit, prepaid).
        /// </summary>
        [DataMember]
        public string Funding
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the card's issuing country code.
        /// </summary>
        [DataMember]
        public string Country
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the bank name for bank account payment methods.
        /// </summary>
        [DataMember]
        public string BankName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the bank routing number for ACH payments.
        /// </summary>
        [DataMember]
        public string RoutingNumber
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the billing address associated with this payment method.
        /// </summary>
        [DataMember]
        public Address BillingAddress
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the fingerprint/unique identifier for this card across customers.
        /// Used for fraud detection and preventing duplicate cards.
        /// </summary>
        [DataMember]
        public string Fingerprint
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is the default payment method for the customer.
        /// </summary>
        [DataMember]
        public bool IsDefault
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this payment method is reusable.
        /// </summary>
        [DataMember]
        public bool IsReusable
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
        /// Gets or sets the creation timestamp.
        /// </summary>
        [DataMember]
        public System.DateTime CreatedAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a live mode payment method.
        /// </summary>
        [DataMember]
        public bool IsLiveMode
        {
            get; set;
        }
    }
}
