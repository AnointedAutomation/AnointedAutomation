// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a dispute/chargeback for a payment across payment providers.
    /// Maps to Stripe Dispute, PayPal Dispute, Braintree Dispute, and Checkout.com Dispute.
    /// </summary>
    [System.Serializable]
    public class Dispute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dispute"/> class.
        /// </summary>
        public Dispute()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CreatedAt = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispute"/> class with specified parameters.
        /// </summary>
        /// <param name="provider">The payment provider.</param>
        /// <param name="paymentIntentId">The disputed payment intent/transaction ID.</param>
        /// <param name="amount">The disputed amount in smallest currency unit.</param>
        /// <param name="reason">The dispute reason code.</param>
        public Dispute(PaymentProvider provider, string paymentIntentId, long amount, string reason)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Provider = provider;
            this.PaymentIntentId = paymentIntentId ?? throw new System.ArgumentNullException(nameof(paymentIntentId));
            this.Amount = amount;
            this.Reason = reason;
            this.CreatedAt = System.DateTime.UtcNow;
            this.Status = DisputeStatus.NeedsResponse;
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
        /// Gets or sets the provider's dispute ID.
        /// </summary>
        [DataMember]
        public string ProviderDisputeId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the disputed payment intent/transaction ID.
        /// </summary>
        [DataMember]
        public string PaymentIntentId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the provider's charge ID that was disputed.
        /// </summary>
        [DataMember]
        public string ProviderChargeId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the disputed amount in smallest currency unit.
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
        /// Gets or sets the current status of the dispute.
        /// </summary>
        [DataMember]
        public DisputeStatus Status
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the dispute reason code.
        /// Common values: fraudulent, duplicate, product_not_received, product_unacceptable, etc.
        /// </summary>
        [DataMember]
        public string Reason
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the network reason code from the card network.
        /// </summary>
        [DataMember]
        public string NetworkReasonCode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the deadline for responding to the dispute.
        /// </summary>
        [DataMember]
        public System.DateTime EvidenceDueBy
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether evidence has been submitted.
        /// </summary>
        [DataMember]
        public bool HasEvidence
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the response is past due.
        /// </summary>
        [DataMember]
        public bool IsPastDue
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a chargeback (funds withdrawn).
        /// </summary>
        [DataMember]
        public bool IsChargeback
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
        /// Gets or sets the last update timestamp.
        /// </summary>
        [DataMember]
        public System.DateTime UpdatedAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a live mode dispute.
        /// </summary>
        [DataMember]
        public bool IsLiveMode
        {
            get; set;
        }
    }

    /// <summary>
    /// Represents the status of a dispute.
    /// </summary>
    public enum DisputeStatus : int
    {
        /// <summary>
        /// Unknown or unspecified status.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Dispute requires a response with evidence.
        /// </summary>
        NeedsResponse = 1,
        /// <summary>
        /// Dispute is under review by the payment network.
        /// </summary>
        UnderReview = 2,
        /// <summary>
        /// Dispute was won by the merchant.
        /// </summary>
        Won = 3,
        /// <summary>
        /// Dispute was lost by the merchant.
        /// </summary>
        Lost = 4,
        /// <summary>
        /// Dispute requires additional action or information.
        /// </summary>
        WarningNeedsResponse = 5,
        /// <summary>
        /// Warning is under review.
        /// </summary>
        WarningUnderReview = 6,
        /// <summary>
        /// Warning was closed in merchant's favor.
        /// </summary>
        WarningClosed = 7,
        /// <summary>
        /// Merchant accepted the dispute without contesting.
        /// </summary>
        ChargeRefunded = 8
    }
}
