// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Collections.Generic;
using System.Runtime.Serialization;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    [System.Serializable]
    /// <summary>
    /// Represents a purchase transaction with billing, shipping, and payment details.
    /// Includes status history tracking for audit purposes.
    /// </summary>
    public class Purchase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Purchase"/> class.
        /// Automatically generates a new transaction ID.
        /// </summary>
        public Purchase()
        {
            this.TransactionId = System.Guid.NewGuid().ToString();
            this.StatusHistory = new List<StatusHistoryEntry>();
            this.OrderStatus = TransactionStatus.Pending;
        }

        /// <summary>
        /// Purchase Constructor
        /// </summary>
        /// <param name="billing">!nullable</param>
        /// <param name="shipping">
        /// if null will assign billing addy : For all intents and purposes this is the recipient
        /// </param>
        /// <param name="company">
        /// !nullable | This is the company the client has purchased from
        /// </param>
        /// <param name="discount"></param>
        /// <param name="discountPerc"></param>
        /// <param name="donation"></param>
        /// <param name="isRefund"></param>
        /// <param name="item">!nullable</param>
        /// <param name="purchaser">!nullable</param>
        /// <param name="paymentType"></param>
        /// <param name="taxAmount"></param>
        /// <param name="taxLocation"></param>
        /// <param name="taxPercentage"></param>
        /// <param name="time">if null will assign now</param>
        /// <param name="tip"></param>
        /// <param name="total"></param>
        /// <summary>
        /// Initializes a new instance of the <see cref="Purchase"/> class with specified properties.
        /// </summary>
        /// <param name="billing">The billing address (required).</param>
        /// <param name="shipping">The shipping address (defaults to billing address if null).</param>
        /// <param name="company">The company the purchase is from (required).</param>
        /// <param name="discount">The flat discount amount.</param>
        /// <param name="discountPerc">The discount percentage.</param>
        /// <param name="donation">The donation amount.</param>
        /// <param name="isRefund">Whether this is a refund transaction.</param>
        /// <param name="item">The purchased product (required).</param>
        /// <param name="paymentType">The type of payment used.</param>
        /// <param name="purchaser">The person making the purchase (required).</param>
        /// <param name="taxLocation">The tax jurisdiction.</param>
        /// <param name="taxPercentage">The tax rate percentage.</param>
        /// <param name="time">The time of purchase (defaults to current time).</param>
        /// <param name="tip">The tip amount.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when required parameters are null.</exception>
        public Purchase(Address billing, Address shipping, string company, decimal discount, decimal discountPerc, decimal donation, bool isRefund, Product item,
            PaymentType paymentType, Contact purchaser, string taxLocation, decimal taxPercentage, System.DateTime time, decimal tip)
        {
            AddressBilling = billing ?? throw new System.ArgumentNullException(nameof(billing));
            AddressShipping = shipping ?? billing;
            this.Company = company ?? throw new System.ArgumentNullException(nameof(company));
            this.discount = discount;
            this.discountPerc = discountPerc <= 0 ? this.discountPerc = 1 : this.discountPerc = discountPerc;
            this.donation = donation;
            TransactionId = System.Guid.NewGuid().ToString();
            this.isRefund = isRefund;
            this.Item = item ?? throw new System.ArgumentNullException(nameof(item));
            this.paymentType = paymentType;
            this.Purchaser = purchaser ?? throw new System.ArgumentNullException(nameof(purchaser));
            this.TaxLocation = taxLocation;
            this.taxPercentage = taxPercentage <= 0 ? this.taxPercentage = 1 : this.taxPercentage = taxPercentage;
            decimal weightedPrice = (Item.cost + (this.Item.price * this.Item.quantitySold));
            this.taxAmount = weightedPrice * (this.taxPercentage / 100);
            this.time = time != default ? time : System.DateTime.Now;
            this.tip = tip;
            this.total = weightedPrice + this.taxAmount + this.tip + this.donation;
        }

        [DataMember]
        public Address AddressBilling
        {
            get; set;
        }

        /// <summary>
        /// For all intents and purposes this is the recipient
        /// </summary>
        [DataMember]
        public Address AddressShipping
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the company from which the purchase was made.
        /// </summary>
        public string Company
        {
            get; set;
        }

        /// <summary>
        /// Flat discount rate
        /// </summary>
        [DataMember]
        public decimal discount
        {
            get; set;
        }

        /// <summary>
        /// Percentage of a discount
        /// </summary>
        [DataMember]
        public decimal discountPerc
        {
            get; set;
        }

        [DataMember]
        public decimal donation
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets a value indicating whether this purchase is a gift.
        /// </summary>
        public bool isGift
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets a value indicating whether this purchase is a refund.
        /// </summary>
        public bool isRefund
        {
            get; set;
        }

        /// <summary>
        /// This is the name of the object(s) or service(s) they just purchased
        /// </summary>
        [DataMember]
        public Product Item
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the type of payment used for the purchase.
        /// </summary>
        public PaymentType paymentType
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the contact information of the purchaser.
        /// </summary>
        public Contact Purchaser
        {
            get; set;
        }

        /// <summary>
        /// Amount of money that went to taxes
        /// </summary>
        [DataMember]
        public decimal taxAmount
        {
            get; set;
        }

        /// <summary>
        /// State/Province for tax purposes
        /// </summary>
        [DataMember]
        public string TaxLocation
        {
            get; set;
        }

        /// <summary>
        /// Sale's tax percentage
        /// </summary>
        [DataMember]
        public decimal taxPercentage
        {
            get; set;
        }

        /// <summary>
        /// Time of purchase
        /// </summary>
        [DataMember]
        public System.DateTime time
        {
            get; set;
        }

        [DataMember]
        public decimal tip
        {
            get; set;
        }

        /// <summary>
        /// Total amount after cost, taxes and possibly tip?, donation?
        /// </summary>
        [DataMember]
        public decimal total
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the unique identifier for this transaction.
        /// </summary>
        public string TransactionId
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the current order status.
        /// </summary>
        public TransactionStatus OrderStatus
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the status change history for audit purposes.
        /// </summary>
        public List<StatusHistoryEntry> StatusHistory
        {
            get; set;
        }

        /// <summary>
        /// Updates the order status with history tracking.
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        /// <param name="changedBy">Who made the change.</param>
        /// <param name="reason">The reason for the change.</param>
        public void UpdateOrderStatus(TransactionStatus newStatus, string changedBy, string reason)
        {
            TransactionStatus previousStatus = OrderStatus;
            OrderStatus = newStatus;

            if (StatusHistory == null)
            {
                StatusHistory = new List<StatusHistoryEntry>();
            }

            StatusHistory.Add(new StatusHistoryEntry(previousStatus.ToString(), newStatus.ToString(), changedBy, reason));
        }
    }
}
