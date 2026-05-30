// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a billing record containing payment information, purchase history, and subscriptions.
    /// </summary>
    public class Bill
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bill"/> class.
        /// </summary>
        public Bill()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bill"/> class with specified properties.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="addressBilling">The billing address.</param>
        /// <param name="orders">List of order IDs.</param>
        /// <param name="paymentTypes">Payment methods on file.</param>
        /// <param name="purchases">List of purchases made.</param>
        /// <param name="refunds">List of refunded purchases.</param>
        /// <param name="subscriptions">List of active subscriptions.</param>
        public Bill(string userId, Address addressBilling, List<string> orders, PaymentCredentials paymentTypes, List<Purchase> purchases, List<Purchase> refunds, List<Subscription> subscriptions)
        {
            this._id = userId;
            this.AdressBilling = addressBilling ?? new Address();
            this.Orders = orders ?? new List<string>();
            this.PaymentTypesOnFile = paymentTypes ?? new PaymentCredentials();
            this.Purchases = purchases ?? new List<Purchase>();
            this.Refunds = refunds ?? new List<Purchase>();
            this.Subscriptions = subscriptions ?? new List<Subscription>();
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the unique identifier for the billing record (user ID).
        /// </summary>
        public string _id { get; set; }

        /// <summary>
        /// Keep a billign address on file
        /// </summary>
        [DataMember]
        public Address AdressBilling
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the list of order IDs associated with this billing record.
        /// </summary>
        public List<string> Orders
        {
            get; set;
        }

        /// <summary>
        /// Credit cards, billing, etc
        /// </summary>
        [DataMember]
        public PaymentCredentials PaymentTypesOnFile
        {
            get; set;
        }

        /// <summary>
        /// All purchases made
        /// </summary>
        [DataMember]
        public List<Purchase> Purchases
        {
            get; set;
        }

        /// <summary>
        /// Purchases that have been refunded
        /// </summary>
        [DataMember]
        public List<Purchase> Refunds
        {
            get; set;
        }

        /// <summary>
        /// Subscriptions
        /// </summary>
        [DataMember]
        public List<Subscription> Subscriptions
        {
            get; set;
        }

        /// <summary>
        /// Gets the primary payment method on file (credit card or payee info).
        /// </summary>
        /// <returns>The primary payment method, or null if none exists.</returns>
        public dynamic GetPrimaryPayment()
        {
            if (PaymentTypesOnFile.CreditCard != null)
            {
                return PaymentTypesOnFile.CreditCard;
            }
            else if (PaymentTypesOnFile.PayeeInfo != null)
            {
                return PaymentTypesOnFile.PayeeInfo;
            }
            else
                return null;
        }
    }
}
