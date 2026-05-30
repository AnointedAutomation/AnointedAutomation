// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents usage tracking for a subscription metric.
    /// Used to track consumption against subscription limits (API calls, storage, users, etc.).
    /// </summary>
    [System.Serializable]
    public class SubscriptionUsage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUsage"/> class.
        /// </summary>
        public SubscriptionUsage()
        {
            this.LastUpdated = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionUsage"/> class with specified parameters.
        /// </summary>
        /// <param name="metricName">The name of the metric being tracked.</param>
        /// <param name="limit">The maximum allowed value for this metric.</param>
        /// <param name="used">The current amount used.</param>
        public SubscriptionUsage(string metricName, long limit, long used)
        {
            this.MetricName = metricName ?? throw new System.ArgumentNullException(nameof(metricName));
            this.Limit = limit;
            this.Used = used;
            this.LastUpdated = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets the name of the metric being tracked (e.g., "api_calls", "storage_gb", "users").
        /// </summary>
        [DataMember]
        public string MetricName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the maximum allowed value for this metric.
        /// A value of -1 indicates unlimited.
        /// </summary>
        [DataMember]
        public long Limit
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the current amount used.
        /// </summary>
        [DataMember]
        public long Used
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the unit of measurement (e.g., "calls", "GB", "users").
        /// </summary>
        [DataMember]
        public string Unit
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the timestamp when usage was last updated.
        /// </summary>
        [DataMember]
        public System.DateTime LastUpdated
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the start of the current usage period.
        /// </summary>
        [DataMember]
        public System.DateTime PeriodStart
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the end of the current usage period.
        /// </summary>
        [DataMember]
        public System.DateTime PeriodEnd
        {
            get; set;
        }

        /// <summary>
        /// Gets the remaining amount available (Limit - Used).
        /// Returns -1 if unlimited.
        /// </summary>
        public long Remaining
        {
            get
            {
                if (Limit < 0)
                {
                    return -1; // Unlimited
                }
                long remaining = Limit - Used;
                return remaining < 0 ? 0 : remaining;
            }
        }

        /// <summary>
        /// Gets the usage percentage (0-100).
        /// Returns 0 if unlimited.
        /// </summary>
        public decimal UsagePercentage
        {
            get
            {
                if (Limit <= 0)
                {
                    return 0;
                }
                decimal percentage = ((decimal)Used / Limit) * 100;
                return percentage > 100 ? 100 : percentage;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the limit has been exceeded.
        /// Always returns false if unlimited.
        /// </summary>
        public bool IsOverLimit
        {
            get
            {
                if (Limit < 0)
                {
                    return false; // Unlimited
                }
                return Used > Limit;
            }
        }

        /// <summary>
        /// Gets a value indicating whether usage is at or above 80% of the limit.
        /// Always returns false if unlimited.
        /// </summary>
        public bool IsNearLimit
        {
            get
            {
                if (Limit < 0)
                {
                    return false; // Unlimited
                }
                return UsagePercentage >= 80;
            }
        }

        /// <summary>
        /// Increments the usage by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to add to usage.</param>
        public void IncrementUsage(long amount)
        {
            if (amount < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative. Use DecrementUsage instead.");
            }
            Used += amount;
            LastUpdated = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Decrements the usage by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to subtract from usage.</param>
        public void DecrementUsage(long amount)
        {
            if (amount < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative. Use IncrementUsage instead.");
            }
            Used -= amount;
            if (Used < 0)
            {
                Used = 0;
            }
            LastUpdated = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Resets the usage to zero, typically at the start of a new billing period.
        /// </summary>
        /// <param name="newPeriodStart">The start of the new period.</param>
        /// <param name="newPeriodEnd">The end of the new period.</param>
        public void ResetForNewPeriod(System.DateTime newPeriodStart, System.DateTime newPeriodEnd)
        {
            Used = 0;
            PeriodStart = newPeriodStart;
            PeriodEnd = newPeriodEnd;
            LastUpdated = System.DateTime.UtcNow;
        }
    }
}
