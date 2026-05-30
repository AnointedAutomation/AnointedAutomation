// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a payment intent or order across payment providers.
    /// Maps to Stripe PaymentIntent, PayPal Order, Braintree Transaction, and Checkout.com Payment Request.
    /// </summary>
    [System.Serializable]
    public class PaymentIntent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentIntent"/> class.
        /// </summary>
        public PaymentIntent()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CreatedAt = System.DateTime.UtcNow;
            this.Status = TransactionStatus.Pending;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentIntent"/> class with specified parameters.
        /// </summary>
        /// <param name="provider">The payment provider.</param>
        /// <param name="amount">The payment amount in smallest currency unit (cents, pence, etc.).</param>
        /// <param name="currency">The three-letter ISO currency code.</param>
        /// <param name="customerId">The customer ID from the payment provider.</param>
        /// <param name="description">Description of the payment.</param>
        public PaymentIntent(PaymentProvider provider, long amount, string currency, string customerId, string description)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Provider = provider;
            this.Amount = amount;
            this.Currency = currency ?? throw new System.ArgumentNullException(nameof(currency));
            this.CustomerId = customerId;
            this.Description = description;
            this.CreatedAt = System.DateTime.UtcNow;
            this.Status = TransactionStatus.Pending;
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
        /// Gets or sets the provider's payment intent/order ID.
        /// For example, Stripe's pi_xxx, PayPal's order ID, etc.
        /// </summary>
        [DataMember]
        public string ProviderPaymentId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the amount in smallest currency unit (cents, pence, etc.).
        /// For example, $10.00 USD would be 1000.
        /// </summary>
        [DataMember]
        public long Amount
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the captured amount (for auth/capture flows).
        /// </summary>
        [DataMember]
        public long AmountCaptured
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the refunded amount.
        /// </summary>
        [DataMember]
        public long AmountRefunded
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the three-letter ISO currency code (e.g., "USD", "EUR", "GBP").
        /// </summary>
        [DataMember]
        public string Currency
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the current status of the payment.
        /// </summary>
        [DataMember]
        public TransactionStatus Status
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer ID from the payment provider.
        /// </summary>
        [DataMember]
        public string CustomerId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the payment method ID used.
        /// </summary>
        [DataMember]
        public string PaymentMethodId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the description of the payment.
        /// </summary>
        [DataMember]
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the client secret for frontend confirmation (Stripe, Checkout.com).
        /// </summary>
        [DataMember]
        public string ClientSecret
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the payment should be captured immediately or authorized only.
        /// </summary>
        [DataMember]
        public bool CaptureImmediately
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
        /// Gets or sets the receipt email address.
        /// </summary>
        [DataMember]
        public string ReceiptEmail
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the statement descriptor that appears on customer's statement.
        /// </summary>
        [DataMember]
        public string StatementDescriptor
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
        /// Gets or sets the error message if payment failed.
        /// </summary>
        [DataMember]
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the error code from the provider.
        /// </summary>
        [DataMember]
        public string ErrorCode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a live mode transaction.
        /// </summary>
        [DataMember]
        public bool IsLiveMode
        {
            get; set;
        }
    }
}
