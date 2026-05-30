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
    /// Represents a subscription purchase with recurring billing details.
    /// Inherits from Purchase to include base purchase information.
    /// Includes pause/resume functionality and usage tracking.
    /// </summary>
    public class Subscription : Purchase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        public Subscription()
        {
            StatusHistory = new List<StatusHistoryEntry>();
            Usage = new Dictionary<string, SubscriptionUsage>();
        }

        /// <summary>
        /// Subscription Constructor
        /// </summary>
        /// <param name="billing">!nullable</param>
        /// <param name="shipping">if null will assign billing addy</param>
        /// <param name="company">
        /// !nullable | This is the company the client has purchased from
        /// </param>
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
        /// Initializes a new instance of the <see cref="Subscription"/> class with specified properties.
        /// </summary>
        /// <param name="billing">The billing address (required).</param>
        /// <param name="shipping">The shipping address (defaults to billing address if null).</param>
        /// <param name="billedMonthly">Whether the subscription is billed monthly.</param>
        /// <param name="billedYearly">Whether the subscription is billed yearly.</param>
        /// <param name="company">The company providing the subscription (required).</param>
        /// <param name="donation">Any additional donation amount.</param>
        /// <param name="isRefund">Whether this is a refund transaction.</param>
        /// <param name="isActive">Whether the subscription is currently active.</param>
        /// <param name="isRecurring">Whether the subscription automatically renews.</param>
        /// <param name="item">The subscribed product or service (required).</param>
        /// <param name="paymentType">The type of payment used.</param>
        /// <param name="purchaser">The subscriber's contact information (required).</param>
        /// <param name="renewalTime">The next renewal date (defaults to current time).</param>
        /// <param name="taxLocation">The tax jurisdiction.</param>
        /// <param name="taxPercentage">The tax rate percentage.</param>
        /// <param name="time">The subscription start time (defaults to current time).</param>
        /// <param name="tip">Any additional tip amount.</param>
        /// <param name="typeOfSubscription">The type or tier of subscription.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when required parameters are null.</exception>
        public Subscription(Address billing, Address shipping, bool billedMonthly, bool billedYearly, string company, decimal donation, bool isRefund, bool isActive, bool isRecurring, Product item,
            PaymentType paymentType, Contact purchaser, System.DateTime renewalTime, string taxLocation, decimal taxPercentage, System.DateTime time, decimal tip, string typeOfSubscription)
        {
            AddressBilling = billing ?? throw new System.ArgumentNullException(nameof(billing));
            AddressShipping = shipping != null ? shipping : billing;
            this.Company = company ?? throw new System.ArgumentNullException(nameof(company));
            this.isBilledMonthly = billedMonthly;
            this.isBilledYearly = billedYearly;
            this.donation = donation;
            TransactionId = System.Guid.NewGuid().ToString();
            this.isRefund = isRefund;
            this.isActive = isActive;
            this.isRecurring = isRecurring;
            this.Item = item ?? throw new System.ArgumentNullException(nameof(item));
            this.paymentType = paymentType;
            this.Purchaser = purchaser ?? throw new System.ArgumentNullException(nameof(purchaser));
            this.renewalTime = renewalTime != default ? renewalTime : System.DateTime.Now;
            this.TaxLocation = taxLocation;
            this.taxPercentage = taxPercentage <= 0 ? this.taxPercentage = 1 : this.taxPercentage = taxPercentage;
            decimal weightedPrice = (Item.cost + (this.Item.price * this.Item.quantitySold));
            decimal taxAmount = weightedPrice * (this.taxPercentage / 100);
            this.time = time != default ? time : System.DateTime.Now;
            this.tip = tip;
            this.total = weightedPrice + this.taxAmount + this.tip + this.donation;
            this.TypeOfSubscription = typeOfSubscription;
            this.Status = SubscriptionStatus.Active;
            StatusHistory = new List<StatusHistoryEntry>();
            Usage = new Dictionary<string, SubscriptionUsage>();
        }

        [DataMember]
        /// <summary>
        /// Gets or sets a value indicating whether the subscription is currently active.
        /// </summary>
        public bool isActive
        {
            get; set;
        }

        /// <summary>
        /// </summary>
        [DataMember]
        /// <summary>
        /// Gets or sets a value indicating whether the subscription is billed monthly.
        /// </summary>
        public bool isBilledMonthly
        {
            get; set;
        }

        /// <summary>
        /// </summary>
        [DataMember]
        /// <summary>
        /// Gets or sets a value indicating whether the subscription is billed yearly.
        /// </summary>
        public bool isBilledYearly
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets a value indicating whether the subscription automatically renews.
        /// </summary>
        public bool isRecurring
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the next renewal date for the subscription.
        /// </summary>
        public System.DateTime renewalTime
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the type or tier of subscription.
        /// </summary>
        public string TypeOfSubscription
        {
            get; set;
        }

        // ============ NEW PROPERTIES ============

        [DataMember]
        /// <summary>
        /// Gets or sets the subscription status.
        /// </summary>
        public SubscriptionStatus Status
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the start of the current billing period.
        /// </summary>
        public System.DateTime CurrentPeriodStart
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the end of the current billing period.
        /// </summary>
        public System.DateTime CurrentPeriodEnd
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the date when the subscription was paused.
        /// </summary>
        public System.DateTime PausedAt
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the date when a paused subscription should automatically resume.
        /// </summary>
        public System.DateTime ResumeAt
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the date when the subscription was cancelled.
        /// </summary>
        public System.DateTime CancelledAt
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets a value indicating whether the subscription ends immediately on cancellation.
        /// If false, the subscription remains active until the end of the current period.
        /// </summary>
        public bool CancelAtPeriodEnd
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the trial end date if applicable.
        /// </summary>
        public System.DateTime TrialEnd
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the usage tracking for various metrics.
        /// Key is the metric name (e.g., "api_calls", "storage_gb").
        /// </summary>
        public Dictionary<string, SubscriptionUsage> Usage
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the list of features included in this subscription tier.
        /// </summary>
        public List<string> Features
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the provider's subscription ID.
        /// </summary>
        public string ProviderSubscriptionId
        {
            get; set;
        }

        [DataMember]
        /// <summary>
        /// Gets or sets the payment provider.
        /// </summary>
        public PaymentProvider Provider
        {
            get; set;
        }

        // ============ NEW METHODS ============

        /// <summary>
        /// Checks if the subscription is currently usable (active or trialing).
        /// </summary>
        /// <returns>True if the subscription can be used.</returns>
        public bool IsUsable()
        {
            return Status == SubscriptionStatus.Active || Status == SubscriptionStatus.Trialing;
        }

        /// <summary>
        /// Checks if the subscription is in a trial period.
        /// </summary>
        /// <returns>True if the subscription is trialing.</returns>
        public bool IsTrialing()
        {
            return Status == SubscriptionStatus.Trialing && TrialEnd > System.DateTime.UtcNow;
        }

        /// <summary>
        /// Pauses the subscription.
        /// </summary>
        /// <param name="resumeDate">Optional date to automatically resume. If not specified, subscription remains paused until manually resumed.</param>
        /// <param name="changedBy">Who initiated the pause (user ID, "system", etc.).</param>
        /// <param name="reason">The reason for pausing.</param>
        public void Pause(System.DateTime? resumeDate, string changedBy, string reason)
        {
            if (Status != SubscriptionStatus.Active && Status != SubscriptionStatus.Trialing)
            {
                throw new System.InvalidOperationException("Only active or trialing subscriptions can be paused.");
            }

            SubscriptionStatus previousStatus = Status;
            Status = SubscriptionStatus.Paused;
            PausedAt = System.DateTime.UtcNow;
            isActive = false;

            if (resumeDate.HasValue)
            {
                ResumeAt = resumeDate.Value;
            }

            AddStatusHistoryEntry(previousStatus.ToString(), Status.ToString(), changedBy, reason);
        }

        /// <summary>
        /// Resumes a paused subscription.
        /// </summary>
        /// <param name="changedBy">Who initiated the resume (user ID, "system", etc.).</param>
        /// <param name="reason">The reason for resuming.</param>
        public void Resume(string changedBy, string reason)
        {
            if (Status != SubscriptionStatus.Paused)
            {
                throw new System.InvalidOperationException("Only paused subscriptions can be resumed.");
            }

            SubscriptionStatus previousStatus = Status;
            Status = SubscriptionStatus.Active;
            isActive = true;
            PausedAt = default;
            ResumeAt = default;

            AddStatusHistoryEntry(previousStatus.ToString(), Status.ToString(), changedBy, reason);
        }

        /// <summary>
        /// Cancels the subscription.
        /// </summary>
        /// <param name="immediately">If true, cancels immediately. If false, cancels at period end.</param>
        /// <param name="changedBy">Who initiated the cancellation (user ID, "system", etc.).</param>
        /// <param name="reason">The reason for cancellation.</param>
        public void Cancel(bool immediately, string changedBy, string reason)
        {
            SubscriptionStatus previousStatus = Status;
            CancelledAt = System.DateTime.UtcNow;
            CancelAtPeriodEnd = !immediately;

            if (immediately)
            {
                Status = SubscriptionStatus.Cancelled;
                isActive = false;
                isRecurring = false;
            }
            else
            {
                // Remains active until period end
                isRecurring = false;
            }

            AddStatusHistoryEntry(previousStatus.ToString(), immediately ? SubscriptionStatus.Cancelled.ToString() : "CancelledAtPeriodEnd", changedBy, reason);
        }

        /// <summary>
        /// Renews the subscription for the next billing period.
        /// </summary>
        /// <param name="changedBy">Who initiated the renewal (usually "system").</param>
        public void Renew(string changedBy)
        {
            if (!isRecurring)
            {
                throw new System.InvalidOperationException("Cannot renew a non-recurring subscription.");
            }

            SubscriptionStatus previousStatus = Status;
            CurrentPeriodStart = CurrentPeriodEnd;
            CurrentPeriodEnd = CalculateNextBillingDate(CurrentPeriodStart);
            renewalTime = CurrentPeriodEnd;
            Status = SubscriptionStatus.Active;
            isActive = true;

            // Reset usage for new period
            foreach (SubscriptionUsage usage in Usage.Values)
            {
                usage.ResetForNewPeriod(CurrentPeriodStart, CurrentPeriodEnd);
            }

            AddStatusHistoryEntry(previousStatus.ToString(), Status.ToString(), changedBy, "Subscription renewed");
        }

        /// <summary>
        /// Calculates the next billing date based on the billing interval.
        /// </summary>
        /// <param name="fromDate">The date to calculate from.</param>
        /// <returns>The next billing date.</returns>
        public System.DateTime CalculateNextBillingDate(System.DateTime fromDate)
        {
            if (isBilledYearly)
            {
                return fromDate.AddYears(1);
            }
            else if (isBilledMonthly)
            {
                return fromDate.AddMonths(1);
            }
            else
            {
                // Default to monthly
                return fromDate.AddMonths(1);
            }
        }

        /// <summary>
        /// Gets the number of days remaining in the current billing period.
        /// </summary>
        /// <returns>Days remaining, or 0 if period has ended.</returns>
        public int GetDaysRemaining()
        {
            if (CurrentPeriodEnd == default)
            {
                return 0;
            }

            System.TimeSpan remaining = CurrentPeriodEnd - System.DateTime.UtcNow;
            return remaining.TotalDays > 0 ? (int)remaining.TotalDays : 0;
        }

        /// <summary>
        /// Updates usage for a specific metric.
        /// </summary>
        /// <param name="metricName">The metric name (e.g., "api_calls").</param>
        /// <param name="amount">The amount to add.</param>
        public void UpdateUsage(string metricName, long amount)
        {
            if (Usage == null)
            {
                Usage = new Dictionary<string, SubscriptionUsage>();
            }

            if (Usage.ContainsKey(metricName))
            {
                Usage[metricName].IncrementUsage(amount);
            }
            else
            {
                throw new System.ArgumentException($"Metric '{metricName}' is not tracked for this subscription.");
            }
        }

        /// <summary>
        /// Adds a usage metric to track.
        /// </summary>
        /// <param name="metricName">The metric name.</param>
        /// <param name="limit">The limit for this metric (-1 for unlimited).</param>
        /// <param name="unit">The unit of measurement.</param>
        public void AddUsageMetric(string metricName, long limit, string unit)
        {
            if (Usage == null)
            {
                Usage = new Dictionary<string, SubscriptionUsage>();
            }

            Usage[metricName] = new SubscriptionUsage(metricName, limit, 0)
            {
                Unit = unit,
                PeriodStart = CurrentPeriodStart,
                PeriodEnd = CurrentPeriodEnd
            };
        }

        /// <summary>
        /// Checks if usage is over the limit for any metric.
        /// </summary>
        /// <returns>True if any metric is over its limit.</returns>
        public bool IsOverUsageLimit()
        {
            if (Usage == null)
            {
                return false;
            }

            foreach (SubscriptionUsage usage in Usage.Values)
            {
                if (usage.IsOverLimit)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a formatted price string (e.g., "$9.99/month").
        /// </summary>
        /// <returns>Formatted price string.</returns>
        public string GetFormattedPrice()
        {
            string interval = isBilledYearly ? "year" : "month";
            return string.Format("${0:F2}/{1}", total, interval);
        }

        /// <summary>
        /// Updates the status with history tracking.
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        /// <param name="changedBy">Who made the change.</param>
        /// <param name="reason">The reason for the change.</param>
        public void UpdateStatus(SubscriptionStatus newStatus, string changedBy, string reason)
        {
            SubscriptionStatus previousStatus = Status;
            Status = newStatus;
            isActive = newStatus == SubscriptionStatus.Active || newStatus == SubscriptionStatus.Trialing;

            AddStatusHistoryEntry(previousStatus.ToString(), newStatus.ToString(), changedBy, reason);
        }

        /// <summary>
        /// Adds an entry to the status history.
        /// </summary>
        private void AddStatusHistoryEntry(string previousStatus, string newStatus, string changedBy, string reason)
        {
            if (StatusHistory == null)
            {
                StatusHistory = new List<StatusHistoryEntry>();
            }

            StatusHistory.Add(new StatusHistoryEntry(previousStatus, newStatus, changedBy, reason));
        }
    }
}
