// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a refund for a payment across payment providers.
    /// Maps to Stripe Refund, PayPal Refund, Braintree Refund, and Checkout.com Refund.
    /// </summary>
    [System.Serializable]
    public class Refund
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Refund"/> class.
        /// </summary>
        public Refund()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CreatedAt = System.DateTime.UtcNow;
            this.Status = TransactionStatus.Pending;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Refund"/> class with specified parameters.
        /// </summary>
        /// <param name="provider">The payment provider.</param>
        /// <param name="paymentIntentId">The original payment intent/transaction ID.</param>
        /// <param name="amount">The refund amount in smallest currency unit.</param>
        /// <param name="reason">The reason for the refund.</param>
        public Refund(PaymentProvider provider, string paymentIntentId, long amount, string reason)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Provider = provider;
            this.PaymentIntentId = paymentIntentId ?? throw new System.ArgumentNullException(nameof(paymentIntentId));
            this.Amount = amount;
            this.Reason = reason;
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
        /// Gets or sets the provider's refund ID.
        /// </summary>
        [DataMember]
        public string ProviderRefundId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the original payment intent/transaction ID.
        /// </summary>
        [DataMember]
        public string PaymentIntentId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the provider's original charge/payment ID.
        /// </summary>
        [DataMember]
        public string ProviderChargeId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the refund amount in smallest currency unit.
        /// </summary>
        [DataMember]
        public long Amount
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [DataMember]
        public string Currency
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the current status of the refund.
        /// </summary>
        [DataMember]
        public TransactionStatus Status
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the reason for the refund.
        /// Common values: duplicate, fraudulent, requested_by_customer, etc.
        /// </summary>
        [DataMember]
        public string Reason
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the detailed reason description.
        /// </summary>
        [DataMember]
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the receipt number for the refund.
        /// </summary>
        [DataMember]
        public string ReceiptNumber
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
        /// Gets or sets the failure reason if refund failed.
        /// </summary>
        [DataMember]
        public string FailureReason
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
        /// Gets or sets a value indicating whether this is a live mode refund.
        /// </summary>
        [DataMember]
        public bool IsLiveMode
        {
            get; set;
        }
    }
}
